using LHEInstaller.API;
using LHEInstaller.Core;
using RestSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;

namespace LHEInstaller
{
    public partial class MainWindow : Window
    {
        private readonly RestClient _client;
        private readonly Telemetry _telemetry = new Telemetry();
        private const string InstallFolder = @"C:\Program Files\LHE";
        private const string ExecutableName = "LHE.exe";
        private const string ProcessNameWithoutExtension = "LHE";
        private const string DownloadUrl =
            "https://api.secvers.org/v1/downloads/lhe";
        public MainWindow()
        {
            InitializeComponent();
            _client = new RestClient("https://api.secvers.org");
            NextButton.IsEnabled = false;
        }
        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (AcceptTermsCheckBox.IsChecked.GetValueOrDefault())
            {
                await API.Certificate.DownloadAndInstallAsync(true);

                TermsScrollViewer.Visibility = Visibility.Hidden;
                PrivacyPolicyText.Visibility = Visibility.Visible;
                AcceptPrivacyPolicyCheckBox.Visibility = Visibility.Visible;
                OepnPrivacy.Visibility = Visibility.Visible;
                AcceptTermsCheckBox.Visibility = Visibility.Hidden;
                NextButton.Visibility = Visibility.Hidden;
                InstallButton.Visibility = Visibility.Visible;
            }
        }

        private async void StartInstallation()
        {
            await _telemetry.SendTelemetryAsync();
            InstallProgress.Visibility = Visibility.Visible;
            InstallStatus.Visibility = Visibility.Visible;

            await DownloadAndInstallAsync();
        }

        private string GetInstallPath() =>
           Path.Combine(InstallFolder, ExecutableName);

        private async Task DownloadAndInstallAsync()
        {
            string tempFile = Path.GetTempFileName();

            using (var http = new HttpClient())
            {
                http.Timeout = TimeSpan.FromMinutes(5);

                using (var response = await http.GetAsync(DownloadUrl))
                {
                    response.EnsureSuccessStatusCode();

                    using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fs);
                    }
                }
            }

            Directory.CreateDirectory(InstallFolder);

            string targetPath = GetInstallPath();

            if (File.Exists(targetPath))
            {
                var processes = Process.GetProcessesByName(ProcessNameWithoutExtension);
                if (processes.Length > 0)
                {
                    MessageBox.Show(
                        "The LHE process appears to be running.\n\n" +
                        "To avoid potential system instability, close it from within the LHE application " +
                        "before continuing.",
                        "LHE Running",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }

            File.Copy(tempFile, targetPath, true);
            File.SetAttributes(targetPath, FileAttributes.Normal);

            AutoStart.EnableAutoStart();

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = targetPath,
                    Verb = "runas",
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            InstallStatus.Text = "Installation Complete!";
            MessageBox.Show("Installation completed successfully!");
        }

        private void AcceptTermsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            NextButton.IsEnabled = true;
        }

        private void AcceptTermsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            NextButton.IsEnabled = false;
        }

        private void AcceptPrivacyPolicyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            InstallButton.IsEnabled = true;
        }

        private void AcceptPrivacyPolicyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            InstallButton.IsEnabled = false;
        }

        private async void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            InstallButton.IsEnabled = false;
            var confirm = MessageBox.Show("Are you sure you want to start the installation?", "Confirm Installation", MessageBoxButton.YesNo);

            if (confirm == MessageBoxResult.Yes)
                StartInstallation();
            else
                InstallButton.IsEnabled = true;
        }
    }
}
