using System.Collections.Generic;
using System.Windows.Input;
using IDE.Common.Model;
using System.ComponentModel;
using IDE.Common.Utilities;
using static IDE.Common.Models.ProgramEditor;
using System.Windows;
using System;
using System.Linq;
using IDE.Common.Models;
using IDE.Common.ViewModels.Commands;
using ManipulatorDriver;
using Driver;
using System.Diagnostics;
using System.Threading;
using IDE.Common.Views;

namespace IDE.Common.ViewModel
{
    public class EditorViewModel : INotifyPropertyChanged
    {
        private List<Program> programList;
        private ProgramEditor programEditor;
        private ListManager listManager;
        private string programName;

        //usun
        private E3JManipulator manipulator;

        #region Constructor

        public EditorViewModel()
        {
            CreateClickCommand = new RelayCommand(Create);
            LoadClickCommand = new RelayCommand(Load);
            SaveClickCommand = new RelayCommand(Save);
            SaveAsClickCommand = new RelayCommand(SaveAs);
            DeleteClickCommand = new RelayCommand(Delete);
            SendClickCommand = new RelayCommand(Send);

            
            listManager = new ListManager();
            ProgramList = listManager.List;

            programEditor = new ProgramEditor(Highlighting.On);
            ProgramEditor = programEditor;


            //deleteTHIS
            manipulator = new E3JManipulator();
            manipulator.Connect("COM4");
        }





        #endregion

        #region Properties

        public List<Program> ProgramList
        {
            get
            {
                return programList;
            }
            set
            {
                programList = value;
                OnPropertyChanged("ProgramList");
            }
        }

        public Program SelectedProgram { set; get; }

        public string ProgramName
        {
            get
            {
                return programName;
            }
            set
            {
                programName = value;
                OnPropertyChanged("ProgramName");
            }
        }

        public ProgramEditor ProgramEditor
        {
            get
            {
                return programEditor;
            }
            set
            {
                programEditor = value;
                OnPropertyChanged("ProgramEditor");
            }
        }

        #endregion

        #region Commands

        public bool CanExecute { set; get; }
        public ICommand CreateClickCommand { set; get; }
        public ICommand LoadClickCommand { set; get; }
        public ICommand SaveClickCommand { set; get; }
        public ICommand SaveAsClickCommand { set; get; }
        public ICommand DeleteClickCommand { set; get; }
        public ICommand SendClickCommand { set; get; }

        #endregion

        #region Actions

        private void Create(object obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(ProgramEditor.Text) && MessageBox.Show("Are you sure you wish to create new program? Note that all unsaved changes will be lost.", 
                    "File creator", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }
                var dialog = new SaveAsDialog();    //this dialog fits perfectly for creating new programs
                if (dialog.ShowDialog() == true)
                {
                    if (ProgramList.Any(criteria => criteria.Name == dialog.ProgramName) && //to prevent we wont overwrite something by accident)
                         (ProgramEditor.CurrentProgram == null || ProgramEditor.CurrentProgram.Name != dialog.ProgramName))
                    {
                        if (MessageBox.Show("Program with this name already exist. Do you want to overwrite it?",
                            "File already exist", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        {
                            return;
                        }
                    }
                    ProgramEditor.CurrentProgram = new Program(dialog.ProgramName) { Content = "" };
                    ProgramEditor.CurrentProgram.SaveProgram(dialog.ProgramName);
                    ProgramName = dialog.ProgramName;
                    ProgramList = new ListManager().List;
                }
            }
            catch (Exception) { };
        }

        public void Load(object obj)
        {
            try
            {
                ProgramEditor.CurrentProgram = SelectedProgram;
                ProgramName = ProgramEditor.CurrentProgram.Name;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Nothing to load. Please select program first.", "No program selected", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Save(object obj)
        {
            if (ProgramEditor.CurrentProgram == null || ProgramEditor.CurrentProgram.Name == null)  //if name is not defined yet, define it with save as
            {
                SaveAs(null);
                return;
            }
            try
            {   
                ProgramEditor.CurrentProgram = new Program(ProgramName) { Content = ProgramEditor.Text };
                ProgramEditor.CurrentProgram.SaveProgram(ProgramName);
                ProgramList = new ListManager().List;
            }
            catch (Exception) { };
        }

        private void SaveAs(object obj)
        {
            try
            {
                var dialog = new SaveAsDialog();
                if (dialog.ShowDialog() == true)
                {
                    if (ProgramList.Any(criteria => criteria.Name == dialog.ProgramName) && //to prevent we wont overwrite something by accident)
                         (ProgramEditor.CurrentProgram == null || ProgramEditor.CurrentProgram.Name != dialog.ProgramName))
                    {
                        if (MessageBox.Show("Program with this name already exist. Do you want to overwrite it?",
                            "File already exist", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        {
                            return;
                        }
                    }
                    ProgramEditor.CurrentProgram = new Program(dialog.ProgramName) { Content = ProgramEditor.Text };
                    ProgramEditor.CurrentProgram.SaveProgram(dialog.ProgramName);
                    ProgramName = dialog.ProgramName;
                    ProgramList = new ListManager().List;
                }
            }
            catch (Exception) { };
        }

        private void Delete(object obj)
        {
            if (SelectedProgram == null)
            {
                MessageBox.Show("You need to select program first.", "File remover",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                if (MessageBox.Show($"Are you sure you want to delete: {SelectedProgram.Name}? This operation cannot be undone!", "File remover", 
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }

                if (SelectedProgram != ProgramEditor.CurrentProgram)
                {
                    SelectedProgram.RemoveProgram(SelectedProgram);
                }
                else    //if we want to delete current program we set first program from list as current one (consider empty program?)
                {
                    SelectedProgram.RemoveProgram(SelectedProgram);
                    ProgramEditor.CurrentProgram = ProgramList[0];
                    ProgramName = ProgramList[0].Name;
                }
                ProgramList = new ListManager().List;
            }
            catch (Exception) { };
        }

        private void Send(object obj)
        {
            try
            {
                string[] lines = ProgramEditor.CurrentProgram.Lines;

                foreach (string line in lines)
                {
                    Thread.Sleep(300);
                    manipulator.SendCustom(line);
                    Debug.WriteLine(line);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("wyjebalo error");
            }
        }

        #endregion

        #region PropertyChangedEvents

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
