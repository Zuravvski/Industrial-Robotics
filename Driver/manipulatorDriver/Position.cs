using System;

namespace manipulatorDriver
{
    public class Position
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float A { get; set; }
        public float B { get; set; }

        public Position()
        {
            
        }

        public Position(string position)
        {
            // TODO: Add regex to validate given string
            // TODO: Add handling of R and A (rotation and something)
            var splitted = position.Replace("+", "").Split(',');
            X = Convert.ToSingle(splitted[0]);
            Y = Convert.ToSingle(splitted[1]);
            Z = Convert.ToSingle(splitted[2]);
            A = Convert.ToSingle(splitted[3]);
            B = Convert.ToSingle(splitted[4]);
        }

        public Position(float x, float y, float z, float a, float b)
        {
            X = x;
            Y = y;
            Z = z;
            A = a;
            B = b;
        }
    }
}
