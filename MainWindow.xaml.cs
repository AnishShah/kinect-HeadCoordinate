using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Kinect;
using System.Media;
using System.IO;
using System.IO.Ports;

namespace Head_Co_ordinate
{
    

    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor myKinect;
        SerialPort myserial;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count == 0)
            {
                MessageBox.Show("No Kinects detected", "Head Position Viewer");
                Application.Current.Shutdown();
                return;
            }

            // Get the first Kinect on the computer
           

            // Start the Kinect running and select the skeleton stream
            try
            {
                myKinect = KinectSensor.KinectSensors[0];
               


                myKinect.SkeletonStream.Enable();
                myKinect.Start();
            }
            catch
            {
                MessageBox.Show("Kinect initialise failed", "Head Position Viewer");
                Application.Current.Shutdown();
            }

            try
            {
                myserial = new SerialPort();
                myserial.PortName = "COM11";
                myserial.BaudRate = 9600;
                myserial.Parity = Parity.None;
                myserial.DataBits = 8;
                myserial.StopBits = StopBits.One;
                myserial.Open();
            }
            catch
            {
                MessageBox.Show("Serial initialise failed", "Head Position Viewer");
                
            }

            myKinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(myKinect_SkeletonFrameReady);
        }

        void myKinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            string message = "Skeleton Not Found!";
            
            Skeleton[] skeletons = null;
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                }
            }

            if (skeletons == null)
            { return; }

            foreach (Skeleton skeleton in skeletons)
            {
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    Joint headJoint = skeleton.Joints[JointType.Head];
                    SkeletonPoint headPosition = headJoint.Position;

                    message = string.Format("Head: X:{0:0.0} Y:{1:0.0} Z:{2:0.0}",
                           headPosition.X,
                           headPosition.Y,
                           headPosition.Z);
                    if(headPosition.Z>1)
                    {
                       string text = "Bot forward"; 
                       coobox2.Text=text;
                       myserial.Write("f");

                    }
                    else if (headPosition.Z < 1)
                    {
                        string text = "Bot backward";
                        coobox2.Text = text;
                        myserial.Write("b");
                    }else
		    {
			string text = "Bot stop";
			coobox2.Text = text;
			myserial.Write("s");
		    }
                }

                coobox.Text = message;
            }
        }

        
       
    }
}
