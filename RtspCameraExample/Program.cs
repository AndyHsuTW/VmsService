using System;
using System.Threading;

namespace RtspCameraExample
{
    class Program
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        static async System.Threading.Tasks.Task Main(string[] args)
        {

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            int port = 8554;
			string username = "account";      // or use NUL if there is no username
			string password = "pwd";  // or use NUL if there is no password
            
            RtspServer s = new RtspServer(port,username,password);
            try {
                s.StartListenAsync();
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
                if (readline == null) Thread.Sleep(500);
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
