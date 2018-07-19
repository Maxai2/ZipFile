using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZipFile
{
    public class ProgressBarItem : INotifyPropertyChanged
    {
        private double barValue;
        public double BarValue
        {
            get => barValue;
            set
            {
                barValue = value;
                OnPropertyChanged();
            }
        }

        private double barMaxValue;
        public double BarMaxValue
        {
            get => barMaxValue;
            set
            {
                barMaxValue = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        //public bool BarVis { get; set; }

    }
}
