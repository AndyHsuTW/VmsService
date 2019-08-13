using RtspCameraExample.NvrExporter;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace RtspCameraExample
{
    class Program
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            NvrExport export = new NvrExport();
            var filePath1 = @"D:\tmp\NvrExporter\Channel07_20190812172500_UTC+0800.raw";
            export.TestExportAsync(filePath1);

            //NvrExport export2 = new NvrExport();
            //var filePath2 = @"D:\tmp\NvrExporter\Channel07_20190812172600_UTC+0800.raw";
            //export2.TestExportAsync(filePath2);

            //SpinWait.SpinUntil(()=>
            //{
            //    return export.IsExportFinish && export2.IsExportFinish;
            //});
            stopwatch.Stop();
            _logger.Info($"Export done using {stopwatch.ElapsedMilliseconds} milliseconds");
            int port = 8554;
			string username = ConfigurationManager.AppSettings["NvrAccount"];      // or use NUL if there is no username
			string password = ConfigurationManager.AppSettings["NvrPwd"];  // or use NUL if there is no password
            // Start listen to specific port, NVR IP, Account, Password is read from config file
            // Channel is controlled by RTSP url parameter:["deviceId"] ["streamId"] ["unixTimestamp"]
            // ex:  RTSP://account:pwd@127.0.0.1:8554?deviceId=3&streamId=1&unixTimestamp=1565418793649
            RtspServer s = new RtspServer(port, username, password);
            try {
                s.StartListen();
            } catch {
                Console.WriteLine("Error: Could not start server");
                return;
            }

            // Wait for user to terminate programme
			String msg = "Connect RTSP client to Port=" + port;
			if (username != null && password != null) {
				msg += " Username=" + username + " Password=" + password;
			}
			Console.WriteLine(msg);
            Console.WriteLine("Press ENTER to exit");
            String readline = null;
            while (readline == null) {
                readline = Console.ReadLine();

                // Avoid maxing out CPU on systems that instantly return null for ReadLine
                if (readline == null) SpinWait.SpinUntil(() => false, 500);
            }

            s.StopListen();

        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (e.IsTerminating)
                {
                    var msg = e.ExceptionObject.ToString();
                    _logger.Fatal("UnhandledExceptionTrapper:{0}", msg);
                }
                else
                {
                    var msg = e.ExceptionObject.ToString();
                    _logger.Error("UnhandledExceptionTrapper:{0}", msg);
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex.ToString());
            }
        }
    }
}
