using Newtonsoft.Json;

namespace Jigdo.Gui
{
    public class WinjigdoSettings
    {
        public string? partsCachePath { get; set; }
        public bool verifyChecksum { get; set; }
        public List<DownloadMirror>? mirrors { get; set; }

        public static WinjigdoSettings Load()
        {
            // ... Deserialize
            try
            {
                string fileSettings = File.ReadAllText("settings.json");
                return JsonConvert.DeserializeObject<WinjigdoSettings>(fileSettings) ?? new WinjigdoSettings();
            }
            catch (Exception)
            {
                //return empty settings (usually the first time this will happen)
                return new WinjigdoSettings();
            }

        }

        public static WinjigdoSettings Load(string filePath)
        {
            // ... Deserialize
            try
            {
                string fileSettings = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<WinjigdoSettings>(fileSettings) ?? new WinjigdoSettings();
            }
            catch (Exception)
            {
                //return empty settings (usually the first time this will happen)
                return new WinjigdoSettings();
            }

        }

        public void Save()
        {
            // ... Serialize
            var settingsstring = JsonConvert.SerializeObject(this);
            File.WriteAllText("settings.json", settingsstring);
        }

        public void Save(string filePath)
        {
            // ... Serialize
            var settingsstring = JsonConvert.SerializeObject(this);
            File.WriteAllText(filePath, settingsstring);
        }

    }
}
