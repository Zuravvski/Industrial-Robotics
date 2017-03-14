using manipulatorDriver;
using System;
using System.Globalization;
using System.Threading;

namespace ManipulatorDriver
{
    public class E2JManipulator : Observer
    {

        #region Enums
        public enum GrabE { Closed, Open };
        #endregion

        #region Fields and Properties

        private readonly SerialComm port;
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public double A { get; private set; }
        public double B { get; private set; }
        public GrabE Grab { get; private set; }
        private ResponsiveCommand lastRequest;
        #endregion

        public E2JManipulator()
        {
            port = new SerialBuilder().Build();
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
            Where();
        }

        public void Where()
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

        public void MoveToolStraight(int pos,int dist)
        {
            port.Write(string.Format("MTS {0},{1},C", pos,dist));
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
                    Parse(data);
                    lastRequest = ResponsiveCommand.INVALID;
                    break;
            }
        }

        public bool Parse(string position)
        {
            string[] splitted = position.Replace("+", "").Replace("\r", "").Split(',');
            if (splitted.Length != 10) return false;

            try
            {
                X = double.Parse(splitted[0], CultureInfo.InvariantCulture);
                Y = double.Parse(splitted[1], CultureInfo.InvariantCulture);
                Z = double.Parse(splitted[2], CultureInfo.InvariantCulture);
                A = double.Parse(splitted[3], CultureInfo.InvariantCulture);
                B = double.Parse(splitted[4], CultureInfo.InvariantCulture);
                Grab = splitted[9] == "O" ? GrabE.Open : GrabE.Closed;
            }
            catch (FormatException ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            return true;
        }
    }
}
