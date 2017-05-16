using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;

namespace IDE.Common.Models.Syntax_Check
{
    public class SyntaxCheckVisualizer
    {
        private readonly TextMarkerService textMarkerService;

        public SyntaxCheckVisualizer(ProgramEditor textEditor)
        { 
            textMarkerService = new TextMarkerService(textEditor.Document);
            textEditor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
            textEditor.TextArea.TextView.LineTransformers.Add(textMarkerService);
            var services =
                (IServiceContainer) textEditor.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            services?.AddService(typeof(TextMarkerService), textMarkerService);
        }

        public void Visualize(bool isValid, DocumentLine line)
        {
            if (isValid || line.Length == 0)
            {
                RemoveMarker(line);
            }
            else
            {
                RemoveMarker(line);
                AddMarker(line);
            }
        }

        private void RemoveMarker(DocumentLine line)
        {
            textMarkerService.RemoveAll(l => l.StartOffset == line.Offset);
        }

        private void AddMarker(DocumentLine line)
        {
            var marker = textMarkerService.Create(line.Offset, line.Length);
            marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
            marker.MarkerColor = Colors.Red;
        }
    }
}
