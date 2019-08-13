using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenH264Lib.Encoder;

namespace RtspCameraExample
{
    /// <summary>
    /// Helper class to deal with OpenH264 OnEncodeCallback didn't provide sender.
    /// </summary>
    class EncoderHelper
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public OpenH264Lib.Encoder OpenH264Encoder { get; set; }
        /// <summary>
        /// Id for user to implement instance mapping logic.
        /// </summary>
        public string Id { get; set; }
        public int Width = 640;
        public int Height = 480;

        #region Resend callback with sender instance

        public delegate void OnEncodeCallback(EncoderHelper sender, uint timestamp, byte[] frameData, bool isKeyFrame );
        private OnEncodeCallback _onEncodeCallback { get; set; }
        /// <summary>
        /// Stopwatch for video timestamp
        /// </summary>
        private Stopwatch _stopwatch { get; set; }

        #endregion


        int _bps = 5000000;
        float _fps = 10;
        float _keyFrameInterval = 2.0f;

        /// <summary>
        /// Instance OpenH264Encoder and register callback for encoded frames.
        /// </summary>
        public EncoderHelper(string id, OnEncodeCallback callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            this.Id = id;
            _onEncodeCallback = callback;
            this.OpenH264Encoder = new OpenH264Lib.Encoder(@"lib\openh264-1.8.0-win32.dll");
            this.OpenH264Encoder.Setup(Width, Height, _bps, _fps, _keyFrameInterval, onEncodeCallback);

            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        private void onEncodeCallback(byte[] data, int length, FrameType frameType)
        {
            var isKeyFrame = (frameType == OpenH264Lib.Encoder.FrameType.IDR) || (frameType == OpenH264Lib.Encoder.FrameType.I);
            //writer.AddImage(data, keyFrame);
            try
            {
                _onEncodeCallback(this, (uint)_stopwatch.ElapsedMilliseconds, data, isKeyFrame);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }
    }
}
