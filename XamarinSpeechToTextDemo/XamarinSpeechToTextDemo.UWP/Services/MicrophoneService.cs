using System;
using System.Threading.Tasks;
using XamarinSpeechToTextDemo.Services;

namespace XamarinSpeechToTextDemo.UWP.Services
{
    internal class MicrophoneService : IMicrophoneService
    {
        public async Task<bool> GetPermissionsAsync()
        {
            var isMicAvailable = true;
            try
            {
                using (var mediaCapture = new Windows.Media.Capture.MediaCapture())
                {
                    var settings = new Windows.Media.Capture.MediaCaptureInitializationSettings
                    {
                        StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Audio
                    };
                    await mediaCapture.InitializeAsync(settings);
                }
            }
            catch (Exception)
            {
                isMicAvailable = false;
            }

            if (!isMicAvailable)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-microphone"));
            }

            return isMicAvailable;
        }

        public void OnRequestPermissionsResult(bool isGranted)
        {
        }
    }
}
