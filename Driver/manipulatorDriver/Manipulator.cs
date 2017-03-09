using System;

namespace manipulatorDriver
{
    public class Manipulator : SerialComm
    {
        public Position Localization { get; }

        public Manipulator()
        {
            // Implement localization
        }

        private void Port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // Booo! Something's got a little tricky
        }

        public void GrabClose()
        {
            Write("GC");
        }

        public void GrabOpen()
        {
            Write("GO");
        }

        public void MovePosition(float x, float y, float z, float a, float b)
        {
            Write($"MP {x},{y},{z},{a},{b}");
        }

        public void MoveAway(float x, float y, float z, float a, float b)
        {
            Write($"MP {x},{y},{z},{a},{b}");
        }

        public void Draw(float x, float y, float z)
        {
            Write($"DW {x},{y},{z}");
        }

        private void Where()
        {
            Write("WH");
        }
    }
}
