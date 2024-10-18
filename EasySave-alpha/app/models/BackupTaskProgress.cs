using System;
using System.Collections.Generic;
using System.Text;

namespace EasySaveV2.app.models
{
    class BackupTaskProgress
    {
        public string state { get; set; }
        public int numberRemainingFiles { get; set; }
        public long sizeRemainingFiles { get; set; }
        public long sizeFiles { get; set; }
        public int numberFiles { get; set; }
        public String currentSourceFile { get; set; }
        public String currentTargetFile { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public int pourcentage { get; set; }
    }
}
