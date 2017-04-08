using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using IDE.Common.ViewModels.Commands;

namespace IDE.Common.Models
{
    public class ListManager : ObservableObject
    {
        public ListManager()
        {
            Programs = AppSession.Instance.LoadLastSession();
        }
      
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
                    // Adds new program to list of local programs
                    Programs.Add(new Program(fileName));  
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

        public void LoadProgram(string path)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            if (string.IsNullOrEmpty(name)) return;
            try
            {
                var program = new Program(name);
                program.Content = File.ReadAllText(@"Programs\" + program.Name + ".txt", Encoding.ASCII);
                Programs.Add(program);
                AppSession.Instance.SaveSession(Programs);
            }
            catch (Exception)
            {
                MessageBox.Show("Could not load program. File may be corrupted.", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SaveProgram(Program program)
        {
            if (string.IsNullOrEmpty(program.Name)) return;
            try
            {
                var path = @"Programs\" + program.Name + ".txt";
                File.WriteAllText(path, program.Content, Encoding.ASCII);
                program.Path = path;
                AppSession.Instance.SaveSession(Programs);
                MessageBox.Show("Saved");

            }
            catch (Exception)
            {
                MessageBox.Show("Could not save program due to invalid data.", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RemoveProgram(Program program)
        {
            if (string.IsNullOrEmpty(program.Name)) return;
            try
            {
                File.Delete(@"Programs\" + program.Name + ".txt");
                Programs.Remove(program);
                //AppSession.Instance.SaveSession(Programs);
            }
            catch (Exception)
            {
                // TBD
            };
        }

        #endregion

    }
}
