using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using IDE.Common.ViewModels.Commands;

namespace POCs
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly Thread consumerThread;

        private int sumX, sumY, sumZ;

        public ICommand Start { get; private set; }

        public int SumX
        {
            get { return sumX; }
            set
            {
                sumX = value;
                NotifyPropertyChanged("SumX");
            }
        }

        public int SumY
        {
            get { return sumY; }
            set
            {
                sumY = value;
                NotifyPropertyChanged("SumY");
            }
        }

        public int SumZ
        {
            get { return sumZ; }
            set
            {
                sumZ = value;
                NotifyPropertyChanged("SumZ");
            }
        }

        public ViewModel()
        {
            Start = new RelayCommand(async (obj) =>
            {
                var rand = new Random();
                SumX += rand.Next(1, 10);
                SumY += rand.Next(1, 10);
                SumZ += rand.Next(1, 10);
                await Task.Delay(50);
            });
            consumerThread = new Thread(Consumer) { IsBackground = true };
            ZeroValues();
            consumerThread.Start();
        }

        private void ZeroValues()
        {
            SumX = 0;
            SumY = 0;
            SumZ = 0;
        }

        private void Consumer()
        {
            while (true)
            {
                Thread.Sleep(250);
                Debug.WriteLine($"Sending {sumX}, {sumY}, {sumZ}...");
                Debug.WriteLine("After sleep");
                ZeroValues();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
