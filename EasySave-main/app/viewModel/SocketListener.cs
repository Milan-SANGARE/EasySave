using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using EasySaveV2.app.models;
using EasySaveV2.app.views;

namespace EasySaveV2.app.viewModel
{
    /// <summary>
    /// class for the server
    /// </summary>
    class SocketListener

    {
        /// <summary>
        /// function that bind and listen on a ip + port
        /// </summary>
        /// <param name="ip"> (string) IP to bind </param>
        /// <param name="port"> (Int32) port to bind </param>
        /// <returns> return the binded socket </returns>
        public static Socket SeConnecter(string ip, Int32 port)
        {
            try
            {
                IPAddress host = IPAddress.Parse(ip);
                IPEndPoint remoteEP = new IPEndPoint(host, port);
                Socket listener = new Socket(host.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(remoteEP);
                listener.Listen(10);
                return listener;
            }
            catch
            {
                MessageBox.Show("Serveur already running");
                return null;
            }
        }

        /// <summary>
        /// function that accept incomming connection
        /// </summary>
        /// <param name="socket"> (Socket) Connection socket</param>
        /// <returns> return accepted socket </returns>
        public static Socket AccepterConnection(Socket socket)
        {
            Socket infos = socket.Accept();
           // MessageBox.Show("I am connected to " + IPAddress.Parse(((IPEndPoint)infos.RemoteEndPoint).Address.ToString()) + " on port number " + ((IPEndPoint)infos.RemoteEndPoint).Port.ToString());
            return infos;
        }
        /// <summary>
        /// function that listen request from client and send response based on request
        /// </summary>
        /// <param name="client"> (Socket) Accept socket </param>
        public static void EcouterReseau(Socket client)
        {
            byte[] bytes = new Byte[1024];
            string datas = null;
            try
            {
                while (true)
                {
                    Socket accept = client.Accept();
                    int bytesRec = accept.Receive(bytes);
                    datas = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    string[] data = datas.Split(" ");
                    if (data[0].Equals("delete"))
                    {
                        BackupTask save = BackupTaskViewModel.readBackupTask(data[1]);
                        BackupTaskViewModel.deleteBackupTask(data[1]);
                        byte[] msg = Encoding.ASCII.GetBytes("La sauvegarde " +save.name + " a ete supprimee");
                        accept.Send(msg);
                    }

                    else if (data[0].Equals("progress"))
                    {
                        try
                        {
                            BackupTask save = BackupTaskViewModel.readBackupTask(data[1]);
                            byte[] msg = Encoding.ASCII.GetBytes(save.progress.pourcentage.ToString());
                            accept.Send(msg);
                        }
                        catch { }
                    }

                    else if (data[0].Equals("read"))
                    {
                        byte[] msg;
                        string names = "";
                        BackupTaskList saveList = BackupTaskViewModel.readBackupTasks();
                        List<BackupTask> saves = saveList.taskList;
                        foreach (BackupTask save in saves)
                        {
                            names += $"{save.uuid}--{save.name}--{save.source}--{save.target}--{save.type},,";
                        }
                        msg = Encoding.ASCII.GetBytes(names);
                        accept.Send(msg);
                    }

                    else if (data[0].Equals("create"))
                    {
                        BackupTaskViewModel.createBackupTask(data[2], data[3], data[4], data[5]);
                        byte[] msg = Encoding.ASCII.GetBytes("La sauvegarde " + data[2] + " a ete creee");
                        accept.Send(msg);
                    }

                    else if (data[0].Equals("update"))
                    {
                        BackupTask save = BackupTaskViewModel.readBackupTask(data[1]);
                        BackupTaskViewModel.updateBackupTask(data[1], data[2], data[3], data[4], data[5], save.progress);
                        byte[] msg = Encoding.ASCII.GetBytes("La sauvegarde " + save.name + " a ete mise a jour");
                        accept.Send(msg);
                    }

                    else if (data[0].Equals("start")) 
                    {
                        BackupTask task = BackupTaskViewModel.readBackupTask(data[1]);
                        BackupTaskViewModel.startThreadBackupTask(task,SaveManage.backupsJsonMutex);
                        byte[] msg = Encoding.ASCII.GetBytes("La sauvegarde " + task.name + " a ete lancee");
                        accept.Send(msg);
                    }

                    else if (data[0].Equals("stop"))
                    {
                        //BackupTask task = BackupTaskViewModel.readBackupTask(data[1]);
                        //BackupTaskViewModel.stopBackupTask(task);
                        //byte[] msg = Encoding.ASCII.GetBytes(task.name + "a ete stopee");
                        //accept.Send(msg);
                    }

                    else if (data[0].Equals("exit"))
                         {
                              byte[] msg = Encoding.ASCII.GetBytes("Serveur stoppe");
                              accept.Send(msg);
                              break;
                          }
                    else
                          {
                                byte[] msg = Encoding.ASCII.GetBytes("Requete inconnue");
                                accept.Send(msg);
                           }    
                }   
            }
                
             catch
                {
                MessageBox.Show("Connexion perdue");
                Deconnecter(client);
                }

        
        }
        /// <summary>
        /// function that close the connection
        /// </summary>
        /// <param name="socket"> (Socket) Connection socket </param>
        public static void Deconnecter(Socket socket)
        {
            socket.Close();
        }

        /// <summary>
        /// function for start the server
        /// </summary>
        /// <param name="ip"> (string) IP to start </param>
        /// <param name="port"> (Int32) Port to start </param>
        public void StartServer(string ip, Int32 port) {
            Socket Conn = SeConnecter(ip, port);
            EcouterReseau(Conn);
            Deconnecter(Conn);
        }

    }
} 


