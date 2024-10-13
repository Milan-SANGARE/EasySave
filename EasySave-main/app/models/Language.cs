using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace EasySaveV2.app.models
{
    public sealed class Language
    {
        private Dictionary<string, string> dico { get; set; }
        private string language;
        private static Language instance;


        private Language(string lang)
        {
            language = lang;
            string json = File.ReadAllText("../../../src/langue/"+this.language+".json");
            dico = JsonConvert.DeserializeObject<Dictionary<string,string>>(json);
        }

        public static Language getInstance()
        {
            if (instance == null)
            {
                instance = new Language("EN");
            }
            return instance;
        }
        public static Language getInstance(string lang)
        {
            instance = new Language(lang);
            return instance;
        }


        public string getSlug(string slug)
        {
            return this.dico[slug];
        }

    }
}
