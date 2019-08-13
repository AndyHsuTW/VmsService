using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using RtspCameraExample.KMPEG4;

namespace ACTi.NVR3.OemDvrMiniDriver
{
    enum PlayerState
    {
        Ready = 0,
        OpenPending,
        Started,
        PausePending,
        Paused,
        StartPending,
    }

    /// <summary>
    /// A shamelessly copied example with some tweaks to accommodate our purposes.
    /// Needless to say, it is very much a means of rendering moving video on to a .NET surface using Media Foundation,
    ///		rather than a definitive example of how a DVR supplier should actually implement.
    ///		
    /// ACTi:
    ///   OpenKmpeg => ImageCallback
    /// </summary>
    public class WmfPlayer
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        
        private const int MfVersion = 0x10070;

        public ConnectInfo VideoConnectInfo { get; set; }

        private AutoResetEvent _closingEvent;

        private PlayerState _state;

        private URLCommand m_URLCommand { get; set; }
        #region KMPEG4 Member
        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        private const int KMPEG_CONN_TIMEOUT = 5;
        /// <summary>
        /// Session id when connecting NVR.
        /// </summary>
        public string ConnSession { get; set; }
        private bool m_KMPEGConnectFail;
        private IntPtr m_KMPEGHandle = IntPtr.Zero;
        private DateTime m_ViewUTCTime;
        private DateTime m_DefaultUTCTime;

        private List<EventHandler<ImageEventArgs>> m_ImageEventHandlers { get; set; }
        private KMPEG4.ImageCallback3 m_ImageCallback3 { get; set; }
        private List<EventHandler<TimeCodeCallbackExEventArgs>> m_TimeCodeExHandlers { get; set; }
        private KMPEG4.TimeCodeCallbackEx m_TimeCodeCallbackex;
        private KMPEG4.FilePlayCompleteCallback m_FilePlayCompleteCallback { get; set; }
        private List<EventHandler<FilePlayCompleteEventArgs>> m_FilePlayCompleteEventHandlers { get; set; }

        public class ConnectInfo
        {
            public string HostName { get; private set; }
            public string UserID { get; private set; }
            public string Password { get; private set; }
            public DateTime StartTime { get; private set; }
            public uint Speed { get; private set; }
            /// <summary>
            /// DeviceId
            /// </summary>
            public string SelectID { get; private set; }
            public uint Port { get; private set; }

            public int StreamId { get; private set; }

            public string FilePath { get; private set; }

            public ConnectInfo(string hostName, uint port, string userID, string password, DateTime startTime, uint speed, string selectID, int streamId)
            {
                HostName = hostName;
                UserID = userID;
                Password = password;
                StartTime = startTime;
                Speed = speed;
                Port = port;
                SelectID = selectID;
                StreamId = streamId;
            }

            public ConnectInfo(string filePath, uint speed=0)
            {
                FilePath = filePath;
                Speed = speed;
            }
        }

        #endregion

        // RTSP
        // Events that applications can receive
        public event ReceivedYUVFrameHandler ReceivedYUVFrame;
        // Delegated functions (essentially the function prototype)
        public delegate void ReceivedYUVFrameHandler(string deviceId, uint timestamp, byte[] data, bool isKeyFrame);

        public event ReceiveNvrFrameHandler OnReceiveNvrFrame;
        public delegate void ReceiveNvrFrameHandler(WmfPlayer sender, uint timestamp, Bitmap bitmap);

        private OpenH264Lib.Encoder _h264Encoder { get; set; }
        private Stopwatch _stopwatch { get; set; }

        public bool HasVideo { get; private set; }

