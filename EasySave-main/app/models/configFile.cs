using System;
using System.Collections.Generic;
using System.Text;

namespace EasySaveV2.app.models
{
    class configFile
    {
        public List<string> extensionChiffre { get; set; }
        public List<string> extensionPriority { get; set; }
        public string process { get; set; }
        public long maxSize { get; set; }

        public long blockSize { get; set; }

        public int threads { get; set;  }
        public string key { get; set; }
    }
}
