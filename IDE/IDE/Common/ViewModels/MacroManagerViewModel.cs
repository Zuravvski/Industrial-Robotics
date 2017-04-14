using IDE.Common.Models;
using IDE.Common.ViewModels.Commands;
using IDE.Common.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDE.Common.ViewModels
{
    public class MacroManagerViewModel : ObservableObject
    {
        private MacroManager macroManager;
        private ProgramEditor macroEditor;
        private string currentMacroText;

        public MacroManagerViewModel()
        {
            DeclareCommands();
            InitializeMacroEditor();

            macroManager = new MacroManager();
        }


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

        public string CurrentMacroText
        {
            private set
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
            MacroEditor = new ProgramEditor(ProgramEditor.Highlighting.On);
        }

        private void Create(object obj)
        {
            SaveAsDialog dialog = new SaveAsDialog("How would you like to call your macro?");

            if (dialog.ShowDialog() == true)
            {
                macroManager.Macros.Add(new Macro(dialog.UserInput, string.Empty));
            }

            MacroEditor.CurrentMacro = macroManager.Macros[macroManager.Macros.Count];  //sets new macro as current one
        }

        private void Load(object obj)
        {
            throw new NotImplementedException();
        }

        private void Save(object obj)
        {
            throw new NotImplementedException();
        }

        private void Delete(object obj)
        {
            throw new NotImplementedException();
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
            LoadCommand = new RelayCommand(Load);
            SaveCommand = new RelayCommand(Save);
            DeleteCommand = new RelayCommand(Delete);
        }


        #endregion

    }
}
