using System.Windows;

namespace ManipulatorDriver
{
    public class E2JManipulator : Observer
    {
        #region Fields and Properties

        private readonly SerialComm port;
        private ResponsiveCommand lastRequest;

        public Position Localization { get; }
        #endregion

        public E2JManipulator()
        {
            port = new SerialComm();
            Localization = new Position();
            lastRequest = ResponsiveCommand.INVALID;
            port.Subscribe(this);
        }

        public void Connect(string portName)
        {
            // TODO: Launching servo seems a pretty good idea
            port.OpenPort(portName);
        }

        public void Disconnect()
        {
            port.ClosePort();
        }

        public void GrabClose()
        {
            port.Write("GC");
        }

        public void GrabOpen()
        {
            port.Write("GO");
        }

        public void MovePosition(float x, float y, float z, float a, float b)
        {
            port.Write($"MP {x},{y},{z},{a},{b}");
        }

        public void MoveAway(float x, float y, float z, float a, float b)
        {
            port.Write($"MP {x},{y},{z},{a},{b}");
        }

        public void Draw(float x, float y, float z)
        {
            port.Write($"DW {x},{y},{z}");
        }

        private void Where()
        {
            port.Write("WH");
            lastRequest = ResponsiveCommand.WH;
        }

        private enum ResponsiveCommand
        {
            INVALID, WH
        };

        // TODO: Consider regex validation
        public void getNotified(string data)
        {
            switch (lastRequest)
            {
                case ResponsiveCommand.WH:
                    if (Localization.parse(data))
                    {
                        lastRequest = ResponsiveCommand.INVALID;
                    }
                    break;
            }
        }
    }
}
