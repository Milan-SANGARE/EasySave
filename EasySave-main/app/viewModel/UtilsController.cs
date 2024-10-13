using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EasySaveV2.app.models;
using System.Windows;

namespace EasySaveV2.app.viewModel
{
    class UtilsController
    {
        /// <summary>
        /// Function that calculate the sum of the size of all the files under a directory
        /// </summary>
        /// <param name="directory">path to the directory</param>
        /// <returns>return the sum of the size of all the files under the directory</returns>
        public static long filesSize(DirectoryInfo directory)
        {
            long size = 0;
            FileInfo[] files = directory.GetFiles();
            foreach (FileInfo file in files)
            {
                size += file.Length;
            }
            DirectoryInfo[] directories = directory.GetDirectories();
            foreach (DirectoryInfo dir in directories)
            {
                size += filesSize(dir);
            }
            return size;
        }

        /// <summary>
        /// Function that calculte how many files are under a directory
        /// </summary>
        /// <param name="directory">path to the directory</param>
        /// <returns>return the count of files under this directory</returns>
        public static int filesCount(DirectoryInfo directory)
        {
            int count = 0;
            FileInfo[] files = directory.GetFiles();
            foreach (FileInfo file in files)
            {
                count += 1;
            }

            DirectoryInfo[] directories = directory.GetDirectories();
            foreach (DirectoryInfo dir in directories)
            {
                count += filesCount(dir);
            }
            return count;
        }

        /// <summary>
        /// Function that list all the folders recursivly under a folder
        /// </summary>
        /// <param name="path">path of the directory</param>
        /// <param name="result">list of all the directories</param>
        static public void listFolders(string path, ref List<string> result)
        {
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                result.Add(folder);
                listFolders(folder, ref result);
            }



        }

        /// <summary>
        /// Function that list all the files recursivly under a folder
        /// </summary>
        /// <param name="path">path of the directory</param>
        /// <param name="result">list of all the files</param>
        static public void listFiles(string path, ref List<string> result)
        {
            
            string[] folders = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                result.Add(file);
            }
            foreach (string folder in folders)
            {

                listFiles(folder, ref result);
            }
        }

        /// <summary>
        /// Function that return a list of all the string of a list that are not in an other list
        /// </summary>
        /// <param name="source">first list </param>
        /// <param name="destination">second list</param>
        /// <returns>returns a list with all the files of the first list that are in the second list</returns>
        static public List<string> stringListNotInList(List<string> source, List<string> destination)
        {
            List<string> result = new List<string>();
            foreach (string dest in destination)
            {
                if (!source.Contains(dest))
                {
                    result.Add(dest);
                }
            }
            return result;

        }

        /// <summary>
        /// Function that search if a string is present in a list
        /// </summary>
        /// <param name="input">string searched in the other list</param>
        /// <param name="destination">list to search in</param>
        /// <returns>returns a bool of the presence of the string in the list</returns>
        static public bool stringtInList(string input, List<string> destination)
        {


            if (destination.Contains(input))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Function that hash a file
        /// </summary>
        /// <param name="fs">file to hash</param>
        /// <returns>hash of the file</returns>
        static public string getHashOfFile(FileStream fs)
        {
            SHA1 sha = new SHA1Managed();
            string hash = BitConverter.ToString(sha.ComputeHash(fs)).Replace("-", "");
            return hash;
        }

        /// <summary>
        /// open a file, used to open the logs
        /// </summary>
        /// <param name="file">path of file to open</param>
        static public void openFile(string file)
        {
            System.Diagnostics.Process.Start("notepad.exe", file);

        }
        /// <summary>
        /// function that check if a process is running
        /// </summary>
        /// <returns> return a boolean</returns>
        public static bool jobProcessIsRunning()
        {
            configFile config = readConfFile();
            string getProcess = config.process;
            string process = getProcess.Remove(getProcess.Length - 4);
            Process[] pname = Process.GetProcessesByName(process);
            if (pname.Length == 0)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Function that write to the  config file
        /// </summary>
        /// <param name="extensionsChiffre">(List<string>) extensions to crypt</param>
        /// <param name="extensionsPrio">(List<string>) extensions to priorize</param>
        /// <param name="process">(string) jobProcess</param>
        /// <param name="maxSize">(int) Max file size</param>
        /// <param name="threads">(int) threads to use</param>
        /// <param name="key">(string) crypt key</param>

        public static void writeConfFile(List<string> extensionsChiffre, List<string> extensionsPrio, string process, long maxSize, int threads,string key,long blockSize)
        {
            configFile config = new configFile();
            config.extensionChiffre = extensionsChiffre;
            config.process = process;
            config.extensionPriority = extensionsPrio;
            config.maxSize = maxSize;
            config.threads = threads;
            config.blockSize = blockSize;
            config.key = key;
            var json = JsonConvert.SerializeObject(config);
            File.WriteAllText("../../../src/config/config.json", json);
        }
        /// <summary>
        /// surcharge of WriteconfFile
        /// </summary>
        /// <param name="extensionsChiffre"> (List<string>) extensions to crypt</param>
        /// <param name="extensionsPrio"> (List<string>) extensions to priorize</param>
        public static void writeConfFile(List<string> extensionsChiffre,List<string> extensionsPrio)
        {
            configFile config = new configFile();
            config.extensionChiffre = extensionsChiffre;
            config.extensionPriority = extensionsPrio;
            var json = JsonConvert.SerializeObject(config);
            File.WriteAllText("../../../src/config/config.json", json);
        }
        /// <summary>
        /// Function that read the config file
        /// </summary>
        /// <returns> return a configFile object</returns>
        public static configFile readConfFile()
        {
            string json = File.ReadAllText("../../../src/config/config.json");
            configFile config = JsonConvert.DeserializeObject<configFile>(json);
            return config;
        }
        /// <summary>
        /// Function that start our application "CryptoSoft"
        /// </summary>
        /// <param name="key"> (string) crypt key</param>
        /// <param name="path"> (string) path to CryptoSoft </param>
        public static void startCryptoSoft(string key, string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("../../../src/CryptoSoft.exe");
            startInfo.Arguments = $"{key} {path}";
            Process.Start(startInfo);
        }
        /// <summary>
        /// Function that sort the prioritaries files
        /// </summary>
        /// <param name="list"> (List<string>) List of files</param>
        /// <returns></returns>

        public static List<string> sortPrioritaryFiles(List<string> list)
        {
            configFile config = readConfFile();
            List<string> configList = config.extensionPriority;
            List<string> result = new List<string>();
            string sum = "";
            foreach(string prio in configList)
            {
                sum += prio;
            }

            if(config != null)
            {
                foreach (string file in list)
                {
                    //Trace.WriteLine(Path.GetExtension(file)+" in "+ sum+" is "+ configList.Contains(Path.GetExtension(file)));

                    if (configList.Contains(Path.GetExtension(file)))
                    {
                        result.Add(file);
                    }

                }

                foreach (string file in list)
                {
                    if (!configList.Contains(Path.GetExtension(file)))
                    {
                        result.Add(file);
                    }

                }
                return result;

            }
            else
            {
                return list;
            }
            
        }

    }
}
