using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinSpeechToTextDemo.Services;

namespace XamarinSpeechToTextDemo
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnRecognitionButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var speechConfig =
                    SpeechConfig.FromSubscription("Your_Cognitive_Service_Key", "Your_Cognitive_Service_Region");

                using (var recognizer = new SpeechRecognizer(speechConfig))
                {
                    var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

                    switch (result.Reason)
                    {
                        case ResultReason.RecognizedSpeech:
                            UpdateUI($"RECOGNIZED: Text={result.Text}");
                            break;
                        case ResultReason.NoMatch:
                            UpdateUI("NOMATCH: Speech could not be recognized.");
                            break;
                        case ResultReason.Canceled:
                            var cancellation = CancellationDetails.FromResult(result);
                            var sb = new StringBuilder();
                            sb.AppendLine($"CANCELED: Reason={cancellation.Reason}\r\n");

                            if (cancellation.Reason == CancellationReason.Error)
                            {
                                sb.AppendLine($"CANCELED: ErrorCode={cancellation.ErrorCode}\r\n");
                                sb.AppendLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}\r\n");
                                sb.AppendLine("CANCELED: Did you update the subscription info?");
                            }
                            UpdateUI(sb.ToString());
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateUI($"Exception: {ex}");
            }
        }

        private async void OnEnableMicrophoneButtonClicked(object sender, EventArgs e)
        {
            var micAccessGranted = await DependencyService.Get<IMicrophoneService>().GetPermissionsAsync();
            if (!micAccessGranted)
            {
                UpdateUI("Please give access to microphone");
            }
        }

        private void UpdateUI(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                RecognitionText.Text = message;
            });
        }
    }
}
