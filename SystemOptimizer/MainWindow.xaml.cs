using System;
using System.Diagnostics;
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
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            // RAM Maximum (Dla uproszczenia załóżmy, że system ma 16GB. W pełnej wersji pobierz z WMI)
            RamProgressBar.Maximum = 16384;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            int cpuUsage = (int)cpuCounter.NextValue();
            int ramAvailable = (int)ramCounter.NextValue();

            CpuProgressBar.Value = cpuUsage;
            CpuText.Text = $"{cpuUsage}%";

            RamProgressBar.Value = 16384 - ramAvailable; // Pokazujemy użycie
            RamText.Text = $"Zajęto: {16384 - ramAvailable} MB / Wolne: {ramAvailable} MB";
        }

        private void OpenTools_Click(object sender, RoutedEventArgs e)
        {
            ToolsWindow toolsWindow = new ToolsWindow();
            toolsWindow.Show();
        }
    }
}