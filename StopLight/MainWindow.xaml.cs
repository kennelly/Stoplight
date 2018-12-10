
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
using System.Timers;
using System.Threading;

namespace StopLight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Author: Cathy Kennelly
    /// Date: 11/22/2014
    /// Description: This runs a stoplight system that allows users to press a button to simulate a car sitting at a light.  
    ///     It defaults to the left light being turned on, and has two turn lights.
    /// </summary>
    public partial class MainWindow : Window
    {
        // The timer that will run the timing of the entire system
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        // The light that is currently green
        int currentGreen;

        // The light that was previously green and should now be yellow
        int previousLight;

        // Whether or not we have a car waiting at each light
        bool[] carsWaiting = { false, false, false, false };

        // The function that is currently running
        string currentlyRunning = "";

        // The amount of time left on the currently running function
        int runningLeft = 0;

        // The amount of time the current function has been running
        int timeRun = 0;

        // Whether or not to cut the current light short
        bool shortLight = false;

        // Definitions for each color
        SolidColorBrush red = new SolidColorBrush(Colors.Red);
        SolidColorBrush green = new SolidColorBrush(Colors.Green);
        SolidColorBrush yellow = new SolidColorBrush(Colors.Yellow);
        SolidColorBrush gray = new SolidColorBrush(Colors.LightGray);

        public MainWindow()
        {
            InitializeComponent();

            // Set the default lights to green
            currentGreen = 2;
            previousLight = 2;
            Lights_Green();

            // Start the timer running
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        // The timer function that keeps the whole system running
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Record that this car was noticed by the system
            carsWaiting[currentGreen] = false;

            // Decrement the timer
            if (runningLeft > 0)
                runningLeft--;

            // Increment the time counter
            timeRun++;
            System.Console.WriteLine("{0} has been running for {1} seconds", currentlyRunning, timeRun);

            // Transition to the function that should be running next
            if (currentlyRunning == "yellow" && runningLeft <= 0)
            {
                runningLeft = 30;
                timeRun = 0;
                currentlyRunning = "green";
                Lights_Green();
            }
            else if (currentlyRunning == "wait" && runningLeft <= 0)
            {
                runningLeft = 3;
                timeRun = 0;
                currentlyRunning = "yellow";
                LightsYellow();
            }
            else if (currentlyRunning == "green" && shortLight == true && timeRun >= 5)
                checkLights();

            // Run the function that should be running at the moment
            if (currentlyRunning == "yellow" && runningLeft > 0)
                LightsYellow();
            else if (currentlyRunning == "green" && runningLeft != 0)
                Lights_Green();
            else if (currentlyRunning == "wait" && runningLeft > 0) { }

            // Otherwise watch for new cars
            else checkLights();
        }

        // If a car stops at a light, plug it into the queue
        private void newCarWaiting(object sender, RoutedEventArgs e)
        {
            // Get the light number from the button that was pressed
            int lightNumber = Convert.ToInt32(((Button)sender).Tag);

            // Start the short light countdown
            if (currentGreen != lightNumber && shortLight == false && runningLeft > 5 && currentGreen != 2)
            {
                carsWaiting[lightNumber] = true;
                shortLight = true;
                timeRun = 0;
            }

            // Make sure that we aren't already giving a green in that direction
            if (currentGreen != lightNumber)
                carsWaiting[lightNumber] = true;

            // If we are giving a green light, make sure the wait time stays at 30
            else if (currentGreen == lightNumber && shortLight == true)
                shortLight = false;
        }

        // Check the array for cars waiting and run the lights system
        private void checkLights()
        {
            System.Console.WriteLine("Checking cars waiting now...");

            // Set the previous light equal to the current one to get ready for a new one
            previousLight = currentGreen;

            // If there are no cars at the intersection, default to have light on right side of screen green
            if (carsWaiting[0] == false && carsWaiting[1] == false && carsWaiting[2] == false && carsWaiting[3] == false && Light3LeftGreen.Fill != green)
            {
                carsWaiting[2] = true;
            }

            // Set the current green to wherever the highest priority car is waiting
            if (carsWaiting[2] == true)
                currentGreen = 2;
            else if (carsWaiting[1] == true)
                currentGreen = 1;
            else if (carsWaiting[3] == true)
                currentGreen = 3;
            else if (carsWaiting[0] == true)
                currentGreen = 0;
            else return;

            // Start the lights!
            System.Console.WriteLine("And we have a car!");

            // Run the appropriate function
            if (shortLight == true)
            {
                currentlyRunning = "yellow";
                runningLeft = 5;
                timeRun = 0;
                shortLight = false;
            }
            else
            {
                currentlyRunning = "wait";
                runningLeft = 5;
                timeRun = 0;
            }
        }

        // Set desired lights to green
        private void Lights_Green()
        {
            if (shortLight == true)
                System.Console.WriteLine("Checking short light...");

            // Color the lights based on current green light
            if (currentGreen == 2)
            {
                Light1Green.Fill = gray;
                Light1Yellow.Fill = gray;
                Light1Red.Fill = red;

                Light2Green.Fill = gray;
                Light2Yellow.Fill = gray;
                Light2Red.Fill = red;

                Light3Green.Fill = green;
                Light3Yellow.Fill = gray;
                Light3Red.Fill = gray;

                Light3LeftGreen.Fill = green;
                Light3LeftYellow.Fill = gray;
                Light3LeftRed.Fill = gray;

                Light4Green.Fill = gray;
                Light4Yellow.Fill = gray;
                Light4Red.Fill = red;

                Light4RightGreen.Fill = green;
                Light4RightYellow.Fill = gray;
                Light4RightRed.Fill = gray;
            }
            if (currentGreen == 1 || currentGreen == 3)
            {
                Light1Green.Fill = gray;
                Light1Yellow.Fill = gray;
                Light1Red.Fill = red;

                Light2Green.Fill = green;
                Light2Yellow.Fill = gray;
                Light2Red.Fill = gray;

                Light3Green.Fill = gray;
                Light3Yellow.Fill = gray;
                Light3Red.Fill = red;

                Light3LeftGreen.Fill = gray;
                Light3LeftYellow.Fill = gray;
                Light3LeftRed.Fill = red;

                Light4Green.Fill = green;
                Light4Yellow.Fill = gray;
                Light4Red.Fill = gray;

                Light4RightGreen.Fill = green;
                Light4RightYellow.Fill = gray;
                Light4RightRed.Fill = gray;
            }
            if (currentGreen == 0)
            {
                Light1Green.Fill = green;
                Light1Yellow.Fill = gray;
                Light1Red.Fill = gray;

                Light2Green.Fill = gray;
                Light2Yellow.Fill = gray;
                Light2Red.Fill = red;

                Light3Green.Fill = gray;
                Light3Yellow.Fill = gray;
                Light3Red.Fill = red;

                Light3LeftGreen.Fill = gray;
                Light3LeftYellow.Fill = gray;
                Light3LeftRed.Fill = red;

                Light4Green.Fill = gray;
                Light4Yellow.Fill = gray;
                Light4Red.Fill = red;

                Light4RightGreen.Fill = gray;
                Light4RightYellow.Fill = gray;
                Light4RightRed.Fill = red;
            }
        }

        // Set desired lights to yellow
        private void LightsYellow()
        {
            // Color lights yellow based on the previously green lights
            if (previousLight == 2)
            {
                Light1Green.Fill = gray;
                Light1Yellow.Fill = gray;
                Light1Red.Fill = red;

                Light2Green.Fill = gray;
                Light2Yellow.Fill = gray;
                Light2Red.Fill = red;

                Light3Green.Fill = gray;
                Light3Yellow.Fill = yellow;
                Light3Red.Fill = gray;

                Light3LeftGreen.Fill = gray;
                Light3LeftYellow.Fill = yellow;
                Light3LeftRed.Fill = gray;

                Light4Green.Fill = gray;
                Light4Yellow.Fill = gray;
                Light4Red.Fill = red;

                if (currentGreen == 0)
                {
                    Light4RightGreen.Fill = gray;
                    Light4RightYellow.Fill = yellow;
                    Light4RightRed.Fill = gray;
                }
                else
                {
                    Light4RightGreen.Fill = green;
                    Light4RightYellow.Fill = gray;
                    Light4RightRed.Fill = gray;
                }
            }
            if (previousLight == 1 || previousLight == 3)
            {
                Light1Green.Fill = gray;
                Light1Yellow.Fill = gray;
                Light1Red.Fill = red;

                Light2Green.Fill = gray;
                Light2Yellow.Fill = yellow;
                Light2Red.Fill = gray;

                Light3Green.Fill = gray;
                Light3Yellow.Fill = gray;
                Light3Red.Fill = red;

                Light3LeftGreen.Fill = gray;
                Light3LeftYellow.Fill = gray;
                Light3LeftRed.Fill = red;

                Light4Green.Fill = gray;
                Light4Yellow.Fill = yellow;
                Light4Red.Fill = gray;

                Light4RightGreen.Fill = green;
                Light4RightYellow.Fill = gray;
                Light4RightRed.Fill = gray;
            }
            if (previousLight == 0)
            {
                Light1Green.Fill = gray;
                Light1Yellow.Fill = yellow;
                Light1Red.Fill = gray;

                Light2Green.Fill = gray;
                Light2Yellow.Fill = gray;
                Light2Red.Fill = red;

                Light3Green.Fill = gray;
                Light3Yellow.Fill = gray;
                Light3Red.Fill = red;

                Light3LeftGreen.Fill = gray;
                Light3LeftYellow.Fill = gray;
                Light3LeftRed.Fill = red;

                Light4Green.Fill = gray;
                Light4Yellow.Fill = gray;
                Light4Red.Fill = red;

                Light4RightGreen.Fill = gray;
                Light4RightYellow.Fill = gray;
                Light4RightRed.Fill = red;
            }
        }
    }
}