using FirstFloor.ModernUI.Windows.Controls;
using System.Windows.Controls;

namespace IDE.Common.Views
{
    /// <summary>
    /// Interaction logic for ConnectionDialog.xaml
    /// </summary>
    public partial class ConnectionDialog : ModernDialog
    {
        public ConnectionDialog()
        {
            InitializeComponent();

            // define the dialog buttons
            Buttons = new[] { OkButton, CancelButton };
        }
    }
}
