using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Driver;
using IDE.Common.Models;
using IDE.Common.ViewModels.Commands;
using robotymobilne_projekt.GUI.ViewModels;

namespace IDE.Common.ViewModels
{
    public class EditorViewModel : ObservableObject
    {
        private ProgramEditor programEditor;
        
        private string programName;
        private Program selectedProgram;

        #region Actions
        private ICommand create;
        private ICommand load;
        private ICommand save;
        private ICommand saveAs;
        private ICommand delete;
        private ICommand send;
        #endregion

        #region Properties

        public ListManager ListManager { get; }
        public Program SelectedProgram
        {
            set
            {
                selectedProgram = value;
                programEditor.CurrentProgram = value;
                NotifyPropertyChanged("SelectedProgram");
            }
            get
            {
                return selectedProgram;
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
                NotifyPropertyChanged("ProgramEditor");
            }
        }

        #endregion

        //usun
        private readonly E3JManipulator manipulator;

        #region Constructor

        public EditorViewModel()
        {    
            ListManager = new ListManager();
            ProgramEditor = new ProgramEditor(ProgramEditor.Highlighting.On);

            //deleteTHIS
            manipulator = new E3JManipulator();
            manipulator.Connect("COM4");
        }

        #endregion

        #region Commands

        public ICommand Create
        {
            get
            {
                return create ?? (create = new DelegateCommand(delegate
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(ProgramEditor.Text) &&
                            MessageBox.Show(
                                "Are you sure you wish to create new program? Note that all unsaved changes will be lost.",
                                "File creator", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            return;
                        }
                        var dialog = new SaveAsDialog(); //this dialog fits perfectly for creating new programs
                        if (dialog.ShowDialog() == true)
                        {
                            if (ListManager.Programs.Any(criteria => criteria.Name == dialog.ProgramName) &&
                                //to prevent we wont overwrite something by accident)
                                (ProgramEditor.CurrentProgram == null ||
                                 ProgramEditor.CurrentProgram.Name != dialog.ProgramName))
                            {
                                if (MessageBox.Show(
                                        "Program with this name already exist. Do you want to overwrite it?",
                                        "File already exist", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                                    MessageBoxResult.No)
                                {
                                    return;
                                }
                            }
                            ListManager.CreateProgram(dialog.ProgramName);
                            SelectedProgram = ListManager.Programs[ListManager.Programs.Count - 1];
                        }
                    }
                    catch (Exception)
                    {
                        // TBD
                    }
                }));
            }
        }

        // Not yet implemented
        public ICommand Load
        {
            get
            {
                return load ?? (load = new DelegateCommand(delegate
                {
                    try
                    {
                        ProgramEditor.CurrentProgram = SelectedProgram;
                    }
                    catch (NullReferenceException)
                    {
                        MessageBox.Show("Nothing to load. Please select program first.", "No program selected", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }));
            }
        }

        public ICommand Save
        {
            get
            {
                return save ?? (save = new DelegateCommand(delegate
                {
                    if (selectedProgram != null)
                    {
                        ListManager.SaveProgram(SelectedProgram);
                    }
                    else
                    {
                        MessageBox.Show("No program was chosen.");
                    }
                }));
            }
        }

        public ICommand SaveAs
        {
            get
            {
                return saveAs ?? (saveAs = new DelegateCommand(delegate
                {
                    try
                    {
                        var dialog = new SaveAsDialog();
                        if (dialog.ShowDialog() == true)
                        {
                            if (ListManager.Programs.Any(criteria => criteria.Name == dialog.ProgramName) &&
                                //to prevent we wont overwrite something by accident)
                                (ProgramEditor.CurrentProgram == null ||
                                 ProgramEditor.CurrentProgram.Name != dialog.ProgramName))
                            {
                                if (MessageBox.Show(
                                        "Program with this name already exist. Do you want to overwrite it?",
                                        "File already exist", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                                    MessageBoxResult.No)
                                {
                                    return;
                                }
                            }
                            ProgramEditor.CurrentProgram = new Program(dialog.ProgramName)
                            {
                                Content = ProgramEditor.Text
                            };
                            ListManager.SaveProgram(ProgramEditor.CurrentProgram);
                        }
                    }
                    catch (Exception)
                    {
                        
                    };
                }));
            }
        }

        public ICommand Delete
        {
            get
            {
                return delete ?? (delete = new DelegateCommand(delegate
                {
                    if (SelectedProgram == null)
                    {
                        MessageBox.Show("You need to select program first.", "File remover",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    try
                    {
                        if (MessageBox.Show(
                                $"Are you sure you want to delete: {SelectedProgram.Name}? This operation cannot be undone!",
                                "File remover",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            return;
                        }

                        if (SelectedProgram != ProgramEditor.CurrentProgram)
                        {
                            ListManager.RemoveProgram(SelectedProgram);
                        }
                        //if we want to delete current program we set first program from list as current one (consider empty program?)
                        else
                        {
                            ListManager.RemoveProgram(SelectedProgram);
                            ProgramEditor.CurrentProgram = ListManager.Programs[0];
                        }
                    }
                    catch (Exception)
                    {
                        
                    };
                }));
            }
        }

        public ICommand Send
        {
            get
            {
                return send ?? (send = new DelegateCommand(delegate
                {
                    try
                    {
                        var lines = ProgramEditor.CurrentProgram.GetLines();

                        foreach (var line in lines)
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
                }));
            }
        }
        #endregion
    }
}
