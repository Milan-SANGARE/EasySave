using System;
using System.Collections.Generic;
using System.Text;

namespace EasySaveV2.app.models
{
    class BackupTaskList
    {
        public List<BackupTask> taskList { get; set; }

        public BackupTaskList(List<BackupTask> list)
        {
            this.taskList = list;
        }
    }
}
