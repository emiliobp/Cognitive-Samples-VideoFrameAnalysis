using Demo_ChinaTown.Libraries;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using OpenCvSharp.Extensions;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Demo_ChinaTown
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //  Initializing Logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("MainWindow.cs");

        //  Initializing Face API
        private const string subscriptionKey = "fa6122356e204afea55c8c590b3caa82";
        private const string faceEndpoint = "https://southcentralus.api.cognitive.microsoft.com";
        
        //  Instances
        FaceAnalysisLibrary faceAnalysis = new FaceAnalysisLibrary();
        GroupPersonLibrary groupPerson = new GroupPersonLibrary();
        LiveCameraResult props = new LiveCameraResult();
        SpeechToLuis speechToLuis = new SpeechToLuis();
        TextToSpeech textToSpeech = new TextToSpeech();


        //  Grabber
        private readonly FrameGrabber<LiveCameraResult> _grabber = null;
        private bool _fuseClientRemoteResults;
        private LiveCameraResult _latestResultsToDisplay = null;

        public MainWindow()
        {
            InitializeComponent();

            props.FaceClientInit(subscriptionKey);
            //  Face API Initiating endpoint
            if (Uri.IsWellFormedUriString(faceEndpoint, UriKind.Absolute))
            {
                props.faceClient.Endpoint = faceEndpoint;
                MessageArea.Text = "Endpoint Initiated";
                log.Info("Endpont Initiated");
            }
            else
            {
                MessageBox.Show(faceEndpoint,
                    "Invalid URI", MessageBoxButton.OK, MessageBoxImage.Error);
                log.Error($"{faceEndpoint} Invalid URI");
                Environment.Exit(0);
            }

            // Create grabber. 
            _grabber = new FrameGrabber<LiveCameraResult>();

            // Set up a listener for when the client receives a new frame.
            _grabber.NewFrameProvided += (s, e) =>
            {
                // The callback may occur on a different thread, so we must use the
                // MainWindow.Dispatcher when manipulating the UI. 
                this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    // Display the image in the left pane.
                    LeftImage.Source = e.Frame.Image.ToBitmapSource();

                    // If we're fusing client-side face detection with remote analysis, show the
                    // new frame now with the most recent analysis available. 
                }));
            };

            //  Start of program
            Program();
        }

        /// <summary> Populate CameraList in the UI, once it is loaded. </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Routed event information. </param>
        private void CameraList_Loaded(object sender, RoutedEventArgs e)
        {
            int numCameras = _grabber.GetNumCameras();

            if (numCameras == 0)
            {
                MessageArea.Text = "No cameras found!";
            }

            var comboBox = sender as ComboBox;
            comboBox.ItemsSource = Enumerable.Range(0, numCameras).Select(i => string.Format("Camera {0}", i + 1));
            comboBox.SelectedIndex = 0;
        }

        private async void Program()
        {
            //  Variables
            String luisResult = "";

            do
            {
                //  Hide Camera
                LeftImage.Visibility = Visibility.Hidden;

                log.Debug(" Initialize Speech to LUIS services");
                MessageArea.Text += "Listening ";
                luisResult = await speechToLuis.RecognitionWithMicrophoneUsingLanguageAsync();
                MessageArea.Text += "End Listening\n ";
                log.Error(luisResult);
                
                
                //  Verifying greeting to start interaction
                if (luisResult.Equals("Greetings"))
                {
                    //  Enable Camera
                    LeftImage.Visibility = Visibility.Visible;
                    faceRecognition();

                    String mensajeTemp = "Hola Usuario \n";
                    MessageArea.Text = mensajeTemp;

                    log.Debug("Initialize Text to Speech services");
                    await textToSpeech.SynthesisToSpeakerAsync(mensajeTemp);
                    log.Debug($"Finished Text to Speech");

                }
            } while (luisResult != null || luisResult != "Intent not recognized." || luisResult != "Speech could not be recognized.");
        }

        private async void faceRecognition()
        {
            log.Info("Enter - Face Recognition Main Window");

            if (!CameraList.HasItems)
            {
                MessageArea.Text = "No cameras found; cannot start processing";
                return;
            }

            //  set property for grouppersonid, this is used across all methods
            string groupPersonId = "myfriends";
            props.groupPersonId = groupPersonId
                ;
            // Define two dates.
            DateTime date1 = new DateTime(2019, 1, 1, 8, 0, 10);
            DateTime date2 = new DateTime(2019, 1, 1, 8, 0, 20);
            // Calculate the interval between the two dates.
            TimeSpan interval = date2 - date1;

            // How often to analyze. 
            _grabber.TriggerAnalysisOnInterval(interval);

            // Reset message. 
            MessageArea.Text = "";

            await _grabber.StartProcessingCameraAsync(CameraList.SelectedIndex);

            string message = await groupPerson.VerifyGroupExist(groupPersonId);
            if (message == "Group already exists")
            {
                log.Debug("Group Exist carry on on analysis");
                _grabber.AnalysisFunction = faceAnalysis.FacesAnalysisFunction;
                MessageArea.Text = "Finalize Face Analysis";
            }
        }
    }
}
