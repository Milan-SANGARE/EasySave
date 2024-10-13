using System;
using System.Collections.Generic;
using System.Text;
using EasySaveV2.app.models;
using EasySaveV2.app.viewModel;

namespace EasySaveV2.app.models
{
    class BackupTask
    {
        public string uuid { get; set; }
        public string name { get; set; }
        public string source { get; set; }
        public string target { get; set; }
        public string type { get; set; }
        public BackupTaskProgress progress { get; set; }

    }

}
