using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;  
using EasySaveV2.app.viewModel;
using EasySaveV2.app.models;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Interop;
using System.Web;
using EasySaveV2.app.views;
using System.Windows.Forms;


namespace EasySaveV2.app.views
{
    /// <summary>
    /// Interaction logic for CreateSave.xaml
    /// </summary>
    public partial class CreateSave : Page
    {
        public string uuid = null;
        public static bool isLoaded = false;
        public CreateSave()
        {
            InitializeComponent();
            updateLanguage();
            Complet.IsChecked = true;
        } /// <summary>
        /// class for create / edit a save
        /// </summary>
        /// <param name="taskName"></param>
        public CreateSave(string taskName)
        {
            InitializeComponent();

            updateLanguage();
            BackupTask task = BackupTaskViewModel.readBackupTask(taskName);
            uuid = task.uuid;  
            SaveNameEnter.Text = task.name;
            SourceSave.Text = task.source;
            TargetSave.Text = task.target;
            if (task.type == "complet")
            {
                Complet.IsChecked = true;

            }
            else
            {
                Differential.IsChecked = true;
            }

        }
        /// <summary>
        /// update the language (fr or en)
        /// </summary>
        public void updateLanguage()
        {
            NameSave.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugSavingName");
            TypeOfSave.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugSavingTypeOfSave");
            Complet.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugCompletType");
            Differential.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugDifferentialType");

            SaveSource.Text = EasySaveV2.app.models.Language.getInstance().getSlug("slugSavingSource");
            SaveTarget.Text = EasySaveV2.app.models.Language.getInstance().getSlug("slugSavingTarget");
            Save.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugsavechanges");

        }
        /// <summary>
        /// Button that create a save with param in textBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void CreateSave1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SaveNameEnter.Text) || string.IsNullOrEmpty(SourceSave.Text) || string.IsNullOrEmpty(TargetSave.Text))
            {
                System.Windows.MessageBox.Show(EasySaveV2.app.models.Language.getInstance().getSlug("slugEmptyError"), EasySaveV2.app.models.Language.getInstance().getSlug("slugError"));
            }
            else
            {
                string typeSave = "";
                if (Complet.IsChecked == true)
                {
                    typeSave = "complet";
                }

                else if (Differential.IsChecked == true)
                {
                    typeSave = "differentiel";
                }

                if (uuid != null)
                {
                    BackupTask task = BackupTaskViewModel.readBackupTask(uuid);
                    BackupTaskViewModel.updateBackupTask(task.uuid, SaveNameEnter.Text, SourceSave.Text, TargetSave.Text, typeSave, task.progress);
                }
                else
                {
                    BackupTaskViewModel.createBackupTask(SaveNameEnter.Text, SourceSave.Text, TargetSave.Text, typeSave);
                }
                Uri uri = new Uri("app\\views\\SaveManage.xaml", UriKind.Relative);
                this.NavigationService.Navigate(uri);
            }
        }
        /// <summary>
        /// Button that open explorer to select source path to save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFiles_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult result = folderBrowser.ShowDialog();
            SourceSave.Text = folderBrowser.SelectedPath;
        }
        /// <summary>
        /// Button that open explorer to select target path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetFiles_Click(object sender, RoutedEventArgs e)
        {

            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult result = folderBrowser.ShowDialog();
            TargetSave.Text = folderBrowser.SelectedPath;
        }

    }
}
