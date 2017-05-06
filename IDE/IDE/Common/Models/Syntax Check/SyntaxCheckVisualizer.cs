using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.SharpDevelop.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace IDE.Common.Models.Syntax_Check
{
    public class SyntaxCheckVisualizer
    {
        private readonly ProgramEditor textEditor;
        private readonly ITextMarkerService textMarkerService;

        public SyntaxCheckVisualizer(ProgramEditor textEditor)
        {
            this.textEditor = textEditor;

            var textMarkerService = new TextMarkerService(textEditor.Document);
            textEditor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
            textEditor.TextArea.TextView.LineTransformers.Add(textMarkerService);
            IServiceContainer services = (IServiceContainer)textEditor.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            if (services != null)
                services.AddService(typeof(ITextMarkerService), textMarkerService);
            this.textMarkerService = textMarkerService;
        }

        public void Visualize(bool isValid, DocumentLine line)
        {
            if (isValid || line.Length == 0)
            {
                RemoveMarker(line);
            }
            else
            {
                AddMarker(line);
            }
        }

        private void RemoveMarker(DocumentLine line)
        {
            textMarkerService.RemoveAll(l => l.StartOffset == line.Offset);
        }

        private void AddMarker(DocumentLine line)
        {
            ITextMarker marker = textMarkerService.Create(line.Offset, line.Length);
            marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
            marker.MarkerColor = Colors.Red;
        }
    }
}
