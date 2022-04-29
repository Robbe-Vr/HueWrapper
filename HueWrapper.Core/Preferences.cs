using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueWrapper.Core
{
    internal class Preferences
    {
        public string PreferedBridge { get; set; }
        public string BridgeIp { get; set; }
        public string AppKey { get; set; }
        public string ClientKey { get; set; }
        public string MusicSyncOutputDeviceName { get; set; }

        internal void Load()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MidiDomotica-HueWrapper", "preferences.json");

            if (File.Exists(path))
            {
                string jsonString = File.ReadAllText(path);

                Preferences pref = System.Text.Json.JsonSerializer.Deserialize<Preferences>(jsonString);

                this.PreferedBridge = pref.PreferedBridge;
                this.BridgeIp = pref.BridgeIp;
                this.AppKey = pref.AppKey;
                this.ClientKey = pref.ClientKey;
                this.MusicSyncOutputDeviceName = pref.MusicSyncOutputDeviceName;
            }
            else
            {
                Store();
            }
        }

        internal void Store()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MidiDomotica-HueWrapper");

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string path = Path.Combine(dir, "preferences.json");

            string content = System.Text.Json.JsonSerializer.Serialize(this);

            File.WriteAllText(path, content);
        }
    }
}
