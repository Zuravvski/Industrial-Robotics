using ICSharpCode.AvalonEdit.Rendering;
using System.Windows.Media;

namespace IDE.Common.Models
{
    public class LineColorizer : DocumentColorizingTransformer
    {
        private int lineNumber;
        private IsValid isValid;

        public enum IsValid
        {
            Yes,
            No,
        }

        public LineColorizer(int lineNumber, IsValid isValid)
        {
            this.lineNumber = lineNumber;
            this.isValid = isValid;
        }

        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line)
        {
            if (!line.IsDeleted && line.LineNumber == lineNumber)
            {
                ChangeLinePart(line.Offset, line.EndOffset, ApplyChanges);
            }
        }

        void ApplyChanges(VisualLineElement element)
        {
            // This is where you do anything with the line
            if (isValid == IsValid.No)
                element.TextRunProperties.SetBackgroundBrush(Brushes.Red);
            else
                element.TextRunProperties.SetBackgroundBrush(Brushes.White);    //is this even needed?
        }
    }
}
