using IDE.Common.Models;
using IDE.Common.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using Driver;
using Microsoft.Win32;
using System.IO;
using IDE.Common.Utilities;

namespace IDE.Common.ViewModels
{
    /// <summary>
    /// EditorViewModel class
    /// </summary>
    /// <seealso cref="IDE.Common.ViewModels.Commands.ObservableObject" />
    public class EditorViewModel : ObservableObject
    {

        #region Fields
        /// <summary>
        /// The program list
        /// </summary>
        private ObservableCollection<Program> programList;

        /// <summary>
        /// The selected tab item
        /// </summary>
        private TabItem selectedTabItem;
        /// <summary>
        /// The tab items
        /// </summary>
        private ObservableCollection<TabItem> tabItems;
        /// <summary>
        /// The intellisense is checked
        /// </summary>
        private bool intellisenseIsChecked, syntaxIsChecked;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorViewModel"/> class.
        /// </summary>
        public EditorViewModel()
        {
            TabItems = new ObservableCollection<TabItem>();
            ProgramList = new ObservableCollection<Program>();
            LoadPreviousSessionTabs();
            DeclareCommands();

            IntellisenseIsChecked = true;
            SyntaxIsChecked = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [intellisense is checked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [intellisense is checked]; otherwise, <c>false</c>.
        /// </value>
        public bool IntellisenseIsChecked
        {
            get
            {
                return intellisenseIsChecked;
            }
            set
            {
                intellisenseIsChecked = value;
                foreach (var tabItem in TabItems)
                {
                    tabItem.ProgramEditor.IsIntellisenseEnabled = intellisenseIsChecked;
                }
                NotifyPropertyChanged("IntellisenseIsChecked");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [syntax is checked].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [syntax is checked]; otherwise, <c>false</c>.
        /// </value>
        public bool SyntaxIsChecked
        {
            get
            {
                return syntaxIsChecked;
            }
            set
            {
                syntaxIsChecked = value;
                foreach (var tabItem in TabItems)
                {
                    tabItem.ProgramEditor.DoSyntaxCheck = syntaxIsChecked;
                }
                NotifyPropertyChanged("SyntaxIsChecked");
            }
        }

        /// <summary>
        /// Gets or sets the program list.
        /// </summary>
        /// <value>
        /// The program list.
        /// </value>
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

        /// <summary>
        /// Gets the appearance.
        /// </summary>
        /// <value>
        /// The appearance.
        /// </value>
        public AppearanceViewModel Appearance => AppearanceViewModel.Instance;

        /// <summary>
        /// Gets or sets the tab items.
        /// </summary>
        /// <value>
        /// The tab items.
        /// </value>
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
        /// <summary>
        /// Gets or sets the selected tab item.
        /// </summary>
        /// <value>
        /// The selected tab item.
        /// </value>
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

        #endregion

        #region Actions

        /// <summary>
        /// Load tabs opened in previous session.
        /// </summary>
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

            OpenTab(new Program(name) { Path = path, Content = content});
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
        /// <returns>
        /// True if saving was succesfull, false if not.
        /// </returns>
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
        /// <returns>
        /// True if saving was succesfull, false if not.
        /// </returns>
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
            selectedTabItem.ProgramEditor.IsIntellisenseEnabled = IntellisenseIsChecked;
            selectedTabItem.ProgramEditor.DoSyntaxCheck = SyntaxIsChecked;
        }

        /// <summary>
        /// Add empty tab
        /// </summary>
        /// <param name="obj">D/C</param>
        private void AddTab(object obj)
        {
            OpenTab(null);
        }

        /// <summary>
        /// Closes tab.
        /// </summary>
        /// <param name="obj">Tab to close.</param>
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

        /// <summary>
        /// Gets the add tab command.
        /// </summary>
        /// <value>
        /// The add tab command.
        /// </value>
        public ICommand AddTabCommand { get; private set; }
        /// <summary>
        /// Gets the close tab command.
        /// </summary>
        /// <value>
        /// The close tab command.
        /// </value>
        public ICommand CloseTabCommand { get; private set; }
        /// <summary>
        /// Gets the change font command.
        /// </summary>
        /// <value>
        /// The change font command.
        /// </value>
        public ICommand ChangeFontCommand { get; private set; }
        /// <summary>
        /// Gets the control s key.
        /// </summary>
        /// <value>
        /// The control s key.
        /// </value>
        public ICommand CtrlSKey { get; private set; }
        /// <summary>
        /// Gets the control n key.
        /// </summary>
        /// <value>
        /// The control n key.
        /// </value>
        public ICommand CtrlNKey { get; private set; }

        /// <summary>
        /// Gets the menu create command.
        /// </summary>
        /// <value>
        /// The menu create command.
        /// </value>
        public ICommand MenuCreateCommand { get; private set; }
        /// <summary>
        /// Gets the menu open command.
        /// </summary>
        /// <value>
        /// The menu open command.
        /// </value>
        public ICommand MenuOpenCommand { get; private set; }
        /// <summary>
        /// Gets the menu save command.
        /// </summary>
        /// <value>
        /// The menu save command.
        /// </value>
        public ICommand MenuSaveCommand { get; private set; }
        /// <summary>
        /// Gets the menu save as command.
        /// </summary>
        /// <value>
        /// The menu save as command.
        /// </value>
        public ICommand MenuSaveAsCommand { get; private set; }
        /// <summary>
        /// Gets the menu save all command.
        /// </summary>
        /// <value>
        /// The menu save all command.
        /// </value>
        public ICommand MenuSaveAllCommand { get; private set; }

        /// <summary>
        /// Declares the commands.
        /// </summary>
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

        /// <summary>
        /// Determines whether [is tab count below maximum] [the specified object].
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///   <c>true</c> if [is tab count below maximum] [the specified object]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsTabCountBelowMax(object obj)
        {
            return TabItems.Count < 15;
        }

        /// <summary>
        /// Doeses the selected tab exist.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        private bool DoesSelectedTabExist(object obj)
        {
            return SelectedTabItem != null;
        }

        /// <summary>
        /// Menus the create.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void MenuCreate(object obj)
        {
            OpenTab(null);
        }

        /// <summary>
        /// Menus the open.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void MenuOpen(object obj)
        {
            OpenFileTab();
        }

        /// <summary>
        /// Menus the save.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void MenuSave(object obj)
        {
            SaveTab(SelectedTabItem);
        }

        /// <summary>
        /// Menus the save as.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void MenuSaveAs(object obj)
        {
            SaveAsTab(SelectedTabItem);
        }

        /// <summary>
        /// Menus the save all.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void MenuSaveAll(object obj)
        {
            SaveAllTabs();
        }

        /// <summary>
        /// Determines whether this instance [can change font] the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can change font] the specified object; otherwise, <c>false</c>.
        /// </returns>
        private bool CanChangeFont(object obj)
        {
            if (TabItems.Count == 0)
                return false;

            var text = TabItems[0].ProgramEditor.FontFamily.ToString();
            return !text.Equals(obj as string);
        }

        /// <summary>
        /// Changes the font.
        /// </summary>
        /// <param name="obj">The object.</param>
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

        /// <summary>
        /// Controls the s.
        /// </summary>
        /// <param name="obj">The object.</param>
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
