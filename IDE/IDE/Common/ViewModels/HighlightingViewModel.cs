using System.Collections.Generic;
using IDE.Common.ViewModels.Commands;
using System.Windows.Input;
using System.Windows.Media;
using IDE.Common.Models.Value_Objects;
using IDE.Common.Utilities;
using Microsoft.Win32;

namespace IDE.Common.ViewModels
{
    public class HighlightingViewModel : ObservableObject
    {
        private readonly Highlighting highlighting;
        private Dictionary<Command.TypeE, Color> colorMap;

        #region Constructor

        public HighlightingViewModel()
        {
            highlighting = Session.Instance.Highlighting;
            RefreshColors();

            ExportClickCommand = new RelayCommand(Export);
            ImportClickCommand = new RelayCommand(Import);
            ApplyClickCommand = new RelayCommand(Apply);
        }

        #endregion

        #region Properties

        public Dictionary<Command.TypeE, Color> Colors
        {
            private set
            {
                colorMap = value;
                NotifyPropertyChanged("Colors");
            }
            get
            {
                return colorMap;
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
            var dialog = new OpenFileDialog
            {
                FileName = "CustomHighlighting",
                DefaultExt = ".xshd",
                Filter = "xshd files (.xshd)|*.xshd",
                ValidateNames = true,
                CheckFileExists = true
            };
            if (dialog.ShowDialog().GetValueOrDefault(false))
            {
                highlighting.Import(dialog.FileName);
                RefreshColors();
            }
        }

        private void Export(object obj)
        {
            var dialog = new SaveFileDialog
            {
                FileName = "CustomHighlighting",
                DefaultExt = ".xshd",
                Filter = "xshd files (.xshd)|*.xshd",
                ValidateNames = true
            };

            if (dialog.ShowDialog().GetValueOrDefault(false))
            {
                highlighting.Export(dialog.FileName);
            }
        }

        private void Apply(object obj)
        {
            highlighting.Apply(Colors);
        }

        private void RefreshColors()
        {
            Colors = highlighting.Colors;
        }

        #endregion

    }
}
