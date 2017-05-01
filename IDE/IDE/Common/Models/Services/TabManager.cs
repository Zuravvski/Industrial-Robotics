using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Driver;
using IDE.Common.Utilities;
using IDE.Common.ViewModels.Commands;

namespace IDE.Common.Models.Services
{
    public class TabManager : ObservableObject
    {

        #region Constructor

        public TabManager()
        {

        }

        #endregion

        #region Properties

        public ObservableCollection<Program> Programs { get; }

        #endregion

        #region Actions

        public void CreateProgram(string name)
        {

        }

        public void LoadProgram(string path)
        {

        }

        public void SaveProgram(Program program)
        {

        }

        public void AddProgram(Program program)
        {

        }

        public void RemoveProgram(Program program)
        {

        }

        public void DeleteProgram(Program program)
        {

        }

        #endregion
    }
}
