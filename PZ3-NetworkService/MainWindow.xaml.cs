using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace PZ3_NetworkService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int count = 15; // Inicijalna vrednost broja objekata u sistemu
                                // ######### ZAMENITI stvarnim brojem elemenata

        public MainWindow()
        {
            this.InitializeComponent();
            this.createListener(); //Povezivanje sa serverskom aplikacijom
        }

        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25565);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        var stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            this.count = Database.ReactorIds.Count;
                            byte[] data = System.Text.Encoding.ASCII.GetBytes(this.count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Debug.WriteLine(incomming); //Na primer: "Objekat_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji
                            var match = Regex.Match(incomming, @"Objekat_(\d+):([0-9.]+)");
                            if (int.TryParse(match.Groups[1].Value, out int objectIndex))
                            {
                                if (objectIndex < 0 || objectIndex >= Database.ReactorIds.Count)
                                {
                                    Trace.TraceError($"Value \"{incomming}\" from server is out of bounds!");
                                }
                                else if (double.TryParse(match.Groups[2].Value, out double newVal))
                                {
                                    int id = Database.ReactorIds[objectIndex];
                                    Database.Reactors[id].Temperature = newVal;
                                    string logStr = Log.ConvertToLogFormat(id, newVal);
                                    Log.Append(logStr);
                                }
                                else
                                {
                                    Trace.TraceError($"Could not parse \"{match.Groups[2].Value}\" to integer.");
                                }
                            }
                            else
                            {
                                Trace.TraceError($"Could not parse \"{match.Groups[1].Value}\" to integer.");
                            }
                        }
                    }, null);
                }
            })
            {
                IsBackground = true
            };
            listeningThread.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var context = this.DataContext as IClosing;
            if (context != null)
            {
                e.Cancel = !context.OnClosing();
            }
        }
    }
}