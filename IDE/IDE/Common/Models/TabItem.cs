using Driver;
using IDE.Common.ViewModels.Commands;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IDE.Common.Models
{
    public class TabItem : ObservableObject
    {

        #region Fields

        private BitmapImage image;
        private string tabText;
        private bool unsavedChanges = true;
        private string header;
        private ObservableCollection<Positions> positionItemSource;

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
                Content = new ProgramEditor { DoSyntaxCheck = true, SyntaxCheckerMode = ProgramEditor.SyntaxCheckerModeE.RealTime };
                TabText = string.Empty;
                //we are setting this twice to trigger rising edge event. Dont change it, just accept fact that it is working this way.
                UnsavedChanged = false;
                UnsavedChanged = true;
            }
            else
            {
                Header = program.Name;
                Content = new ProgramEditor { DoSyntaxCheck = true, SyntaxCheckerMode = ProgramEditor.SyntaxCheckerModeE.RealTime };
                TabText = program.Content;
                UnsavedChanged = false;
            }
            Program = program;
            PositionItemSource = new ObservableCollection<Positions>();
            GenerateDumpPositions();
        }

        #endregion

        #region Properties

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

        public string TabText
        {
            get
            {
                return tabText;
            }
            set
            {
                tabText = value;
                NotifyPropertyChanged("TabText");
            }
        }

        public ProgramEditor Content { get; private set; }

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

        #endregion

        #region Actions

        private void GenerateDumpPositions()
        {
            var rand = new Random();

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


        private void Content_TextChanged(object sender, System.EventArgs e)
        {
            if (Program != null)
            {
                //if the text has changed and it does not match saved content anymore
                if (!Content.Text.Equals(Program.Content))
                {
                    UnsavedChanged = true;
                }
                else
                {
                    UnsavedChanged = false;
                }
            }
        }

        public BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
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
