using FirstFloor.ModernUI.Windows.Controls;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace IDE.Common.Views
{
    /// <summary>
    /// Interaction logic for DeleteProgramDialog.xaml
    /// </summary>
    public partial class DeleteProgramDialog : ModernDialog
    {
        public DeleteProgramDialog()
        {
            InitializeComponent();

            // define the dialog buttons
            Buttons = new[] { OkButton, CancelButton };
        }

        public bool IsDeleteChecked => checkBox.IsChecked.GetValueOrDefault();
    }
}
