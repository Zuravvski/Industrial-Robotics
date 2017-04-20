using System.Collections.ObjectModel;
using System.IO;
using IDE.Common.Models.Value_Objects;
using IDE.Common.Utilities;
using IDE.Common.ViewModels.Commands;

namespace IDE.Common.Models.Services
{
    public class MacroManager : ObservableObject
    {

        #region Fields

        private readonly ProgramEditor macroEditor;
        private Macro currentMacro;
        private ObservableCollection<Macro> macros;

        #endregion

        #region Constructor

        public MacroManager(ProgramEditor macroEditor)
        {
            Macros = new ObservableCollection<Macro>();
            this.macroEditor = macroEditor;
            LoadMacrosFromFile();  //to populate list with already created macros
        }

        #endregion

        #region Properties

        public ObservableCollection<Macro> Macros
        {
            get
            {
                return macros;
            }
            set
            {
                macros = value;
                NotifyPropertyChanged("Macros");
            }
        }

        public Macro CurrentMacro
        {
            get
            {
                return currentMacro;
            }
            set
            {
                currentMacro = value;
                if (currentMacro == null)
                    macroEditor.IsEnabled = false;
                else
                    macroEditor.IsEnabled = true;
            }
        }

        #endregion

        #region Actions

        public void SaveMacrosToFile()
        {
            Json.SerializeObject(Macros, "MacroDefinitions");
        }


        public void LoadMacrosFromFile()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@".\");
            FileInfo[] info = dirInfo.GetFiles("MacroDefinitions.FLAJS");

            if (info.Length == 0)
                File.Create("MacroDefinitions.FLAJS").Close();

            Macros = Json.DeserializeObject<ObservableCollection<Macro>>("MacroDefinitions");
        }

        #endregion

    }
}
