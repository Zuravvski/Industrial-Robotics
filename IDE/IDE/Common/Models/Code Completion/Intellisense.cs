using System.Collections.Generic;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;
using IDE.Common.Models.Value_Objects;
using IDE.Common.Utilities;
using IDE.Common.Utilities.Extensions;

namespace IDE.Common.Models.Code_Completion
{
    public class Intellisense
    {
        private readonly ISet<Command> commands;
        private CompletionWindow completionWindow;
        private readonly TextArea textArea;

        public bool IsShowing => completionWindow != null && completionWindow.IsVisible;

        public Intellisense(TextArea textArea)
        {
            commands = Session.Instance.Commands.CommandsMap;
            this.textArea = textArea;
        }

        public void Prepare(TextCompositionEventArgs e)
        {
            if (completionWindow == null)
            {
                completionWindow = new CompletionWindow(textArea);

                foreach (var command in commands)
                {
                    completionWindow.CompletionList.CompletionData.Add(new CompletionData(command.Content, command.Description, command.Type.Description()));
                }
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }
            
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open, insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }

        public void Submit(KeyEventArgs e, bool isOneLine)
        {
            if (e.Key == Key.Enter)
            {
                if (completionWindow != null)
                {
                    completionWindow?.Focus();
                    e.Handled = true;
                }

                if (isOneLine)
                {
                    e.Handled = true;
                }
            }
        }

        public void Show()
        {
            completionWindow?.Show();
        }
    }
}
