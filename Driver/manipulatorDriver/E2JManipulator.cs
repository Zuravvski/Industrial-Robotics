using manipulatorDriver;

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
            port = new SerialBuilder().Build();
            Localization = new Position();
            lastRequest = ResponsiveCommand.INVALID;
            port.Subscribe(this);
        }

        public void Connect(string portName)
        {
            // TODO: Launching servo seems a pretty good idea
            port.OpenPort(portName);
            Where();
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

        public void GripFlag(int flag)
        {
           port.Write(string.Format("GF {0})", flag));
        }

        public void GripPressure(int sf, int rf, int rt)
        {
            port.Write(string.Format("GP {0},{1},{2})", sf, rf, rt));
        }
        public void Here(int num)
        {
            port.Write(string.Format("HE {0}", num));
        }
        public void Home()
        {
            port.Write(string.Format("HO"));
        }

        public void IncrementPostion()
        {
            port. Write(string.Format("IP"));
        }

        public void JointRollChange(int num)
        {
            port.Write(string.Format("JRC {0}", num));
        }

        public void MoveContinuous(int pos1, int pos2)
        {
            port.Write(string.Format("MC {0},{1}", pos1, pos2));
        }

        public void MoveJoint(int pos1, int pos2, int pos3, int pos4)
        {
            port.Write(string.Format("MJ {0},{1},{2},{3},{4}", pos1, pos2, pos3, pos4));
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
                    if (Localization.Parse(data))
                    {
                        lastRequest = ResponsiveCommand.INVALID;
                    }
                    break;
            }
        }
    }
}
