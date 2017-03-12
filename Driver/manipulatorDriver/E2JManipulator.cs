using System.Text.RegularExpressions;

namespace manipulatorDriver
{
    public class E2JManipulator : SerialComm
    {
        public Position Localization { get; }
        public string RawDataReceived { get; private set; }

        private ResponsiveCommand lastRequest;

        public E2JManipulator()
        {
            // Implement localization

            // Receiving data
            lastRequest = ResponsiveCommand.INVALID;
            port.DataReceived += Port_DataReceived;
        }

        private void Port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            RawDataReceived = port.ReadExisting();

            // TODO: Reconsider this structure, as it can grow quite large
            switch (lastRequest)
            {
                case ResponsiveCommand.WH:
                    // TODO: Write suitable regex 
                    if (Regex.IsMatch(RawDataReceived, ".*")) 
                    {
                        // update position
                    }
                    break;
            }

            NotifyObservers(RawDataReceived);
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
            lastRequest = ResponsiveCommand.WH;
        }

        private enum ResponsiveCommand
        {
            INVALID, WH
        };
    }
}
