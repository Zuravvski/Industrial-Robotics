using System;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System.Windows.Media.Imaging;

namespace IDE.Views
{
    /// Implements AvalonEdit ICompletionData interface to provide the entries in the
    /// completion drop down.
    public class MyCompletionData : ICompletionData
    {
        string type;

        public MyCompletionData(string text, string description, string type)
        {
            Text = text;
            Description = description;
            this.type = type;
        }

        public System.Windows.Media.ImageSource Image
        {
            get
            {
                BitmapImage bitmapImage;
                switch (type)
                {
                    case "Comment":
                        bitmapImage = new BitmapImage(new Uri(@"C:\Users\PR6\workspace\VisualStudio2015\STM\WpfApplication2\WpfApplication2\Icons\comment.png"));
                        break;

                    case "Movement":
                        bitmapImage = new BitmapImage(new Uri(@"C:\Users\PR6\workspace\VisualStudio2015\STM\WpfApplication2\WpfApplication2\Icons\movement.png"));
                        break;

                    case "Grip":
                        bitmapImage = new BitmapImage(new Uri(@"C:\Users\PR6\workspace\VisualStudio2015\STM\WpfApplication2\WpfApplication2\Icons\grip.png"));
                        break;

                    case "TimersCounters":
                        bitmapImage = new BitmapImage(new Uri(@"C:\Users\PR6\workspace\VisualStudio2015\STM\WpfApplication2\WpfApplication2\Icons\timer.png"));
                        break;

                    case "Programming":
                        bitmapImage = new BitmapImage(new Uri(@"C:\Users\PR6\workspace\VisualStudio2015\STM\WpfApplication2\WpfApplication2\Icons\programming.png"));
                        break;

                    case "Information":
                        bitmapImage = new BitmapImage(new Uri(@"C:\Users\PR6\workspace\VisualStudio2015\STM\WpfApplication2\WpfApplication2\Icons\information.png"));
                        break;

                    case "Macro":
                        bitmapImage = new BitmapImage(new Uri(@"C:\Users\PR6\workspace\VisualStudio2015\STM\WpfApplication2\WpfApplication2\Icons\macro.png"));
                        break;

                    default:
                        bitmapImage = new BitmapImage(new Uri(@"C:\Users\PR6\workspace\VisualStudio2015\STM\WpfApplication2\WpfApplication2\Icons\invalid.png"));
                        break;
                }
                return bitmapImage;
            }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return this.Text; }
        }

        public object Description { set; get; }

        public double Priority
        {
            get
            {
                return 0;
            }
        }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }
    }
}