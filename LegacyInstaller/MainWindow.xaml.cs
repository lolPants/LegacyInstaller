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
using Ookii.Dialogs.Wpf;
using LegacyInstaller.Utils;
using LegacyInstaller.Versions;

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
        }

        private void BeatSaber_Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog()
            {
                SelectedPath = BeatSaber_Input.Text,
            };

            if ((bool)dialog.ShowDialog(this))
            {
                var beatSaberDir = dialog.SelectedPath;
                if (Directories.CheckSteamDirectory(beatSaberDir))
                {
                    BeatSaber_Input.Text = beatSaberDir;
                }
                else
                {
                    using (var task = new TaskDialog())
                    {
                        task.WindowTitle = "Directory Error";
                        task.Content = "Invalid Beat Saber Directory";

                        task.MainIcon = TaskDialogIcon.Error;
                        task.Buttons.Add(new TaskDialogButton(ButtonType.Ok));

                        _ = task.ShowDialog(this);
                    }
                }
            }
        }

        private void Steam_Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog()
            {
                SelectedPath = Steam_Input.Text,
            };

            if ((bool)dialog.ShowDialog(this))
            {
                var steamDir = dialog.SelectedPath;
                if (Directories.CheckSteamDirectory(steamDir))
                {
                    Steam_Input.Text = steamDir;
                }
                else
                {
                    using (var task = new TaskDialog())
                    {
                        task.WindowTitle = "Directory Error";
                        task.Content = "Invalid Steam Directory";

                        task.MainIcon = TaskDialogIcon.Error;
                        task.Buttons.Add(new TaskDialogButton(ButtonType.Ok));

                        _ = task.ShowDialog(this);
                    }
                }
            }
        }
    }
}
