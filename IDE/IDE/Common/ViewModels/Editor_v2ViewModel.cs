using IDE.Common.Models;
using IDE.Common.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System;
using RadialMenu.Controls;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Linq;
using Driver;
using Microsoft.Win32;
using System.IO;

namespace IDE.Common.ViewModels
{
    public class Editor_v2ViewModel : ObservableObject
    {

        #region Fields

        private TabItem selectedTabItem;
        private int selectedTabIndex;
        private Visibility positionManagerVisibility = Visibility.Hidden;
        private ObservableCollection<TabItem> tabItems;
        private ObservableCollection<RadialMenuItem> radialMenuItems;
        private ObservableCollection<Positions> positionItemSource;
        private bool radialMenuIsOpen;
        
        private Visibility radialMenuEnableButtonVisibility;
        private Visibility radialMenuItem1Visibility;
        private Visibility radialMenuItem2Visibility;
        private Visibility radialMenuItem3Visibility;
        private Visibility radialMenuItem1IconVisibility;
        private Visibility radialMenuItem2IconVisibility;
        private Visibility radialMenuItem3IconVisibility;
        private Visibility radialMenuCheckbox1Visibility;
        private Visibility radialMenuCheckbox2Visibility;
        private SolidColorBrush radialMenuArrow1Background;
        private SolidColorBrush radialMenuArrow2Background;
        private SolidColorBrush radialMenuArrow3Background;
        private string radialMenuItem1Text;
        private string radialMenuItem2Text;
        private string radialMenuItem3Text;
        private string radialMenuCentralItemTooltip;
        private string radialMenuItem1Tooltip;
        private string radialMenuItem2Tooltip;
        private string radialMenuItem3Tooltip;
        private bool radialMenuCheckbox1IsChecked;
        private bool radialMenuCheckbox2IsChecked;

        #endregion

        #region Constructor

        public Editor_v2ViewModel()
        {
            TabItems = new ObservableCollection<TabItem>();
            PositionItemSource = new ObservableCollection<Positions>();

            DeclareCommands();
            SetupMainSubmenu();
            GenerateDumpPositions();
        }

        private void GenerateDumpPositions()
        {
            Random rand = new Random();

            PositionItemSource.Add(new Positions()
            {
                Pos = 1,
                X = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Y = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Z = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                A = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                B = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                L1 = Math.Round(rand.NextDouble() * (2000 - 1000) + 1000, 2),
                OC = "O"
            });
            PositionItemSource.Add(new Positions()
            {
                Pos = 2,
                X = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Y = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Z = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                A = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                B = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                L1 = Math.Round(rand.NextDouble() * (2000 - 1000) + 1000, 2),
                OC = "O"
            });
            PositionItemSource.Add(new Positions()
            {
                Pos = 3,
                X = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Y = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Z = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                A = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                B = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                L1 = Math.Round(rand.NextDouble() * (2000 - 1000) + 1000, 2),
                OC = "O"
            });
            PositionItemSource.Add(new Positions()
            {
                Pos = 5,
                X = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Y = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Z = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                A = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                B = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                L1 = Math.Round(rand.NextDouble() * (2000 - 1000) + 1000, 2),
                OC = "O"
            });
            PositionItemSource.Add(new Positions()
            {
                Pos = 8,
                X = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Y = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Z = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                A = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                B = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                L1 = Math.Round(rand.NextDouble() * (2000 - 1000) + 1000, 2),
                OC = "C"
            });
            PositionItemSource.Add(new Positions()
            {
                Pos = 9,
                X = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Y = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Z = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                A = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                B = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                L1 = Math.Round(rand.NextDouble() * (2000 - 1000) + 1000, 2),
                OC = "C"
            });
            PositionItemSource.Add(new Positions()
            {
                Pos = 15,
                X = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Y = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Z = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                A = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                B = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                L1 = Math.Round(rand.NextDouble() * (2000 - 1000) + 1000, 2),
                OC = "C"
            });
            PositionItemSource.Add(new Positions()
            {
                Pos = 22,
                X = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Y = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Z = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                A = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                B = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                L1 = Math.Round(rand.NextDouble() * (2000 - 1000) + 1000, 2),
                OC = "O"
            });
        }


        #endregion

        #region Properties

        public Visibility PositionManagerVisibility
        {
            get
            {
                return positionManagerVisibility;
            }
            set
            {
                positionManagerVisibility = value;
                NotifyPropertyChanged("PositionManagerVisibility");
            }
        }

