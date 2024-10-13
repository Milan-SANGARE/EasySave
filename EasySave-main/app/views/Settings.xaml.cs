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
using System.IO;
using Microsoft.Win32;
using EasySaveV2.app.viewModel;
using System.Text.RegularExpressions;
using System.Linq;
using EasySaveV2;
using EasySaveV2.app.models;

namespace EasySaveV2.app.views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        List<string> chifExt = new List<string>();
        List<string> PriofExt = new List<string>();
        /// <summary>
        /// class Settings to set settings
        /// </summary>
        public Settings()
        {

            InitializeComponent();
            updateLanguage();
            //Combobox type of logs
            app.models.Logs check = EasySaveV2.app.models.Logs.getInstance();
            if (check.logType == "json")
            {
                json.IsChecked = true;
            }
            else
            {
                xml.IsChecked = true;
            }
            //init içi avec fichier config le reste
            configFile readConf = UtilsController.readConfFile();
            ChoixCryptage.ItemsSource = readConf.extensionChiffre;
            ChoixPrio.ItemsSource = readConf.extensionPriority;
            choice_key.Text = readConf.key;
            workJob.Text = readConf.process;
            numberThreads.Text = readConf.threads.ToString();
            maxSizeFile.Text = readConf.maxSize.ToString();
            blockSize.Text = readConf.blockSize.ToString();

        }
        /// <summary>
        /// update the language (fr or en)
        /// </summary>
        public void updateLanguage()
        {
            //Settings page
            jobSoftware.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugChoiceOfJobSoftware");
            Extensions.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugChoiceOfExtensions");
            ExtensionsPrio.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugExtPrio");

            AddExtension.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugAddExtension");
            ChoixExtPrio.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugChoixExtPrio");

            ExtAjout.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugAdd");
            AddChoixPrio.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugAdd");
            SupChoixCrypt.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugDeleteSave");
            SupChoixPrio.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugDeleteSave");

            MaxSize.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugMaxSize");
            Blocksize.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugBlocksize");
            ThreadNumber.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugThreadNumber");
            LogFormat.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugLogFormat");
            SaveConfig.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugSaveConfig");
            key.Content = EasySaveV2.app.models.Language.getInstance().getSlug("slugKey");
        }

        /// <summary>
        /// RadioButton that call singleton to change log type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (json.IsChecked == true)
            {
                app.models.Logs.getInstance("json");
            }

            else if (xml.IsChecked == true)
            {
                app.models.Logs.getInstance("xml");

            }
        }
        /// <summary>
        /// Button that add extensions to crypt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void ExtAjout_Click(object sender, RoutedEventArgs e)
        {
            chifExt = ChoixCryptage.Items.Cast<string>().ToList();
            chifExt.Add(InputExt.Text);
            ChoixCryptage.ItemsSource = chifExt;
            InputExt.Text = ".";
        }
        /// <summary>
        /// Button that add extensions to priorize
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddChoixPrio_Click(object sender, RoutedEventArgs e)
        {

            PriofExt = ChoixPrio.Items.Cast<string>().ToList();
            PriofExt.Add(InputPrio.Text);
            ChoixPrio.ItemsSource = PriofExt;
            InputPrio.Text = ".";
            
        }
        /// <summary>
        /// Button that delete selected extension to crypt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SupChoixCrypt_Click(object sender, RoutedEventArgs e)
        {
            if (ChoixCryptage.SelectedItems.Count == 1)
            {

                chifExt = ChoixCryptage.Items.Cast<string>().ToList();
                chifExt.Remove(ChoixCryptage.SelectedItem.ToString());
                ChoixCryptage.ItemsSource = chifExt;
            }

        }
        /// <summary>
        /// Button that delete selected extension to priorize
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void SupChoixPrio_Click(object sender, RoutedEventArgs e)
        {
            if (ChoixPrio.SelectedItems.Count == 1)
            {
                PriofExt = ChoixPrio.Items.Cast<string>().ToList();
                PriofExt.Remove(ChoixPrio.SelectedItem.ToString());
                ChoixPrio.ItemsSource = PriofExt;
            }
        }

        /// <summary>
        /// Function that prevent write characters in certains TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Button that save config to the config file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void SaveConfig_Click_2(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(choice_key.Text) || String.IsNullOrEmpty(workJob.Text) || String.IsNullOrEmpty(numberThreads.Text) || String.IsNullOrEmpty(maxSizeFile.Text))
            {

                string message = EasySaveV2.app.models.Language.getInstance().getSlug("slugEmptyError");
                string title = EasySaveV2.app.models.Language.getInstance().getSlug("slugError");
                MessageBox.Show(message, title);
            }
            else
            {
                chifExt = ChoixCryptage.Items.Cast<string>().ToList();
                PriofExt = ChoixPrio.Items.Cast<string>().ToList();
                UtilsController.writeConfFile(chifExt, PriofExt, workJob.Text, Convert.ToInt32(maxSizeFile.Text), Convert.ToInt32(numberThreads.Text), choice_key.Text, Convert.ToInt32(blockSize.Text));
            }
        }

        /// <summary>
        /// Function that let .exe in process TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void workJob_TextChanged(object sender, TextChangedEventArgs e)
        {
            Regex regex = new Regex(".exe");
            if (regex.IsMatch(workJob.Text))
            {

            }
            else
            {
                workJob.Text += ".exe";
            }
        }
    }
}
