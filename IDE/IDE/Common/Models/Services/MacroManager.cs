using System.Collections.ObjectModel;
using System.IO;
using IDE.Common.Models.Value_Objects;
using IDE.Common.ViewModels.Commands;
using Newtonsoft.Json;

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
            FromTxt();  //to populate list with already created macros
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

        public void ToTxt()
        {
            string json = JsonConvert.SerializeObject(Macros);
            File.WriteAllText("MacroDefinitions.FLAJS", json);
        }

        public void FromTxt()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(@".\");
            FileInfo[] info = dirInfo.GetFiles("MacroDefinitions.FLAJS");

            if (info.Length == 0)
                File.Create("MacroDefinitions.FLAJS").Close();
            
            string text = File.ReadAllText("MacroDefinitions.FLAJS");

            var json = JsonConvert.DeserializeObject<ObservableCollection<Macro>>(text);
            if (!string.IsNullOrEmpty(text))
                Macros = json;
        }

        #endregion

    }
}
