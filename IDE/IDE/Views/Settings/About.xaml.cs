using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace IDE.Views.Settings
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
            UpdateAboutText();
        }

        private void UpdateAboutText()
        {
            textBlock_GeneralInformations.Text = "";    //clear XAML text and update with new one
            textBlock_GeneralInformations.Inlines.Add("This project was created as part of ");
            textBlock_GeneralInformations.Inlines.Add(new Run("Basics of Industrial Robotics ") { FontStyle = FontStyles.Italic, FontWeight = FontWeights.Bold });
            textBlock_GeneralInformations.Inlines.Add("subject. Our task was to create Integrated Development Environment " +
                "that will allow to easily program Mitsubishi RV-E3J industrial robot. IDE should also have ability to browse " +
                "through programs in manipulator's memory as well as keywords highlighting and IntelliSense-like support.");

            textBlock_AboutTheCreators.Text = "";       //clear XAML text and update with new one
            textBlock_AboutTheCreators.Inlines.Add("We are students at West Pomeranian University of Technology Szczecin, " +
                "Faculty of Electrical Engineering. Our course of studies is Automatic Control and Robotics. Developers team " +
                "consisted of 4 people:\n\t" +
                "Adam Baniuszewicz\n\t" +
                "Ewelina Chołodowicz\n\t" +
                "Bartosz Flis\n\t" +
                "Michał Żurawski\n" +
                "You can contact each of us directly through the help section in top-right corner.");
        }
    }
}
