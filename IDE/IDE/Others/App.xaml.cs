using System.Windows;
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
            MissingFileManager.CheckForRequiredFiles();
            Session.Instance.Initialize();
        }
    }
}
