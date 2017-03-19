using System.Diagnostics;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using ManipulatorDriver;
using static System.String;

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
          // manipulator.LineRead(counter+=10);
          manipulator.Speed(25);
        }

        public void downloadProgram()
        {
            //manipulator.MoveTool(5, 0, E2JManipulator.GrabE.Open);
            //var linesOfCode = new TextRange(document.ContentStart, document.ContentEnd).Text.Split('\r', '\n');
            ////manipulator.Number("LOL");
            //foreach (var line in linesOfCode)
            //{
            //    if (IsNullOrEmpty(line)) continue;
            //    manipulator.Port.Write(line);
            //    Thread.Sleep(500);
            //}
            ////manipulator.End();
            ////manipulator.Run(10, 50, "2");
            //Debug.WriteLine("Written successfully (hopefully)");
            manipulator.Run();
        }
    }
}
