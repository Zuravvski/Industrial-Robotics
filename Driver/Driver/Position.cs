namespace Driver
{
    public class Position
    {
        public int ID { get; private set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; } 
        public double A { get; private set; }
        public double B { get; private set; }
        public E3JManipulator.GrabE Grab { get; private set; }

        public Position(int id)
        {
            ID = id;
        }
    }
}
