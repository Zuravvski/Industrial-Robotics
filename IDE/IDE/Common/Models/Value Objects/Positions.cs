namespace IDE.Common.Models.Value_Objects
{
    public class Positions
    {
        /*this is just a dump class to demonstrate how it could be done.
          It should be put in "Program" class but i didn't want to mess there. 
          It's only used to populate PositionManager datagrid in Editor */
        
        public int Pos { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double L1 { get; set; }
        public string OC { get; set; }

    }
}
