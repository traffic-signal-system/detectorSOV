using System;
using System.Collections.Generic;
using System.IO;
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

namespace detectorSOV
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static byte GET_REQUEST = 0x80;
        public static byte OSCILLATOR_FREQUENCY_SENSITIVITY = 0xe2;
        public static byte[] GET_DETECTOR_SENSITIVITY_1_1_8 = { GET_REQUEST, OSCILLATOR_FREQUENCY_SENSITIVITY, 0x00, 0x05, 0x00 };
        public static byte[] GET_DETECTOR_SENSITIVITY_1_9_16 = { GET_REQUEST, OSCILLATOR_FREQUENCY_SENSITIVITY, 0x00, 0x06, 0x00 };
        public static byte[] GET_DETECTOR_SENSITIVITY_2_1_8 = { GET_REQUEST, OSCILLATOR_FREQUENCY_SENSITIVITY, 0x00, 0x05, 0x01 };
        public static byte[] GET_DETECTOR_SENSITIVITY_2_9_16 = { GET_REQUEST, OSCILLATOR_FREQUENCY_SENSITIVITY, 0x00, 0x06, 0x01 };
        public static byte[] GET_DETECTOR_OSCILLATOR_FREQUENCY_1 = { GET_REQUEST, OSCILLATOR_FREQUENCY_SENSITIVITY, 0x00, 0x18, 0x00 };
        public static byte[] GET_DETECTOR_OSCILLATOR_FREQUENCY_2 = { GET_REQUEST, OSCILLATOR_FREQUENCY_SENSITIVITY, 0x00, 0x18, 0x01 };
       
        public static byte[] DETECTOR_SENSITIVITY = { 0x81, OSCILLATOR_FREQUENCY_SENSITIVITY, 0x00, 0x0b, 0x00 };
        public static byte[] DETECTOR_SENSITIVITY_2 = { 0x81, OSCILLATOR_FREQUENCY_SENSITIVITY, 0x00, 0x0b, 0x01 };
        public static byte OSCILLATOR_FREQUENCY = 0x19;
        public static byte[] DETECTOR_OSCILLATOR_FREQUENCY = { 0x81, OSCILLATOR_FREQUENCY_SENSITIVITY, 0x00, OSCILLATOR_FREQUENCY, 0x00 };
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] hex = new byte[DETECTOR_SENSITIVITY.Length + 4];
                Stream s = new MemoryStream();
                s.Write(DETECTOR_SENSITIVITY, 0, DETECTOR_SENSITIVITY.Length);
                byte se = Convert.ToByte(level.Value);
                byte sen = se;
                sen = (byte)(sen | se << 4);

                s.WriteByte(sen);
                s.WriteByte(sen);
                s.WriteByte(sen);
                s.WriteByte(sen);
                s.Position = 0;
                int count = s.Read(hex, 0, hex.Length);
                hex[3] = 0x0b;
                hex[4] = 0x00;
                detectorSOV.Udp.sendUdpNoReciveData(ip.Text, 5435, hex);
                hex[3] = 0x0c;
                hex[4] = 0x00;

                detectorSOV.Udp.sendUdpNoReciveData(ip.Text, 5435, hex);
                hex[3] = 0x0b;
                hex[4] = 0x01;
                detectorSOV.Udp.sendUdpNoReciveData(ip.Text, 5435, hex);
                hex[3] = 0x0c;
                hex[4] = 0x01;
                detectorSOV.Udp.sendUdpNoReciveData(ip.Text, 5435, hex);
            }
            catch(Exception ex )
            {
                MessageBox.Show(ex.ToString());
            }
          
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] result = detectorSOV.Udp.recvUdp(ip.Text, 5435, GET_DETECTOR_SENSITIVITY_1_1_8);
                byte[] result1 = detectorSOV.Udp.recvUdp(ip.Text, 5435, GET_DETECTOR_SENSITIVITY_1_9_16);
                byte[] result2 = detectorSOV.Udp.recvUdp(ip.Text, 5435, GET_DETECTOR_SENSITIVITY_2_1_8);
                byte[] result3 = detectorSOV.Udp.recvUdp(ip.Text, 5435, GET_DETECTOR_SENSITIVITY_2_9_16);
                tbkResult.Text = result[5] + " " + result[6] + " " + result[7] + " " + result[8];
                tbkResult1.Text = result1[5] + " " + result1[6] + " " + result1[7] + " " + result1[8];
                tbkResult2.Text = result2[5] + " " + result2[6] + " " + result2[7] + " " + result2[8];
                tbkResult3.Text = result3[5] + " " + result3[6] + " " + result3[7] + " " + result3[8];
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
    }
}
