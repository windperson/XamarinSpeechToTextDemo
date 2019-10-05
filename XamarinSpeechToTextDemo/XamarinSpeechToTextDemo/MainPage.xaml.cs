using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private ISpeechToTextService speechRecongnitionInstance;

        public MainPage()
        {
            InitializeComponent();

            try
            {
                speechRecongnitionInstance = DependencyService.Get<ISpeechToTextService>();
            }
            catch (Exception ex)
            {
                UpdateUI($"ERROR: {ex}");
            }

            MessagingCenter.Subscribe<ISpeechToTextService, string>(this, "STT", (sender, args) =>
            {
                UpdateUI($"RECOGNIZED: Text={args}");
            });

            MessagingCenter.Subscribe<ISpeechToTextService>(this, "Final", (sender) =>
            {
                RecognitionButton.IsEnabled = true;
            });

            MessagingCenter.Subscribe<IMessageSender, string>(this, "STT", (sender, args) =>
            {
                UpdateUI($"RECOGNIZED: Text={args}");
            });
        }

        private async void OnEnableMicrophoneButtonClicked(object sender, EventArgs e)
        {
            var micAccessGranted = await DependencyService.Get<ISpeechToTextService>().GetPermissionsAsync();
            if (!micAccessGranted)
            {
                RecognitionButton.IsEnabled = false;
                UpdateUI("Please give access to microphone");
            }
            else
            {
                RecognitionButton.IsEnabled = true;
            }
        }

        private void OnRecognitionButtonClicked(object sender, EventArgs e)
        {
            try
            {
                RecognitionButton.IsEnabled = false;
                speechRecongnitionInstance.StartSpeechToText();
            }
            catch (Exception ex)
            {
                UpdateUI($"ERROR: {ex}");
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
