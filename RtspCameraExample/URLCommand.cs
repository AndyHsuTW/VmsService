using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.IO;

namespace ACTi.NVR3.OemDvrMiniDriver
{
    public class URLCommand
    {
        private string m_ServerIP; private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private uint m_ServerPort;
        private string m_Admin;
        private string m_Password;

        private string m_MediaLiveCommand;
        private string m_MediaPlaybackCommand;
        private string m_Session;

        public URLCommand(string ip, uint port, string admin, string password, string session=null)
        {
            m_ServerIP = ip;
            m_ServerPort = port;
            m_Admin = admin;
            m_Password = password;
            m_Session = session;
            m_MediaLiveCommand = "http://" + m_ServerIP+":"+ m_ServerPort + "/Media";
            m_MediaPlaybackCommand = "http://" + m_ServerIP + ":" + m_ServerPort + "/Media/SyncPlayback";
        }

        public bool Login(ref string info)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)(WebRequest.Create(m_MediaLiveCommand + "/UserGroup/login"));
                request.Proxy = null;
                request.KeepAlive = false;
                request.Timeout = 20000;
                request.Credentials = new NetworkCredential(m_Admin, m_Password);

                GetResponse(request, ref info);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool GetAllDeviceList(ref string info)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)(WebRequest.Create(m_MediaLiveCommand + "/Device/getDevice"));
                request.Proxy = null;
                request.KeepAlive = false;
                request.Timeout = 20000;
                request.Credentials = new NetworkCredential(m_Admin, m_Password);
                GetResponse(request, ref info);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool GetDeviceConfig(ref string info, string DeviceID)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)(WebRequest.Create(m_MediaLiveCommand + "/Device/getDevice?deviceid=" + DeviceID));
                request.Proxy = null;
                request.KeepAlive = false;
                request.Timeout = 20000;
                request.Credentials = new NetworkCredential(m_Admin, m_Password);
                GetResponse(request, ref info);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool SetCurrentTime(string utcTime)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)(WebRequest.Create(m_MediaPlaybackCommand + $"/setcurrenttime?syncsession={m_Session}&currenttime=" + utcTime + "000"));
                request.Proxy = null;
                request.KeepAlive = false;
                request.Timeout = 20000;
                request.Credentials = new NetworkCredential(m_Admin, m_Password);

                string info = string.Empty;
                GetResponse(request, ref info);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Play()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)(WebRequest.Create(m_MediaPlaybackCommand + $"/Play?syncsession={m_Session}"));
                request.Proxy = null;
                request.KeepAlive = false;
                request.Timeout = 20000;
                request.Credentials = new NetworkCredential(m_Admin, m_Password);

                string info = string.Empty;
                GetResponse(request, ref info);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool LiveStream(string deviceID)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)(WebRequest.Create(m_MediaLiveCommand + "/Streaming?deviceid=" + deviceID));
                request.Proxy = null;
                request.KeepAlive = false;
                request.Timeout = 20000;
                request.Credentials = new NetworkCredential(m_Admin, m_Password);

                string info = string.Empty;
                GetResponse(request, ref info);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Pause()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)(WebRequest.Create(m_MediaPlaybackCommand + $"/Pause?syncsession={m_Session}"));
                request.Proxy = null;
                request.KeepAlive = false;
                request.Timeout = 20000;
                request.Credentials = new NetworkCredential(m_Admin, m_Password);

                string info = string.Empty;
                GetResponse(request, ref info);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool SetPlayMode(string speed)
        {
            try
            {
                string mode = string.Empty;
                if (speed.Contains("-"))
                {
                    mode = "B";
                }
                else
                {
                    mode = "F";
                }

                speed = speed.Replace("-", "") + ".0";

                HttpWebRequest request = (HttpWebRequest)(WebRequest.Create(m_MediaPlaybackCommand + $"/SetPlayMode?syncsession={m_Session}&action=" + mode + "&playrate=" + speed));
                request.Proxy = null;
                request.KeepAlive = false;
                request.Timeout = 20000;
                request.Credentials = new NetworkCredential(m_Admin, m_Password);

                string info = string.Empty;
                GetResponse(request, ref info);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void GetResponse(HttpWebRequest request, ref string info)
        {
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                WebResponse_To_String(response, ref info);
                CloseWebResponse(request, response);
            }
            catch(Exception ex)
            {
                _logger.Error(ex.ToString());
                info = ex.Message;
                CloseWebResponse(request, null);
                throw;
            }
        }

        private void WebResponse_To_String(HttpWebResponse response, ref string info)
        {
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            info = reader.ReadToEnd();
        }

        private bool CloseWebResponse(HttpWebRequest request, HttpWebResponse response)
        {
            try
            {
                request.Abort();
                if (response != null)
                {
                    response.Close();
                }
                response = null;
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
