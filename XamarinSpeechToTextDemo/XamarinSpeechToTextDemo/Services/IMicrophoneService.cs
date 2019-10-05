using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XamarinSpeechToTextDemo.Services
{
    public interface IMicrophoneService
    {
        Task<bool> GetPermissionsAsync();
        void OnRequestPermissionsResult(bool isGranted);
    }
}
