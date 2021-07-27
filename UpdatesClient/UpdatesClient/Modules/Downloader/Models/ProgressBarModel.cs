using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UpdatesClient.Modules.Downloader.Models
{
    public class ProgressBarModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string progress;
        private string time;
        private string speed;
        private float fprogress;
        private bool isIndeterminate;

        public bool IsIndeterminate
        {
            get { return isIndeterminate; }
            set { isIndeterminate = value; OnPropertyChanged(); }
        }

        public float FProgress
        {
            get { return fprogress; }
            set { fprogress = value; Progress = $"{(int)fprogress}%"; OnPropertyChanged(); }
        }

        public string Speed
        {
            get { return speed; }
            set { speed = value; OnPropertyChanged(); }
        }

        public string Time
        {
            get { return time; }
            set { time = value; OnPropertyChanged(); }
        }

        public string Progress
        {
            get { return progress; }
            set { progress = value; OnPropertyChanged(); }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
