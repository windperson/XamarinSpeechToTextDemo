using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using XamarinSpeechToTextDemo.Services;
using static Android.Support.V4.App.ActivityCompat;

namespace XamarinSpeechToTextDemo.Droid.Services
{
    class SpeechToTextService : ISpeechToTextService
    {
        private const int RECORD_AUDIO = 1;
        private readonly string[] permissions = { Manifest.Permission.RecordAudio };

        private const int VOICE = 10;

        private TaskCompletionSource<bool> tcsPermissions;

        public Task<bool> GetPermissionsAsync()
        {
            tcsPermissions = new TaskCompletionSource<bool>();

            // Permissions are required only for Marshmallow and up
            if ((int)Build.VERSION.SdkInt < 23)
            {
                tcsPermissions.TrySetResult(true);
            }

            else
            {
                var currentActivity = MainActivity.Instance;
                if (ActivityCompat.CheckSelfPermission(currentActivity, Manifest.Permission.RecordAudio) != (int)Android.Content.PM.Permission.Granted)
                {
                    RequestMicPermission();
                }
                else
                {
                    tcsPermissions.TrySetResult(true);
                }
            }

            return tcsPermissions.Task;
        }

        public void OnRequestPermissionsResult(bool isGranted)
        {
            tcsPermissions.TrySetResult(isGranted);
        }

        public void StartSpeechToText()
        {
            StartRecordingAndRecognizing();
        }

        public void StopSpeechToText()
        {
            //do nothing
        }

        #region private methods

        private void RequestMicPermission()
        {
            var currentActivity = MainActivity.Instance;
            if (ShouldShowRequestPermissionRationale(currentActivity, Manifest.Permission.RecordAudio))
            {
                Snackbar.Make(currentActivity.FindViewById(Android.Resource.Id.Content),
                    "App requires microphone permission.",
                    Snackbar.LengthIndefinite).SetAction("Ok",
                    v =>
                    {
                        currentActivity.RequestPermissions(permissions, RECORD_AUDIO);
                    }).Show();
            }
            else
            {
                RequestPermissions(currentActivity, permissions, RECORD_AUDIO);
            }
        }

        private void StartRecordingAndRecognizing()
        {
            string rec = global::Android.Content.PM.PackageManager.FeatureMicrophone;
            if (rec == "android.hardware.microphone")
            {
                var currentActivity = MainActivity.Instance;
                try
                {
                    var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                    voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);

                    voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Speak now");

                    voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                    voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                    voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                    voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
                    voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                    currentActivity.StartActivityForResult(voiceIntent, VOICE);
                }
                catch (ActivityNotFoundException)
                {
                    String appPackageName = "com.google.android.googlequicksearchbox";
                    try
                    {
                        Intent intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse("market://details?id=" + appPackageName));
                        currentActivity.StartActivityForResult(intent, VOICE);

                    }
                    catch (ActivityNotFoundException)
                    {
                        Intent intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=" + appPackageName));
                        currentActivity.StartActivityForResult(intent, VOICE);
                    }
                }
            }
            else
            {
                throw new Exception("No mic found");
            }
        }

        #endregion
    }
}