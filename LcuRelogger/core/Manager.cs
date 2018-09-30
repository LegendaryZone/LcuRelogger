using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace LcuRelogger.core
{
    public class Entry
    {
        public string Username { get; private set; }
        public string Password { get; private set; }

        public Entry(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }
    }

    public class Manager
    {
        public bool Connected { get; private set; }
        public bool LolExists { get; private set; }
        public string LeaguePath { get; private set; }
        public Api Api { get; private set; }
        public List<Entry> Entries { get; private set; }

        public string FilePath =
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.accounts.lculogger";

        public Manager()
        {
            Connected = false;
            LolExists = false;
            LeaguePath = "";
            Entries = new List<Entry>();
            setup();
        }

        public void LoadApi()
        {
            Api = new Api();
            Connected = Api.init(LeaguePath);
        }

        public void updateLeaguePath(string path)
        {
            if (pathValid(path))
            {
                LeaguePath = new DirectoryInfo(path).FullName;
                LolExists = true;
                writeConfig();
            }
        }
        private bool pathValid(string path)
        {
            if (!Directory.Exists(path)) return false;
            var info = new DirectoryInfo(path);
            string[] needed = {"Config", "RADS", "Cookies"};
            foreach (var s in needed)
            {
                var found = false;
                foreach (var directoryInfo in info.GetDirectories())
                {
                    if (directoryInfo.Name == s)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found) return false;
            }

            return true;
        }

        public void AddEntry(string username, string password)
        {
            Entries.Add(new Entry(username, password));
            writeConfig();
        }

        public void writeConfig()
        {
            var obj = new JObject();
            obj["path"] = LeaguePath;
            var arr = new JArray();

            foreach (var entry in Entries)
            {
                var jEntry = new JObject();
                jEntry["username"] = entry.Username;
                jEntry["password"] = entry.Password;

                arr.Add(jEntry);
            }

            obj["entries"] = arr;

            File.WriteAllText(FilePath, obj.ToString());
        }

        private void setup()
        {
            if (!File.Exists(FilePath))
            {
                if (Directory.Exists(@"C:\Riot Games\League of Legends"))
                {
                    if (pathValid(@"C:\Riot Games\League of Legends"))
                    {
                        LolExists = true;
                        LeaguePath = @"C:\Riot Games\League of Legends";
                        writeConfig();
                    }
                }
            }
            else
            {
                var content = File.ReadAllText(FilePath);
                var obj = JObject.Parse(content);
                string lolPath = (string) obj["path"];

                LolExists = pathValid(lolPath);
                if (LolExists) LeaguePath = lolPath;
                Entries.Clear();

                var arr = (JArray) obj["entries"];

                foreach (JObject entry in arr)
                {
                    Entries.Add(new Entry((string) entry["username"], (string) entry["password"]));
                }
            }
        }
    }
}