        public AppearanceViewModel Appearance => AppearanceViewModel.Instance;

        public ObservableCollection<Positions> PositionItemSource
        {
            get
            {
                return positionItemSource;
            }
            set
            {
                positionItemSource = value;
                NotifyPropertyChanged("PositionItemSource");
            }
        }

        public ObservableCollection<RadialMenuItem> RadialMenuItems
        {
            get
            {
                return radialMenuItems;
            }
            private set
            {
                radialMenuItems = value;
                NotifyPropertyChanged("RadialMenuItems");
            }
        }
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

        
        public Visibility RadialMenuEnableButtonVisibility
        {
            get
            {
                return radialMenuEnableButtonVisibility;
            }
            private set
            {
                radialMenuEnableButtonVisibility = value;
                NotifyPropertyChanged("RadialMenuEnableButtonVisibility");
            }
        }
        public Visibility RadialMenuItem1Visibility
        {
            get
            {
                return radialMenuItem1Visibility;
            }
            private set
            {
                radialMenuItem1Visibility = value;
                NotifyPropertyChanged("RadialMenuItem1Visibility");
            }
        }
        public Visibility RadialMenuItem2Visibility
        {
            get
            {
                return radialMenuItem2Visibility;
            }
            private set
            {
                radialMenuItem2Visibility = value;
                NotifyPropertyChanged("RadialMenuItem2Visibility");
            }
        }
        public Visibility RadialMenuItem3Visibility
        {
            get
            {
                return radialMenuItem3Visibility;
            }
            private set
            {
                radialMenuItem3Visibility = value;
                NotifyPropertyChanged("RadialMenuItem3Visibility");
            }
        }
        public Visibility RadialMenuItem1IconVisibility
        {
            get
            {
                return radialMenuItem1IconVisibility;
            }
            private set
            {
                radialMenuItem1IconVisibility = value;
                NotifyPropertyChanged("RadialMenuItem1IconVisibility");
            }
        }
        public Visibility RadialMenuItem2IconVisibility
        {
            get
            {
                return radialMenuItem2IconVisibility;
            }
            private set
            {
                radialMenuItem2IconVisibility = value;
                NotifyPropertyChanged("RadialMenuItem2IconVisibility");
            }
        }
        public Visibility RadialMenuItem3IconVisibility
        {
            get
            {
                return radialMenuItem3IconVisibility;
            }
            private set
            {
                radialMenuItem3IconVisibility = value;
                NotifyPropertyChanged("RadialMenuItem3IconVisibility");
            }
        }
        public Visibility RadialMenuCheckbox1Visibility
        {
            get
            {
                return radialMenuCheckbox1Visibility;
            }
            private set
            {
                radialMenuCheckbox1Visibility = value;
                NotifyPropertyChanged("RadialMenuCheckbox1Visibility");
            }
        }
        public Visibility RadialMenuCheckbox2Visibility
        {
            get
            {
                return radialMenuCheckbox2Visibility;
            }
            private set
            {
                radialMenuCheckbox2Visibility = value;
                NotifyPropertyChanged("RadialMenuCheckbox2Visibility");
            }
        }
        public SolidColorBrush RadialMenuArrow1Background
        {
            get
            {
                return radialMenuArrow1Background;
            }
            private set
            {
                radialMenuArrow1Background = value;
                NotifyPropertyChanged("RadialMenuArrow1Background");
            }
        }
        public SolidColorBrush RadialMenuArrow2Background
        {
            get
            {
                return radialMenuArrow2Background;
            }
            private set
            {
                radialMenuArrow2Background = value;
                NotifyPropertyChanged("RadialMenuArrow2Background");
            }
        }
        public SolidColorBrush RadialMenuArrow3Background
        {
            get
            {
                return radialMenuArrow3Background;
            }
            private set
            {
                radialMenuArrow3Background = value;
                NotifyPropertyChanged("RadialMenuArrow3Background");
            }
        }
        public bool RadialMenuCheckbox1IsChecked
        {
            get
            {
                return radialMenuCheckbox1IsChecked;
            }
            private set
            {
                radialMenuCheckbox1IsChecked = value;
                NotifyPropertyChanged("RadialMenuCheckbox1IsChecked");
            }
        }
        public bool RadialMenuCheckbox2IsChecked
        {
            get
            {
                return radialMenuCheckbox2IsChecked;
            }
            private set
            {
                radialMenuCheckbox2IsChecked = value;
                NotifyPropertyChanged("RadialMenuCheckbox2IsChecked");
            }
        }
        public string RadialMenuItem1Text
        {
            get
            {
                return radialMenuItem1Text;
            }
            private set
            {
                radialMenuItem1Text = value;
                NotifyPropertyChanged("RadialMenuItem1Text");
            }
        }
        public string RadialMenuItem2Text
        {
            get
            {
                return radialMenuItem2Text;
            }
            private set
            {
                radialMenuItem2Text = value;
                NotifyPropertyChanged("RadialMenuItem2Text");
            }
        }
        public string RadialMenuItem3Text
        {
            get
            {
                return radialMenuItem3Text;
            }
            private set
            {
                radialMenuItem3Text = value;
                NotifyPropertyChanged("RadialMenuItem3Text");
            }
        }
        public string RadialMenuCentralItemTooltip
        {
            get
            {
                return radialMenuCentralItemTooltip;
            }
            private set
            {
                radialMenuCentralItemTooltip = value;
                NotifyPropertyChanged("RadialMenuCentralItemTooltip");
            }
        }
        public string RadialMenuItem1Tooltip
        {
            get
            {
                return radialMenuItem1Tooltip;
            }
            private set
            {
                radialMenuItem1Tooltip = value;
                NotifyPropertyChanged("RadialMenuItem1Tooltip");
            }
        }
        public string RadialMenuItem2Tooltip
        {
            get
            {
                return radialMenuItem2Tooltip;
            }
            private set
            {
                radialMenuItem2Tooltip = value;
                NotifyPropertyChanged("RadialMenuItem2Tooltip");
            }
        }
        public string RadialMenuItem3Tooltip
        {
            get
            {
                return radialMenuItem3Tooltip;
            }
            private set
            {
                radialMenuItem3Tooltip = value;
                NotifyPropertyChanged("RadialMenuItem3Tooltip");
            }
        }
        public bool RadialMenuIsOpen
        {
            get
            {
                return radialMenuIsOpen;
            }
            private set
            {
                radialMenuIsOpen = value;
                NotifyPropertyChanged("RadialMenuIsOpen");
            }
        }
        public bool RadialMenuSubmenuFileMode { get; private set; }
        public bool RadialMenuSubmenuSaveMode { get; private set; }
        public bool RadialMenuSubmenuSettingsMode { get; private set; }

