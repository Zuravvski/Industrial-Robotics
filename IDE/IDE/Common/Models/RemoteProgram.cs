using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Common.Models
{
    public class RemoteProgram
    {
        
        public RemoteProgram(string name)
        {
            Name = name;
        }


        #region Properties

        public string Name { get; private set; }

        #endregion

    }
}
