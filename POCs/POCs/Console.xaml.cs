using System;
using System.Windows.Controls;
using System.Windows.Documents;

namespace POCs
{
    /// <summary>
    /// Interaction logic for Console.xaml
    /// </summary>
    public partial class Console : UserControl
    {
        private int _lineCounter = 1;
        private readonly FlowDocument document;

        public Console()
        {
            InitializeComponent();
            document = new FlowDocument {LineHeight = 2};
            richTextBox.Document = document;
            richTextBox.TextChanged += RichTextBox_TextChanged;
            lineCountingBlock.LineHeight = 18;
        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textLength = new TextRange(document.ContentStart, document.ContentEnd).Text.Split('\n').Length - 1;
            if (textLength > _lineCounter)
            {
                lineCountingBlock.Text += System.Environment.NewLine + (++_lineCounter);
            }
            else if (textLength < _lineCounter)
            {
                lineCountingBlock.Text =
                    lineCountingBlock.Text.Replace(Convert.ToString(System.Environment.NewLine + _lineCounter--), "");
            }
        }
    }
}
