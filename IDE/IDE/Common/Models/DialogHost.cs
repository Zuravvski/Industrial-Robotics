using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Common.Models
{
    public class DialogHost
    {

        #region Constructor

        public DialogHost()
        {

        }

        #endregion

        #region Properties
        
        public string CurrentAction { get; set; }
        public string CurrentProgress { get; set; }
        public string Message { get; set; }

        #endregion

    }
}
