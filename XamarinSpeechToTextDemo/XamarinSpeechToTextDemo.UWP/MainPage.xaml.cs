using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XamarinSpeechToTextDemo.Services;
using XamarinSpeechToTextDemo.UWP.Services;

namespace XamarinSpeechToTextDemo.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            Xamarin.Forms.DependencyService.Register<IMicrophoneService, MicrophoneService>();
            LoadApplication(new XamarinSpeechToTextDemo.App());
        }
    }
}
