using System.ComponentModel;
using IDE.Common.Models;

namespace IDE.Common.ViewModels
{
    /// <summary>
    /// AboutViewModel class
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    class AboutViewModel : INotifyPropertyChanged
    {

        #region Fields

        /// <summary>
        /// The about model
        /// </summary>
        AboutModel aboutModel;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutViewModel"/> class.
        /// </summary>
        public AboutViewModel()
        {
            aboutModel = new AboutModel
            {
                GeneralInfo = "This project was created as part of Basics of Industrial Robotics subject. Our task was to create " +
                "Integrated Development Environment that will allow to easily program Mitsubishi RV-E3J industrial robot. IDE should " +
                "also have ability to browse through programs in manipulator's memory as well as keywords highlighting and " +
                "IntelliSense-like support.",

                AboutCreators = "We are students at West Pomeranian University of Technology Szczecin, " +
                "Faculty of Electrical Engineering. Our course of studies is Automatic Control and Robotics. Developers team " +
                "consisted of 4 people:\n\t"
            };
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the about model.
        /// </summary>
        /// <value>
        /// The about model.
        /// </value>
        public AboutModel AboutModel
        {
            get
            {
                return aboutModel;
            }
            set
            {
                aboutModel = value;
            }
        }

        /// <summary>
        /// Gets or sets the general information.
        /// </summary>
        /// <value>
        /// The general information.
        /// </value>
        public string GeneralInfo
        {
            get
            {
                return AboutModel.GeneralInfo;
            }
            set
            {
                AboutModel.GeneralInfo = value;
                OnPropertyChanged("GeneralInfo");
            }
        }

        /// <summary>
        /// Gets or sets the about creators.
        /// </summary>
        /// <value>
        /// The about creators.
        /// </value>
        public string AboutCreators
        {
            get
            {
                return AboutModel.AboutCreators;
            }
            set
            {
                AboutModel.AboutCreators = value;
                OnPropertyChanged("AboutCreators");
            }
        }
        #endregion

        #region PropertyChangedEvents

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}
