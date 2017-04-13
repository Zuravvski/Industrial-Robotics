using IDE.Common.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Common.Models
{
    public class RemoteListManager : ObservableObject
    {

        public RemoteListManager()
        {

        }

        #region Properties

        public ObservableCollection<RemoteProgram> RemotePrograms { get; }

        #endregion

        #region Actions

        private void RefreshList()
        {

        }

        private void DownloadProgram()
        {

        }

        private void UploadProgram()
        {

        }

        private void DeleteProgram()
        {

        }

        #endregion
    }
}
