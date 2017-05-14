using IDE.Common.Models;
using IDE.Common.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using RadialMenu.Controls;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Linq;
using Driver;
using Microsoft.Win32;
using System.IO;
using IDE.Common.Utilities;
using System;

namespace IDE.Common.ViewModels
{
    public class EditorViewModel : ObservableObject
    {

        #region Fields
        private ObservableCollection<Program> programList;

        private TabItem selectedTabItem;
        private int selectedTabIndex;
        private ObservableCollection<TabItem> tabItems;

        #endregion

        #region Constructor

        public EditorViewModel()
        {
            TabItems = new ObservableCollection<TabItem>();
            ProgramList = new ObservableCollection<Program>();
            LoadPreviousSessionTabs();
            DeclareCommands();
            //SetupMainSubmenu();
        }

        #endregion

        #region Properties

        public ObservableCollection<Program> ProgramList
        {
            get
            {
                return programList;
            }
            set
            {
                programList = value;
                NotifyPropertyChanged("ProgramList");
            }
        }

        public AppearanceViewModel Appearance => AppearanceViewModel.Instance;

        public ObservableCollection<TabItem> TabItems
        {
            get
            {
                return tabItems;
            }
            set
            {
                tabItems = value;
                NotifyPropertyChanged("TabItems");

            }
        }
        public TabItem SelectedTabItem
        {
            get
            {
                return selectedTabItem;
            }
            set
            {
                selectedTabItem = value;
                NotifyPropertyChanged("SelectedTabItem");
            }
        }

        public int SelectedTabIndex
        {
            get
            {
                return selectedTabIndex;
            }
            set
            {
                selectedTabIndex = value;
                NotifyPropertyChanged("SelectedTabIndex");
            }
        }

        #endregion

        #region Actions

        private void LoadPreviousSessionTabs()
        {
            foreach (var program in Session.Instance.LoadPrograms())
            {
                TabItems.Add(new TabItem(0, program));
            }
        }

        /// <summary>
        /// Opens new tab or reloads content if tab already exist.
        /// </summary>
        private void OpenFileTab()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "txt files (.txt)|*.txt"
            };

            if (!dialog.ShowDialog().GetValueOrDefault(false))
            {
                //if user fails to select a file
                return;
            }

            var path = Path.GetFullPath(dialog.FileName);
            var name = Path.GetFileNameWithoutExtension(dialog.FileName);
            var content = File.ReadAllText($"{dialog.FileName}");

            var doesTabAlreadyExist = TabItems.FirstOrDefault(i => i.Program != null && i.Program.Path == path);

            if (!Equals(doesTabAlreadyExist, null))
            {
                //if this file is already open reload it's content and set is as current tab
                doesTabAlreadyExist.ProgramEditor.Text = doesTabAlreadyExist.Program.Content;
                doesTabAlreadyExist.UnsavedChanged = false;
                SelectedTabItem = doesTabAlreadyExist;
                return;
            }