        #endregion

        #region Actions


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

            if (dialog.ShowDialog() == false)
            {
                //if user fails to select a file
                return;
            }

            var path = Path.GetFullPath(dialog.FileName);
            var name = Path.GetFileNameWithoutExtension(dialog.FileName);
            var content = File.ReadAllText($"{dialog.FileName}");

            var doesTabAlreadyExist = TabItems.Where(i => i.Program != null && i.Program.Path == path).FirstOrDefault();

            if (!object.Equals(doesTabAlreadyExist, null))
            {
                //if this file is already open reload it's content and set is as current tab
                doesTabAlreadyExist.Content.Text = doesTabAlreadyExist.Program.Content;
                doesTabAlreadyExist.UnsavedChanged = false;
                SelectedTabItem = doesTabAlreadyExist;
                return;
            }

            OpenTab(new Program(name) { Path = path, Content = content });
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

            if (dialog.ShowDialog() == false)
            {
                return false;
            }

            var path = Path.GetFullPath(dialog.FileName);
            var name = Path.GetFileNameWithoutExtension(dialog.FileName);

            tabItem.Program = new Program(name) { Path = path, Content = tabItem.Content.Text };
            File.WriteAllText(tabItem.Program.Path, tabItem.Content.Text);

            //update UI
            tabItem.Header = tabItem.Program.Name;
            tabItem.UnsavedChanged = false;

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
            File.WriteAllText(tabItem.Program.Path, tabItem.Content.Text);

            //update gui
            tabItem.UnsavedChanged = false;
            return true;
        }

        /// <summary>
        /// Opens new tab
        /// </summary>
        /// <param name="program">Pass existing program or null for new, fresh tab.</param>
        private void OpenTab(Program program)
        {
            int matches = 0;
            TabItem tabToAdd;

            if (program != null)
            {
                tabToAdd = new TabItem(0, program);
            }
            else
            {
                matches = 1;
                for (int i = 0; i < TabItems.Count; i++)
                {
                    //if there is "Untitled <nr>" tab, repeat loop to pick another number
                    if (TabItems[i].Header.Contains($"Untitled {matches}"))
                    {
                        matches++;
                        i = -1;
                        continue;
                    }
                }
                tabToAdd = new TabItem(matches, null);
            }
            TabItems.Add(tabToAdd);
            SelectedTabItem = tabToAdd;

            if (tabItems.Count < 1)
                PositionManagerVisibility = Visibility.Hidden;
            else
                PositionManagerVisibility = Visibility.Visible;
        }

