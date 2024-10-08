using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
using System.Windows.Shapes;

namespace WPFClientNew
{
    /// <summary>
    /// Interaction logic for PrivateMessage.xaml
    /// </summary>
    public partial class PrivateMessage : Window
    {
        private string recipientName, senderName;
        private NetworkStream stream;
        public PrivateMessage(string recipientName, string senderName, NetworkStream stream)
        {
            InitializeComponent();
            this.recipientName = recipientName;
            this.stream = stream;
            this.senderName = senderName;
            recipient.Text = recipientName;
            Thread readThread = new Thread(Receive);
            readThread.Start();
        }

        private void SendMessageOnEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                if (MessageBoxEnter.Text.Contains("@->@"))
                {
                    MessageBox.Show("Message cannot contain @->@:" + MessageBoxEnter.Text);
                }
                else if (!string.IsNullOrEmpty(MessageBoxEnter.Text))
                {
                    //string message = MessageBoxEnter.Text.ToString();
                    //AddMessageToChat(message, true);
                    //MessageBoxEnter.Clear();
                    //string formattedMessage = $"{recipientName}@->@{message}";
                    //byte[] data = Encoding.ASCII.GetBytes(formattedMessage);
                    //stream.Write(data, 0, data.Length);
                    SendMessage(MessageBoxEnter.Text);
                }
            }
        }
        private void SendMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                AddMessageToChat(message, true);
                MessageBoxEnter.Clear();
                string formattedMessage = $"{recipientName}@->@{message}";
                byte[] data = Encoding.ASCII.GetBytes(formattedMessage);
                stream.Write(data, 0, data.Length);
                MessageBox.Show($"Sent: {formattedMessage}");
            }
        }
        private void Receive()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                if (stream != null)
                {
                    try
                    {
                        int byteCount = stream.Read(buffer, 0, buffer.Length);

                        if (byteCount == 0) break;

                        string responseFromServer = Encoding.ASCII.GetString(buffer, 0, byteCount);

                        if (responseFromServer.ToLower() == "exit")
                        {
                            break;
                        }

                        Dispatcher.Invoke(() => AddMessageToChat(responseFromServer, false));
                        MessageBox.Show($"Received: {responseFromServer}");
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show("Error in receiving: " + ex.Message);
                        break;
                    }
                }
            }
        }
        private void AddMessageToChat(string message, bool isSender)
        {
            TextBlock messageBlock = new TextBlock();
            messageBlock = new TextBlock
            {
                Text = message,
                Margin = new Thickness(5),
                TextWrapping = TextWrapping.Wrap,
                Background = new SolidColorBrush(Colors.LightBlue),
                HorizontalAlignment = isSender ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                Padding = new Thickness(1),
                MaxWidth = 300
            };
            ChatDisplay.Children.Add(messageBlock);
            ChatScrollViewer.ScrollToEnd();
        }

        private void SendBtnClicked(object sender, RoutedEventArgs e)
        {
            //string message = MessageBoxEnter.Text.ToString();
            //if (!string.IsNullOrEmpty(MessageBoxEnter.Text)) {
            //    AddMessageToChat(message, true);
            //    MessageBoxEnter.Clear();
            //}
            //string formattedMessage = $"{recipientName}@->@{message}";//aizat
            //byte[] data = Encoding.ASCII.GetBytes(formattedMessage);
            //stream.Write(data, 0, data.Length);//send message to server
            SendMessage(MessageBoxEnter.Text);
        }

        private void BackBtnClicked(object sender, RoutedEventArgs e)
        {

        }
        private void aReceive()
        {
            try
            {
                byte[] buffer = new byte[1024];
                while (true)
                {
                    if (stream != null)
                    {
                        MessageBox.Show("masuk atas");
                        int byteCount = stream.Read(buffer, 0, buffer.Length);
                        if (byteCount == 0) break;
                        string responseFromServer = Encoding.ASCII.GetString(buffer, 0, byteCount);

                        if (responseFromServer.ToLower() == "exit")
                        {
                            break;
                        }

                        MessageBox.Show("masuk atas");
                        Dispatcher.Invoke(() => AddMessageToChat(responseFromServer, false));

                        MessageBox.Show("masuk bawah");
                    }
                    else
                    {
                        MessageBox.Show("Tak masuk");
                    }

                }
            }
            catch (Exception ex) {
                MessageBox.Show("Error in receiving: " + ex.Message);
            }
        }
    }
}
