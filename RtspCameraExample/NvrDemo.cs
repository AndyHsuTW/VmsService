using RtspCameraExample.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RtspCameraExample
{
    class NvrDemo
    {
        /// <summary>
        /// Default timeout milliseconds.
        /// </summary>
        public const int ConnectionTimeout = 5000;

        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static HttpClient _httpClient { get; set; }
        private int _deviceId { get; set; }
        private string _syncSession { get; set; }
        public bool IsStarted { get; set; }
        /// <summary>
        /// NVR URL. IP or domain name.
        /// </summary>
        public string Url { get; set; }

        // Events that applications can receive
        public event ReceivedYUVFrameHandler ReceivedYUVFrame;

        // Delegated functions (essentially the function prototype)
        public delegate void ReceivedYUVFrameHandler(string deviceId, uint timestamp_ms, byte[] yuv_data, bool isKeyFrame);


        public enum PlayModeAction
        {
            Forward,
            Back
        }
        private Stopwatch _stopwatch { get; set; }

        public NvrDemo(string baseUrl, string user, string password, int deviceId, string syncSession)
        {
            this.Url = baseUrl;
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(this.Url)
            };
            var authBytes = Encoding.ASCII.GetBytes(String.Format($"{user}:{password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
            _deviceId = deviceId;
            _syncSession = syncSession;
            IsStarted = false;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        // mime connection
        // check contect type, call ReceivedYUVFrame if got video/h264
        public async Task PlaybackAsync()
        {
            var maxLength = 0;
            var url = String.Format($"Media/SyncPlayback?deviceid={_deviceId}&syncsession={_syncSession}");
            //_httpClient.GetStreamAsync
            var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var reader = new BinaryReader(stream))
            {
                bool readEnd = false;
                do
                {
                    var currentLine =  reader.ReadLine();
                    var binaryLength = 0;
                    _logger.Trace(currentLine);
                    if (currentLine.Contains("Content-Type"))
                    {
                    }
                    else if (currentLine.Contains("Content-Length"))
                    {
                        if (!IsStarted)
                        {
                            IsStarted = true;
                        }
                        
                        var contentLength = currentLine.Split(':');
                        if (contentLength.Length == 2)
                        {
                            try
                            {
                                binaryLength = int.Parse(contentLength[1]);
                            }
                            catch (FormatException ex)
                            {
                                _logger.Warn(ex.ToString());
                            }
                            _logger.Trace($"Parsed content length : {binaryLength}");
                        }
                        currentLine = reader.ReadLine();// empty line after length
                        _logger.Trace($"{currentLine} <- new line after content length");
                    }
                    if (binaryLength > 0)
                    {
                        maxLength = Math.Max(maxLength, binaryLength);
                        byte[] videoBinary = null;
                        var readIndex = 0;
                        do
                        {
                            videoBinary = reader.ReadBytes(binaryLength);
                            var count = videoBinary.Length;
                            readIndex += count;
                            if(count != binaryLength)
                            {
                                _logger.Warn("Read count not equal");
                            }
                        } while (readIndex < binaryLength);
                        binaryLength = 0;
                        if (ReceivedYUVFrame != null)
                        {
                            ReceivedYUVFrame("3", (uint)_stopwatch.ElapsedMilliseconds, videoBinary, binaryLength > 50);
                        }
                        _logger.Debug($"Max size:{maxLength}, read count:{ readIndex}");
                    }
                } while (!readEnd);
                //while (!reader.EndOfStream)
                //{
                //    //We are ready to read the stream
                //    var currentLine = await reader.ReadLineAsync();
                //    var binaryLength = 0;
                //    Console.WriteLine($"Length:{currentLine.Length}");
                //    if (currentLine.Contains("Content-Type"))
                //    {
                //        Console.WriteLine($"Content:{currentLine}");
                //    }
                //    else if (currentLine.Contains("Content-Length"))
                //    {
                //        if (!IsStarted)
                //        {
                //            IsStarted = true;
                //        }
                //        Console.WriteLine($"Content:{currentLine}");
                //        var contentLength = currentLine.Split(':');
                //        if (contentLength.Length == 2)
                //        {
                //            try
                //            {
                //                binaryLength = int.Parse(contentLength[1]);
                //            }
                //            catch (FormatException ex)
                //            {
                //                Console.WriteLine(ex.ToString());
                //            }
                //            Console.WriteLine($"Parsed content length : {binaryLength}");
                //        }
                //        currentLine = await reader.ReadLineAsync();// empty line after length
                //    }
                //    if (binaryLength > 0)
                //    {
                //        maxLength = Math.Max(maxLength, binaryLength);
                //        var videoB = new char[binaryLength];
                //        var videoBinary = new byte[binaryLength];
                //        var readIndex = 0;
                //        do
                //        {
                //            var count = await reader.BaseStream.ReadAsync(videoBinary, readIndex, binaryLength- readIndex);
                //            readIndex += count;
                //        } while (readIndex != binaryLength);
                        
                //        var count2 = await reader.ReadAsync(videoB, 0, binaryLength);

                //        binaryLength = 0;
                //        if (ReceivedYUVFrame != null)
                //        {
                //            ReceivedYUVFrame((uint)_stopwatch.ElapsedMilliseconds, videoBinary);
                //        }
                //        Console.WriteLine($"Max size:{maxLength}, read count:{ readIndex} / { count2}");
                //    }
                //}
            }
        }

        public async Task LivePlayAsync()
        {
            var maxLength = 0;
            var url = String.Format($"/Media/Streaming?deviceid={_deviceId} ");
            //_httpClient.GetStreamAsync
            var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            {

                while (!reader.EndOfStream)
                {
                    //We are ready to read the stream
                    var currentLine = await reader.ReadLineAsync();
                    var binaryLength = 0;
                    Console.WriteLine($"Length:{currentLine.Length}");
                    if (currentLine.Contains("Content-Type"))
                    {
                        Console.WriteLine($"Content:{currentLine}");
                    }
                    else if (currentLine.Contains("Content-Length"))
                    {
                        if (!IsStarted)
                        {
                            IsStarted = true;
                        }
                        Console.WriteLine($"Content:{currentLine}");
                        var contentLength = currentLine.Split(':');
                        if (contentLength.Length == 2)
                        {
                            try
                            {
                                binaryLength = int.Parse(contentLength[1]);
                            }
                            catch (FormatException ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                            Console.WriteLine($"Parsed content length : {binaryLength}");
                        }
                        currentLine = await reader.ReadLineAsync();// empty line after length
                    }
                    if (binaryLength > 0)
                    {
                        maxLength = Math.Max(maxLength, binaryLength);
                        var videoB = new char[binaryLength];
                        var videoBinary = new byte[binaryLength];
                        var readIndex = 0;
                        do
                        {
                            var count = await reader.BaseStream.ReadAsync(videoBinary, readIndex, binaryLength - readIndex);
                            readIndex += count;
                        } while (readIndex != binaryLength);

                        var count2 = await reader.ReadAsync(videoB, 0, binaryLength);

                        binaryLength = 0;
                        if (ReceivedYUVFrame != null)
                        {
                            ReceivedYUVFrame("3", (uint)_stopwatch.ElapsedMilliseconds, videoBinary, binaryLength > 50);
                        }
                        Console.WriteLine($"Max size:{maxLength}, read count:{ readIndex} / { count2}");
                    }
                }
            }
        }


        // command client : set current time
        public async Task SetCurrentTimeAsync(DateTime utcTime)
        {
            Int32 unixTimestamp = (Int32)(utcTime - new DateTime(1970, 1, 1)).TotalSeconds;
            var url = String.Format($"Media/SyncPlayback/setcurrenttime?syncsession={_syncSession}&currenttime={unixTimestamp}000");
            //_httpClient.GetStreamAsync
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                //do nothing
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }
        }
        // command client : set play mode
        public async Task SetPlayModeAsync(PlayModeAction action, float playRate)
        {
            var actionString = "F";//default forward
            switch (action)
            {
                case PlayModeAction.Back:
                    actionString = "B";
                    break;
                default:
                    actionString = "F";
                    break;
            }
            var playRateString = playRate.ToString("0.0");

            var url = String.Format($"Media/SyncPlayback/SetPlayMode?syncsession={_syncSession}&action={actionString}&playrate={playRateString}");
            //_httpClient.GetStreamAsync
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                //do nothing
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }
        }
        // command client : start play
        public async Task StartPlayAsync()
        {
            var url = String.Format($"Media/SyncPlayback/Play?syncsession={_syncSession}");
            //_httpClient.GetStreamAsync
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                //do nothing
            }
            else
            {
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