        private void AddTab(object obj)
        {
            OpenTab(null);
        }

        private void CloseTab(object obj)
        {
            var tabItem = obj as TabItem;
            SelectedTabItem = tabItem;  //show user what he is about to close

            if (tabItem.Program != null && tabItem.UnsavedChanged || tabItem.Program == null && tabItem.Content.Text != string.Empty)
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

            if (tabItems.Count < 1)
                PositionManagerVisibility = Visibility.Hidden;
            else
                PositionManagerVisibility = Visibility.Visible;
        }

        #endregion

        #region Commands

        public ICommand AddTabCommand { get; private set; }
        public ICommand CloseTabCommand { get; private set; }
        public ICommand OpenRadialMenuCommand { get; private set; }
        public ICommand CloseRadialMenuCommand { get; private set; }
        public ICommand RadialMenuItem1Command { get; private set; }
        public ICommand RadialMenuItem2Command { get; private set; }
        public ICommand RadialMenuItem3Command { get; private set; }

        public ICommand CtrlSKey { get; private set; }
        public ICommand CtrlNKey { get; private set; }
        public ICommand EscKey { get; private set; }

        private void DeclareCommands()
        {
            AddTabCommand = new RelayCommand(AddTab);
            CloseTabCommand = new RelayCommand(CloseTab);
            OpenRadialMenuCommand = new RelayCommand(OpenRadialMenu);
            CloseRadialMenuCommand = new RelayCommand(CloseRadialMenu);
            RadialMenuItem1Command = new RelayCommand(RadialMenuItem1Execute, RadialMenuItem1CanExecute);
            RadialMenuItem2Command = new RelayCommand(RadialMenuItem2Execute, RadialMenuItem2CanExecute);
            RadialMenuItem3Command = new RelayCommand(RadialMenuItem3Execute, RadialMenuItem3CanExecute);

            CtrlSKey = new RelayCommand(CtrlS);
            CtrlNKey = new RelayCommand(AddTab);
            EscKey = new RelayCommand(Esc);
        }


        private void CtrlS(object obj)
        {
            //if tab exist, save it
            if (!SelectedTabItem.Equals(null))
            {
                SaveTab(SelectedTabItem);
            }
        }

        private void Esc(object obj)
        {
            //close radial menu
            RadialMenuIsOpen = false;
            SetupMainSubmenu();
        }

        private bool RadialMenuItem1CanExecute(object obj)
        {
            if (RadialMenuSubmenuFileMode)
            {
                //you can always create new file
                return true;
            }
            else if (RadialMenuSubmenuSaveMode)
            {
                //you cannot save tab that does not exist
                if (SelectedTabItem == null)
                    return false;
                else
                    return true;
            }
            else if (RadialMenuSubmenuSettingsMode)
            {
                //you can always check this checkbox
                return true;
            }
            else
            {
                //you can always go to file menu
                return true;
            }
        }

