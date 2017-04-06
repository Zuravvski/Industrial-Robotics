using System.Collections.Generic;
using System.IO;
using System.Windows;
using Driver;
using IDE.Common.Model;

namespace IDE.Common.Models
{
    public class ListManager
    {
        private List<Program> programList;

        #region Constructor

        public ListManager()
        {
            programList = new List<Program>();
            PopulateList();
        }

        #endregion

        #region Properties

        public List<Program> List
        {
            private set
            {
                programList = value;
            }
            get
            {
                return programList;
            }
        }

        #endregion

        #region Actions

        public void PopulateList()
        {
            programList.Clear();

            try
            {
                var files = Directory.GetFiles(@"Programs", "*.txt");

                foreach (var file in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    programList.Add(new Program(fileName));   //adds new program to list of local programs
                }

            } 
            catch(DirectoryNotFoundException)
            {
                if (MessageBox.Show("Local storage folder not found. Create one?", "Data not found", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Directory.CreateDirectory(@"Programs");
                }
                else
                {
                    MessageBox.Show("In order to start using Editor you must create folder named Programs upon bin/Debug directory. " +
                        "If you want to do this automatically, please accept after error occurs.",
                        "Data not found",MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        #endregion

    }
}
