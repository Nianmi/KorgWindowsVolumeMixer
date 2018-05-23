using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sanford.Multimedia;
using Sanford.Multimedia.Midi;

namespace KorgWindowsVolumeMixer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private InputDevice midiInDevice = null;

        public MainWindow()
        {
            InitializeComponent();
            midiInit();
           
        }

        public void midiInit()
        {
            for (var i = 0; i < InputDevice.DeviceCount; i++)
            {
                midiDeviceBox.Items.Add(InputDevice.GetDeviceCapabilities(i).name);
            }
        }

        private void midiConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if(midiDeviceBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please Selecte the Korg naonKontrol 2");
            }
            else
            {
                midiInDevice = new InputDevice(midiDeviceBox.SelectedIndex);
                midiInDevice.ChannelMessageReceived += HandleChannelMessageReceived;
                midiInDevice.Error += new EventHandler<ErrorEventArgs>(inDevice_Error);
                midiInDevice.StartRecording();
            }
        }

        private void inDevice_Error(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(e.Error.Message);
            //Kill Programm?
        }

        private void HandleChannelMessageReceived(object sender, ChannelMessageEventArgs e)
        {
            int resultLow = 0;
            int resultMidle = 0;
            int resultHigh = 0;
            int bitMaskLowestByte = 0x0000FF;
            int bitMaskMidleByte = 0x00FF00;
            int bitMaskHighByte = 0xFF0000;
            resultLow = e.Message.Message & bitMaskLowestByte;
            resultMidle = (e.Message.Message & bitMaskMidleByte) >> 8;
            resultHigh = (e.Message.Message & bitMaskHighByte) >> 16;
            Console.WriteLine(resultLow.ToString("X") + " " + resultMidle.ToString("X") + " " + resultHigh.ToString("X"));

            switch (resultMidle)
            {
                case 0x44:
                    Console.WriteLine("Right Button");
                    break;
                default:
                    Console.WriteLine("Wrong Button");
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (midiInDevice != null)
            {
                midiInDevice.StopRecording();
                midiInDevice.Close();
            }
        }
    }
}
