using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LegacyInstaller.Patcher;
using LegacyInstaller.Utils;
using LegacyInstaller.Versions;
using Version = LegacyInstaller.Versions.Version;

namespace LegacyInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var beatSaberDir = Directories.GetBeatSaberDirectory();
            if (beatSaberDir != null)
            {
                BeatSaber_Input.Text = beatSaberDir;
            }

            var steamDir = Directories.GetSteamDirectory();
            if (steamDir != null)
            {
                Steam_Input.Text = steamDir;
            }

            if (VersionManager.Versions != null)
            {
                Version_DropDown.ItemsSource = VersionManager.Versions;
                Version_DropDown.SelectedIndex = 0;
            }

            RefreshUI();
        }

        #region Methods
        private void RefreshUI()
        {
            bool valid = ValidateUI();
            Install_Button.IsEnabled = valid;
        }

        private bool ValidateUI()
        {
            if (!Directories.CheckBeatSaberDirectory(BeatSaber_Input.Text))
            {
                return false;
            }

            if (!Directories.CheckSteamDirectory(Steam_Input.Text))
            {
                return false;
            }

            var selectedVersion = (Version)Version_DropDown.SelectedItem;
            if (selectedVersion == null)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Events
        private void BeatSaber_Browse_Click(object sender, RoutedEventArgs e)
        {
            var beatSaberDir = Dialogs.ShowFolderDialog(this, BeatSaber_Input.Text);
            if (!string.IsNullOrEmpty(beatSaberDir))
            {
                if (Directories.CheckSteamDirectory(beatSaberDir))
                {
                    BeatSaber_Input.Text = beatSaberDir;
                }
                else
                {
                    Dialogs.ShowErrorDialog(this, "Directory Error", "Invalid Beat Saber Directory");
                }
            }
        }

        private void Steam_Browse_Click(object sender, RoutedEventArgs e)
        {
            var steamInstallDir = Dialogs.ShowFolderDialog(this, Steam_Input.Text);
            if (!string.IsNullOrEmpty(steamInstallDir))
            {
                if (Directories.CheckSteamDirectory(steamInstallDir))
                {
                    Steam_Input.Text = steamInstallDir;
                }
                else
                {
                    Dialogs.ShowErrorDialog(this, "Directory Error", "Invalid Steam Directory");
                }
            }
        }

        private void Version_DropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshUI();
        }

        private void Install_Button_Click(object sender, RoutedEventArgs e)
        {
            SteamPatcher.ApplyPatch();
        }
        #endregion
    }
}
