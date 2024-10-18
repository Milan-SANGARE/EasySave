using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EasySaveV2.app.viewModel;

namespace EasySaveV2.app.views
{
    /// <summary>
    /// Interaction logic for Server.xaml
    /// </summary>
    public partial class Server : Page
    {
        private static Mutex mut = new Mutex();
        private static bool serverStarted = false;    

        /// <summary>
        /// Server class in order to start listening for a client
        /// </summary>
        public Server()
        {
            InitializeComponent();
            StartServ.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugStartServ");
        }

        /// <summary>
        /// Function that prevent enter string in IP and port
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Button that can start the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
    
        private void Server_Click(object sender, RoutedEventArgs e)
        {
            if (!serverStarted)
            {
                serverStarted = true;   
                string ip = IP.Text;
                Int32 port = Convert.ToInt32(Port.Text);

                //mut.WaitOne();
                SocketListener sl = new SocketListener();
                Thread thr = new Thread(() =>
                { 
                    sl.StartServer(ip, port);
                });
                thr.Start();
                MessageBox.Show("Serveur lancé!");
            }

            //mut.ReleaseMutex();
        }

    }
}
