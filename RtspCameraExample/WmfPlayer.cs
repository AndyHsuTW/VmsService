using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

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
    /// </summary>
    public class WmfPlayer
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        
        private const int MfVersion = 0x10070;
        private readonly IntPtr _handle;
        private AutoResetEvent _closingEvent;

        private PlayerState _state;

        private URLCommand m_URLCommand;
        #region KMPEG4 Member

        private bool m_KMPEGConnectFail;
        private IntPtr m_KMPEGHandle = IntPtr.Zero;
        private DateTime m_ViewUTCTime;
        private DateTime m_DefaultUTCTime;

        private List<EventHandler<ImageEventArgs>> m_ImageEventHandlers;
        private KMPEG4.ImageCallback3 m_ImageCallback3;
        private List<EventHandler<TimeCodeCallbackExEventArgs>> m_TimeCodeExHandlers;
        private KMPEG4.TimeCodeCallbackEx m_TimeCodeCallbackex;

        public struct Timeval
        {
            public UInt32 tv_sec;
            public UInt32 tv_usec;
        }

        public class TimeCodeCallbackExEventArgs : EventArgs
        {
            public uint UserParam { get; private set; }
            public Timeval TimeVal { get; private set; }
            public TimeCodeCallbackExEventArgs(uint userParam, Timeval time_val)
            {
                UserParam = userParam;
                TimeVal = time_val;
            }
        }

        public class ImageEventArgs : EventArgs
        {
            public IntPtr B2Header;
            public IntPtr Bmpinfo;
            public IntPtr Buffer;
            public uint Len;
            public uint dwWidth;
            public uint dwHeight;

            public ImageEventArgs(IntPtr b2, IntPtr bmpinfo, IntPtr buf, uint len, uint dwwidth, uint dwheight)
            {
                B2Header = b2;
                Bmpinfo = bmpinfo;
                Buffer = buf;
                Len = len;
                dwWidth = dwwidth;
                dwHeight = dwheight;
            }
        }

        public class ConnectInfo
        {
            public string HostName { get; private set; }
            public string UserID { get; private set; }
            public string Password { get; private set; }
            public DateTime StartTime { get; private set; }
            public uint Speed { get; private set; }
            public string SelectID { get; private set; }
            public uint Port { get; private set; }

            public int StreamId { get; private set; }

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
        }

        #endregion

        // RTSP
        // Events that applications can receive
        public event ReceivedYUVFrameHandler ReceivedYUVFrame;
        // Delegated functions (essentially the function prototype)
        public delegate void ReceivedYUVFrameHandler(string deviceId, uint timestamp, byte[] data, bool isKeyFrame);
        private OpenH264Lib.Encoder _h264Encoder { get; set; }
        private Stopwatch _stopwatch { get; set; }

        public bool HasVideo { get; private set; }

        public WmfPlayer(IntPtr hWindowAndEvents)
        {
            m_DefaultUTCTime = new DateTime(1970, 1, 1);
            _handle = hWindowAndEvents;

            _closingEvent = new AutoResetEvent(false);
            
            _stopwatch = new Stopwatch();
            _stopwatch.Start();

        }

        /// <summary>
        /// Open the media file.
        /// </summary>
        /// <param name="mediaLocation">Media file location.</param>
        /// 

        public string m_HostName;
        public uint m_Port;
        public string m_UserID;
        public string m_Password;
        public DateTime m_StartTime;
        public uint m_Speed;
        public string m_SelectID;
        public bool m_ReciveInfo;

        public void SetInfo(string HostName, uint Port, string userID, string Password, DateTime StartTime, uint speed, int width, int height, string selectID)
        {
            m_HostName = HostName;
            m_Port = Port;
            m_UserID = userID;
            m_Password = Password;
            m_StartTime = StartTime;
            m_Speed = speed;
            m_SelectID = selectID;
            m_ReciveInfo = true;
        }

        public async Task OpenVideoAsync(string HostName, uint Port, string userID, string Password, DateTime StartTime, uint speed, string selectID, int streamId)
        {
            logger.Info("OpenVideo");
            try
            {
                if (m_KMPEGHandle != IntPtr.Zero)
                {
                    // try close before open
                    Task close = Task.Run(CloseKmpeg);
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

                ConnectInfo info = new ConnectInfo(HostName, Port, userID, Password, StartTime, speed, selectID,streamId);
                m_KMPEGConnectFail = false;
                await Task.Factory.StartNew(OpenKmpeg, info);

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
                    CloseKmpeg();
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

        public void TestK()
        {
            m_KMPEGHandle = KMPEG4.KOpenInterface();
        }

        #region KMPEG4

        private void OpenKmpeg(object info)
        {
            logger.Trace("OpenKmpeg start");
            ConnectInfo connectInfo = (ConnectInfo)info;
            uint time = Convert.ToUInt32((connectInfo.StartTime - new DateTime(1970, 1, 1)).TotalSeconds);

            bool success = false;
            m_KMPEGHandle = KMPEG4.KOpenInterface();
            KMPEG4.Media_Connect_Config4 mediaConfig = new KMPEG4.Media_Connect_Config4();

            mediaConfig.ConnectTimeOut = 2;
            mediaConfig.ContactType = (int)KMPEG4.CONTACT_TYPE.CONTACT_TYPE_HTTP_WOC_PREVIEW;
            mediaConfig.HTTPPort = connectInfo.Port;
            mediaConfig.Password = connectInfo.Password;
            mediaConfig.UniCastIP = connectInfo.HostName;
            mediaConfig.UserID = connectInfo.UserID;
            mediaConfig.TCPVideoStreamID = 0;
            string session = "1234" + m_SelectID;
            mediaConfig.PlayFileName = time > 0 ? $"/Media/SyncPlayback?deviceid={connectInfo.SelectID}&syncsession={session}" :
                $"/Media/Streaming?deviceid={connectInfo.SelectID}&streamid={connectInfo.StreamId}";
            logger.Debug($"Prepare play file:{mediaConfig.PlayFileName}");

            success = KMPEG4.KSetMediaConfig4(m_KMPEGHandle, ref mediaConfig);

            SetImageCallback3();
            SetTimeCodeExRecive();

            if (KMPEG4.KConnect(m_KMPEGHandle))
            {
                if (KMPEG4.KStartStreaming(m_KMPEGHandle))
                {
                    KMPEG4.KPlay(m_KMPEGHandle);

                    m_URLCommand = new URLCommand(connectInfo.HostName, connectInfo.Port, connectInfo.UserID, connectInfo.Password, session);
                    if (time > 0)
                    {
                        m_URLCommand.SetCurrentTime(time.ToString());
                        System.Threading.SpinWait.SpinUntil(() => false, 1000);
                        m_URLCommand.SetPlayMode("1");
                        m_URLCommand.Play();
                    }
                    else
                    {
                    }

                    KMPEG4.KSetVolume(m_KMPEGHandle, 100, 100);
                    KMPEG4.KSetMute(m_KMPEGHandle, false);
                }
                else
                {
                    m_KMPEGConnectFail = true;
                }
            }
            else
            {
                m_KMPEGConnectFail = true;
            }
            logger.Trace("OpenKmpeg done");
        }

        private void CloseKmpeg()
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
                KMPEG4.KCloseInterface(m_KMPEGHandle);
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            m_KMPEGHandle = IntPtr.Zero;

            _state = PlayerState.Ready;
            logger.Trace("CloseKmpeg done");
        }

        private void SetImageCallback3()
        {
            this.InstallImageCallback3 -= new EventHandler<ImageEventArgs>(OnImageCallback3Recive);
            this.InstallImageCallback3 += new EventHandler<ImageEventArgs>(OnImageCallback3Recive);
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

        private void SetTimeCodeExRecive()
        {
            this.InstallTimeCodeExCallback -= new EventHandler<TimeCodeCallbackExEventArgs>(OnTimeCodeRecive);
            this.InstallTimeCodeExCallback += new EventHandler<TimeCodeCallbackExEventArgs>(OnTimeCodeRecive);
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
        private void OnKMpegTimeCodeExCallback(uint userparam, KMPEG4.TimeVal time_val)
        {
            Timeval Time_Val = new Timeval();
            Time_Val.tv_sec = (uint)time_val.tv_sec;
            Time_Val.tv_usec = (uint)time_val.tv_usec;

            OnKMpegTimeCodeExReceive(new TimeCodeCallbackExEventArgs(userparam, Time_Val));
        }
        protected virtual void OnKMpegTimeCodeExReceive(TimeCodeCallbackExEventArgs e)
        {
            var list = m_TimeCodeExHandlers.ToArray();
            foreach (var i in list)
            {
                if (i == null) continue;
                i(this, e);
            }
        }
        public static Bitmap Resize(Bitmap originImage, Double times)
        {
            int width = Convert.ToInt32(originImage.Width * times);
            int height = Convert.ToInt32(originImage.Height * times);

            return Process(originImage, originImage.Width, originImage.Height, width, height);
        }

        private static Bitmap Process(Bitmap originImage, int oriwidth, int oriheight, int width, int height)
        {
            Bitmap resizedbitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(resizedbitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(Color.Transparent);
            g.DrawImage(originImage, new Rectangle(0, 0, width, height), new Rectangle(0, 0, oriwidth, oriheight), GraphicsUnit.Pixel);
            return resizedbitmap;
        }
        private void OnImageCallback3Recive(object sender, ImageEventArgs e)
        {
            try
            {
                logger.Debug($"device: {m_SelectID} OnImageCallback3Recive");
                System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;// .Format32bppRgb; .Format4bppIndexed
                int bitsPerPixel = ((int)format & 0xff00) >> 8;
                int bytesPerPixel = (bitsPerPixel + 7) / 8;
                int stride = ((Convert.ToInt16(e.dwWidth) * bytesPerPixel + 3) / 4) * 4;

                using (Bitmap oBitmap = new Bitmap(Convert.ToInt32(e.dwWidth), Convert.ToInt32(System.Math.Abs(e.dwHeight)), stride, format, e.Buffer))//.Format32bppRgb  
                {
                    if (_h264Encoder == null)
                    {
                        OpenH264Lib.Encoder.OnEncodeCallback onEncode = (data, length, frameType) =>
                        {
                            var keyFrame = (frameType == OpenH264Lib.Encoder.FrameType.IDR) || (frameType == OpenH264Lib.Encoder.FrameType.I);
                            //writer.AddImage(data, keyFrame);
                            if (ReceivedYUVFrame != null)
                            {
                                if (data == null)
                                {
                                    logger.Warn($"onEncode get null data");
                                }
                                try
                                {
                                    ReceivedYUVFrame(m_SelectID, (uint)_stopwatch.ElapsedMilliseconds, data, keyFrame);
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex.ToString());
                                }
                            }
                        };

                        _h264Encoder = new OpenH264Lib.Encoder(@"lib\openh264-1.8.0-win32.dll");
                        _h264Encoder.Setup(640, 480, 5000000, 10, 2.0f, onEncode);
                    }
                    var resizedBitmap = Process(oBitmap, oBitmap.Width, oBitmap.Height, 640, 480);
                    this.HasVideo = true;
                    _h264Encoder.Encode(resizedBitmap, _stopwatch.ElapsedMilliseconds);
                }
            }
            catch(Exception ex)
            {
                this.HasVideo = false;
                logger.Error(ex.ToString());
            }
        }

        private void OnTimeCodeRecive(object sender, TimeCodeCallbackExEventArgs e)
        {
            m_ViewUTCTime = m_DefaultUTCTime.AddSeconds(e.TimeVal.tv_sec).AddMilliseconds(e.TimeVal.tv_usec / 1000);
        }

        #endregion
    }
}

