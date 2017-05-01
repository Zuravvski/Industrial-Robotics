using Driver;
using IDE.Common.ViewModels.Converters;
using System.Collections.ObjectModel;
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

        /// <summary>
        /// Initializes new tab.
        /// </summary>
        /// <param name="program">Pass existing program or null for new, fresh tab.</param>
        public TabItem(int untitledsCount, Program program)
        {
            if (program == null)
            {
                Header = $"Untitled {untitledsCount}*";
                Content = new ProgramEditor(ProgramEditor.Highlighting.On, ProgramEditor.UseIntellisense.Yes) { Text = string.Empty };
            }
            else
            {
                Header = program.Name;
                Content = new ProgramEditor(ProgramEditor.Highlighting.On, ProgramEditor.UseIntellisense.Yes) { Text = program.Content };
            }
        }

        #endregion

        #region Properties

        public string Header { get; private set; }
        public ProgramEditor Content { get; private set; }
        public BitmapImage Image { get; private set; }

        #endregion

    }
}
