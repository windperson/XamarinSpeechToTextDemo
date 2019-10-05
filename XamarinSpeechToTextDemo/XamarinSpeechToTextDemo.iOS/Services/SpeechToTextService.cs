using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using Speech;
using UIKit;
using Xamarin.Forms;
using XamarinSpeechToTextDemo.Services;

namespace XamarinSpeechToTextDemo.iOS.Services
{
    class SpeechToTextService : ISpeechToTextService
    {
        private AVAudioEngine audioEngine = new AVAudioEngine();
        private SFSpeechRecognizer speechRecognizer = new SFSpeechRecognizer();
        private SFSpeechAudioBufferRecognitionRequest recognitionRequest;
        private SFSpeechRecognitionTask recognitionTask;
        
        private string recognizedString;
        private NSTimer timer;

        private TaskCompletionSource<bool> tcsPermissions;


        public Task<bool> GetPermissionsAsync()
        {
            tcsPermissions = new TaskCompletionSource<bool>();
            RequestSpeechPermission();
            return tcsPermissions.Task;
        }

        public void OnRequestPermissionsResult(bool isGranted)
        {
            tcsPermissions.TrySetResult(isGranted);
        }

        public void StartSpeechToText()
        {
            if (audioEngine.Running)
            {
                StopRecordingAndRecognition();

            }
            StartRecordingAndRecognizing();
        }

        public void StopSpeechToText()
        {
            StopRecordingAndRecognition();
        }

        #region private methods

        private void RequestSpeechPermission()
        {
            SFSpeechRecognizer.RequestAuthorization((SFSpeechRecognizerAuthorizationStatus status) =>
            {
                switch (status)
                {
                    case SFSpeechRecognizerAuthorizationStatus.Authorized:
                        tcsPermissions.TrySetResult(true);
                        break;
                    case SFSpeechRecognizerAuthorizationStatus.Denied:
                    case SFSpeechRecognizerAuthorizationStatus.NotDetermined:
                    case SFSpeechRecognizerAuthorizationStatus.Restricted:
                    default:
                        tcsPermissions.TrySetResult(false);
                        break;
                }
            });
        }

        private void StartRecordingAndRecognizing()
        {
            timer = NSTimer.CreateRepeatingScheduledTimer(15, delegate
            {
                DidFinishTalk();
            });

            recognitionTask?.Cancel();
            recognitionTask = null;

            var audioSession = AVAudioSession.SharedInstance();
            NSError nsError;
            nsError = audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);
            audioSession.SetMode(AVAudioSession.ModeDefault, out nsError);
            if(nsError != null)
            {
                throw new Exception("audio session SetMode() fail!");
            }

            nsError = audioSession.SetActive(true, AVAudioSessionSetActiveOptions.NotifyOthersOnDeactivation);
            audioSession.OverrideOutputAudioPort(AVAudioSessionPortOverride.Speaker, out nsError);
            if(nsError != null)
            {
                throw new Exception("audio session OverrideOutputAudioPort() fail!");
            }
            recognitionRequest = new SFSpeechAudioBufferRecognitionRequest();

            var inputNode = audioEngine.InputNode;
            if (inputNode == null)
            {
                throw new Exception();
            }

            var recordingFormat = inputNode.GetBusOutputFormat(0);
            inputNode.InstallTapOnBus(0, 1024, recordingFormat, (buffer, when) =>
            {
                recognitionRequest?.Append(buffer);
            });

            audioEngine.Prepare();
            audioEngine.StartAndReturnError(out nsError);
            if (nsError != null)
            {
                throw new Exception("audio engine StartAndReturnError() fail!");
            }

            recognitionTask = speechRecognizer.GetRecognitionTask(recognitionRequest, (result, error) =>
            {
                var isFinal = false;
                if (result != null)
                {
                    recognizedString = result.BestTranscription.FormattedString;
                    MessagingCenter.Send<ISpeechToTextService, string>(this, "STT", recognizedString);
                    timer.Invalidate();
                    timer = null;
                    timer = NSTimer.CreateRepeatingScheduledTimer(2, delegate
                    {
                        DidFinishTalk();
                    });
                }
                if (error != null || isFinal)
                {
                    MessagingCenter.Send<ISpeechToTextService>(this, "Final");
                    StopRecordingAndRecognition(audioSession);
                }
            });
        }

        private void DidFinishTalk()
        {
            MessagingCenter.Send<ISpeechToTextService>(this, "Final");
            if (timer != null)
            {
                timer.Invalidate();
                timer = null;
            }

            if (audioEngine.Running)
            {
                StopRecordingAndRecognition();
            }
        }

        private void StopRecordingAndRecognition(AVAudioSession avAudioSession = null)
        {
            if (audioEngine.Running)
            {
                audioEngine.Stop();
                audioEngine.InputNode.RemoveTapOnBus(0);
                recognitionTask?.Cancel();
                recognitionRequest.EndAudio();
                recognitionRequest = null;
                recognitionTask = null;
            }
        }

        #endregion
    }
}