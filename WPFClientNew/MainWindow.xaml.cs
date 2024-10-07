using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
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

namespace WPFClientNew
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string CurrentClientName = string.Empty;
        private NetworkStream stream;
        public MainWindow()
        {
            InitializeComponent();
            //for (int i = 0; i < 5; i++) {
            //    ListBoxItem clientItem = new ListBoxItem();
            //    clientItem.Content = "Client Name" + (i+1); // Replace with the actual client name
            //    ListDisplay.Items.Add(clientItem);
            //}
            
        }

        private void ClientSelected(object sender, SelectionChangedEventArgs e)
        {
            if (ListDisplay.SelectedItem is ListBoxItem selectedItem)
            {
                string clientName = selectedItem.Content.ToString();
                // Open private chat with the clientName
                MessageBox.Show($"Chatting with {clientName}");
            }
        }

        private void clientNameKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                this.CurrentClientName = clientName.Text.ToString();
                
                if (!string.IsNullOrEmpty(CurrentClientName))
                {
                    clientName.IsEnabled = false;
                    ConnectWithServerAsync();
                }
            }
        }
        private async Task ConnectWithServerAsync()
        {
            try
            {
                TcpClient client = new TcpClient("192.168.100.11", 12345); // Update with the server IP
                stream = client.GetStream();
                SetName(CurrentClientName);

                // Start listening for incoming messages after setting the name
                await HandleIncomingMessages(stream);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SetName(string name)
        {
            byte[] data = Encoding.ASCII.GetBytes(name);
            stream.Write(data, 0, data.Length);//send name to server
        }
        private void ReadaFromServer()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                if (stream != null)
                {
                    int byteCount = stream.Read(buffer, 0, buffer.Length);
                    string responseFromServer = Encoding.ASCII.GetString(buffer, 0, byteCount);

                }

            }
        }
        private void ReadFromServer()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                if (stream != null)
                {
                    int byteCount = stream.Read(buffer, 0, buffer.Length);
                    string responseFromServer = Encoding.ASCII.GetString(buffer, 0, byteCount);

                    if (responseFromServer.StartsWith("clientlist:"))
                    {
                        // Update the ListBox with the new client list
                        string clientList = responseFromServer.Substring("clientlist:".Length);
                        UpdateClientList(clientList.Split(','));
                    }
                    else
                    {
                        // Handle chat messages (if any)
                    }
                }
            }
        }

        private async Task HandleIncomingMessages(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];

            while (true)
            {
                int byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (byteCount == 0) break; // Connection closed

                string message = Encoding.ASCII.GetString(buffer, 0, byteCount);

                if (message.StartsWith("clientlist:"))
                {
                    // Update the ListBox with the new client list
                    string clientList = message.Substring("clientlist:".Length);
                    UpdateClientList(clientList.Split(','));
                }
                else
                {
                    // Handle other types of messages (e.g., chat messages)
                }
            }
        }
        private void UpdateClientList(string[] clients)
        {
            // Ensure we update the UI from the UI thread
            Dispatcher.Invoke(() =>
            {
                ListDisplay.Items.Clear(); // Clear the current list

                foreach (string client in clients)
                {
                    if (client == CurrentClientName)
                        continue; // Skip adding self to the list

                    ListBoxItem clientItem = new ListBoxItem
                    {
                        Content = client
                    };
                    ListDisplay.Items.Add(clientItem); // Add each client except self
                }
            });
        }


        private async Task StartListeningForMessages(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            await HandleIncomingMessages(stream);
        }

        private void ExitBtnClicked(object sender, RoutedEventArgs e)
        {
            byte[] data = Encoding.ASCII.GetBytes("exit");
            stream.Write(data, 0, data.Length);
            Application.Current.Shutdown();
            stream.Close();
        }
    }
}
