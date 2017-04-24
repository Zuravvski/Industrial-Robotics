using IDE.Common.ViewModels.Commands;
using System.IO;
using System.Windows.Input;
using System.Windows.Media;
using IDE.Common.Utilities;

namespace IDE.Common.ViewModels
{
    public class HighlightingViewModel : ObservableObject
    {
        private Color colorMovement, colorGrip, colorCounters, colorProgramming, colorInformations, colorNumbers, colorComments;
        private Brush foregroundMovement, foregroundGrip, foregroundCounters, foregroundProgramming, foregroundInformations, foregroundNumbers, foregroundComments;
        private readonly Highlighting highlighting;


        #region Constructor

        public HighlightingViewModel()
        {
            highlighting = Session.Instance.Highlighting;

            //default colors
            var currentDefinitions = highlighting.HighlightingDefinitionColors;
            ColorMovement = currentDefinitions[0];
            ColorGrip = currentDefinitions[1];
            ColorCounters = currentDefinitions[2];
            ColorProgramming = currentDefinitions[3];
            ColorInformations = currentDefinitions[4];
            ColorNumbers = currentDefinitions[5];
            ColorComments = currentDefinitions[6];


            ExportClickCommand = new RelayCommand(Export, IsFileAvailable);
            ImportClickCommand = new RelayCommand(Import, IsFileAvailable);
            ApplyClickCommand = new RelayCommand(Apply, IsFileAvailable);
        }

        private bool IsFileAvailable(object obj)
        {
            string[] files = Directory.GetFiles(".", "*xshd");

            foreach (string file in files)
            {
                if (file == ".\\CustomHighlighting.xshd")
                    return true;
            }

            return false;
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
                NotifyPropertyChanged("ColorMovement");
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
                NotifyPropertyChanged("ColorGrip");
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
                NotifyPropertyChanged("ColorCounters");
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
                NotifyPropertyChanged("ColorProgramming");
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
                NotifyPropertyChanged("ColorInformations");
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
                NotifyPropertyChanged("ColorNumbers");
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
                NotifyPropertyChanged("ColorComments");
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
                NotifyPropertyChanged("ForegroundMovement");
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
                NotifyPropertyChanged("ForegroundGrip");
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
                NotifyPropertyChanged("ForegroundCounters");
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
                NotifyPropertyChanged("ForegroundProgramming");
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
                NotifyPropertyChanged("ForegroundInformations");
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
                NotifyPropertyChanged("ForegroundNumbers");
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
                NotifyPropertyChanged("ForegroundComments");
            }
            get
            {
                return foregroundComments;
            }
        }

        #endregion

        #region Actions

        public ICommand ExportClickCommand { get; private set; }
        public ICommand ImportClickCommand { get; private set; }
        public ICommand ApplyClickCommand { get; private set; }


        private void Import(object obj)
        {
            //read highlight definition and write it into "main" HighlightingDefinition file
            highlighting.HighlightingDefinitionColors = highlighting.ImportDefinitionsColors;
        }

        private void Export(object obj)
        {
            Color[] colorList = { ColorMovement, ColorGrip, ColorCounters, ColorProgramming, ColorInformations, ColorNumbers, ColorComments };
            Session.Instance.SubmitHighlighting("CustomHighlighting.xshd");
            highlighting.Export(colorList);   //send current colors into export method
        }

        private void Apply(object obj)
        {
            var colorList = new[] { ColorMovement, ColorGrip, ColorCounters, ColorProgramming, ColorInformations, ColorNumbers, ColorComments };
            highlighting.HighlightingDefinitionColors = colorList;  //
        }

        #endregion

    }
}
