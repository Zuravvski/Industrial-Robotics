﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using Driver;
using IDE.Common.Utilities;
using IDE.Common.ViewModels.Commands;

namespace IDE.Common.Models.Services
{
    public class ListManager : ObservableObject
    {
        public ListManager()
        {
            Programs = AppSession.LoadSession();
        }
      
        #region Properties

        public ObservableCollection<Program> Programs { get; }

        #endregion

        #region Actions

        public void CreateProgram(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            Programs.Add(new Program(name));
        }

        public void LoadProgram(string path)
        {
            try
            {
                var name = Path.GetFileNameWithoutExtension(path);
                if (string.IsNullOrEmpty(name)) return;
                var program = new Program(name)
                {
                    Path = path
                };
                program.Content = File.ReadAllText(program.Path, Encoding.ASCII);
                Programs.Add(program);
                AppSession.SaveSession(Programs);
            }
            catch (Exception)
            {
                MessageBox.Show("Could not load program. File may be corrupted.", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SaveProgram(Program program)
        {
            if (string.IsNullOrEmpty(program.Name) && string.IsNullOrEmpty(program.Path)) return;
            try
            {
                var path = program.Path;
                File.WriteAllText(path, program.Content, Encoding.ASCII);
                AppSession.SaveSession(Programs);
            }
            catch (Exception)
            {
                MessageBox.Show("Could not save program due to invalid data.", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AddProgram(Program program)
        {
            Programs.Add(program);
        }

        public void RemoveProgram(Program program)
        {
            Programs.Remove(program);
        }

        public void DeleteProgram(Program program)
        {
            if (string.IsNullOrEmpty(program.Name)) return;
            if (File.Exists(program.Path))
            {
                File.Delete(program.Path);
            }
            RemoveProgram(program);
            AppSession.SaveSession(Programs);
        }
        #endregion

    }
}