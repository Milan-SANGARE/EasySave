using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.IO;
using Microsoft.Win32;
using EasySaveV2.app.views;
using System.Linq;
using System.Diagnostics;
using EasySaveV2.app.models;
using EasySaveV2.app.viewModel;

namespace EasySaveV2
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

       
        public MainWindow()
        {
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                string message = "L'application est déjà lancée!";
                string title = "Erreur";
                MessageBox.Show(message, title);
                Environment.Exit(-1);
            }
            else
            {
                InitializeComponent();

                Language.Text = EasySaveV2.app.models.Language.getInstance().getSlug("slugLanguage");
                updateSelectChoicesLanguage();
                updateLanguage();
                SaveManage sv = new SaveManage();
                FrameMain.Content = sv;

                //Thread thrd = new Thread(sv.UpdateFront);
                //thrd.Start();
                foreach(string file in Directory.GetFiles(Environment.CurrentDirectory + "..\\..\\..\\..\\src"))
                {
                    Trace.WriteLine(file);

                }
            }

        }

        public void updateLanguage()
        {
            Settings.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugSettings");
            LogsDaily.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugLogDaily");
            LogsState.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugLogState");
            Server.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugServer");
            Save.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugManageSave");
            Language.Text = EasySaveV2.app.models.Language.getInstance().getSlug("slugLanguage");
        }

        private void LanguageSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            //value = Directory.GetFiles("..\\..\\..\\src\\langue")[LanguageComboBox.SelectedIndex];
            //value= value.Substring(0,value.Length-5);
            //Trace.WriteLine(value);
            string value = LanguageComboBox.SelectedItem.ToString();
            value = value.Substring(value.Length - 2);// triché pour récupéré les deux dernier caractères, il faudrait récuperer le contenu
            Trace.WriteLine(value);
            EasySaveV2.app.models.Language.getInstance(value);
            updateLanguage();
            FrameMain.Content = new SaveManage();
        }

        public void updateSelectChoicesLanguage()
        {
            string[] languages = Directory.GetFiles("..\\..\\..\\src\\langue");
            for (int x = 0; x < languages.Length; x++)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = System.IO.Path.GetFileNameWithoutExtension(languages[x]);
                LanguageComboBox.Items.Add(cbi);
            }
            LanguageComboBox.SelectedIndex = 0;
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            FrameMain.Content = new SaveManage();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            FrameMain.Content = new Settings();
        }

        private void LogsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Logs.IsEditable = true;
            var selectedItem = (ComboBoxItem)Logs.SelectedItem;

            if (Logs.SelectedItem == null) return;

            else if ((string)selectedItem.Tag == "daily_Logs")
            {
                Microsoft.Win32.OpenFileDialog selectFile = new Microsoft.Win32.OpenFileDialog();
                selectFile.Filter = "All files (*.json;*.xml)|*.json;*.xml";
                string CombinedPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\src\\log\\");
                selectFile.InitialDirectory = System.IO.Path.GetFullPath(CombinedPath);
                selectFile.ShowDialog();
                if(selectFile.FileName != "")
                {

                    var p = new Process();
                    p.StartInfo = new ProcessStartInfo(selectFile.FileName)
                    {
                        UseShellExecute = true
                    };
                    p.Start();
                }
            }
            else if ((string)selectedItem.Tag == "state_Logs")
            {

                var p = new Process();
                p.StartInfo = new ProcessStartInfo("..\\..\\..\\src\\config\\backups.json")
                {
                    UseShellExecute = true
                };
                p.Start();
            }

        }

        private void Server_Click(object sender, RoutedEventArgs e)
        {
            FrameMain.Content = new Server();
        }
    }

}
