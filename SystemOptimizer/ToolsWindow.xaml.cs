using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SystemOptimizer
{
    public partial class ToolsWindow : Window
    {
        public ToolsWindow()
        {
            InitializeComponent();
        }

        private void RunCommandSilent(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                CreateNoWindow = true,
                UseShellExecute = true,
                Verb = "runas"
            };
            Process.Start(psi);
        }

        private void CleanWinStore_Click(object sender, RoutedEventArgs e)
        {
            RunCommandSilent("wsreset.exe");
            System.Windows.MessageBox.Show("Czyszczenie cache Windows Store rozpoczęte.");
        }

        private void CleanWinUpdate_Click(object sender, RoutedEventArgs e)
        {
            string cmd = "net stop wuauserv && del /s /q /f %windir%\\SoftwareDistribution\\Download\\* && net start wuauserv";
            RunCommandSilent(cmd);
            System.Windows.MessageBox.Show("Zadanie czyszczenia Windows Update zostało uruchomione.");
        }

        private void CleanDrivers_Click(object sender, RoutedEventArgs e)
        {
            string cmd = "DISM /online /cleanup-image /spsuperseded";
            RunCommandSilent(cmd);
            System.Windows.MessageBox.Show("Zadanie usuwania starych sterowników (DISM) uruchomione.");
        }

        private void CleanWindowsOld_Click(object sender, RoutedEventArgs e)
        {
            string cmd = "rd /s /q %systemdrive%\\Windows.old";
            RunCommandSilent(cmd);
            System.Windows.MessageBox.Show("Komenda usunięcia folderu Windows.old wysłana.");
        }

        private void DeleteStubbornFolder_Click(object sender, RoutedEventArgs e)
        {
            // Wywołujemy okno dialogowe z biblioteki WinForms
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = "Wybierz oporny folder do usunięcia";
                dialog.UseDescriptionForTitle = true;

                // Jeśli użytkownik wybrał folder i kliknął "OK"
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Pobieramy ścieżkę wybraną przez użytkownika
                    string targetFolder = dialog.SelectedPath;

                    // Konstruujemy naszą komendę
                    string cmd = $"takeown /f \"{targetFolder}\" /r /d y && " +
                                 $"icacls \"{targetFolder}\" /grant administrators:F /t /c /q && " +
                                 $"rd /s /q \"{targetFolder}\"";

                    RunCommandSilent(cmd);

                    // Informacja zwrotna
                    System.Windows.MessageBox.Show(
                        $"Rozpoczęto ciche przejmowanie uprawnień i usuwanie dla lokalizacji:\n\n{targetFolder}",
                        "Usuwanie w toku",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);
                }
            }
        }
    }
}