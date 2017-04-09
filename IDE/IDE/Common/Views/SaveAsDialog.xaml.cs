using FirstFloor.ModernUI.Windows.Controls;
using System.Windows.Controls;

namespace IDE.Common.Views
{
    /// <summary>
    /// Interaction logic for SaveAsDialog.xaml
    /// </summary>
    public partial class SaveAsDialog : ModernDialog
    {
        public SaveAsDialog(string textAboveInput)
        {
            InitializeComponent();

            TextBlock_AboveInput.Text = textAboveInput;
            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton, this.CancelButton };
        }

        public string UserInput
        {
            get { return TextBox_UserInput.Text; }
        }
    }
}
