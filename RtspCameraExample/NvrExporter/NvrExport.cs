using Accord.Video.FFMPEG;
using ACTi.NVR3.OemDvrMiniDriver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtspCameraExample.NvrExporter
{
    public class NvrExport
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        //  Download raw files through NVR3 ocx
        //  Download multiple raw files by split datetime
        //v instance wmfplayer
        //v get nvr frame
        //v send frame to file writer

        // next:
        //v Create multiple wmfplayer and following steps based on time range
        //o combine multiple video files into one (ffmpeg.exe -f concat -i list.txt -c copy output.avi)

        private static readonly DateTime DefaultUTCTime = new DateTime(1970, 1, 1);

        private string _nvrIp = ConfigurationManager.AppSettings["NvrIp"];
        private string _nvrPort = ConfigurationManager.AppSettings["NvrPort"];

        private string _nvrAccount = ConfigurationManager.AppSettings["NvrAccount"];
        private string _nvrPwd = ConfigurationManager.AppSettings["NvrPwd"];
        private WmfPlayer _wmfPlayer { get; set; }
        private VideoFileWriter _videoWriter { get; set; }
        private DateTime _startUtcTime { get; set; }
        private DateTime _endUtcTime { get; set; }

        int _width = 640;
        int _height = 480;
        bool _isEndTimeArrived = false;

        public bool IsExportFinish { get; set; }

        public FileInfo OutputFileInfo { get; set; }

        public NvrExport()
        {
            IsExportFinish = false;
        }

        public async void TestExportAsync(string filePath)
        {
            FileInfo f = new FileInfo(filePath);
            _wmfPlayer = new WmfPlayer();
            uint beginTimeStemp = _wmfPlayer.GetVideoBeginUtcTime(filePath);
            _startUtcTime = DefaultUTCTime.AddSeconds(beginTimeStemp);

            var videoConfig = _wmfPlayer.GetVideoConfig(filePath);
            _width = (int)videoConfig.Width;
            _height = (int)videoConfig.Height;

            _wmfPlayer.OnReceiveNvrFrame += WmfPlayer_OnReceiveNvrFrame;
            _wmfPlayer.InstallFilePlayCompleteCallback += _wmfPlayer_InstallFilePlayCompleteCallback;
            //_wmfPlayer.InstallRawDataCallback += _wmfPlayer_InstallRawDataCallback;
            await _wmfPlayer.OpenVideoAsync(filePath);

            _videoWriter = new VideoFileWriter();
            OutputFileInfo = new FileInfo($@"D:\tmp\NvrExporter\{f.Name}_1.avi");
            _videoWriter.Open(OutputFileInfo.FullName, _width, _height, videoConfig.Fps, VideoCodec.MPEG4, 5000000);
        }

        public async void TestWebExportAsync(DateTime startTimeUtc, DateTime endTimeUtc)
        {
            _endUtcTime = endTimeUtc;
            string deviceId = "14";
            int streamId = 1;
            _wmfPlayer = new WmfPlayer();

            _width = 1920;
            _height = 1080;

            _wmfPlayer.OnReceiveNvrFrame += WmfPlayer_OnReceiveNvrFrame;
            _wmfPlayer.InstallTimeCodeExCallback += _wmfPlayer_InstallTimeCodeExCallback;
            _wmfPlayer.InstallFilePlayCompleteCallback += _wmfPlayer_InstallFilePlayCompleteCallback;
            //_wmfPlayer.InstallRawDataCallback += _wmfPlayer_InstallRawDataCallback;
            await _wmfPlayer.OpenVideoAsync(_nvrIp, uint.Parse(_nvrPort), _nvrAccount, _nvrPwd, startTimeUtc, 1, deviceId, streamId);

            _videoWriter = new VideoFileWriter();
            OutputFileInfo = new FileInfo($@"D:\tmp\NvrExporter\{Guid.NewGuid().ToString()}_1.avi");
            _videoWriter.Open(OutputFileInfo.FullName, _width, _height, 30, VideoCodec.MPEG4, 5000000);
        }

        private void _wmfPlayer_InstallTimeCodeExCallback(object sender, KMPEG4.TimeCodeCallbackExEventArgs e)
        {
            var time = DefaultUTCTime.AddSeconds(e.TimeVal.tv_sec).AddMilliseconds(e.TimeVal.tv_usec / 1000);
            _logger.Trace($"{time.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
            if((_endUtcTime - time).TotalMilliseconds <= 0)
            {
                _wmfPlayer.Shutdown();
                _videoWriter.Close();
                _videoWriter.Dispose();
                IsExportFinish = true;
                _logger.Debug($"Endtime");
            }
        }

        private void _wmfPlayer_InstallRawDataCallback(object sender, KMPEG4.RawDataEventArgs e)
        {
            _logger.Trace($"{e.DataType}, {e.Len}");
        }

        private void _wmfPlayer_InstallFilePlayCompleteCallback(object sender, KMPEG4.FilePlayCompleteEventArgs e)
        {
            _videoWriter.Close();
            _videoWriter.Dispose();
            IsExportFinish = true;
            _logger.Info($"Export finish");
        }

        private void WmfPlayer_OnReceiveNvrFrame(WmfPlayer sender, uint timestamp, System.Drawing.Bitmap bitmap)
        {
            var resizedBitmap = WmfPlayer.ResizeBitmap(bitmap, bitmap.Width, bitmap.Height, _width, _height);
            try
            {
                _videoWriter.WriteVideoFrame(resizedBitmap);

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            bitmap.Dispose();
            resizedBitmap.Dispose();
        }
    }
}
