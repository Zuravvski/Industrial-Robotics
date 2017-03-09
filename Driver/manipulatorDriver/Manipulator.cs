using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace manipulatorDriver
{
    public class Manipulator : SerialComm
    {
        public void GrabClose()
        {
            Write("GC");
        }

        public void GrabOpen()
        {
            Write("GO");
        }

        // TODO: Fix this
        public void Tool(int length)
        {
            if (length >= 0 && length <= 300)
            {
                Write("TL " + Convert.ToString(length));
            }
        }

        public void MovePosition(float x, float y, float z, float a, float b)
        {
            Write(string.Format("MP {0},{1},{2},{3},{4}", x, y, z, a, b));
        }

        public void MoveAway(float x, float y, float z, float a, float b)
        {
            Write(string.Format("MP {0},{1},{2},{3},{4}", x, y, z, a, b));
        }

        public void Draw(float x, float y, float z)
        {
            Write(string.Format("DW {0},{1},{2}", x, y, z));
        }
    }
}
