using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static NetworkStream networkStream { get; set; }
        string responseData=string.Empty;
        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer dispatcherTimer;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(temp);
            dispatcherTimer.Interval = new TimeSpan(0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            TcpClient tcpClient = new TcpClient("116.62.171.238", 8888);
            networkStream = tcpClient.GetStream();
            Thread thread = new Thread(ReceiveData);

        }
        void temp(object sender, EventArgs e)
        {
            this.richTextBox.AppendText($"Received: {responseData}");
            responseData = string.Empty;
        }
        public void ReceiveData()
        {
            byte[] data = new byte[256];
            while (true)
            {
                Int32 bytes = networkStream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
            }
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            byte[] vs = Encoding.Default.GetBytes(this.textBox.Text);
            networkStream.Write(vs, 0, vs.Length);
            this.textBox.Text ="";
        }
    }

    public class PercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 360 *(double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value) / 360;
        }
    }

}
