using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Driver;
using ManipulatorDriver;
using IDE.Common.ViewModel;
using IDE.Common.Views;
using System.Collections.Specialized;

namespace IDE.Views
{
    /// <summary>
    /// Interaction logic for Browse.xaml
    /// </summary>
    public partial class Browse : UserControl//, Observer
    {

        public Browse()
        {
            InitializeComponent();
            DataContext = new BrowseViewModel();

            //there is NO OTHER WAY to keep ListView autoscrolling than in code-behind:
            ((INotifyCollectionChanged)msglist.Items).CollectionChanged += MessageList_CollectionChanged;
        }

        private void MessageList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // scroll the new item into view   
                msglist.ScrollIntoView(e.NewItems[0]);
            }
        }

        private void msglist_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateColumnsWidth(sender as ListView);
        }

        private void msglist_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateColumnsWidth(sender as ListView);
        }

        private void UpdateColumnsWidth(ListView listView)
        {
            int autoFillColumnIndex = (listView.View as GridView).Columns.Count - 1;
            if (listView.ActualWidth == Double.NaN)
                listView.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            double remainingSpace = listView.ActualWidth;
            for (int i = 0; i < (listView.View as GridView).Columns.Count; i++)
                if (i != autoFillColumnIndex)
                    remainingSpace -= (listView.View as GridView).Columns[i].ActualWidth;
            (listView.View as GridView).Columns[autoFillColumnIndex].Width = remainingSpace >= 0 ? remainingSpace : 0;
        }

        //private FlowDocument document;
        //private E3JManipulator manipulator;
        //int k = 0;  //for iterating program infos in Notifer
        //string[] receivedProgramInfo;
        //string[] programNames;
        //string receivedProgram;

        //public Browse()
        //{
        //    InitializeComponent();

        //    manipulator = new E3JManipulator();
        //    manipulator.Connect("COM3");
        //    manipulator.Port.Subscribe(this);

        //    receivedProgramInfo = new string[50];   //too big
        //    programNames = new string[50];  //too big

        //    Task.Run(() => RefreshList());
        //}

        //private void RefreshList()
        //{
        //    for (int i = 0; i < 10; i++)    //send request for full program info;;; note: 10 is just a random number, has to be improved
        //    {                               //10 - numbers of programs in manipulator memory (till we are not getting only "QoK" alone)
        //        if (i == 0)
        //        {
        //           manipulator.SendCustom(@"EXE0, ""Fd<""");
        //        }
        //        else
        //        {
        //            manipulator.SendCustom(@"EXE0, ""Fd+1""");
        //        }
        //        Thread.Sleep(500);
        //    }


        //    for (int i = 0; i < receivedProgramInfo.Length; i++)    //cut only program name
        //    {
        //        if (receivedProgramInfo[i] != null && receivedProgramInfo[i] != "QoK\r")
        //        {
        //            int startIndex = receivedProgramInfo[i].IndexOf("QoK") + "QoK".Length;
        //            int endIndex = receivedProgramInfo[i].IndexOf(".RE2", startIndex);
        //            programNames[i] = receivedProgramInfo[i].Substring(startIndex, endIndex - startIndex);
        //        }
        //    }

        //    listView_RobotList.Items.Clear();
        //    foreach(string programName in programNames) //adds program names to list
        //    {
        //        if (programName != null)
        //        {
        //            listView_RobotList.Items.Add(programName);
        //        }
        //    }
        //}

        //private void button_Save_Click(object sender, RoutedEventArgs e)
        //{
        //    if (listView_RobotList.SelectedItem != null)
        //    {
        //        SaveSelectedProgram();
        //        SaveSelectedPositions();
        //    }
        //}

        //private void SaveSelectedPositions()
        //{

        //}

        //private void SaveSelectedProgram()
        //{
        //    receivedProgram = "";
        //    string selectedItem = listView_RobotList.SelectedItem.ToString();
        //    int numberOfLines = 55;

        //    for (uint i = 0; i < numberOfLines; i++)
        //    {
        //        if (i == 0)
        //        {
        //            manipulator.Number(selectedItem); //selects program e.g. >> N "WTOREK" <<
        //            Thread.Sleep(1000);
        //        }
        //        else
        //        {
        //            manipulator.StepRead(i);
        //        }
        //        Thread.Sleep(500);
        //    }
        //    MessageBox.Show($@"Program {selectedItem}.txt saved in \bin\Programs", "Download finished", MessageBoxButton.OK);

        //    try
        //    {
        //        File.WriteAllText(@"Programs\" + selectedItem + ".txt", receivedProgram, Encoding.ASCII);
        //    }
        //    catch (Exception) { };
        //}

        //private void button_Load_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void listView_RobotList_click(object sender, MouseButtonEventArgs e)
        //{

        //}

        //public void getNotified(string data)
        //{
        //    if (data != null)
        //    {
        //        if (data.Contains("QoK") && data != "QoK")
        //        {
        //            receivedProgramInfo[k++] = data;
        //        }
        //        else if (Regex.IsMatch(data, @"\A\d+"))
        //        {
        //            receivedProgram += data + "\n";
        //        }
        //    }
        //}
    }
}
