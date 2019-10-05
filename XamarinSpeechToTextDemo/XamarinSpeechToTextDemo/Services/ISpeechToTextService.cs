using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XamarinSpeechToTextDemo.Services
{
    public interface ISpeechToTextService
    {
        Task<bool> GetPermissionsAsync();

        void StartSpeechToText();
        void StopSpeechToText();
    }
}
