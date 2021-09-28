using System;
using System.Windows;
using Ookii.Dialogs.Wpf;

namespace LegacyInstaller.Utils
{
    internal static class Dialogs
    {
        public static void ShowErrorDialog(Window window, string title, string content, string extra = null)
        {
            using (var task = new TaskDialog())
            {
                task.WindowTitle = title;
                task.Content = content;
                task.ExpandedInformation = extra;

                task.MainIcon = TaskDialogIcon.Error;
                task.Buttons.Add(new TaskDialogButton(ButtonType.Ok));

                _ = task.ShowDialog(window);
            }
        }

        public static string ShowFolderDialog(Window window, string currentPath)
        {
            var dialog = new VistaFolderBrowserDialog()
            {
                SelectedPath = currentPath,
            };

            if ((bool)dialog.ShowDialog(window))
            {
                return dialog.SelectedPath;
            }
            else
            {
                return null;
            }
        }
    }
}
