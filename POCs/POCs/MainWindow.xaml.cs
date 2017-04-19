using System.Windows;
using Driver;

namespace POCs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ProgramService programService;

        public MainWindow()
        {
            InitializeComponent();
            var manipulator = new E3JManipulator(DriverSettings.CreateDefaultSettings());
            manipulator.Connect("COM5");
            
            programService = new ProgramService(manipulator);
            doIt();
        }

        private async void doIt()
        {
            await programService.UploadProgram("1");
        }
    }
}
