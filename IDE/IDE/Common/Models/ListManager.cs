using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using IDE.Common.ViewModels.Commands;

namespace IDE.Common.Models
{
    public class ListManager : ObservableObject
    {
        #region Constructor

        public ListManager()
        {
            Programs = new ObservableCollection<Program>();
            PopulateList();
        }

        #endregion

        #region Properties

        public ObservableCollection<Program> Programs { get; }

        #endregion

        #region Actions

        private void PopulateList()
        {
            Programs.Clear();

            try
            {
                var files = Directory.GetFiles(@"Programs", "*.txt");

                foreach (var file in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    Programs.Add(new Program(fileName));   //adds new program to list of local programs
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


        public void CreateProgram(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            Programs.Add(new Program(name));
        }

        public void LoadProgram(Program program)
        {
            if (!string.IsNullOrEmpty(program.Name))
            {
                try
                {
                    program.Content = File.ReadAllText(@"Programs\" + program.Name + ".txt", Encoding.ASCII);
                }
                catch (Exception)
                {
                    MessageBox.Show("Could not load program. File may be corrupted.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void SaveProgram(Program program)
        {
            if (!string.IsNullOrEmpty(program.Name))
            {
                try
                {
                    File.WriteAllText(@"Programs\" + program.Name + ".txt", program.Content, Encoding.ASCII);
                }
                catch (Exception)
                {
                    MessageBox.Show("Could not save program due to invalid data.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void RemoveProgram(Program program)
        {
            if (!string.IsNullOrEmpty(program.Name))
            {
                try
                {
                    File.Delete(@"Programs\" + program.Name + ".txt");
                    Programs.Remove(program);
                }
                catch (Exception) { };
            }
        }

        #endregion

    }
}
