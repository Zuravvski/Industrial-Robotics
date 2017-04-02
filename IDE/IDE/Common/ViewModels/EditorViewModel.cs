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

namespace IDE.Common.ViewModel
{
    public class EditorViewModel : INotifyPropertyChanged
    {
        private List<Program> programList;
        private ProgramEditor programEditor;
        private ListManager listManager;
        private string programName;

        #region Constructor

        public EditorViewModel()
        {
            SaveClickCommand = new RelayCommand(Save);
            LoadClickCommand = new RelayCommand(Load);           
            
            listManager = new ListManager();
            ProgramList = listManager.List;

            programEditor = new ProgramEditor(Highlighting.On);
            ProgramEditor = programEditor;
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
        public ICommand SaveClickCommand { set; get; }
        public ICommand LoadClickCommand { set; get; }

        #endregion

        #region Actions

        public void Save(object obj)
        {
            try
            {   
                if (ProgramList.Any(criteria => criteria.Name == ProgramName) && 
                    (ProgramEditor.CurrentProgram == null || ProgramEditor.CurrentProgram.Name != ProgramName))
                {
                    //to prevent we wont overwrite something by accident (it wont pop if we just "save" not "save as")
                    if (MessageBox.Show("Program with this name already exist. Do you want to overwrite it?",
                        "File already exist", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                ProgramEditor.CurrentProgram = new Program(ProgramName) { Content = ProgramEditor.Text };
                ProgramEditor.CurrentProgram.SaveProgram(ProgramName);
                ProgramList = new ListManager().List;
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
