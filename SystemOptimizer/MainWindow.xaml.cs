using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace SystemOptimizer
{
    public partial class MainWindow : Window
    {
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeCounters();
        }

        private void InitializeCounters()
        {
            // Inicjalizacja liczników wydajności Windows
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            // Ustawienie maksymalnej wartości paska RAM (zakładamy 16GB = 16384 MB)
            RamProgressBar.Maximum = 16384;

            // Uruchomienie zegara, który odświeża interfejs co 1 sekundę
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // --- 1. Aktualizacja procesora (CPU) ---
            int cpuUsage = (int)cpuCounter.NextValue();
            CpuProgressBar.Value = cpuUsage;
            CpuText.Text = $"{cpuUsage}%";

            // --- 2. Aktualizacja pamięci RAM ---
            int ramAvailable = (int)ramCounter.NextValue();
            RamProgressBar.Value = 16384 - ramAvailable; // Pokazujemy ilość zajętą
            RamText.Text = $"Zajęto: {16384 - ramAvailable} MB / Wolne: {ramAvailable} MB";

            // --- 3. Aktualizacja pojemności dysku C: ---
            try
            {
                DriveInfo cDrive = new DriveInfo("C");
                if (cDrive.IsReady)
                {
                    long totalGb = cDrive.TotalSize / 1073741824; // Konwersja bajtów na GB
                    long freeGb = cDrive.AvailableFreeSpace / 1073741824;
                    long usedGb = totalGb - freeGb;

                    DiskProgressBar.Maximum = totalGb;
                    DiskProgressBar.Value = usedGb;
                    DiskText.Text = $"Wolne: {freeGb} GB / {totalGb} GB";
                }
            }
            catch
            {
                // Ciche zignorowanie błędu, jeśli dysk jest w danej ułamku sekundy zablokowany przez system
            }
        }

        private void OpenTools_Click(object sender, RoutedEventArgs e)
        {
            // Otwieranie drugiego okna z narzędziami czyszczącymi
            ToolsWindow toolsWindow = new ToolsWindow();
            toolsWindow.Show();
        }

        private void ShowDiskInfo_Click(object sender, RoutedEventArgs e)
        {
            // Odczyt sprzętowych informacji o dysku za pomocą WMI
            try
            {
                string info = "Fizyczne dyski podłączone do komputera:\n\n";

                // Pytamy system o listę fizycznych nośników
                System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

                foreach (System.Management.ManagementObject wmi_HD in searcher.Get())
                {
                    string model = wmi_HD["Model"]?.ToString();
                    string interfaceType = wmi_HD["InterfaceType"]?.ToString(); // SATA, NVMe, USB itp.
                    string serial = wmi_HD["SerialNumber"]?.ToString()?.Trim();

                    info += $"Model:\t{model}\nInterfejs:\t{interfaceType}\nNr seryjny:\t{serial}\n-----------------------------------\n";
                }

                System.Windows.MessageBox.Show(
                    info,
                    "Szczegółowe informacje o dysku",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Wystąpił błąd odczytu danych sprzętowych.\nUpewnij się, że zainstalowano pakiet System.Management z NuGet.\n\nSzczegóły: {ex.Message}",
                    "Błąd WMI",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }
    }
}