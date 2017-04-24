using System.Windows;
using Driver;
using IDE.Common.Utilities;

namespace IDE.Others
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Session.Instance.Initialize();
        }
    }
}
