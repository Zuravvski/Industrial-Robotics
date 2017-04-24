using IDE.Common.Models;
using IDE.Common.ViewModels.Commands;
using IDE.Common.Views;
using System.Windows.Input;
using IDE.Common.Models.Services;
using IDE.Common.Models.Value_Objects;

namespace IDE.Common.ViewModels
{
    public class MacroManagerViewModel : ObservableObject
    {

        #region Fields

        private ProgramEditor macroEditor;
        private Macro selectedMacro;
        private string currentMacroText;

        #endregion

        #region Constructor

        public MacroManagerViewModel()
        {
            DeclareCommands();
            InitializeMacroEditor();

            MacroManager = new MacroManager(MacroEditor);
            MacroManager.CurrentMacro = null;   //after startup our editor will be disabled till we create/load program
        }

        #endregion

        #region Properties

        public ProgramEditor MacroEditor
        {
            private set
            {
                macroEditor = value;
                NotifyPropertyChanged("MacroEditor");
            }
            get
            {
                return macroEditor;
            }
        }

        public MacroManager MacroManager { get; private set; }

        public Macro SelectedMacro
        {
            set
            {
                selectedMacro = value;
                NotifyPropertyChanged("SelectedMacro");
            }
            get
            {
                return selectedMacro;
            }
        }

        public string CurrentMacroText
        {
            set
            {
                currentMacroText = value;
                NotifyPropertyChanged("CurrentMacroText");
            }
            get
            {
                return currentMacroText;
            }
        }

        #endregion

        #region Actions

        private void InitializeMacroEditor()
        {
            MacroEditor = new ProgramEditor(ProgramEditor.HighlightingE.On, ProgramEditor.UseIntellisense.Yes);
        }

        private void Create(object obj)
        {
            SaveAsDialog dialog = new SaveAsDialog("How would you like to call your macro?");

            if (dialog.ShowDialog() == true)
            {
                MacroManager.Macros.Add(new Macro(dialog.UserInput, string.Empty));
            }

            MacroManager.CurrentMacro = MacroManager.Macros[MacroManager.Macros.Count - 1]; //sets new macro as current one
            CurrentMacroText = MacroManager.CurrentMacro.Name;                              //and update text above editor
            MacroEditor.Text = MacroManager.CurrentMacro.Content;                           //aswell as editor itself

            MacroManager.SaveMacrosToFile();
        }

        private void Load(object obj)
        {
            if (SelectedMacro != null)
            {
                MacroManager.CurrentMacro = SelectedMacro;
                CurrentMacroText = MacroManager.CurrentMacro.Name;
                MacroEditor.Text = MacroManager.CurrentMacro.Content;

                SelectedMacro = null;
            }
        }

        private void Save(object obj)
        {
            MacroManager.CurrentMacro.Content = MacroEditor.Text;

            var macros = MacroManager.Macros;

            MacroManager.SaveMacrosToFile();

            SelectedMacro = null;
        }

        private void Delete(object obj)
        {
            if (SelectedMacro != null)
            {
                if (SelectedMacro == MacroManager.CurrentMacro)
                {
                    MacroEditor.Text = string.Empty;
                    MacroManager.CurrentMacro = null;
                    CurrentMacroText = null;
                }
                MacroManager.Macros.Remove(SelectedMacro);

                MacroManager.SaveMacrosToFile();

                SelectedMacro = null;
            }
        }

        #endregion

        #region Commands

        public ICommand CreateCommand { get; private set; }
        public ICommand LoadCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }


        private void DeclareCommands()
        {
            CreateCommand = new RelayCommand(Create);
            LoadCommand = new RelayCommand(Load, IsItemSelected);
            SaveCommand = new RelayCommand(Save, IsCurrentMacroNotNull);
            DeleteCommand = new RelayCommand(Delete, IsItemSelected);
        }

        private bool IsItemSelected(object obj)
        {
            return SelectedMacro != null;
        }

        private bool IsCurrentMacroNotNull(object obj)
        {
            return MacroManager.CurrentMacro != null;
        }

        #endregion

    }
}
