using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Xml;
using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;

namespace IDE.Views
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {
        CompletionWindow completionWindow;
        TextEditor textEditor;

        public Editor()
        {
            InitializeComponent();

            InitializeAvalon();
            PopulateFileList();
        }

        #region Avalon

        private void TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\n")
            {
                ////textEditor.LineUp();
                //int i = textEditor.LineCount * 10;
                ////int index = textEditor.Text.IndexOf(' ');
                //completionWindow = new CompletionWindow(textEditor.TextArea);
                //IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                //data.Add(new MyCompletionData(i.ToString()));
                //completionWindow.Show();
                //completionWindow.Closed += delegate
                //{
                //    completionWindow = null;
                //};
            }
        }

        private void TextEntering(object sender, TextCompositionEventArgs e)
        {
            //add text entering handling
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //tbd
            }
        }

        private void InitializeAvalon()
        {
            textEditor = new TextEditor();
            textEditor.ShowLineNumbers = false;
            textEditor.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));  //here you can set backgroud color
            IHighlightingDefinition definition = HighlightingLoader.Load(XmlReader.Create("CustomHighlighting.xshd"), HighlightingManager.Instance);
            HighlightingManager.Instance.RegisterHighlighting("CustomHighlighting", new[] { ".txt" }, definition);
            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("CustomHighlighting");

            Grid.SetRow(textEditor, 1);
            Grid.SetColumn(textEditor, 1);
            grid.Children.Add(textEditor);

            textEditor.TextArea.TextEntering += TextEntering;
            textEditor.TextArea.TextEntered += TextEntered;
            textEditor.TextArea.PreviewKeyDown += KeyIsDown;
        }

        #endregion

        #region FileViewer

        private void PopulateFileList()
        {
            listView_FolderList.Items.Clear();
            try
            {
                string[] files = Directory.GetFiles(@"Programs", "*.txt");

                foreach (string file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    listView_FolderList.Items.Add(fileName);
                }
            }
            catch (DirectoryNotFoundException)
            {
                if (MessageBox.Show("Local storage folder not found. Create one?", "Data not found", MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Directory.CreateDirectory(@"Programs");
                }
                else
                {
                    MessageBox.Show("In order to start using Editor you must create folder named Programs upon bin/Debug directory. " +
                        "If you want to do this automatically, please restart application and accept after error occurs.", 
                        "Data not found", MessageBoxButton.OK);
                }
            }
        }

        private void listView_FolderList_click(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                textBox_ProgramName.Text = item.ToString();
            }
        }

        #endregion

        #region Save&Load

        private void button_Load_Click(object sender, RoutedEventArgs e)
        {
            LoadSelectedItem();
        }

        private void button_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentProgram();
        }

        private void LoadSelectedItem()
        {
            try
            {
                string selectedItem = listView_FolderList.SelectedItem.ToString();
                textBox_ProgramName.Text = selectedItem;
                textEditor.Text = "";
                textEditor.Text = File.ReadAllText(@"Programs\" + selectedItem + ".txt", Encoding.ASCII);
            }
            catch (Exception) { };
        }

        private void SaveCurrentProgram()
        {
            if (textBox_ProgramName.Text != "" && textBox_ProgramName != null)
            {
                try
                {
                    File.WriteAllText(@"Programs\" + textBox_ProgramName.Text + ".txt", textEditor.Text, Encoding.ASCII);
                    PopulateFileList();
                }
                catch (Exception) { };
            }
        }

        private void TextValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^ąćęłńóśźżĄĆĘŁŃÓŚŹŻ<>:/\"|?*]");   //letters that are not allowed
            e.Handled = !regex.IsMatch(e.Text);
        }

        #endregion

    }
}
