using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using EasySaveV2.app.models;
using System.Threading;

namespace EasySaveV2.app.viewModel
{
    class LoggingViewModel
    {

        /// <summary>
        /// Function that save dailyLogs in XML or in JSON
        /// </summary>
        /// <param name="name">name of the backupTask</param>
        /// <param name="source">source file </param>
        /// <param name="target">target file</param>
        /// <param name="size">size of the file</param>
        /// <param name="startTime">DateTime of the start when saving the file</param>
        /// <param name="endTime">DateTime of the end when saving the file</param>
        static public void dailyLogs(string name, string source, string target, long size, DateTime startTimeCopy, DateTime endTimeCopy, DateTime startTimeCrypt, DateTime endTimeCrypt)
        {
            string path = "../../../src/log/" + name + "." + Logs.getInstance().logType;
            double transferTime = new TimeSpan(endTimeCopy.Ticks).TotalMilliseconds - new TimeSpan(startTimeCopy.Ticks).TotalMilliseconds;
            double cryptTime = new TimeSpan(endTimeCrypt.Ticks).TotalMilliseconds - new TimeSpan(startTimeCrypt.Ticks).TotalMilliseconds;

            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(new TimeSpan(startTimeCopy.Ticks).TotalSeconds).ToLocalTime();

            if (Logs.getInstance().logType == "json")
            {
                var test = new
                {
                    Name = name,
                    Source = source,
                    Target = target,
                    Progress = new
                    {
                        sizeFiles = size + " bytes",
                        startTime = dtDateTime,
                        TransfertTime = transferTime + " ms",
                        CryptTime = cryptTime+" ms",
                    },
                    // CryptTime = cryptTime
                };


                string json = JsonConvert.SerializeObject(test, Newtonsoft.Json.Formatting.Indented);


                
                File.AppendAllText(path, json + ",");
                

            }
            else if (Logs.getInstance().logType == "xml")
            {
                var settings = new System.Xml.XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    Indent = true
                };
                using (XmlWriter writer = XmlWriter.Create(path, settings))
                {
                    writer.WriteStartElement("Save");
                    writer.WriteAttributeString("Name", name);
                    writer.WriteElementString("SourceFolder", source);
                    writer.WriteElementString("TargetFolder", target);
                    writer.WriteStartElement("Progress");
                    writer.WriteElementString("sizeFiles", size + " bytes");
                    writer.WriteElementString("startTime", XmlConvert.ToString(dtDateTime)); ;
                    writer.WriteElementString("TransfertTime", transferTime + " ms");
                    writer.WriteEndElement();
                }
            }
        }
    }
}
