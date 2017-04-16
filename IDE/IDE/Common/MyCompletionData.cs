using System;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace IDE.Common
{
    public class MyCompletionData : ICompletionData
    {
        public MyCompletionData(string text, string description)
        {
            Text = text;
            Description = description;
        }

        public System.Windows.Media.ImageSource Image => null;

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content => Text;

        public object Description { get; private set; }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }

        public double Priority { get; }
    }
}