using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using EasySaveV2.app.viewModel;
using EasySaveV2.app.models;
using System.Diagnostics;
using System.Windows;
using System.Threading;


namespace EasySaveV2.app.viewModel
{

    /// <summary>
    /// BackupTask ViewModel, gestion for the BackupTask and BackupTaskList Models
    /// </summary>
    class BackupTaskViewModel
    {


        /// <summary>
        /// create BackupTask and save it in backups.json as an object
        /// </summary>
        /// <param name="nameInput">
        /// (string) name input of the created object
        /// </param>
        /// <param name="sourceInput">
        /// (string) source input of the created object
        /// </param>
        /// <param name="targetInput">
        /// (string) target input of the created object
        /// </param>
        /// <param name="typeInput">
        /// (string) type input of the created object
        /// </param>
        /// <returns>
        /// (string) uuid of the BackupTask created
        /// </returns>        
        static public bool createBackupTask(string nameInput, string sourceInput, string targetInput, string typeInput)
        {

            //Créé notre nouvel objet

            BackupTask newTask = new BackupTask();
            newTask.uuid = Guid.NewGuid().ToString();
            newTask.name = nameInput;
            newTask.source = sourceInput;
            newTask.target = targetInput;
            newTask.type = typeInput;
            newTask.progress = new BackupTaskProgress();
            newTask.progress.state = "NOT ACTIVE";

            //Charge la sauvegarde
            string json = File.ReadAllText("../../../src/config/backups.json");
            BackupTaskList backupTaskList = JsonConvert.DeserializeObject<BackupTaskList>(json);

            if (backupTaskList == null)
            {
                List<BackupTask> list = new List<BackupTask>();
                list.Add(newTask);
                backupTaskList = new BackupTaskList(list);
            }

            else
            {
                //ajoute le nouvel objet à la sauvegarde
                backupTaskList.taskList.Add(newTask);
            }


            //Ecrit la sauvegarde
            string backupTaskListJson = JsonConvert.SerializeObject(backupTaskList);
            File.WriteAllText("../../../src/config/backups.json", backupTaskListJson);

            return true;

        }

        /// <summary>
        /// Function that return an object selected with his identifier (uuid or name)
        /// </summary>
        /// <param name="identifier">uuid or name of the BackupTask</param>
        /// <returns>BackupTask object</returns>
        static public BackupTask readBackupTask(string identifier)
        {
            string json = File.ReadAllText("../../../src/config/backups.json");
            BackupTaskList backupTaskList = JsonConvert.DeserializeObject<BackupTaskList>(json);
            for (int x = 0; x < backupTaskList.taskList.Count; x++)
            {
                if (backupTaskList.taskList[x].name == identifier || backupTaskList.taskList[x].uuid == identifier)
                {
                    return backupTaskList.taskList[x];
                }
            }
            return null;

        }

        /// <summary>
        /// Function that return all the Backup existing in the savefile
        /// </summary>
        /// <returns>return a BackupTaskList (list of BackupTask object)</returns>
        static public BackupTaskList readBackupTasks()
        {
            string json = File.ReadAllText("../../../src/config/backups.json");
            BackupTaskList backupTaskList = JsonConvert.DeserializeObject<BackupTaskList>(json);
            return backupTaskList;
        }

        /// <summary>
        /// update BackupTask and save it in backups.json as an object
        /// </summary>
        /// 
        /// <param name="uuidInput">
        /// (string) uuid input of the created object
        /// </param>
        /// <param name="nameInput">
        /// (string) name input of the created object
        /// </param>
        /// <param name="sourceInput">
        /// (string) source input of the created object
        /// </param>
        /// <param name="targetInput">
        /// (string) target input of the created object
        /// </param>
        /// <param name="typeInput">
        /// (string) type input of the created object
        /// </param>
        /// <param name="progressInput">
        /// (BackupTaskProgress) progress input of the created object
        /// </param>
        /// <returns>
        /// return a BackupTask Object
        /// </returns>
        static public BackupTask updateBackupTask(string uuidInput, string nameInput, string sourceInput, string targetInput, string typeInput, BackupTaskProgress progressInput)
        {
            string json = File.ReadAllText("../../../src/config/backups.json");

            BackupTaskList backupTaskList = JsonConvert.DeserializeObject<BackupTaskList>(json);

            for (int x = 0; x < backupTaskList.taskList.Count; x++)
            {
                if (backupTaskList.taskList[x].uuid == uuidInput)
                {
                    //modify
                    backupTaskList.taskList[x].name = nameInput;
                    backupTaskList.taskList[x].source = sourceInput;
                    backupTaskList.taskList[x].target = targetInput;
                    backupTaskList.taskList[x].type = typeInput;
                    backupTaskList.taskList[x].progress = progressInput;



                    //Ecrit la sauvegarde
                    string backupTaskListJson = JsonConvert.SerializeObject(backupTaskList);
                    File.WriteAllText("../../../src/config/backups.json", backupTaskListJson);

                    //return object
                    return backupTaskList.taskList[x];
                }
            }
            return null;

        }

