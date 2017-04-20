using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System.Windows.Media.Imaging;

namespace IDE.Common.Models.Code_Completion
{
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
                        bitmapImage = Bitmap2BitmapImage(Properties.Resources.Comment);
                        break;

                    case "Movement":
                        bitmapImage = Bitmap2BitmapImage(Properties.Resources.Movement);
                        break;

                    case "Grip":
                        bitmapImage = Bitmap2BitmapImage(Properties.Resources.Grip);
                        break;

                    case "TimersCounters":
                        bitmapImage = Bitmap2BitmapImage(Properties.Resources.TimersCounters);
                        break;

                    case "Programming":
                        bitmapImage = Bitmap2BitmapImage(Properties.Resources.Programming);
                        break;

                    case "Information":
                        bitmapImage = Bitmap2BitmapImage(Properties.Resources.Information);
                        break;

                    case "Macro":
                        bitmapImage = Bitmap2BitmapImage(Properties.Resources.Macros);
                        break;

                    default:
                        bitmapImage = Bitmap2BitmapImage(Properties.Resources.Invalid);
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

        public BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            var bitmapImage = new BitmapImage();
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }
    }
}
