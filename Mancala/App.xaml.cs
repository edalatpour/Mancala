using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Marketplace;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace Mancala
{
    public partial class App : Application
    {

        public LicenseInformation licenseInfo = new LicenseInformation();
        public MarketplaceDetailTask detailTask = new MarketplaceDetailTask();

        // Easy access to the root frame
        public PhoneApplicationFrame RootFrame { get; private set; }

        public Game Game;

        // Constructor
        public App()
        {
            // Global handler for uncaught exceptions. 
            // Note that exceptions thrown by ApplicationBarItem.Click will not get caught here.
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            ApplicationLifetimeObjects.Add(new XNAAsyncDispatcher(TimeSpan.FromMilliseconds(50)));
        
        }

        public bool CheckTrialExpired()
        {
            // This is where we check to see if they are in a trial
            // First thing we want to do is increment the number of times we have opened a deck...
            bool trialExpired = false;
            if (licenseInfo.IsTrial())
            {
                int numberOfTrialRuns = IsolatedStorageHelper.GetObject<int>("NumberOfTrialRuns");
                numberOfTrialRuns++;
                if (numberOfTrialRuns > 5)
                {
                    trialExpired = true;
                    MessageBoxResult result = MessageBox.Show("The trial version of Mancala is limited to 5 games. Would you like to go to the Marketplace to purchase the full version?", "Trial Expired", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        detailTask.Show();
                    }
                }
                else
                {
                    IsolatedStorageHelper.SaveObject<int>("NumberOfTrialRuns", numberOfTrialRuns);
                }
            }
            return (trialExpired);
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            this.Game = new Game();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {

                //this.Game = (Game)(PhoneApplicationService.Current.State["Game"]);
                //Try to locate previous data in transient state of the application
                if (PhoneApplicationService.Current.State.ContainsKey("Game"))
                {
                    if (!e.IsApplicationInstancePreserved)
                    {

                        //If found, initialize the data variable and remove in from application's state
                        Game game = (Game)(PhoneApplicationService.Current.State["Game"]);
                        game.Activated = true;
                        //System.Diagnostics.Debug.WriteLine(cup.Count.ToString());
                        this.Game = game;
                    }

                    PhoneApplicationService.Current.State.Remove("Game");
                
                }

        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            //PhoneApplicationService.Current.State["Game"] = this.Game;

            //XmlSerializer serializer = new XmlSerializer(typeof(Game));
            //TextWriter writer = new StreamWriter(filename);
            //// Serialize the purchase order, and close the TextWriter.
            //serializer.Serialize(writer, this.Game);

            Game game = this.Game;

            PhoneApplicationService.Current.State.Add("Game", game);
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break in the debugger
                System.Diagnostics.Debugger.Break();
            }

            e.Handled = true;
            ErrorPage.Exception = e.ExceptionObject;
            (RootVisual as Microsoft.Phone.Controls.PhoneApplicationFrame).Source =
                new Uri("/ErrorPage.xaml", UriKind.Relative);
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}