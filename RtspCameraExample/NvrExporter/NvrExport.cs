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
        
        int _width = 640;
        int _height = 480;
        bool _isEndTimeArrived = false;

        public bool IsExportFinish { get; set; }

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
            await _wmfPlayer.OpenVideoAsync(filePath);
            
            _videoWriter = new VideoFileWriter();
            _videoWriter.Open($@"D:\tmp\NvrExporter\{f.Name}_1.avi", _width, _height, videoConfig.Fps, VideoCodec.MPEG4, 5000000);
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
            var datetime = _startUtcTime.AddMilliseconds(timestamp);
            _logger.Trace($"Frame time:{datetime.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
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
