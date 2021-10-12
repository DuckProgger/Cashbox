using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cashbox.Visu
{
    public class MessageProvider : INotifyPropertyChanged
    {
        private string _message;
        private readonly bool disappearing;
        private readonly TimeSpan disappearingDelay;
        private const double defaultDisappearingDelay = 3.0;

        public MessageProvider(bool disappearing = false)
        {
            this.disappearing = disappearing;
            disappearingDelay = TimeSpan.FromSeconds(defaultDisappearingDelay);
        }

        public MessageProvider(TimeSpan disappearingDelay, bool disappearing = false)
        {
            this.disappearing = disappearing;
            this.disappearingDelay = disappearingDelay;
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasMessage));
                if (disappearing)
                    ClearMessage();
            }
        }

        public bool HasMessage => !string.IsNullOrEmpty(Message);

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private async void ClearMessage()
        {
            await Task.Delay(disappearingDelay);
            Message = string.Empty;
        }
    }
}