        private bool RadialMenuItem2CanExecute(object obj)
        {
            if (RadialMenuSubmenuFileMode)
            {
                //you can always open a new file
                return true;
            }
            else if (RadialMenuSubmenuSaveMode)
            {
                //you cannot save tab that does not exist
                if (SelectedTabItem != null)
                    return true;
                else
                    return false;
            }
            else if (RadialMenuSubmenuSettingsMode)
            {
                //you can always check this checkbox
                return true;
            }
            else
            {
                //you can't go to save submenu unless you have at least one tab open
                if (TabItems.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        private bool RadialMenuItem3CanExecute(object obj)
        {
            if (RadialMenuSubmenuFileMode)
            {
                //this is disabled now
                return false;
            }
            else if (RadialMenuSubmenuSaveMode)
            {
                //you can't save all unless you have at least one tab open
                if (TabItems.Count > 0)
                    return true;
                else
                    return false;
            }
            else if (RadialMenuSubmenuSettingsMode)
            {
                //this is disabled now
                return false;
            }
            else
            {
                //you can always go to settings menu
                return true;
            }
        }

        private void SetupMainSubmenu()
        {
            RadialMenuSubmenuFileMode = false;
            RadialMenuSubmenuSettingsMode = false;
            RadialMenuSubmenuSaveMode = false;

            //change text of buttons
            RadialMenuItem1Text = "File";
            RadialMenuItem2Text = "Save";
            RadialMenuItem3Text = "Settings";

            //change tooltips of buttons
            RadialMenuCentralItemTooltip = "Close";
            RadialMenuItem1Tooltip = "File submenu";
            RadialMenuItem2Tooltip = "Save submenu";
            RadialMenuItem3Tooltip = "Settings submenu";

            //change visibility of icons
            RadialMenuItem1IconVisibility = Visibility.Visible;
            RadialMenuItem2IconVisibility = Visibility.Visible;
            RadialMenuItem3IconVisibility = Visibility.Visible;

            //change color of arrows
            RadialMenuArrow1Background = Brushes.White;
            RadialMenuArrow2Background = Brushes.White;
            RadialMenuArrow3Background = Brushes.White;

            //change visibility of checkboxes
            RadialMenuCheckbox1Visibility = Visibility.Collapsed;
            RadialMenuCheckbox2Visibility = Visibility.Collapsed;

            //change visibility of buttons
            RadialMenuItem1Visibility = Visibility.Visible;
            RadialMenuItem2Visibility = Visibility.Visible;
            RadialMenuItem3Visibility = Visibility.Visible;
        }

        private void SetupFileSubmenu()
        {
            RadialMenuSubmenuFileMode = true;
            RadialMenuSubmenuSettingsMode = false;
            RadialMenuSubmenuSaveMode = false;

            //change text of buttons
            RadialMenuItem1Text = "Create";
            RadialMenuItem2Text = "Open";
            RadialMenuItem3Text = "";

            //change tooltips of buttons
            RadialMenuCentralItemTooltip = "Back";
            RadialMenuItem1Tooltip = "Create new file";
            RadialMenuItem2Tooltip = "Open existing file";
            RadialMenuItem3Tooltip = string.Empty;

            //change visibility of icons
            RadialMenuItem1IconVisibility = Visibility.Hidden;
            RadialMenuItem2IconVisibility = Visibility.Hidden;
            RadialMenuItem3IconVisibility = Visibility.Hidden;

            //change color of arrows
            RadialMenuArrow1Background = Brushes.Transparent;
            RadialMenuArrow2Background = Brushes.Transparent;
            RadialMenuArrow3Background = Brushes.Transparent;

            //change visibility of checkboxes
            RadialMenuCheckbox1Visibility = Visibility.Collapsed;
            RadialMenuCheckbox2Visibility = Visibility.Collapsed;

            //change visibility of buttons
            RadialMenuItem1Visibility = Visibility.Visible;
            RadialMenuItem2Visibility = Visibility.Visible;
            RadialMenuItem3Visibility = Visibility.Hidden;
        }

        private void SetupSettingsSubmenu()
        {
            RadialMenuSubmenuFileMode = false;
            RadialMenuSubmenuSettingsMode = true;
            RadialMenuSubmenuSaveMode = false;

            //change text of buttons
            RadialMenuItem1Text = "Intellisense";
            RadialMenuItem2Text = "Syntax check";
            RadialMenuItem3Text = "";

            //change tooltips of buttons
            RadialMenuCentralItemTooltip = "Back";
            RadialMenuItem1Tooltip = "Enable/Disable intellisense";
            RadialMenuItem2Tooltip = "Enable/Disable syntax check";
            RadialMenuItem3Tooltip = string.Empty;

            //change visibility of icons
            RadialMenuItem1IconVisibility = Visibility.Hidden;
            RadialMenuItem2IconVisibility = Visibility.Hidden;
            RadialMenuItem3IconVisibility = Visibility.Hidden;

            //change color of arrows
            RadialMenuArrow1Background = Brushes.Transparent;
            RadialMenuArrow2Background = Brushes.Transparent;
            RadialMenuArrow3Background = Brushes.Transparent;

            //change visibility of checkboxes
            RadialMenuCheckbox1Visibility = Visibility.Visible;
            RadialMenuCheckbox2Visibility = Visibility.Visible;

            //change visibility of buttons
            RadialMenuItem1Visibility = Visibility.Visible;
            RadialMenuItem2Visibility = Visibility.Visible;
            RadialMenuItem3Visibility = Visibility.Hidden;
        }

        private void SetupSaveSubmenu()
        {
            RadialMenuSubmenuFileMode = false;
            RadialMenuSubmenuSettingsMode = false;
            RadialMenuSubmenuSaveMode = true;

            //change text of buttons
            RadialMenuItem1Text = "Save";
            RadialMenuItem2Text = "Save as";
            RadialMenuItem3Text = "Save all";

            //change tooltips of buttons
            RadialMenuCentralItemTooltip = "Back";
            RadialMenuItem1Tooltip = "Save current tab";
            RadialMenuItem2Tooltip = "Save as current tab";
            RadialMenuItem3Tooltip = "Save all tabs";

            //change visibility of icons
            RadialMenuItem1IconVisibility = Visibility.Hidden;
            RadialMenuItem2IconVisibility = Visibility.Hidden;
            RadialMenuItem3IconVisibility = Visibility.Hidden;

            //change color of arrows
            RadialMenuArrow1Background = Brushes.Transparent;
            RadialMenuArrow2Background = Brushes.Transparent;
            RadialMenuArrow3Background = Brushes.Transparent;

            //change visibility of checkboxes
            RadialMenuCheckbox1Visibility = Visibility.Collapsed;
            RadialMenuCheckbox2Visibility = Visibility.Collapsed;

            //change visibility of buttons
            RadialMenuItem1Visibility = Visibility.Visible;
            RadialMenuItem2Visibility = Visibility.Visible;
            RadialMenuItem3Visibility = Visibility.Visible;
        }

        private async void RadialMenuItem3Execute(object obj) //settings
        {
            if (RadialMenuSubmenuFileMode)
            {
                //this is disabled now
            }
            else if (RadialMenuSubmenuSaveMode)
            {
                SaveAllTabs();
            }
            else if (RadialMenuSubmenuSettingsMode)
            {
                //this is disabled now
            }
            else
            {
                //we are using delays to achieve animation effect
                RadialMenuIsOpen = false;
                RadialMenuEnableButtonVisibility = Visibility.Hidden;
                await Task.Delay(300);
                SetupSettingsSubmenu();
                RadialMenuIsOpen = true;
                await Task.Delay(150);
                RadialMenuEnableButtonVisibility = Visibility.Visible;
            }
        }

        private async void RadialMenuItem2Execute(object obj) //save
        {
            if (RadialMenuSubmenuFileMode)
            {
                OpenFileTab();
            }
            else if (RadialMenuSubmenuSaveMode)
            {
                SaveAsTab(SelectedTabItem);
            }
            else if (RadialMenuSubmenuSettingsMode)
            {
                RadialMenuCheckbox2IsChecked = !RadialMenuCheckbox2IsChecked;
            }
            else
            {
                //we are using delays to achieve animation effect
                RadialMenuIsOpen = false;
                RadialMenuEnableButtonVisibility = Visibility.Hidden;
                await Task.Delay(300);
                SetupSaveSubmenu();
                RadialMenuIsOpen = true;
                await Task.Delay(150);
                RadialMenuEnableButtonVisibility = Visibility.Visible;
            }
        }

        private async void RadialMenuItem1Execute(object obj)
        {
            if (RadialMenuSubmenuFileMode)
            {
                OpenTab(null);
            }
            else if (RadialMenuSubmenuSaveMode)
            {
                SaveTab(SelectedTabItem);
            }
            else if (RadialMenuSubmenuSettingsMode)
            {
                RadialMenuCheckbox1IsChecked = !RadialMenuCheckbox1IsChecked;
            }
            else
            {
                //we are using delays to achieve animation effect
                RadialMenuIsOpen = false;
                RadialMenuEnableButtonVisibility = Visibility.Hidden;
                await Task.Delay(300);
                SetupFileSubmenu();
                RadialMenuIsOpen = true;
                await Task.Delay(150);
                RadialMenuEnableButtonVisibility = Visibility.Visible;
            }
        }

        private async void CloseRadialMenu(object obj)
        {
            if (RadialMenuSubmenuFileMode || RadialMenuSubmenuSaveMode || RadialMenuSubmenuSettingsMode)
            {
                //we are using delays to achieve animation effect
                RadialMenuIsOpen = false;
                RadialMenuEnableButtonVisibility = Visibility.Hidden;
                await Task.Delay(300);
                SetupMainSubmenu();
                RadialMenuIsOpen = true;
                await Task.Delay(150);
                RadialMenuEnableButtonVisibility = Visibility.Visible;
            }
            else
            {
                RadialMenuIsOpen = false;
            }
        }

        private void OpenRadialMenu(object obj)
        {
            RadialMenuIsOpen = true;
        }

        #endregion

    }
}
