namespace IDE.Common.Models.Value_Objects
{
    /// <summary>
    /// Position class
    /// </summary>
    public class Positions
    {
        /*this is just a dump class to demonstrate how it could be done.
          It should be put in "Program" class but i didn't want to mess there. 
          It's only used to populate PositionManager datagrid in Editor */

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public int Pos { get; set; }
        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public double X { get; set; }
        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public double Y { get; set; }
        /// <summary>
        /// Gets or sets the z.
        /// </summary>
        /// <value>
        /// The z.
        /// </value>
        public double Z { get; set; }
        /// <summary>
        /// Gets or sets a.
        /// </summary>
        /// <value>
        /// a.
        /// </value>
        public double A { get; set; }
        /// <summary>
        /// Gets or sets the b.
        /// </summary>
        /// <value>
        /// The b.
        /// </value>
        public double B { get; set; }
        /// <summary>
        /// Gets or sets the l1.
        /// </summary>
        /// <value>
        /// The l1.
        /// </value>
        public double L1 { get; set; }
        /// <summary>
        /// Gets or sets the oc.
        /// </summary>
        /// <value>
        /// The oc.
        /// </value>
        public string OC { get; set; }

    }
}
