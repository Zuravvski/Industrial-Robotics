using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Common.Models
{
    public class MacroManager
    {
       // Collection<Macro> macros;

        public MacroManager()
        {
            Macros = new Collection<Macro>();
        }

        public Collection<Macro> Macros { get; private set; }

    }
}
