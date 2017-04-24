using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit;

namespace IDE.Common.Models.Services
{
    public class LineColorizer : DocumentColorizingTransformer
    {
        private readonly int lineNumber;
        private readonly ValidityE isValid;

        public enum ValidityE
        {
            Yes,
            No
        }

        public LineColorizer(int lineNumber, ValidityE isValid)
        {
            this.lineNumber = lineNumber;
            this.isValid = isValid;
        }

        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line)
        {

            if (!line.IsDeleted && line.LineNumber == lineNumber)
            {
                ChangeLinePart(line.Offset, line.EndOffset, element =>
                {
                    var x = element.TextRunProperties.ForegroundBrush;
                    element.TextRunProperties.SetBackgroundBrush(isValid == ValidityE.No ? Brushes.Red : new SolidColorBrush(Color.FromRgb(61, 61, 61)));
                });
            }
        }
    }
}
