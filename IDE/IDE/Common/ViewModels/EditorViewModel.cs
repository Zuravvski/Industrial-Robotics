using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Driver;
using IDE.Common.Models;
using IDE.Common.ViewModels.Commands;
using IDE.Common.Views;
using System.Windows.Media;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using IDE.ViewModels;

namespace IDE.Common.ViewModels
{
    public class EditorViewModel : ObservableObject
    {
        private ProgramEditor programEditor;
        private readonly ProgramService programService;
        private string programName;
        private Program selectedProgram;
        private SolidColorBrush themeColor;

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

        public SolidColorBrush ThemeColor
        {
            set
            {
                themeColor = value;
                NotifyPropertyChanged("ThemeColor");
            }
            get
            {
                return themeColor;
            }
        }

        public static EditorViewModel Instance { set; get; }

        public string Text
        {
            get { return SelectedProgram.Content; }
            set
            {
                SelectedProgram.Content = value;
                NotifyPropertyChanged("Text");
            }
        }

        #endregion

        //usun
        private readonly E3JManipulator manipulator;

        #region Constructor

        public EditorViewModel()
        {    
            ListManager = new ListManager();
            programEditor = new ProgramEditor(ProgramEditor.Highlighting.On, ProgramEditor.ReadOnly.No);

            manipulator = new E3JManipulator();
            manipulator.Connect("COM5");
            programService = new ProgramService(manipulator);

            //ThemeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1BA1E2")); //default color

            ThemeColor = AppearanceViewModel.Instance != null ? 
                new SolidColorBrush(AppearanceViewModel.Instance.SelectedAccentColor) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1BA1E2"));

            Instance = this;
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
                        var dialog = new SaveAsDialog("Please enter name for your program:"); //this dialog fits perfectly for creating new programs
                        if (dialog.ShowDialog() == true)
                        {
                            if (ListManager.Programs.Any(criteria => criteria.Name == dialog.UserInput) &&
                                //to prevent we wont overwrite something by accident)
                                (ProgramEditor.CurrentProgram == null ||
                                 ProgramEditor.CurrentProgram.Name != dialog.UserInput))
                            {
                                if (MessageBox.Show(
                                        "Program with this name already exist. Do you want to overwrite it?",
                                        "File already exist", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                                    MessageBoxResult.No)
                                {
                                    return;
                                }
                            }
                            ListManager.CreateProgram(dialog.UserInput);
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

        public ICommand Load
        {
            get
            {
                return load ?? (load = new DelegateCommand(delegate
                {
                    var dialog = new OpenFileDialog()
                    {
                        Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                        Multiselect = false,
                        RestoreDirectory = true,
                        InitialDirectory = Directory.GetCurrentDirectory()
                    };
                    if (dialog.ShowDialog().GetValueOrDefault())
                    {
                        var name = dialog.FileName;
                        if (!string.IsNullOrEmpty(name))
                        {
                            ListManager.LoadProgram(name);
                        }
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
                        // Program is was previously saved in certain location
                        // we just want to override it from now on
                        if (!string.IsNullOrEmpty(selectedProgram.Path))
                        {
                            ListManager.SaveProgram(SelectedProgram);
                        }
                        // First save requires the user to enter the path
                        // Subsequent saves will override file specified below
                        else
                        {
                            var saveDialog = new SaveFileDialog()
                            {
                                InitialDirectory = Directory.GetCurrentDirectory(),
                                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                                DefaultExt = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                                Title = "Save program as...",
                                FileName = SelectedProgram.Name,
                                OverwritePrompt = true
                            };
                            if (!saveDialog.ShowDialog().GetValueOrDefault()) return;
                            var path = saveDialog.FileName;

                            // Check wheter chosen file path is valid
                            if (!string.IsNullOrEmpty(path))
                            {
                                SelectedProgram.Path = path;
                                ListManager.SaveProgram(SelectedProgram);
                            }  
                        }
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
                    if (selectedProgram != null)
                    {
                        var saveDialog = new SaveFileDialog()
                        {
                            InitialDirectory = string.IsNullOrEmpty(selectedProgram.Path) ? 
                                    Directory.GetCurrentDirectory() : selectedProgram.Path,
                            Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                            DefaultExt = "txt",
                            Title = "Save program as...",
                            FileName = selectedProgram.Name,
                            OverwritePrompt = true
                        };
                        
                        if (saveDialog.ShowDialog().GetValueOrDefault())
                        {
                            var path = saveDialog.FileName;
                            if (!string.IsNullOrEmpty(path))
                            {
                                var program = new Program(Path.GetFileNameWithoutExtension(path))
                                {
                                    Path = path,
                                    Content = selectedProgram.Content
                                };
                                if (selectedProgram.Path.Equals(path))
                                {
                                    ListManager.SaveProgram(selectedProgram);
                                }
                                else
                                {
                                    ListManager.AddProgram(program);
                                    ListManager.SaveProgram(program);
                                    SelectedProgram = program;
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No program was chosen.");
                    }
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
                    var deleteDialog = new DeleteProgramDialog();
                    if (deleteDialog.ShowDialog().GetValueOrDefault())
                    {
                        if (deleteDialog.IsDeleteChecked)
                        {
                            ListManager.DeleteProgram(selectedProgram);
                        }
                        else
                        {
                            ListManager.RemoveProgram(selectedProgram);
                        }
                        SelectedProgram = null;
                    } 
                }));
            }
        }

        public ICommand Send
        {
            get
            {
                return send ?? (send = new DelegateCommand(delegate
                {
                    programService.DownloadProgram(ProgramEditor.CurrentProgram);
                }));
            }
        }
        #endregion
    }
}
