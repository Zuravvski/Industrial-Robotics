using Dragablz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Common.Utilities
{
    public static class TabItemFactory
    {
        public static Func<HeaderedItemViewModel> Factory
        {
            get
            {
                return
                    () =>
                    {
                        return new HeaderedItemViewModel(DateTime.Now.Date.ToString(), null)
                        {
                            Header = DateTime.Now.Date.ToString(),
                            Content = null,
                        };
                    };
            }
        }
    }
}
