using IDE.Common.ViewModels.Converters;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace IDE.Common.Models
{
    public class TabItem
    {
        public HeaderImage headerImage;

        #region enums

        public enum HeaderImage
        {
            Saved,
            Unsaved
        }

        #endregion

        #region Constructor

        public TabItem(string header = "new")
        {
            Header = header;
            Content = new ProgramEditor(ProgramEditor.Highlighting.On, ProgramEditor.UseIntellisense.Yes);
        }

        #endregion

        #region Properties

        public string Header { get; private set; }
        public ProgramEditor Content { get; private set; }
        public BitmapImage Image { get; private set; }

        #endregion

    }
}