        /// <summary>
        /// Function that delete a BackupTask from save file and app
        /// </summary>
        /// <param name="uuid">uuid identifier to delete the object</param>
        /// <returns>return a bool, depend on the work done by the function; true: work done, false: backup not found</returns>
        static public bool deleteBackupTask(string uuid)
        {
            //ouvre la save
            string json = File.ReadAllText("../../../src/config/backups.json");
            BackupTaskList backupTaskList = JsonConvert.DeserializeObject<BackupTaskList>(json);
            //si la save est vide on stop
            if (backupTaskList == null)
            {
                return false;
            }
            List<BackupTask> list = new List<BackupTask>();
            for (int x = 0; x < backupTaskList.taskList.Count; x++)
            {
                if (backupTaskList.taskList[x].uuid != uuid)
                {
                    list.Add(backupTaskList.taskList[x]);
                }
                else
                {
                    if (File.Exists("../../../src/hash/" + backupTaskList.taskList[x].uuid + ".json"))
                    {
                        File.Delete("../../../src/hash/" + backupTaskList.taskList[x].uuid + ".json");
                    }
                }
            }
            backupTaskList = new BackupTaskList(list);
            string backupTaskListJson = JsonConvert.SerializeObject(backupTaskList);
            File.WriteAllText("../../../src/config/backups.json", backupTaskListJson);
            return true;

        }
        /// <summary>
        /// function to start the save in multithreading
        /// </summary>
        /// <param name="backupTask"> (BackupTask) BackupTask to start </param>
        /// <param name="backupsJsonMutex"> (Mutex) mutex to use</param>
        /// 
        public static void startThreadBackupTask(BackupTask backupTask, Mutex backupsJsonMutex)
        {
            Trace.WriteLine("startThreadBackupTask : " + backupTask.name);

            processStartBackup bt = new processStartBackup(backupTask, backupsJsonMutex);
            System.Threading.Thread thrd = new System.Threading.Thread(bt.startBackupTask);

            //dicoThrd.Add(backupTask.uuid,thrd);
            thrd.Start();
        }
        /// <summary>
        /// function that stop a backupTask
        /// </summary>
        /// <param name="backupTask"> (BackupTask) BackupTask to stop </param>
        public static void stopBackupTask(BackupTask backupTask)
        {

            backupTask.progress.state = "PAUSE";
            BackupTaskViewModel.updateBackupTask(backupTask.uuid, backupTask.name, backupTask.source, backupTask.target, backupTask.type, backupTask.progress);


        }





    }
    class processStartBackup
    {
        public BackupTask backupTask;
        public Mutex backupJsonMutex;
        private static configFile config = UtilsController.readConfFile();
        private string key = config.key;
        private int threads = config.threads;
        private long blockSize = config.blockSize;
        private long maxSize = config.maxSize;
        private Semaphore sema = new System.Threading.Semaphore(3, 3);

        /// <summary>
        /// Function that process the BackupTask, depend on the type of the backup (complet, differentiel)
        /// </summary>
        /// <param name="BT">(BackupTask) BackupTask object took in input to process the backup</param>
        /// <param name="bk">(Mutex) mutex to use </param>


