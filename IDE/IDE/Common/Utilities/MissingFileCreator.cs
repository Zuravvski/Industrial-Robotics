using System.Diagnostics;
using System.IO;
using System.Windows;

namespace IDE.Common.Utilities
{
    public static class MissingFileCreator
    {
        public static bool HighlightingErrorWasAlreadyShown;


        //this string was generated online so this is why it is so f*$*#cking long
        private static string HighlightingContent = "<?xml version =\"1.0\"?>\r\n<SyntaxDefinition name=\"Custom Highlighting\" xmlns=\"http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008\">\r\n\t<Color name=\"Comment\" foreground=\"#228B22\" />\r\n\t\r\n\t<!-- This is the main ruleset. -->\r\n\t<RuleSet>\r\n\t\t<Span color=\"Comment\" begin=\"'\" />\r\n\t\t<!-- <Span color=\"Comment\" multiline=\"true\" begin=\"/\\*\" end=\"\\*/\" /> -->\r\n\t\t\r\n\t\t\r\n\t\t<Keywords fontWeight = \"bold\" foreground = \"#FF0000\" > <!--MOVEMENT COMMANDS-->\r\n\t\t\t<Word>DJ</Word>\r\n\t\t\t<Word>DP</Word>\r\n\t\t\t<Word>DS</Word>\r\n\t\t\t<Word>DW</Word>\r\n\t\t\t<Word>IP</Word>\r\n\t\t\t<Word>JRC</Word>\r\n\t\t\t<Word>MA</Word>\r\n\t\t\t<Word>MC</Word>\r\n\t\t\t<Word>MJ</Word>\r\n\t\t\t<Word>MO</Word>\r\n\t\t\t<Word>MP</Word>\r\n\t\t\t<Word>MPB</Word>\r\n\t\t\t<Word>MPC</Word>\r\n\t\t\t<Word>MR</Word>\r\n\t\t\t<Word>MRA</Word>\r\n\t\t\t<Word>MS</Word>\r\n\t\t\t<Word>MT</Word>\r\n\t\t\t<Word>MTS</Word>\r\n\t\t\t<Word>OG</Word>\r\n\t\t\t<!-- ... -->\r\n\t\t</Keywords>\r\n\t\t\r\n\t\t<Keywords fontWeight=\"bold\" foreground=\"#008000\">\t<!--GRIP COMMANDS-->\r\n\t\t\t<Word>GC</Word>\r\n\t\t\t<Word>GF</Word>\r\n\t\t\t<Word>GO</Word>\r\n\t\t\t<Word>GP</Word>\r\n\t\t\t<Word>SC</Word>\r\n\t\t\t<Word>TL</Word>\r\n\t\t</Keywords>\r\n\t\t\r\n\t\t<Keywords fontWeight=\"bold\" foreground=\"#FF6347\">\t<!--TIMERS, COUNTERS-->\r\n\t\t\t<Word>AN</Word>\r\n\t\t\t<Word>CL</Word>\r\n\t\t\t<Word>CP</Word>\r\n\t\t\t<Word>CR</Word>\r\n\t\t\t<Word>DC</Word>\r\n\t\t\t<Word>IC</Word>\r\n\t\t\t<Word>LG</Word>\r\n\t\t\t<Word>NE</Word>\r\n\t\t\t<Word>OR</Word>\r\n\t\t\t<Word>PW</Word>\r\n\t\t\t<Word>SM</Word>\r\n\t\t\t<Word>TI</Word>\r\n\t\t\t<Word>XO</Word>\r\n\t\t</Keywords>\r\n\t\t\r\n\t\t<Keywords fontWeight=\"bold\" foreground=\"#800080\">\t<!--PROGRAMMING COMMANDS-->\r\n\t\t\t<Word>DA</Word>\r\n\t\t\t<Word>DL</Word>\r\n\t\t\t<Word>EA</Word>\r\n\t\t\t<Word>ED</Word>\r\n\t\t\t<Word>EQ</Word>\r\n\t\t\t<Word>GS</Word>\r\n\t\t\t<Word>GT</Word>\r\n\t\t\t<Word>HLT</Word>\r\n\t\t\t<Word>HO</Word>\r\n\t\t\t<Word>ID</Word>\r\n\t\t\t<Word>NT</Word>\r\n\t\t\t<Word>NW</Word>\r\n\t\t\t<Word>NX</Word>\r\n\t\t\t<Word>OB</Word>\r\n\t\t\t<Word>OPN</Word>\r\n\t\t\t<Word>OVR</Word>\r\n\t\t\t<Word>PA</Word>\r\n\t\t\t<Word>PC</Word>\r\n\t\t\t<Word>PD</Word>\r\n\t\t\t<Word>PL</Word>\r\n\t\t\t<Word>PMW</Word>\r\n\t\t\t<Word>PT</Word>\r\n\t\t\t<Word>PX</Word>\r\n\t\t\t<Word>RC</Word>\r\n\t\t\t<Word>RN</Word>\r\n\t\t\t<Word>RS</Word>\r\n\t\t\t<Word>RT</Word>\r\n\t\t\t<Word>SD</Word>\r\n\t\t\t<Word>SF</Word>\r\n\t\t\t<Word>SP</Word>\r\n\t\t\t<Word>TB</Word>\r\n\t\t\t<Word>TBD</Word>\r\n\t\t</Keywords>\r\n\t\t\r\n\t\t<Keywords fontWeight=\"bold\" foreground=\"#FFA500\">\t<!--INFORMATION COMMANDS-->\r\n\t\t\t<Word>DR</Word>\r\n\t\t\t<Word>ER</Word>\r\n\t\t\t<Word>HE</Word>\r\n\t\t\t<Word>INP</Word>\r\n\t\t\t<Word>LR</Word>\r\n\t\t\t<Word>N</Word>\r\n\t\t\t<Word>OC</Word>\r\n\t\t\t<Word>OD</Word>\r\n\t\t\t<Word>PMR</Word>\r\n\t\t\t<Word>PR</Word>\r\n\t\t\t<Word>PRN</Word>\r\n\t\t\t<Word>QN</Word>\r\n\t\t\t<Word>SRT</Word>\r\n\t\t\t<Word>VR</Word>\r\n\t\t\t<Word>WH</Word>\r\n\t\t\t<Word>WT</Word>\r\n\t\t</Keywords>\r\n\t\t\r\n\t\t<Keywords fontWeight=\"bold\" foreground=\"#B22222\">\t<!--ADDITIONAL COMMANDS-->\r\n\t\t\t<Word>,</Word>\r\n\t\t\t<Word>'</Word>\r\n\t\t</Keywords>\r\n\t\t\r\n\t\t\r\n\t\t<!-- Digits -->\r\n\t\t<Rule foreground=\"#D1D1D1\">\r\n            \\b0[xX][0-9a-fA-F]+  # hex number\r\n        |    \\b\r\n            (    \\d+(\\.[0-9]+)?   #number with optional floating point\r\n            |    \\.[0-9]+         #or just starting with floating point\r\n            )\r\n            ([eE][+-]?[0-9]+)? # optional exponent\r\n        </Rule>\r\n\t</RuleSet>\r\n</SyntaxDefinition>\r\n";


        public static void CreateHighlightingDefinitionFile()
        {
            HighlightingErrorWasAlreadyShown = true; //to make sure we wont show the same error on others Avalon Editors

            if (MessageBox.Show("Highlighting definitions not found. Would you like to create a new definition file " +
                 "(restart will be performed)?", "No definitions", MessageBoxButton.YesNoCancel, MessageBoxImage.Information)
                 == MessageBoxResult.Yes)
            {
                File.WriteAllText("CustomHighlighting.xshd", HighlightingContent);
                Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }
    }
}
