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
using EasySaveV2.app.models;
using EasySaveV2.app.viewModel;
using EasySaveV2.app.views;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Threading;
using System.Windows.Forms;
using ProgressBar = System.Windows.Forms.ProgressBar;
using System.Diagnostics;

namespace EasySaveV2
{

    /// <summary>
    /// Interaction logic for SaveManege.xaml
    /// </summary>
    public partial class SaveManage : Page
    {

        public static Mutex backupsJsonMutex = new Mutex();
        private BackupTaskList backupTaskList = BackupTaskViewModel.readBackupTasks();
        /// <summary>
        /// Class page Backup management
        /// </summary>
        public SaveManage()
        {
            InitializeComponent();
            updateLanguage();
            LoadSaves.ItemsSource = backupTaskList.taskList;
            LoadSaves.SelectedItem = null;
            Thread thrd = new Thread(this.UpdateFront);
            thrd.Start();

        }
        /// <summary>
        /// update the language (fr or en)
        /// </summary>
        public void updateLanguage()
        {
            Start.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugStartSave");
            Stop.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugStopSave");
            Edit.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugEditSave");
            Delete.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugDeleteSave");
            Create.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugCreateSave");

            saveStatus.Header = EasySaveV2.app.models.Language.getInstance().getSlug("slugSaveStatus");
            saveName.Header = EasySaveV2.app.models.Language.getInstance().getSlug("slugSaveName");
            saveSource.Header = EasySaveV2.app.models.Language.getInstance().getSlug("slugSaveSource");
            saveTarget.Header = EasySaveV2.app.models.Language.getInstance().getSlug("slugSaveTarget");
            saveType.Header = EasySaveV2.app.models.Language.getInstance().getSlug("slugSaveType");
            progressBar.Header = EasySaveV2.app.models.Language.getInstance().getSlug("slugProgressBar");
            progressValue.Header = EasySaveV2.app.models.Language.getInstance().getSlug("slugProgressValue");

        }

        /// <summary>
        /// Funtion that update the datagrid which contains backups
        /// </summary>
        public void UpdateFront()
        {
            //Trace.WriteLine("UpdateFront()");

            backupsJsonMutex.WaitOne();
            BackupTaskList btl = BackupTaskViewModel.readBackupTasks();
            backupsJsonMutex.ReleaseMutex();

            System.Windows.Application.Current.Dispatcher.Invoke(
            System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
            {
                // Update UI component here
                if (LoadSaves.SelectedItems.Count == 0)
                {


                    if (LoadSaves.ItemsSource != btl.taskList)
                    {
                        LoadSaves.ItemsSource = btl.taskList;
                        //Trace.WriteLine("front updated");

                    }
                }
            });


            Thread.Sleep(500);
            UpdateFront();
        }
        /// <summary>
        /// Button that delete the selected backup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (LoadSaves.SelectedItems.Count == 1)
            {
                BackupTask task = (BackupTask)LoadSaves.SelectedItems[0];
                BackupTaskViewModel.deleteBackupTask(task.uuid);
                backupTaskList.taskList.RemoveAt(LoadSaves.SelectedIndex);
            }
        }
        /// <summary>
        /// Button that can edit the selected backup, with a redirection to CreateSave
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (LoadSaves.SelectedItems.Count == 1)
            {
                BackupTask task = (BackupTask)LoadSaves.SelectedItems[0];
                //Uri uri = new Uri("app\\views\\CreateSave.xaml?taskId="+task.uuid, UriKind.Relative);
                var page = new CreateSave(task.name);
                NavigationService.Navigate(page);
            }
        }
        /// <summary>
        /// Button that can create a backup with a redirection to CreateSave
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Create_Click(object sender, RoutedEventArgs e)
        {

            Uri uri = new Uri("app\\views\\CreateSave.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);

        }

        /// <summary>
        /// Button that can stop the selected backup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            //suppr le process de start backup 
            //met a jour létat de sauvegarde a stop 
            if (LoadSaves.SelectedItems.Count == 1)
            {

                BackupTask bt = backupTaskList.taskList[LoadSaves.SelectedIndex];
                bt.progress.state = "STOP";
                BackupTaskViewModel.updateBackupTask(bt.uuid, bt.name, bt.source, bt.target, bt.type, bt.progress);
                LoadSaves.SelectedItem = null;
            }
        }
        /// <summary>
        /// Button that can start the selected backup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Click(object sender, RoutedEventArgs e)
        {

            if (LoadSaves.SelectedItems.Count == 1)
            {

                BackupTask bt = backupTaskList.taskList[LoadSaves.SelectedIndex];
                BackupTaskViewModel.startThreadBackupTask(bt, backupsJsonMutex);
                Trace.WriteLine("Start_Click : " + backupTaskList.taskList[LoadSaves.SelectedIndex].name);
                LoadSaves.SelectedItem = null;

            }
        }

        private void LoadSaves_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