        public processStartBackup(BackupTask BT, Mutex bk)
        {
            backupTask = BT;
            backupJsonMutex = bk;
        }
        /// <summary>
        /// function  that copy recursively files, with a source and a target ,based on extensions to crypt and to prioritise, max file size, crypt key and threads
        /// </summary>
        /// <param name="source">(string) source path </param>
        /// <param name="target"> (string) target path </param>
        /// <param name="file"> (string) file check for extensions</param>
        public void fileCopy(string source, string target, string file, string hash, hashLog oldHashs, hashLog finalHashs, DateTime startTimeCrypt, DateTime endTimeCrypt)
        {
            DateTime startTime = DateTime.Now;



            if (backupTask.type == "differentiel")
            {
                //est-ce que le fichier est a copié ou deja présent dans la destination
                if (!UtilsController.stringtInList(file, new List<string>(oldHashs.dico.Keys)) || !(hash == oldHashs.dico[file]))
                {





                    if ((long)new FileInfo(file).Length < maxSize)
                    {
                        if (config.extensionChiffre.Contains(Path.GetExtension(file)))
                        {
                            Trace.WriteLine("differentiel Copying with crypt" + file);
                            Directory.CreateDirectory(Path.GetDirectoryName(target));

                            Process myProcess = new Process();

                            //Trace.WriteLine(key + " " + @source + " " + @target + " " + threads + " " + blockSize);

                            // Start a process to print a file and raise an event when done.
                            myProcess.StartInfo.FileName = @"..\..\..\src\cryptoSoftV2\cryptoSoftV2.exe";
                            myProcess.StartInfo.Arguments = key + " " + @source + " " + @target + " " + threads + " " + blockSize;
                            myProcess.StartInfo.UseShellExecute = false;
                            myProcess.StartInfo.CreateNoWindow = true;
                            myProcess.StartInfo.Verb = "runas";
                            using (Process exeProcess = Process.Start(myProcess.StartInfo))
                            {
                                exeProcess.WaitForExit();
                            }
                        }
                        else
                        {
                            Trace.WriteLine("differentiel Copying without crypt " + file);
                            Directory.CreateDirectory(Path.GetDirectoryName(target));
                            File.Copy(source, target, true);
                        }
                    }



                }

            }
            else if (backupTask.type == "complet")
            {
                if ((long)new FileInfo(file).Length < maxSize)
                {
                    if (config.extensionChiffre.Contains(Path.GetExtension(file)))
                    {
                        Trace.WriteLine("Complet Copying with crypt" + file);
                        Directory.CreateDirectory(Path.GetDirectoryName(target));

                        Process myProcess = new Process();

                        Trace.WriteLine(key + " " + @source + " " + @target + " " + threads + " " + blockSize);

                        // Start a process to print a file and raise an event when done.
                        myProcess.StartInfo.FileName = @"..\..\..\src\cryptoSoftV2\cryptoSoftV2.exe";
                        myProcess.StartInfo.Arguments = key + " " + @source + " " + @target + " " + threads + " " + blockSize;
                        myProcess.StartInfo.UseShellExecute = false;
                        myProcess.StartInfo.CreateNoWindow = true;
                        myProcess.StartInfo.Verb = "runas";
                        using (Process exeProcess = Process.Start(myProcess.StartInfo))
                        {
                            exeProcess.WaitForExit();
                        }
                    }
                    else
                    {
                        Trace.WriteLine("Complet Copying without crypt " + file);
                        Directory.CreateDirectory(Path.GetDirectoryName(target));

                        File.Copy(source, target, true);
                    }
                }
            }

            finalHashs.dico.Add(file, hash);

            File.WriteAllText("../../../src/hash/" + backupTask.uuid + ".temp.json", JsonConvert.SerializeObject(finalHashs));

            DateTime endTime = DateTime.Now;
            //update dailylogs
            //update log état
            backupJsonMutex.WaitOne();

            BackupTask bt = BackupTaskViewModel.readBackupTask(backupTask.uuid);
            bt.progress.currentSourceFile = source;
            bt.progress.currentTargetFile = target;
            bt.progress.startTime = startTime;
            bt.progress.endTime = endTime;
            bt.progress.numberRemainingFiles -= 1;
            bt.progress.sizeRemainingFiles -= (long)new FileInfo(file).Length;
            bt.progress.pourcentage = (int)((bt.progress.sizeFiles - bt.progress.sizeRemainingFiles) * 100 / bt.progress.sizeFiles);

            //Trace.WriteLine("progress " +bt.progress.pourcentage+"%");
            //Trace.WriteLine((bt.progress.sizeFiles - bt.progress.sizeRemainingFiles) + "/"+ bt.progress.sizeFiles);
            //Trace.WriteLine("File Size" + file + " is "+ (long)new FileInfo(file).Length);
            if (bt.progress.numberRemainingFiles == 0)
            {
                bt.progress.state = "END";
            }


            BackupTaskViewModel.updateBackupTask(bt.uuid, bt.name, bt.source, bt.target, bt.type, bt.progress);
            LoggingViewModel.dailyLogs(bt.name, source, target, new FileInfo(file).Length, startTime, endTime, startTimeCrypt, endTimeCrypt);
            backupJsonMutex.ReleaseMutex();


        }
        /// <summary>
        /// function that start the BackupTask with hash check, and fill logs files
        /// </summary>
        public void startBackupTask()
        {
            //Starting unique backup
            Trace.WriteLine("startBackupTask : " + backupTask.name);


            //initializing the backup
            backupTask.progress.state = "RUNNING";
            backupTask.progress.numberFiles = UtilsController.filesCount(new DirectoryInfo(backupTask.source));
            backupTask.progress.sizeFiles = UtilsController.filesSize(new DirectoryInfo(backupTask.source));
            backupTask.progress.numberRemainingFiles = backupTask.progress.numberFiles;
            backupTask.progress.sizeRemainingFiles = backupTask.progress.sizeFiles;
            backupTask.progress.pourcentage = 0;

            backupJsonMutex.WaitOne();
            BackupTaskViewModel.updateBackupTask(backupTask.uuid, backupTask.name, backupTask.source, backupTask.target, backupTask.type, backupTask.progress);
            backupJsonMutex.ReleaseMutex();



            //retrieve all file available to copy
            List<string> files = new List<string>();
            UtilsController.listFiles(backupTask.source, ref files);


            //sort file in priority order
            if (config.extensionPriority != null)
            {
                files = UtilsController.sortPrioritaryFiles(files);
            }



            //on récupère les anciens HASH si ils existent
            bool oldHashExist = File.Exists("../../../src/hash/" + backupTask.uuid + ".json");
            hashLog oldHashs = new hashLog();
            List<string> filesToDelete = new List<string>();
            if (oldHashExist)
            {
                string json = File.ReadAllText("../../../src/hash/" + backupTask.uuid + ".json");
                oldHashs = JsonConvert.DeserializeObject<hashLog>(json);
            }
            else
            {
                oldHashs.dico = new Dictionary<string, string>();
            }


            //creating new file hash list
            hashLog finalHashs = new hashLog();
            finalHashs.dico = new Dictionary<string, string>();



            //process each file
            foreach (string file in files)
            {
                backupJsonMutex.WaitOne();
                string isRunning = BackupTaskViewModel.readBackupTask(backupTask.uuid).progress.state;
                backupJsonMutex.ReleaseMutex();
                if (isRunning == "RUNNING")
                {


                    string name = file.Substring(file.IndexOf(backupTask.source) + backupTask.source.Length);
                    string source = backupTask.source + name;
                    string dest = backupTask.target + name;


                    DateTime startTimeCrypt = DateTime.Now;
                    //hash du fichier
                    string hash = UtilsController.getHashOfFile(File.OpenRead(file));
                    DateTime endTimeCrypt = DateTime.Now;


                    //ajout du hash dans notre liste des olds fichiers/hashs
                    //if (UtilsController.jobProcessIsRunning())
                   // for (int cptFois = 1; cptFois <= 1; cptFois++)
                    //{
                     //   MessageBox.Show(Language.getInstance().getSlug("slugJobProcess"));                        
                   // }
                    Trace.WriteLine("File Copy:" + file);
                    sema.WaitOne();
                    fileCopy(source, dest, file, hash, oldHashs, finalHashs, startTimeCrypt, endTimeCrypt);
                    sema.Release();

                }
                else
                {
                    break;
                }


            }

            //fichiers à supprimer (fichier qui ont changés ou fichiers qui ne sont plus présent 
            //Console.WriteLine("Deleting unwnated files ...");
            filesToDelete = UtilsController.stringListNotInList(new List<string>(finalHashs.dico.Keys), new List<string>(oldHashs.dico.Keys));
            foreach (string file in filesToDelete.AsEnumerable().Reverse())
            {
                string dest = backupTask.target + file.Substring(file.IndexOf(backupTask.source) + backupTask.source.Length);
                string folder = Path.GetDirectoryName(dest);
                File.Delete(dest);
                if (Directory.GetFiles(folder) == null && Directory.GetDirectories(folder) == null)
                {
                    Directory.Delete(folder);
                }
            }

            //deleting temp after save completed

            if (File.Exists("../../../src/hash/" + backupTask.uuid + ".temp.json"))
            {
                File.Copy("../../../src/hash/" + backupTask.uuid + ".temp.json", "../../../src/hash/" + backupTask.uuid + ".json", true);
                File.Delete("../../../src/hash/" + backupTask.uuid + ".temp.json");
            }
        }
    }

}
