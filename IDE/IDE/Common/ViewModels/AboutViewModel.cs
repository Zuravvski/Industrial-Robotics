using System.ComponentModel;
using IDE.Common.Models;

namespace IDE.Common.ViewModels
{
    class AboutViewModel : INotifyPropertyChanged
    {

        #region Fields

        AboutModel aboutModel;

        #endregion

        #region Constructor

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
                "consisted of 5 people:\n\t" +
                "Adam Baniuszewicz\n\t" +
                "Ewelina Chołodowicz\n\t" +
                "Bartosz Flis\n\t" +
                "Jakub Sybidło\n\t" +
                "Michał Żurawski\n" +
                "You can contact each of us directly through the GitHub @ https://github.com/Zuravvski/Industrial-Robotics"
            };
        }

        #endregion

        #region Properties
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

        public event PropertyChangedEventHandler PropertyChanged;

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
