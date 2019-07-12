using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;

namespace MeteringSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static double value = -1;
        private static int objectNum = 0;
        private int numObjects = -1;
        private readonly Random r = new Random();

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Proveri broj objekata pod monitoringom
            this.askForCount();
            //Pocni prijavljivanje novih vrednosti za objekte
            this.startReporting();
        }

        private void askForCount()
        {
            try
            {
                //Pita koliko aplikacija ima objekata
                //Request
                int port = 25565;
                var client = new TcpClient("localhost", port);
                byte[] data = System.Text.Encoding.ASCII.GetBytes("Need object count");
                var stream = client.GetStream();
                stream.Write(data, 0, data.Length);

                //Obrada odgovora
                //Response
                byte[] responseData = new byte[1024];
                string response = "";
                int bytess = stream.Read(responseData, 0, responseData.Length);
                response = System.Text.Encoding.ASCII.GetString(responseData, 0, bytess);

                //Parsiranje odgovora u int vrednost
                this.numObjects = int.Parse(response);

                //Zatvaranje konekcije
                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
        }

        private void startReporting()
        {
            //Na radnom vreme posalji izmenu vrednosti nekog random objekta i nastavi da to radis u rekurziji
            int waitTime = this.r.Next(1000, 5000);
            Task.Delay(waitTime).ContinueWith(_ =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    //Slanje izmene stanja nekog objekta
                    this.sendReport();
                    //Upis u text box, radi lakse provere
                    this.textBox.Text = "Object_" + objectNum + " changed state to: " + value.ToString() + "\n" + this.textBox.Text;
                    //Pocni proces ispocetka
                    this.startReporting();
                });
            });
        }

        private void sendReport()
        {
            try
            {
                //Slanje nove vrednosti objekta
                //Request
                int port = 25565;
                var client = new TcpClient("localhost", port);
                int rInt = this.r.Next(0, this.numObjects); //Brojimo od nule, maxValue nije ukljucen u range
                objectNum = rInt;
                value = this.r.Next(150, 450); //Uzete su nasumicne i realne vrednosti
                byte[] data = System.Text.Encoding.ASCII.GetBytes("Objekat_" + rInt + ":" + value);
                var stream = client.GetStream();
                stream.Write(data, 0, data.Length);

                //Zatvaranje konekcije
                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}