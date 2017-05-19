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
    /// <summary>
    /// TabItem class
    /// </summary>
    /// <seealso cref="IDE.Common.ViewModels.Commands.ObservableObject" />
    public class TabItem : ObservableObject
    {

        #region Fields

        /// <summary>
        /// The image
        /// </summary>
        private BitmapImage image;
        /// <summary>
        /// The unsaved changes
        /// </summary>
        private bool unsavedChanges = true;
        /// <summary>
        /// The header
        /// </summary>
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

        /// <summary>
        /// Gets or sets the content of the tab.
        /// </summary>
        /// <value>
        /// The content of the tab.
        /// </value>
        public Border TabContent { get; set; }

        /// <summary>
        /// Gets or sets the tab grid.
        /// </summary>
        /// <value>
        /// The tab grid.
        /// </value>
        public Grid TabGrid { get; set; }

        /// <summary>
        /// Gets or sets the program editor.
        /// </summary>
        /// <value>
        /// The program editor.
        /// </value>
        public ProgramEditor ProgramEditor { get; set; }

        /// <summary>
        /// Gets or sets the data grid.
        /// </summary>
        /// <value>
        /// The data grid.
        /// </value>
        public DataGrid DataGrid { get; set; }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
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

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
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

        /// <summary>
        /// Gets or sets the program.
        /// </summary>
        /// <value>
        /// The program.
        /// </value>
        public Program Program { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [unsaved changed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [unsaved changed]; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets or sets the position item source.
        /// </summary>
        /// <value>
        /// The position item source.
        /// </value>
        public List<Positions> PositionItemSource { get; set; }

        #endregion

        #region Actions

        /// <summary>
        /// Organizes the content of the tab.
        /// </summary>
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

        /// <summary>
        /// Generates the dump positions.
        /// </summary>
        /// <param name="rand">The rand.</param>
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

        /// <summary>
        /// Handles the TextChanged event of the ProgramEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ProgramEditor_TextChanged(object sender, EventArgs e)
        {
            if (Program != null)
            {
                //if the text has changed and it does not match saved content anymore
                UnsavedChanged = !ProgramEditor.Text.Equals(Program.Content);
            }
        }

        /// <summary>
        /// Bitmap2s the bitmap image.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns></returns>
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
