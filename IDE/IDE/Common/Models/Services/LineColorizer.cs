using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;

namespace IDE.Common.Models.Services
{
    public class LineColorizer : DocumentColorizingTransformer
    {
        private readonly int lineNumber;
        private readonly ValidityE isValid;
        private readonly Brush validColor;
        private readonly Brush invalidColor;

        public enum ValidityE
        {
            Yes,
            No
        }

        public LineColorizer(int lineNumber, ValidityE isValid, Brush validColor, Brush invalidColor = null)
        {
            this.lineNumber = lineNumber;
            this.isValid = isValid;
            this.validColor = validColor;
            this.invalidColor = invalidColor ?? Brushes.Red;
        }

        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line)
        {
            if (!line.IsDeleted && line.LineNumber == lineNumber)
            {
                ChangeLinePart(line.Offset, line.EndOffset, element =>
                {
                    element.TextRunProperties.SetBackgroundBrush(isValid == ValidityE.No ? invalidColor : validColor);
                });
            }
        }
    }
}