        public WmfPlayer()
        {
            m_KMPEGHandle = KMPEG4.KOpenInterface();
             m_DefaultUTCTime = new DateTime(1970, 1, 1);
            _closingEvent = new AutoResetEvent(false);
            
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        /// <summary>
        /// Open the media file.
        /// </summary>
        /// <param name="mediaLocation">Media file location.</param>
        /// 
        public async Task OpenVideoAsync(string HostName, uint Port, string userID, string Password, DateTime StartTime, uint speed, string selectID, int streamId)
        {
            logger.Info("OpenVideo");
            try
            {
                if (m_KMPEGHandle != IntPtr.Zero)
                {
                    // try close before open
                    Task close = Task.Run(() => { CloseKmpeg(); });

                    // TODO: check continueWith works well
                    close.ContinueWith(task =>
                     {
                         if (task.Exception != null)
                         {
                             logger.Error(task.Exception.ToString());
                         }
                     }, TaskContinuationOptions.OnlyOnFaulted);
                    await close;
                }
                this.HasVideo = false;

                _state = PlayerState.StartPending;

                m_DefaultUTCTime = new DateTime(1970, 1, 1);
                this.VideoConnectInfo = new ConnectInfo(HostName, Port, userID, Password, StartTime, speed, selectID, streamId);
                m_KMPEGConnectFail = false;
                await Task.Factory.StartNew(OpenKmpeg, this.VideoConnectInfo);

                if (!m_KMPEGConnectFail)
                {
                    _state = PlayerState.Started;
                    //this.HasVideo = true;
                }
                else
                {
                    _state = PlayerState.Ready;
                    this.HasVideo = false;
                }
                
            }
            catch (Exception excp)
            {
                logger.Error(excp.ToString());
                _state = PlayerState.Ready;
            }
            logger.Info("HasVideo={0};PlayerState={1};KMPEGConnectFail={2};", this.HasVideo, _state.ToString(),
                               m_KMPEGConnectFail);
        }

        public async Task OpenVideoAsync(string filePath)
        {
            logger.Info("OpenVideo");
            try
            {
                if (m_KMPEGHandle != IntPtr.Zero)
                {
                    // try close before open
                    Task close = Task.Run(() => { CloseKmpeg(); });

                    // TODO: check continueWith works well
                    close.ContinueWith(task =>
                    {
                        if (task.Exception != null)
                        {
                            logger.Error(task.Exception.ToString());
                        }
                    }, TaskContinuationOptions.OnlyOnFaulted);
                    await close;
                }
                this.HasVideo = false;

                _state = PlayerState.StartPending;

                m_DefaultUTCTime = new DateTime(1970, 1, 1);
                this.VideoConnectInfo = new ConnectInfo(filePath);
                m_KMPEGConnectFail = false;
                await Task.Factory.StartNew(OpenKmpeg, this.VideoConnectInfo);

                if (!m_KMPEGConnectFail)
                {
                    _state = PlayerState.Started;
                    //this.HasVideo = true;
                }
                else
                {
                    _state = PlayerState.Ready;
                    this.HasVideo = false;
                }

            }
            catch (Exception excp)
            {
                logger.Error(excp.ToString());
                _state = PlayerState.Ready;
            }
            logger.Info("HasVideo={0};PlayerState={1};KMPEGConnectFail={2};", this.HasVideo, _state.ToString(),
                               m_KMPEGConnectFail);
        }

        public bool Play(float speed)
        {
            logger.Info("Play requested.");

            if (_state == PlayerState.Started)
            {
                return ChangeSpeed(speed);
            }

            if (_state != PlayerState.Paused)
            {
                logger.Error("Expected paused state but was actually {0}", _state);
                return false;
            }

            //if ((_mediaSession == null) || (_mediaSource == null))
            //{
            //    _logger.Error("Media sesson or source was null");
            //    return false;
            //}

            try
            {
                _state = PlayerState.StartPending;
                StartPlayback();
                logger.Info("Starting playback");
                _state = PlayerState.Started;
                ChangeSpeed(speed);
                return true;
            }
            catch (Exception excp)
            {
                logger.Error(excp.ToString());
            }

            return false;

        }

        public bool Pause()
        {
            logger.Info("Pause requested");
            if (_state != PlayerState.Started)
            {
                logger.Error("Expected Started state, but was {0}", _state);
            }

            try
            {
                _state = PlayerState.PausePending;
                logger.Info("Pause pending");
                m_URLCommand.Pause();
                KMPEG4.KPause(m_KMPEGHandle);
                _state = PlayerState.Paused;
                return true;
            }
            catch (Exception excp)
            {
                logger.Error(excp.ToString());
            }
            return false;
        }

        public bool Shutdown()
        {
            logger.Info("Shutdown requested");
            try
            {
                if (m_KMPEGHandle != IntPtr.Zero)
                {
                    CloseKmpeg(true);
                }

                if (_closingEvent != null)
                {
                    _closingEvent.Close();
                    _closingEvent = null;
                    return true;
                }
            }
            catch (Exception excp)
            {
                logger.Error(excp.ToString());
            }

            return false;
        }

        private bool ChangeSpeed(float rate)
        {
            try
            {
                if (_state != PlayerState.Started)
                {
                    logger.Warn("Player not running.");
                    return false;
                }
                m_URLCommand.SetPlayMode(rate.ToString());
                return true;
            }
            catch (Exception excp)
            {
                logger.Error(excp.ToString());
            }
            return false;

        }


        private void StartPlayback()
        {
            logger.Info("CPlayer::StartPlayback");
            m_URLCommand.Play();
            KMPEG4.KPlay(m_KMPEGHandle);

            //if (_mediaSession == null)
            //{
            //    _logger.Error("Media session reference missing");
            //    return;
            //}

            //int hr = _mediaSession.Start(Guid.Empty, new PropVariant());
            //MFError.ThrowExceptionForHR(hr);
        }

        #region KMPEG4

        /// <summary>
        /// Get video configuration from raw file.
        /// Will consume a KMPEG connection, don't call this funtion while playing video.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public KMPEG4.Media_Video_Config3 GetVideoConfig(string filePath)
        {
            bool success = false;
            KMPEG4.KEnableDecoder(m_KMPEGHandle, true);
            KMPEG4.Media_Connect_Config4 mediaConfig = new KMPEG4.Media_Connect_Config4();
            mediaConfig.ConnectTimeOut = KMPEG_CONN_TIMEOUT;
            mediaConfig.ContactType = (int)KMPEG4.CONTACT_TYPE.CONTACT_TYPE_PLAYBACK;
            mediaConfig.PlayFileName = filePath;
            success = KMPEG4.KSetMediaConfig4(m_KMPEGHandle, ref mediaConfig);

            var videoConfig = new KMPEG4.Media_Video_Config3();
            if (KMPEG4.KConnect(m_KMPEGHandle))
            {
                success = KMPEG4.KGetVideoConfig3(m_KMPEGHandle, ref videoConfig);
            }
            KMPEG4.KDisconnect(m_KMPEGHandle);
            return videoConfig;
        }

        /// <summary>
        /// Timestamp in seconds.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public uint GetVideoBeginUtcTime(string filePath)
        {
            bool success = false;
            KMPEG4.KEnableDecoder(m_KMPEGHandle, true);
            KMPEG4.Media_Connect_Config4 mediaConfig = new KMPEG4.Media_Connect_Config4();
            mediaConfig.ConnectTimeOut = KMPEG_CONN_TIMEOUT;
            mediaConfig.ContactType = (int)KMPEG4.CONTACT_TYPE.CONTACT_TYPE_PLAYBACK;
            mediaConfig.PlayFileName = filePath;
            success = KMPEG4.KSetMediaConfig4(m_KMPEGHandle, ref mediaConfig);

            uint beginTimeStamp = 0;
            if (KMPEG4.KConnect(m_KMPEGHandle))
            {
                KMPEG4.KGetBeginTimeUTC(m_KMPEGHandle, ref beginTimeStamp);
            }
            KMPEG4.KDisconnect(m_KMPEGHandle);
            return beginTimeStamp;
        }

        /// <summary>
        /// Start connect to video source using Kmpeg library.
        /// </summary>
        /// <param name="info"></param>
        private void OpenKmpeg(object info)
        {
            logger.Trace("OpenKmpeg start");
            ConnectInfo connectInfo = (ConnectInfo)info;
            uint time = 0;
            bool success = false;
            KMPEG4.KEnableDecoder(m_KMPEGHandle, true);
            KMPEG4.Media_Connect_Config4 mediaConfig = new KMPEG4.Media_Connect_Config4();
            mediaConfig.ConnectTimeOut = KMPEG_CONN_TIMEOUT;

            if (!String.IsNullOrEmpty(connectInfo.FilePath))
            {
                mediaConfig.ContactType = (int)KMPEG4.CONTACT_TYPE.CONTACT_TYPE_PLAYBACK;
                mediaConfig.PlayFileName = connectInfo.FilePath;
            }
            else
            {
                time = Convert.ToUInt32((connectInfo.StartTime - new DateTime(1970, 1, 1)).TotalSeconds);
                ConnSession = Guid.NewGuid().ToString();
                mediaConfig.ContactType = (int)KMPEG4.CONTACT_TYPE.CONTACT_TYPE_HTTP_WOC_PREVIEW;
                mediaConfig.HTTPPort = connectInfo.Port;
                mediaConfig.Password = connectInfo.Password;
                mediaConfig.UniCastIP = connectInfo.HostName;
                mediaConfig.UserID = connectInfo.UserID;
                mediaConfig.TCPVideoStreamID = 0;
                mediaConfig.PlayFileName = time > 0 ? $"/Media/SyncPlayback?deviceid={connectInfo.SelectID}&syncsession={ConnSession}" :
                    $"/Media/Streaming?deviceid={connectInfo.SelectID}&streamid={connectInfo.StreamId}";
            }

            logger.Debug($"Prepare play file:{mediaConfig.PlayFileName}");
            success = KMPEG4.KSetMediaConfig4(m_KMPEGHandle, ref mediaConfig);

            SetImageCallback3();
            SetTimeCodeExRecive();

            if (KMPEG4.KConnect(m_KMPEGHandle))
            {
                if (KMPEG4.KStartStreaming(m_KMPEGHandle))
                {
                    KMPEG4.KPlay(m_KMPEGHandle);
                    m_URLCommand = new URLCommand(connectInfo.HostName, connectInfo.Port, connectInfo.UserID, connectInfo.Password, ConnSession);
                    if (time > 0)//streaming playback
                    {
                        m_URLCommand.SetCurrentTime(time.ToString());
                        System.Threading.SpinWait.SpinUntil(() => false, 1000);
                        m_URLCommand.SetPlayMode("1");
                        m_URLCommand.Play();
                    }
                    KMPEG4.KSetVolume(m_KMPEGHandle, 100, 100);
                    KMPEG4.KSetMute(m_KMPEGHandle, false);
                }
                else
                {
                    m_KMPEGConnectFail = true;
                }
            }
            
            logger.Trace("OpenKmpeg done");
        }

        private void CloseKmpeg(bool clearHandle = false)
        {
            logger.Trace("CloseKmpeg start");
            this.InstallImageCallback3 -= new EventHandler<ImageEventArgs>(OnImageCallback3Recive);
            this.InstallTimeCodeExCallback -= new EventHandler<TimeCodeCallbackExEventArgs>(OnTimeCodeRecive);
            if (m_KMPEGHandle == IntPtr.Zero)
            {
                return;
            }
            try
            {
                KMPEG4.KStop(m_KMPEGHandle);
                KMPEG4.KStopStreaming(m_KMPEGHandle);
                KMPEG4.KDisconnect(m_KMPEGHandle);
                if (clearHandle)
                {
                    KMPEG4.KCloseInterface(m_KMPEGHandle);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            if (clearHandle)
            {
                m_KMPEGHandle = IntPtr.Zero;
            }

            _state = PlayerState.Ready;
            logger.Trace("CloseKmpeg done");
        }

        public event EventHandler<FilePlayCompleteEventArgs> InstallFilePlayCompleteCallback
        {
            add
            {
                if (value == null) return;
                if (m_ImageEventHandlers == null)
                {
                    m_FilePlayCompleteEventHandlers = new List<EventHandler<FilePlayCompleteEventArgs>>();
                    m_FilePlayCompleteCallback = new KMPEG4.FilePlayCompleteCallback(OnKmpegFilePlayCompleteCallback);
                    KMPEG4.KSetFilePlayCompleteCallback(m_KMPEGHandle, 0, m_FilePlayCompleteCallback);
                }
                m_FilePlayCompleteEventHandlers.Add(value);
            }

            remove
            {
                if (value == null) return;
                if (m_FilePlayCompleteEventHandlers == null) return;
                if (!m_FilePlayCompleteEventHandlers.Contains(value)) return;
                m_FilePlayCompleteEventHandlers.Remove(value);
                if (m_FilePlayCompleteEventHandlers.Count >= 1) return;
                m_FilePlayCompleteCallback = null;
                m_FilePlayCompleteEventHandlers = null;
                KMPEG4.KSetFilePlayCompleteCallback(m_KMPEGHandle, 0, null);
            }
        }

        /// <summary>
        /// Only for bypass KMPEG event.
        /// </summary>
        /// <param name="userParam"></param>
        public void OnKmpegFilePlayCompleteCallback(uint userParam)
        {
            OnFilePlayComplete(new FilePlayCompleteEventArgs(userParam));
        }

        protected virtual void OnFilePlayComplete(FilePlayCompleteEventArgs e)
        {
            if (m_FilePlayCompleteEventHandlers == null) return;
            var list = m_FilePlayCompleteEventHandlers.ToArray();
            foreach (var i in list)
            {
                if (i == null) continue;
                i(this, e);
            }
        }

        public event EventHandler<ImageEventArgs> InstallImageCallback3
        {
            add
            {
                if (value == null) return;
                if (m_ImageEventHandlers == null)
                {
                    m_ImageEventHandlers = new List<EventHandler<ImageEventArgs>>();
                    m_ImageCallback3 = new KMPEG4.ImageCallback3(OnKMpegImageCallback3);
                    KMPEG4.KSetImageCallback3(m_KMPEGHandle, 0, m_ImageCallback3);
                }
                m_ImageEventHandlers.Add(value);
            }

            remove
            {
                if (value == null) return;
                if (m_ImageEventHandlers == null) return;
                if (!m_ImageEventHandlers.Contains(value)) return;
                m_ImageEventHandlers.Remove(value);
                if (m_ImageEventHandlers.Count >= 1) return;
                m_ImageCallback3 = null;
                m_ImageEventHandlers = null;
                KMPEG4.KSetImageCallback3(m_KMPEGHandle, 0, null);
            }
        }

        /// <summary>
        /// Only for bypass KMPEG event.
        /// </summary>
        /// <param name="userparam"></param>
        /// <param name="pHeader"></param>
        /// <param name="bmpinfo"></param>
        /// <param name="buf"></param>
        /// <param name="len"></param>
        /// <param name="dwwidth"></param>
        /// <param name="dwheight"></param>
        private void OnKMpegImageCallback3(uint userparam, IntPtr pHeader, IntPtr bmpinfo, IntPtr buf, uint len,
            uint dwwidth, uint dwheight)
        {
            OnImageCallback3(new ImageEventArgs(pHeader, bmpinfo, buf, len, dwwidth, dwheight));
        }

        protected virtual void OnImageCallback3(ImageEventArgs e)
        {
            if (m_ImageEventHandlers == null) return;
            var list = m_ImageEventHandlers.ToArray();
            foreach (var i in list)
            {
                if (i == null) continue;
                i(this, e);
            }
        }

        public event EventHandler<TimeCodeCallbackExEventArgs> InstallTimeCodeExCallback
        {
            add
            {
                if (value == null) return;
                if (m_TimeCodeExHandlers == null)
                {
                    m_TimeCodeExHandlers = new List<EventHandler<TimeCodeCallbackExEventArgs>>();
                    m_TimeCodeCallbackex = new KMPEG4.TimeCodeCallbackEx(OnKMpegTimeCodeExCallback);
                    KMPEG4.KSetTimeCodeCallbackEx(m_KMPEGHandle, 0, m_TimeCodeCallbackex);
                }
                m_TimeCodeExHandlers.Add(value);
            }

            remove
            {
                if (value == null) return;
                if (m_TimeCodeExHandlers == null) return;
                if (!m_TimeCodeExHandlers.Contains(value)) return;
                m_TimeCodeExHandlers.Remove(value);
                if (m_TimeCodeExHandlers.Count < 1)
                {
                    m_TimeCodeCallbackex = null;
                    m_TimeCodeExHandlers = null;
                    KMPEG4.KSetTimeCodeCallbackEx(m_KMPEGHandle, 0, null);
                }
            }
        }

        /// <summary>
        /// Only for bypass KMPEG event.
        /// </summary>
        /// <param name="userparam"></param>
        /// <param name="time_val"></param>
        private void OnKMpegTimeCodeExCallback(uint userparam, KMPEG4.TimeVal time_val)
        {
            OnTimeCodeReceive(new TimeCodeCallbackExEventArgs(userparam, time_val));
        }

        protected virtual void OnTimeCodeReceive(TimeCodeCallbackExEventArgs e)
        {
            var list = m_TimeCodeExHandlers.ToArray();
            foreach (var i in list)
            {
                if (i == null) continue;
                i(this, e);
            }
        }

        #endregion

        /// <summary>
        /// Register for my custom image handle logic.
        /// </summary>
        private void SetImageCallback3()
        {
            this.InstallImageCallback3 -= new EventHandler<ImageEventArgs>(OnImageCallback3Recive);
            this.InstallImageCallback3 += new EventHandler<ImageEventArgs>(OnImageCallback3Recive);
        }

        /// <summary>
        /// Extract bitmap and pass to OnReceiveNvrFrame event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnImageCallback3Recive(object sender, ImageEventArgs e)
        {
            try
            {
                logger.Debug($"device: {VideoConnectInfo.SelectID} OnImageCallback3Recive");
                System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;// .Format32bppRgb; .Format4bppIndexed
                int bitsPerPixel = ((int)format & 0xff00) >> 8;
                int bytesPerPixel = (bitsPerPixel + 7) / 8;
                int stride = ((Convert.ToInt16(e.dwWidth) * bytesPerPixel + 3) / 4) * 4;
                Bitmap bitmap = new Bitmap(Convert.ToInt32(e.dwWidth), Convert.ToInt32(System.Math.Abs(e.dwHeight)), stride, format, e.Buffer);
                if (OnReceiveNvrFrame != null)
                {
                    OnReceiveNvrFrame(this, (uint)_stopwatch.ElapsedMilliseconds, bitmap);
                }

            }
            catch (Exception ex)
            {
                this.HasVideo = false;
                logger.Error(ex.ToString());
            }
        }

        /// <summary>
        /// Register for my custom timecode handle logic.
        /// Unnessary for now.
        /// </summary>
        private void SetTimeCodeExRecive()
        {
            this.InstallTimeCodeExCallback -= OnTimeCodeRecive;
            this.InstallTimeCodeExCallback += OnTimeCodeRecive;
        }

        /// <summary>
        /// Unnessary block for now.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimeCodeRecive(object sender, TimeCodeCallbackExEventArgs e)
        {
            m_ViewUTCTime = m_DefaultUTCTime.AddSeconds(e.TimeVal.tv_sec).AddMilliseconds(e.TimeVal.tv_usec / 1000);
            //logger.Trace($"{m_ViewUTCTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
        }

        /// <summary>
        /// Register for my custom timecode handle logic.
        /// Unnessary for now.
        /// </summary>
        private void SetFilePlaybackCompleteCallback()
        {
            this.InstallFilePlayCompleteCallback -= OnFilePlayComplete;
            this.InstallFilePlayCompleteCallback += OnFilePlayComplete;
        }

        /// <summary>
        /// Unnessary block for now.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFilePlayComplete(object sender, FilePlayCompleteEventArgs e)
        {
            
        }

        /// <summary>
        /// 按比例縮放, 提供指定的縮放倍率.
        /// </summary>
        /// <param name="originImage"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public static Bitmap Resize(Bitmap originImage, Double times)
        {
            int width = Convert.ToInt32(originImage.Width * times);
            int height = Convert.ToInt32(originImage.Height * times);

            return ResizeBitmap(originImage, originImage.Width, originImage.Height, width, height);
        }

        /// <summary>
        /// 縮放成指定的寬與高.
        /// </summary>
        /// <param name="originImage"></param>
        /// <param name="oriwidth"></param>
        /// <param name="oriheight"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap ResizeBitmap(Bitmap originImage, int oriwidth, int oriheight, int width, int height)
        {
            Bitmap resizedbitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resizedbitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.Clear(Color.Transparent);
                g.DrawImage(originImage, new Rectangle(0, 0, width, height), new Rectangle(0, 0, oriwidth, oriheight), GraphicsUnit.Pixel);
            }
            return resizedbitmap;
        }
    }
}

