using Driver;
using IDE.Common.Models.Value_Objects;
using IDE.Common.Utilities;
using IDE.Common.Utilities.Extensions;
using IDE.Common.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IDE.Common.Models
{
    public class TabItem : ObservableObject
    {

        #region Fields

        private BitmapImage image;
        private bool unsavedChanges = true;
        private string header;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes new tab.
        /// </summary>
        /// <param name="untitledsCount">If this is tab for new program, pass number to be displayed eg. ~Untitled 3~</param>
        /// <param name="program">Pass existing program or null for new, fresh tab.</param>
        public TabItem(int untitledsCount, Program program)
        {
            if (program == null)
            { 
                Header = $"Untitled {untitledsCount}";
                ProgramEditor = new ProgramEditor
                {
                    Text = string.Empty,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    IsHighlightingEnabled = true,
                    SyntaxCheckerMode = ProgramEditor.SyntaxCheckerModeE.RealTime,
                    ShowLineNumbers = true,
                };
                //we are setting this twice to trigger rising edge event. Dont change it, just accept fact that it is working this way.
                UnsavedChanged = false;
                UnsavedChanged = true;
            }
            else
            {
                Header = program.Name;
                ProgramEditor = new ProgramEditor()
                {
                    Text = program.Content,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    IsHighlightingEnabled = true,
                    SyntaxCheckerMode = ProgramEditor.SyntaxCheckerModeE.RealTime,
                    ShowLineNumbers = true
                };

                UnsavedChanged = false;
            }
            Program = program;

            ProgramEditor.TextChanged += ProgramEditor_TextChanged;
            OrganizeTabContent();
        }

        #endregion

        #region Properties

        public Border TabContent { get; set; }

        public Grid TabGrid { get; set; }

        public ProgramEditor ProgramEditor { get; set; }

        public DataGrid DataGrid { get; set; }

        public string Header
        {
            get
            {
                return header;
            }
            set
            {
                header = value;
                NotifyPropertyChanged("Header");
            }
        }

        public BitmapImage Image
        {
            get
            {
                return image;
            }
            private set
            {
                image = value;
                NotifyPropertyChanged("Image");
            }
        }

        public Program Program { get; set; }

        public bool UnsavedChanged
        {
            get
            {
                return unsavedChanges;
            }
            set
            {
                if (UnsavedChanged != value)    //if there has been a change of value (edge)
                {
                    if (value == true)  //if there are unsaved changed
                    {
                        Image = Bitmap2BitmapImage(Properties.Resources.floppy_disk_red);
                        Header = "*" + Header;
                    }
                    else                //or if there are not
                    {
                        Image = Bitmap2BitmapImage(Properties.Resources.floppy_disk_blue);
                        Header = Header.Replace("*", string.Empty);
                    }
                }
                unsavedChanges = value;
            }
        }
        public List<Positions> PositionItemSource { get; set; }

        #endregion

        #region Actions

        private void OrganizeTabContent()
        {
            PositionItemSource = new List<Positions>();
            Random rand = new Random();
            var imax = rand.Next(8, 10);
            for (int i = 0; i < imax; i++)
            {
                GenerateDumpPositions(rand);
            }
            var qry = new List<Positions>(PositionItemSource.DistinctBy(i => i.Pos).OrderBy(i => i.Pos));
            AsynchronousQueryExecutor.Call(qry, l => PositionItemSource = new List<Positions>(l), null);

            

            TabGrid = new Grid();
            TabGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(5, System.Windows.GridUnitType.Star) });
            TabGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = System.Windows.GridLength.Auto });
            TabGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(4, System.Windows.GridUnitType.Star) });

            GridSplitter gridSplitter = new GridSplitter() { Width = 6 };

            DataGrid = new DataGrid
            {
                Name = "PositionGrid",
                CanUserAddRows = true,
                ItemsSource = PositionItemSource
            };

            ProgramEditor.SetValue(Grid.ColumnProperty, 0);
            gridSplitter.SetValue(Grid.ColumnProperty, 1);
            DataGrid.SetValue(Grid.ColumnProperty, 2);

            TabGrid.Children.Add(ProgramEditor);
            TabGrid.Children.Add(gridSplitter);
            TabGrid.Children.Add(DataGrid);

            TabContent = new Border
            {
                BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(221, 221, 221)),
                BorderThickness = new System.Windows.Thickness(1),
                CornerRadius = new System.Windows.CornerRadius(0, 0, 10, 10),
                Padding = new System.Windows.Thickness(5),
                Child = TabGrid
            };
        }

        private void GenerateDumpPositions(Random rand)
        {
            var OC = rand.Next(1, 3) % 2 == 0 ? "O" : "C";

            PositionItemSource.Add(new Positions()
            {
                Pos = rand.Next(1, 100),
                X = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Y = Math.Round(rand.NextDouble() * (500 - 100) + 100, 2),
                Z = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                A = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                B = Math.Round(rand.NextDouble() * (200 - 100) + 100, 2),
                L1 = Math.Round(rand.NextDouble() * (2000 - 1000) + 1000, 2),
                OC = OC
            });
        }

        private void ProgramEditor_TextChanged(object sender, EventArgs e)
        {
            if (Program != null)
            {
                //if the text has changed and it does not match saved content anymore
                UnsavedChanged = !ProgramEditor.Text.Equals(Program.Content);
            }
        }

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            var bitmapImage = new BitmapImage();
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        #endregion

    }
}
