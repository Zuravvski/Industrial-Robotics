using IDE.Common.Models;
using IDE.Common.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace IDE.Common.ViewModels
{
    class HighlightingViewModel : INotifyPropertyChanged
    {
        private Color colorMovement, colorGrip, colorCounters, colorProgramming, colorInformations, colorNumbers, colorComments;
        private Brush foregroundMovement, foregroundGrip, foregroundCounters, foregroundProgramming, foregroundInformations, foregroundNumbers, foregroundComments;
        CustomHighlighting highlighting;
        


        #region Constructor

        public HighlightingViewModel()
        {
            highlighting = new CustomHighlighting();

            //default colors
            var currentDefinitions = highlighting.HighlightingDefinitionColors;
            ColorMovement = currentDefinitions[0];
            ColorGrip = currentDefinitions[1];
            ColorCounters = currentDefinitions[2];
            ColorProgramming = currentDefinitions[3];
            ColorInformations = currentDefinitions[4];
            ColorNumbers = currentDefinitions[5];
            ColorComments = currentDefinitions[6];

            TestClickCommand = new RelayCommand(Test);
        }
    
        #endregion

        #region Properties

        public Color ColorMovement
        {
            set
            {
                colorMovement = value;
                SolidColorBrush brush = new SolidColorBrush(ColorMovement);
                ForegroundMovement = brush;
                OnPropertyChanged("ColorMovement");
            }
            get
            {
                return colorMovement;
            }
        }

        public Color ColorGrip
        {
            set
            {
                colorGrip = value;
                SolidColorBrush brush = new SolidColorBrush(ColorGrip);
                ForegroundGrip = brush;
                OnPropertyChanged("ColorGrip");
            }
            get
            {
                return colorGrip;
            }
        }

        public Color ColorCounters
        {
            set
            {
                colorCounters = value;
                SolidColorBrush brush = new SolidColorBrush(ColorCounters);
                ForegroundCounters = brush;
                OnPropertyChanged("ColorCounters");
            }
            get
            {
                return colorCounters;
            }
        }

        public Color ColorProgramming
        {
            set
            {
                colorProgramming = value;
                SolidColorBrush brush = new SolidColorBrush(ColorProgramming);
                ForegroundProgramming = brush;
                OnPropertyChanged("ColorProgramming");
            }
            get
            {
                return colorProgramming;
            }
        }

        public Color ColorInformations
        {
            set
            {
                colorInformations = value;
                SolidColorBrush brush = new SolidColorBrush(ColorInformations);
                ForegroundInformations = brush;
                OnPropertyChanged("ColorInformations");
            }
            get
            {
                return colorInformations;
            }
        }

        public Color ColorNumbers
        {
            set
            {
                colorNumbers = value;
                SolidColorBrush brush = new SolidColorBrush(ColorNumbers);
                ForegroundNumbers = brush;
                OnPropertyChanged("ColorNumbers");
            }
            get
            {
                return colorNumbers;
            }
        }

        public Color ColorComments
        {
            set
            {
                colorComments = value;
                SolidColorBrush brush = new SolidColorBrush(ColorComments);
                ForegroundComments = brush;
                OnPropertyChanged("ColorComments");
            }
            get
            {
                return colorComments;
            }
        }


        public Brush ForegroundMovement
        {
            set
            {
                foregroundMovement = value;
                OnPropertyChanged("ForegroundMovement");
            }
            get
            {
                return foregroundMovement;
            }
        }

        public Brush ForegroundGrip
        {
            set
            {
                foregroundGrip = value;
                OnPropertyChanged("ForegroundGrip");
            }
            get
            {
                return foregroundGrip;
            }
        }

        public Brush ForegroundCounters
        {
            set
            {
                foregroundCounters = value;
                OnPropertyChanged("ForegroundCounters");
            }
            get
            {
                return foregroundCounters;
            }
        }

        public Brush ForegroundProgramming
        {
            set
            {
                foregroundProgramming = value;
                OnPropertyChanged("ForegroundProgramming");
            }
            get
            {
                return foregroundProgramming;
            }
        }

        public Brush ForegroundInformations
        {
            set
            {
                foregroundInformations = value;
                OnPropertyChanged("ForegroundInformations");
            }
            get
            {
                return foregroundInformations;
            }
        }

        public Brush ForegroundNumbers
        {
            set
            {
                foregroundNumbers = value;
                OnPropertyChanged("ForegroundNumbers");
            }
            get
            {
                return foregroundNumbers;
            }
        }

        public Brush ForegroundComments
        {
            set
            {
                foregroundComments = value;
                OnPropertyChanged("ForegroundComments");
            }
            get
            {
                return foregroundComments;
            }
        }

        #endregion

        #region Actions

        public ICommand TestClickCommand { get; private set; }

        private void Test(object obj)
        {
            Color[] colorList = new Color[] { ColorMovement, ColorGrip, ColorCounters, ColorProgramming, ColorInformations, ColorNumbers, ColorComments };
            highlighting.HighlightingDefinitionColors = colorList;
        }

        #endregion

        #region PropertyChangedEvents

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
