using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using XamarinSpeechToTextDemo.Services;
using XamarinSpeechToTextDemo.Droid.Services;
using Android.Content;
using Android.Speech;
using Xamarin.Forms;

namespace XamarinSpeechToTextDemo.Droid
{
    [Activity(Label = "XamarinSpeechToTextDemo", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private const int RECORD_AUDIO = 1;
        private const int VOICE = 10;
        private ISpeechToTextService sttService;
        internal static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Xamarin.Forms.DependencyService.Register<ISpeechToTextService, SpeechToTextService>();
            LoadApplication(new App());
            sttService = Xamarin.Forms.DependencyService.Get<ISpeechToTextService>();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            switch (requestCode)
            {
                case RECORD_AUDIO:
                    sttService.OnRequestPermissionsResult(grantResults[0] == Permission.Granted);
                    break;
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == VOICE)
            {
                if (resultCode == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        string recognizedString = matches[0];
                        MessagingCenter.Send(sttService, "STT", recognizedString);
                    }
                    else
                    {
                        MessagingCenter.Send(sttService, "STT", "Cannot recognize voice");
                    }

                }
                MessagingCenter.Send(sttService, "Final");
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}