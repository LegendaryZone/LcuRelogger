using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace LcuRelogger.core
{
    public class Api
    {
        private int _port;
        private string _lolPath;
        private string _password;
        private bool _restarting;

        private string request(string path, string data = null, string method = "GET")
        {
            string requestUrl = "https://127.0.0.1:" + _port + path;

            var request = (HttpWebRequest) WebRequest.Create(requestUrl);
            request.Method = method;
            request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.Headers.Set("Authorization", "Basic " + Base64Encode("riot:" + _password));
            if (method == "POST" && data != null)
                request.GetRequestStream()
                    .Write(Encoding.ASCII.GetBytes(data), 0, Encoding.ASCII.GetBytes(data).Length);

            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            response.Close();
            return responseFromServer;
        }

        public string startSession(Entry entry)
        {
            var obj = new JObject();
            obj["username"] = entry.Username;
            obj["password"] = entry.Password;

            return request("/lol-login/v1/session", obj.ToString(), "POST");
        }

        public bool reloadApi()
        {
            return init(_lolPath);
        }

        public void restartClient(Action callback, int delay = 1)
        {
            if (_restarting) return;
            _restarting = true;
            request("/process-control/v1/process/restart?delaySeconds=" + delay, null, "POST");


            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                var oldPassword = _password;
                while (true)
                {
                    Thread.Sleep(25);

                    init(_lolPath);
                    if (_password != oldPassword) break;
                }

                _restarting = false;
                Thread.Sleep(7500);
                callback();
            }).Start();
        }

        private static string ReadLockfile(string leagueLocation)
        {
            File.Copy(leagueLocation + "lockfile", leagueLocation + "lockfile-temp");
            var contents = File.ReadAllText(leagueLocation + "lockfile-temp", Encoding.UTF8);
            File.Delete(leagueLocation + "lockfile-temp");
            return contents;
        }

        public bool init(string path)
        {
            var info = new DirectoryInfo(path);
            foreach (var fileInfo in info.GetFiles())
            {
                if (fileInfo.Name == "lockfile")
                {
                    var lockFileContent = ReadLockfile(info.FullName + "\\").Split(':');
                    _port = Convert.ToInt32(lockFileContent[2]);
                    _password = lockFileContent[3];
                    _lolPath = info.FullName;
                    return true;
                }
            }

            return false;
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}