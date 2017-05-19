using System.Collections.Generic;
using IDE.Common.ViewModels.Commands;
using System.Windows.Input;
using System.Windows.Media;
using IDE.Common.Models.Value_Objects;
using IDE.Common.Utilities;
using Microsoft.Win32;

namespace IDE.Common.ViewModels
{
    /// <summary>
    /// HighlightingViewModel class
    /// </summary>
    /// <seealso cref="IDE.Common.ViewModels.Commands.ObservableObject" />
    public class HighlightingViewModel : ObservableObject
    {
        /// <summary>
        /// The highlighting
        /// </summary>
        private readonly Highlighting highlighting;
        /// <summary>
        /// The color map
        /// </summary>
        private Dictionary<Command.TypeE, Color> colorMap;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightingViewModel"/> class.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the colors.
        /// </summary>
        /// <value>
        /// The colors.
        /// </value>
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

        /// <summary>
        /// Gets the export click command.
        /// </summary>
        /// <value>
        /// The export click command.
        /// </value>
        public ICommand ExportClickCommand { get; private set; }
        /// <summary>
        /// Gets the import click command.
        /// </summary>
        /// <value>
        /// The import click command.
        /// </value>
        public ICommand ImportClickCommand { get; private set; }
        /// <summary>
        /// Gets the apply click command.
        /// </summary>
        /// <value>
        /// The apply click command.
        /// </value>
        public ICommand ApplyClickCommand { get; private set; }


        /// <summary>
        /// Imports the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
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

        /// <summary>
        /// Exports the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
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

        /// <summary>
        /// Applies the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void Apply(object obj)
        {
            highlighting.Apply(Colors);
        }

        /// <summary>
        /// Refreshes the colors.
        /// </summary>
        private void RefreshColors()
        {
            Colors = highlighting.Colors;
        }

        #endregion

    }
}
