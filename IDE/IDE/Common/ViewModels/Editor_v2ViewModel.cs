using ChromeTabs;
using IDE.Common.Models;
using IDE.Common.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System;

namespace IDE.Common.ViewModels
{
    public class Editor_v2ViewModel : ObservableObject
    {

        #region Fields

        private ObservableCollection<TabItem> tabItems;

        #endregion

        #region Constructor
        public Editor_v2ViewModel()
        {
            DeclareCommands();

            TabItems = new ObservableCollection<TabItem>();
            
        }



        #endregion

        #region Properties
        

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

        public TabItem SelectedTab { get; set; }

        #endregion

        #region Actions


        #endregion

        #region Commands

        public ICommand AddTabCommand { get; private set; }
        public ICommand CloseTabCommand { get; private set; }

        private void DeclareCommands()
        {
            AddTabCommand = new RelayCommand(AddTab);
            CloseTabCommand = new RelayCommand(CloseTab);
        }

        private void CloseTab(object obj)
        {
            TabItems.Remove((TabItem)obj);
        }


        private void AddTab(object obj)
        {
            TabItems.Add(new TabItem($"{TabItems.Count}"));
        }


        #endregion

    }
}
