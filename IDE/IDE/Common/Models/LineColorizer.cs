using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace IDE.Common.Models
{
    class LineColorizer : DocumentColorizingTransformer
    {
        int lineNumber;
        IsValid isValid;

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
