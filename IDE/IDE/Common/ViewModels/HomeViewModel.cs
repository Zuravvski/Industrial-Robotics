using IDE.Common.ViewModels.Commands;
using System.Windows.Input;
using System.Diagnostics;
using IDE.Common.Utilities;

namespace IDE.Common.ViewModels
{
    /// <summary>
    /// HomeViewModel class
    /// </summary>
    /// <seealso cref="IDE.Common.ViewModels.Commands.ObservableObject" />
    public class HomeViewModel : ObservableObject
    {

        #region Fields



        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
        /// </summary>
        public HomeViewModel()
        {
            DeclareCommands();
            Session.Instance.InitializeColors();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the appearance.
        /// </summary>
        /// <value>
        /// The appearance.
        /// </value>
        public AppearanceViewModel Appearance => AppearanceViewModel.Instance;

        #endregion

        #region Actions



        #endregion

        #region Commands

        /// <summary>
        /// Gets the open overview clip command.
        /// </summary>
        /// <value>
        /// The open overview clip command.
        /// </value>
        public ICommand OpenOverviewClipCommand { get; private set; }

        /// <summary>
        /// Declares the commands.
        /// </summary>
        private void DeclareCommands()
        {
            OpenOverviewClipCommand = new RelayCommand(OpenOverviewClip);
        }

        /// <summary>
        /// Opens the overview clip.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OpenOverviewClip(object obj)
        {
            Process.Start("https://www.youtube.com/watch?v=7LKHpM1UeDA");
        }


        #endregion

    }
}
