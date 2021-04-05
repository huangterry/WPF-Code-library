using InteractiveDataDisplay.WPF;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public static NetworkStream networkStream { get; set; }
        string responseData = string.Empty;
        DrawGraph drawGraph = new DrawGraph();
        public Window1()
        {
            //Thread thread = new Thread(test);
            //thread.Start(drawGraph);
            InitializeComponent();
            drawGraph.chart = this.plotter;
            lines.Children.Add(drawGraph.lg);
            lines.Children.Add(drawGraph.lg2);
            lines.Children.Add(drawGraph.lg3);
            DispatcherTimer dispatcherTimer;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(temp);
            dispatcherTimer.Interval = new TimeSpan(0);
            dispatcherTimer.Start();
        }
        void temp(object sender, EventArgs e)
        {
            this.richTextBox.AppendText(responseData);
            responseData = string.Empty;
        }
        //static void test(object draw)
        //{
        //    DrawGraph drawGraph = (draw as DrawGraph);
        //    Random rnd1 = new Random();
        //    while (true)
        //    {
        //        Thread.Sleep(1000);
        //        if (drawGraph.timeaxis.Count() > 3600)
        //        {
        //            drawGraph.y0.Clear();
        //            drawGraph.y1.Clear();
        //            drawGraph.y2.Clear();
        //            drawGraph.timeaxis.Clear();

        //            drawGraph.timeaxis.Add(0);
        //            drawGraph.time = 0;
        //            drawGraph.y0.Add(0);
        //            drawGraph.y1.Add(0);
        //            drawGraph.y2.Add(0);
        //        }
        //        drawGraph.y0.Add((byte)(rnd1.Next(1, 100)));
        //        drawGraph.timeaxis.Add(++drawGraph.time);
        //        drawGraph.y1.Add((byte)(rnd1.Next(1, 100)));
        //        drawGraph.y2.Add((byte)(rnd1.Next(1, 100)));
        //    }

        //}
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (drawGraph.dispatcherTimer.IsEnabled)
            {
                drawGraph.dispatcherTimer.Stop();
            }
            else
            {
                drawGraph.dispatcherTimer.Start();
            }
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            TcpClient tcpClient = new TcpClient("116.62.171.238", 8888);
            networkStream = tcpClient.GetStream();
            Thread thread = new Thread(ReceiveData);
            thread.Start(drawGraph);
        }
        public void ReceiveData(object draw)
        {
            byte[] data = new byte[256];
            DrawGraph temp = (draw as DrawGraph);
            while (true)
            {
                Int32 bytes = networkStream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                Info info = JsonConvert.DeserializeObject<Info>(responseData);
                temp.y0 .Add(byte.Parse(info.CPU) );
                temp.y1.Add(byte.Parse(info.Disk));
                temp.y2.Add(byte.Parse(info.Mem));
                drawGraph.timeaxis.Add(++drawGraph.time);
            }
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            byte[] vs = Encoding.Default.GetBytes(this.textBox.Text);
            networkStream.Write(vs, 0, vs.Length);
            this.textBox.Text = "";
        }
    }

    class DrawGraph
    {
        public List<byte> y0 = new List<byte> { 0 };
        public List<byte> y1 = new List<byte> { 0 };
        public List<byte> y2 = new List<byte> { 0 };
        public List<int> timeaxis = new List<int> { 0 };
        public Chart chart = new Chart();
        public int time = 0;
        public LineGraph lg = new LineGraph();
        public LineGraph lg2 = new LineGraph();
        public LineGraph lg3 = new LineGraph();
        public DispatcherTimer dispatcherTimer;
        public Random rnd1 = new Random();
        public DrawGraph()
        {
            TimerSetting();
            lg.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, (byte)(10 * 10), 0));
            lg2.Stroke = new SolidColorBrush(Color.FromArgb(255, 155, (byte)(10 * 10), 0));
            lg3.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, (byte)(10 * 10), 235));
        }
        void TimerSetting()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            lg.Plot(timeaxis, y0);
            lg2.Plot(timeaxis, y1);
            lg3.Plot(timeaxis, y2);
            chart.PlotOriginX = time - 10;
        }
    }

    public class VisibilityToCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
    
    public class Info
    {
        public string CPU { get; set; }
        public string Mem { get; set; }
        public string Disk { get; set; }
    }
}

