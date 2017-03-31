using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Xml;
using System.Windows.Input;
using System.IO;
using System.Windows;
using IDE.Common.ViewModel;

namespace IDE.Views
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {

        #region Constructor

        public Editor()
        {
            InitializeComponent();
            this.DataContext = new EditorViewModel();
        }

        #endregion

    }
}
