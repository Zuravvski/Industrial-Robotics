using System.Windows;
using Driver;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;

namespace POCs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ProgramService programService;
        E3JManipulator manipulator;

        public MainWindow()
        {
            InitializeComponent();
            manipulator = new E3JManipulator(DriverSettings.CreateDefaultSettings());
            manipulator.Connect("COM3");
            
            programService = new ProgramService(manipulator);
            doIt();
        }

        private async void doIt()
        {
            //var program = await programService.UploadProgram("MOJ");
            Program newProgram = new Program("newProgram");
            newProgram.Content = File.ReadAllText("ProgramLaborkaAiR.txt");
            programService.DownloadProgram(newProgram);
            await Task.Delay(1000);
            manipulator.Run();
        }
    }
}
