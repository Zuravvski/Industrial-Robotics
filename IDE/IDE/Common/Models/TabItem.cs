﻿using Driver;
using IDE.Common.ViewModels.Commands;
using IDE.Common.ViewModels.Converters;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
                Content = new ProgramEditor(ProgramEditor.Highlighting.On, ProgramEditor.UseIntellisense.Yes) { Text = string.Empty };
                //we are setting this twice to trigger rising edge event. Dont change it, just accept fact that it is working this way.
                UnsavedChanged = false;
                UnsavedChanged = true;
            }
            else
            {
                Header = program.Name;
                Content = new ProgramEditor(ProgramEditor.Highlighting.On, ProgramEditor.UseIntellisense.Yes) { Text = program.Content };
                UnsavedChanged = false;
            }
            Program = program;

            Content.TextChanged += Content_TextChanged;
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

        #endregion

        #region Actions

        private void Content_TextChanged(object sender, System.EventArgs e)
        {
            if (Program != null)
            {
                //if the text has changed and it does not match saved content anymore, display * in the header of a tab
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
