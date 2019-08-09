using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RtspCameraExample
{
    class TestJpegH264
    {
        // Events that applications can receive
        public event ReceivedYUVFrameHandler ReceivedYUVFrame;

        // Delegated functions (essentially the function prototype)
        public delegate void ReceivedYUVFrameHandler(uint timestamp, byte[] data, bool isKeyFrame);
        private Stopwatch _stopwatch { get; set; }

        private OpenH264Lib.Encoder _h264Encoder { get; set; }

        public void Start()
        {

            _h264Encoder = new OpenH264Lib.Encoder(@"lib\openh264-1.8.0-win32.dll");

            OpenH264Lib.Encoder.OnEncodeCallback onEncode = (data, length, frameType) =>
            {
                var keyFrame = (frameType == OpenH264Lib.Encoder.FrameType.IDR) || (frameType == OpenH264Lib.Encoder.FrameType.I);
                //writer.AddImage(data, keyFrame);
                if (ReceivedYUVFrame != null)
                {
                    ReceivedYUVFrame((uint)_stopwatch.ElapsedMilliseconds, data, keyFrame);
                }
                Console.WriteLine("Encord {0} bytes, KeyFrame:{1}", length, keyFrame);
            };

            _h264Encoder.Setup(960, 480, 5000000, 15, 2.0f, onEncode);

            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            Task.Run(() =>
            {
                while (true)
                {
                    Bitmap bitmap = ConvertToBitmap(@"C:\Users\hamow\OneDrive\Pictures\aa.jpg");
                    _h264Encoder.Encode(bitmap, _stopwatch.ElapsedMilliseconds);
                    SpinWait.SpinUntil(() => false, 50);
                }
                
            });
        }
        public Bitmap ConvertToBitmap(string fileName)
        {
            Bitmap bitmap;
            using (Stream bmpStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);

                bitmap = new Bitmap(image);

            }
            return bitmap;
        }
    }
}