            OpenTab(new Program(name) { Path = path, Content = content });
            Session.Instance.SavePrograms(TabItems);
        }

        /// <summary>
        /// Performs saving all tabs. If it is new program Save as will be called, 
        /// else program will be saved in path declared in ~Program.path~.
        /// </summary>
        private void SaveAllTabs()
        {
            foreach (var tab in TabItems)
            {
                SaveTab(tab);
            }
        }

        /// <summary>
        /// Performs save as operation on a new file. 
        /// It's basically the same as ~SaveTab~ operation, except for the fact
        /// that ~tabItem.Program~ might not be null, but we still want to create
        /// new instance of ~Program~ for this ~tabItem.content~.
        /// </summary>
        /// <param name="tabItem">Tab item which content will be saved.</param>
        /// <returns>True if saving was succesfull, false if not.</returns>
        private bool SaveAsTab(TabItem tabItem)
        {
            var dialog = new SaveFileDialog
            {
                FileName = $"{tabItem.Header.Replace("*", string.Empty)}",
                DefaultExt = ".txt",
                Filter = "txt files (.txt)|*.txt"
            };

            if (!dialog.ShowDialog().GetValueOrDefault(false))
            {
                return false;
            }

            var path = Path.GetFullPath(dialog.FileName);
            var name = Path.GetFileNameWithoutExtension(dialog.FileName);

            tabItem.Program = new Program(name) { Path = path, Content = tabItem.ProgramEditor.Text };
            File.WriteAllText(tabItem.Program.Path, tabItem.ProgramEditor.Text);

            //update UI
            tabItem.Header = tabItem.Program.Name;
            tabItem.UnsavedChanged = false;

            Session.Instance.SavePrograms(TabItems);
            return true;
        }

        /// <summary>
        /// Performs "regular" save operation on existing or new file. 
        /// In case of new file you will have to declare file name and location first.
        /// </summary>
        /// <param name="tabItem">Tab item which content will be saved.</param>
        /// <returns>True if saving was succesfull, false if not.</returns>
        private bool SaveTab(TabItem tabItem)
        {
            if (tabItem.Program == null)
            {
                //if it's new tab there is no program corresponding, so create one
                return SaveAsTab(tabItem);
            }
            //else just save it under path declared in ~Program.path~
            File.WriteAllText(tabItem.Program.Path, tabItem.ProgramEditor.Text);


            //update UI
            tabItem.UnsavedChanged = false;
            return true;
        }

        /// <summary>
        /// Opens new tab
        /// </summary>
        /// <param name="program">Pass existing program or null for new, fresh tab.</param>
        private void OpenTab(Program program)
        {
            if (TabItems.Count >= 15)
                return;

            TabItem tabToAdd;

            if (program != null)
            {
                tabToAdd = new TabItem(0, program);
            }
            else
            {
                var matches = 1;
                for (int i = 0; i < TabItems.Count; i++)
                {
                    //if there is "Untitled <nr>" tab, repeat loop to pick another number
                    if (TabItems[i].Header.Contains($"Untitled {matches}"))
                    {
                        matches++;
                        i = -1;
                    }
                }
                tabToAdd = new TabItem(matches, null);
            }
            TabItems.Add(tabToAdd);
            SelectedTabItem = tabToAdd;
        }

        private void AddTab(object obj)
        {
            OpenTab(null);
        }

        private void CloseTab(object obj)
        {
            var tabItem = obj as TabItem;
            SelectedTabItem = tabItem;  //show user what he is about to close

            if (tabItem != null && (tabItem.Program != null && tabItem.UnsavedChanged || tabItem.Program == null && tabItem.ProgramEditor.Text != string.Empty))
            {
                var dialog = MessageBox.Show($"Save file {tabItem.Header.Replace("*", string.Empty)}?",
                    "Unsaved data", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                //if tab contains unsaved changes show dialog to determine wheter user wants to save it or not. User chooses:
                // *cancel - do nothing
                // *yes - show saveAs dialog, if he fails to save: do nothing, if he saves sucessfully: close tab
                // *no - close it without saving,
                if (dialog == MessageBoxResult.Cancel || (dialog == MessageBoxResult.Yes && SaveTab(tabItem) == false))
                {
                    return;
                }
            }

            TabItems.Remove(tabItem);
            Session.Instance.SavePrograms(TabItems);
        }

        #endregion

        #region Commands

        public ICommand AddTabCommand { get; private set; }
        public ICommand CloseTabCommand { get; private set; }
        public ICommand ChangeFontCommand { get; private set; }

        public ICommand CtrlSKey { get; private set; }
        public ICommand CtrlNKey { get; private set; }

        //new menu
        public ICommand MenuCreateCommand { get; private set; }
        public ICommand MenuOpenCommand { get; private set; }
        public ICommand MenuSaveCommand { get; private set; }
        public ICommand MenuSaveAsCommand { get; private set; }
        public ICommand MenuSaveAllCommand { get; private set; }

        private void DeclareCommands()
        {
            AddTabCommand = new RelayCommand(AddTab);
            CloseTabCommand = new RelayCommand(CloseTab);
            ChangeFontCommand = new RelayCommand(ChangeFont, CanChangeFont);

            //new menu
            MenuCreateCommand = new RelayCommand(MenuCreate, IsTabCountBelowMax);
            MenuOpenCommand = new RelayCommand(MenuOpen, IsTabCountBelowMax);
            MenuSaveCommand = new RelayCommand(MenuSave, DoesSelectedTabExist);
            MenuSaveAsCommand = new RelayCommand(MenuSaveAs, DoesSelectedTabExist);
            MenuSaveAllCommand = new RelayCommand(MenuSaveAll, DoesSelectedTabExist);

            CtrlSKey = new RelayCommand(CtrlS);
            CtrlNKey = new RelayCommand(AddTab);
        }

        private bool IsTabCountBelowMax(object obj)
        {
            return TabItems.Count < 15;
        }

        private bool DoesSelectedTabExist(object obj)
        {
            return SelectedTabItem != null;
        }

        private void MenuCreate(object obj)
        {
            OpenTab(null);
        }

        private void MenuOpen(object obj)
        {
            OpenFileTab();
        }

        private void MenuSave(object obj)
        {
            SaveTab(SelectedTabItem);
        }

        private void MenuSaveAs(object obj)
        {
            SaveAsTab(SelectedTabItem);
        }

        private void MenuSaveAll(object obj)
        {
            SaveAllTabs();
        }

        private bool CanChangeFont(object obj)
        {
            if (TabItems.Count == 0)
                return false;

            var text = TabItems[0].ProgramEditor.FontFamily.ToString();
            return !text.Equals(obj as string);
        }

        private void ChangeFont(object obj)
        {
            var fontName = obj as string;
            var fontFamily = TabItems[0]?.ProgramEditor.FontFamily;

            try
            {
                foreach (var tab in TabItems)
                {
                    tab.ProgramEditor.FontFamily = new FontFamily(fontName);
                    tab.DataGrid.FontFamily = new FontFamily(fontName);
                }
            }
            catch
            {
                foreach (var tab in TabItems)
                {
                    tab.ProgramEditor.FontFamily = fontFamily;
                    tab.DataGrid.FontFamily = fontFamily;
                }
            }
        }

        private void CtrlS(object obj)
        {
            //if tab exist, save it
            if (!SelectedTabItem.Equals(null))
            {
                SaveTab(SelectedTabItem);
            }
        }

        #endregion

    }
}
