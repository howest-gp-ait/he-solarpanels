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
using System.Windows.Threading;
using System.Net.Sockets;
using System.Net;
using AIT.SolarPanels.Server.Core.SocketHelpers;
using AIT.SolarPanels.Server.Core.Services;
using AIT.SolarPanels.Server.Core.Entities;

namespace AIT.SolarPanels.Server.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SolarPanelService solarPanelService = new SolarPanelService();
        Socket mainSocket;
        IPEndPoint mainEndPoint;
        bool serverOnline;
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Event Handlers        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartupConfig();
            PopulateStations();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BtnStopServer_Click(null, null);
        }
        private void CmbIPs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveConfig();
        }
        private void CmbPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveConfig();
        }
        private void BtnStartServer_Click(object sender, RoutedEventArgs e)
        {
            btnStartServer.Visibility = Visibility.Hidden;
            btnStopServer.Visibility = Visibility.Visible;
            cmbIPs.IsEnabled = false;
            cmbPorts.IsEnabled = false;
            serverOnline = true;
            StartListening();
        }

        private void BtnStopServer_Click(object sender, RoutedEventArgs e)
        {
            btnStartServer.Visibility = Visibility.Visible;
            btnStopServer.Visibility = Visibility.Hidden;
            cmbIPs.IsEnabled = true;
            cmbPorts.IsEnabled = true;

            serverOnline = false;
            try
            {
                if (mainSocket != null)
                    mainSocket.Close();
            }
            catch
            { }
            mainSocket = null;
            mainEndPoint = null;
        }

        private void CmbStations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstPanels.ItemsSource = null;
            lstPanels.Items.Refresh();
            grpDetails.Visibility = Visibility.Hidden;
            if (cmbStations.SelectedItem != null)
            {
                SolarStation solarStation = (SolarStation)cmbStations.SelectedItem;
                lstPanels.ItemsSource = solarStation.SolarPanels;
                lstPanels.Items.Refresh();
                sldSuncondition.Value = solarStation.Suncondition;
                lblStationId.Content = solarStation.StationId;
                lblStationPerformance.Content = solarStation.StationPerformance.ToString("0.00");
                lblTotalPerformance.Content = solarPanelService.TotalPerformance.ToString("0.00");
                grpDetails.Visibility = Visibility.Visible;
                
            }
        }

        #endregion

        #region Methods
        public static void DoEvents()
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch (Exception fout)
            {
                System.Windows.Application.Current.Dispatcher.DisableProcessing();
            }
        }
        private void StartupConfig()
        {
            cmbIPs.ItemsSource = IPv4.GetActiveIP4s();
            for (int port = 49200; port <= 49500; port++)
            {
                cmbPorts.Items.Add(port);
            }
            Config.GetConfig(out string savedIP, out int savedPort);
            try
            {
                cmbIPs.SelectedItem = savedIP;
            }
            catch
            {
                cmbIPs.SelectedItem = "127.0.0.1";
            }
            try
            {
                cmbPorts.SelectedItem = savedPort;
            }
            catch
            {
                cmbPorts.SelectedItem = 49200;
            }
            btnStartServer.Visibility = Visibility.Visible;
            btnStopServer.Visibility = Visibility.Hidden;
        }
        private void SaveConfig()
        {
            if (cmbIPs.SelectedItem == null) return;
            if (cmbPorts.SelectedItem == null) return;

            string ip = cmbIPs.SelectedItem.ToString();
            int port = int.Parse(cmbPorts.SelectedItem.ToString());
            Config.WriteConfig(ip, port);
        }
        private void PopulateStations()
        {
            cmbStations.ItemsSource = solarPanelService.SolarStations;
            cmbStations.Items.Refresh();

        }

        private void StartListening()
        {
            // lees het geselecteerde IP nummer uit (cmbIps)
            // lees het geselecteerde poortnummer uit (cmbPorts)
            // instantieer de globale variabele "mainEntPoint" (zie helemaal bovenaan)
            // instantieer de globale variabele "mainSocket" (zie helemaal bovenaan)
            // voorzie hier de nodige logica zodat deze applicatie "luistert" naar
            //   eventueel binnenkomende berichten

            // als er door een client communicatie wordt opgezet, dan stuur je deze
            // meteen door naar de methode HandleClientCall in de vorm van een socket
            // (de nog door jou aan te maken methode hieronder)

        }
        private void HandleClientCall(Socket clientCall)
        {
            // haal hier het bericht uit de aangeleverde socket (hier dus "clientCall").
            // denk er aan : het bericht zal eindigen met het symbool "#@@#" !
            //
            // van zodra je het bericht hebt uitgelezen stuur je het naar de
            // (reeds aanwezige) solarPanelService.ProcessMessage methode.

            // deze methode verwacht 2 argumenten : 
            //  - het bericht (string)
            //  - een outputparameter van het type string dat het stationId (dat in je
            //    bericht zit) zal teruggeven (*)
            // de methode retourneert een bool? (nullable boolean)
            //  - is deze retourwaarde = null =>            //         
            //        er wordt NIETS naar de client teruggestuurd
            //        roep de bestaande methode "PopulateStations" op
            //        zorg er voor dat geen enkel station geselecteerd staat in CmbStations
            //        onderbreek deze (HandleClient) methode
            //  - is deze retourwaarde = false =>
            //       stuur de tekst "ERROR#@@#" terug naar de client
            //  - is deze retourwaarde = true => 
            //       stuur de tekst "OK#@@#" terug naar de client
            // (*) ben je hiermee klaar, bekijk dan de inhoud van de outputparameter 
            //  - is deze = null => 
            //        roep de bestaande methode "PopulateStations" op
            //        zorg er voor dat geen enkel station geselecteerd staat in CmbStations
            //  - is deze <> null =>
            //        roep de bestaande methode "PopulateStations" op
            //        zorg er voor dat het betrokken station geselecteerd staat in CmbStations
            //        en zorg er voor dat zijn panelen getoond worden 





        }
        #endregion


    }
}
