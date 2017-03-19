using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;
using ManipulatorDriver;

namespace IDE
{
    public class Console : RichTextBox
    {
        // TODO: Text wrapper
        // TODO: Add line counting
        // TODO: Consider ";" as the end of instruction
        private readonly FlowDocument document;
        private readonly E2JManipulator manipulator;

        private uint counter = 0; // Just for test

        public Console()
        {
            document = new FlowDocument();
            Document = document; 
            manipulator = new E2JManipulator();
            manipulator.Connect("COM3");

            document.LineHeight = 5.0;
        }

        public void uploadProgram()
        {
           manipulator.LineRead(counter+=10);
        }

        public void downloadProgram()
        {
            var linesOfCode = new TextRange(document.ContentStart, document.ContentEnd).Text.Split('\n');
            var lines = 0;
            manipulator.Number("10");
            foreach(var line in linesOfCode)
            {
                if (line.Length != 0)
                {
                    manipulator.Port.Write(line);
                }
            }
            manipulator.End();
            manipulator.Run();
        }
    }
}
