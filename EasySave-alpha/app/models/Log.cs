using System;
using System.Collections.Generic;
using System.Text;
namespace EasySaveV2.app.models
{
    public sealed class Logs
    {
        public string logType;
        private static Logs instance;
        private Logs(string type)
        {
            this.logType = type;
        }
        public static Logs getInstance()
        {
            if (instance == null)
            {
                instance = new Logs("json");
            }
            return instance;
        }
        public static Logs getInstance(string type)
        {
            instance = new Logs(type);
            return instance;
        }
    }
}