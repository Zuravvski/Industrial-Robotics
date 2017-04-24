using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace IDE.Common.Utilities
{
    public static class MissingFileCreator
    {
        public static bool MissingFileErrorAlreadyShown;

        //these string were generated online so this is why it they are f*$*#cking long
        private static string HighlightingContent = "<?xml version =\"1.0\"?>\r\n<SyntaxDefinition name=\"Custom Highlighting\" xmlns=\"http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008\">\r\n\t<Color name=\"Comment\" foreground=\"#228B22\" />\r\n\t\r\n\t<!-- This is the main ruleset. -->\r\n\t<RuleSet>\r\n\t\t<Span color=\"Comment\" begin=\"'\" />\r\n\t\t<!-- <Span color=\"Comment\" multiline=\"true\" begin=\"/\\*\" end=\"\\*/\" /> -->\r\n\t\t\r\n\t\t\r\n\t\t<Keywords fontWeight = \"bold\" foreground = \"#FF0000\" > <!--MOVEMENT COMMANDS-->\r\n\t\t\t<Word>DJ</Word>\r\n\t\t\t<Word>DP</Word>\r\n\t\t\t<Word>DS</Word>\r\n\t\t\t<Word>DW</Word>\r\n\t\t\t<Word>IP</Word>\r\n\t\t\t<Word>JRC</Word>\r\n\t\t\t<Word>MA</Word>\r\n\t\t\t<Word>MC</Word>\r\n\t\t\t<Word>MJ</Word>\r\n\t\t\t<Word>MO</Word>\r\n\t\t\t<Word>MP</Word>\r\n\t\t\t<Word>MPB</Word>\r\n\t\t\t<Word>MPC</Word>\r\n\t\t\t<Word>MR</Word>\r\n\t\t\t<Word>MRA</Word>\r\n\t\t\t<Word>MS</Word>\r\n\t\t\t<Word>MT</Word>\r\n\t\t\t<Word>MTS</Word>\r\n\t\t\t<Word>OG</Word>\r\n\t\t\t<!-- ... -->\r\n\t\t</Keywords>\r\n\t\t\r\n\t\t<Keywords fontWeight=\"bold\" foreground=\"#008000\">\t<!--GRIP COMMANDS-->\r\n\t\t\t<Word>GC</Word>\r\n\t\t\t<Word>GF</Word>\r\n\t\t\t<Word>GO</Word>\r\n\t\t\t<Word>GP</Word>\r\n\t\t\t<Word>SC</Word>\r\n\t\t\t<Word>TL</Word>\r\n\t\t</Keywords>\r\n\t\t\r\n\t\t<Keywords fontWeight=\"bold\" foreground=\"#FF6347\">\t<!--TIMERS, COUNTERS-->\r\n\t\t\t<Word>AN</Word>\r\n\t\t\t<Word>CL</Word>\r\n\t\t\t<Word>CP</Word>\r\n\t\t\t<Word>CR</Word>\r\n\t\t\t<Word>DC</Word>\r\n\t\t\t<Word>IC</Word>\r\n\t\t\t<Word>LG</Word>\r\n\t\t\t<Word>NE</Word>\r\n\t\t\t<Word>OR</Word>\r\n\t\t\t<Word>PW</Word>\r\n\t\t\t<Word>SM</Word>\r\n\t\t\t<Word>TI</Word>\r\n\t\t\t<Word>XO</Word>\r\n\t\t</Keywords>\r\n\t\t\r\n\t\t<Keywords fontWeight=\"bold\" foreground=\"#800080\">\t<!--PROGRAMMING COMMANDS-->\r\n\t\t\t<Word>DA</Word>\r\n\t\t\t<Word>DL</Word>\r\n\t\t\t<Word>EA</Word>\r\n\t\t\t<Word>ED</Word>\r\n\t\t\t<Word>EQ</Word>\r\n\t\t\t<Word>GS</Word>\r\n\t\t\t<Word>GT</Word>\r\n\t\t\t<Word>HLT</Word>\r\n\t\t\t<Word>HO</Word>\r\n\t\t\t<Word>ID</Word>\r\n\t\t\t<Word>NT</Word>\r\n\t\t\t<Word>NW</Word>\r\n\t\t\t<Word>NX</Word>\r\n\t\t\t<Word>OB</Word>\r\n\t\t\t<Word>OPN</Word>\r\n\t\t\t<Word>OVR</Word>\r\n\t\t\t<Word>PA</Word>\r\n\t\t\t<Word>PC</Word>\r\n\t\t\t<Word>PD</Word>\r\n\t\t\t<Word>PL</Word>\r\n\t\t\t<Word>PMW</Word>\r\n\t\t\t<Word>PT</Word>\r\n\t\t\t<Word>PX</Word>\r\n\t\t\t<Word>RC</Word>\r\n\t\t\t<Word>RN</Word>\r\n\t\t\t<Word>RS</Word>\r\n\t\t\t<Word>RT</Word>\r\n\t\t\t<Word>SD</Word>\r\n\t\t\t<Word>SF</Word>\r\n\t\t\t<Word>SP</Word>\r\n\t\t\t<Word>TB</Word>\r\n\t\t\t<Word>TBD</Word>\r\n\t\t</Keywords>\r\n\t\t\r\n\t\t<Keywords fontWeight=\"bold\" foreground=\"#FFA500\">\t<!--INFORMATION COMMANDS-->\r\n\t\t\t<Word>DR</Word>\r\n\t\t\t<Word>ER</Word>\r\n\t\t\t<Word>HE</Word>\r\n\t\t\t<Word>INP</Word>\r\n\t\t\t<Word>LR</Word>\r\n\t\t\t<Word>N</Word>\r\n\t\t\t<Word>OC</Word>\r\n\t\t\t<Word>OD</Word>\r\n\t\t\t<Word>PMR</Word>\r\n\t\t\t<Word>PR</Word>\r\n\t\t\t<Word>PRN</Word>\r\n\t\t\t<Word>QN</Word>\r\n\t\t\t<Word>SRT</Word>\r\n\t\t\t<Word>VR</Word>\r\n\t\t\t<Word>WH</Word>\r\n\t\t\t<Word>WT</Word>\r\n\t\t</Keywords>\r\n\t\t\r\n\t\t<Keywords fontWeight=\"bold\" foreground=\"#B22222\">\t<!--ADDITIONAL COMMANDS-->\r\n\t\t\t<Word>,</Word>\r\n\t\t\t<Word>'</Word>\r\n\t\t</Keywords>\r\n\t\t\r\n\t\t\r\n\t\t<!-- Digits -->\r\n\t\t<Rule foreground=\"#D1D1D1\">\r\n            \\b0[xX][0-9a-fA-F]+  # hex number\r\n        |    \\b\r\n            (    \\d+(\\.[0-9]+)?   #number with optional floating point\r\n            |    \\.[0-9]+         #or just starting with floating point\r\n            )\r\n            ([eE][+-]?[0-9]+)? # optional exponent\r\n        </Rule>\r\n\t</RuleSet>\r\n</SyntaxDefinition>\r\n";
        private static string CommandsContent = "<Commands>\r\n  <Command name=\"Grab open\" content=\"GO\" regex=\"^\\s*GO\\s*$\" type=\"Grip\">\r\n    <Description>Opens the grip of the hand.</Description>\r\n  </Command>\r\n  <Command name=\"Grab close\" content=\"GC\" regex=\"^\\s*GC\\s*$\" type=\"Grip\">\r\n    <Description>Close the grip of hand.</Description>\r\n  </Command>\r\n  <Command name=\"Where\" content=\"WH\" regex=\" ^\\s*WH\\s*$\" type=\"Information\">\r\n    <Description>Reads the coordinates of the current position and the open or close state of the hand. (Using RS-232C)</Description>\r\n  </Command>\r\n  <Command name=\"Comment\" content=\"'\" regex=\"^\\s*'[\\w\\s]+$\" type=\"Comment\">\r\n    <Description>Allows the programmer to write a comment. [up to 120 characters]</Description>\r\n  </Command>\r\n  <Command name=\"Timer\" content=\"TI\" regex=\"^\\s*TI\\s+([1-9]|[1-9][0-9]|[1-9][0-9][0-9]|[1-9][0-9][0-9][0-9]|[1-3][0-2][0-7][0-6][0-7])\\s*$\" type=\"TimersCounters\">\r\n    <Description>Halts the motion for the specified length of time.</Description>\r\n  </Command>\r\n  <Command name=\"Go sub\" content=\"GS\" regex=\"^\\s*GS\\s+([1-9]|[1-9][0-9]|[1-9][0-9][0-9]|[1-9][0-9][0-9][0-9])\\s*$\" type=\"Programming\">\r\n    <Description>Carries out subroutine beginning with the specified line number.</Description>\r\n  </Command>\r\n  <Command name=\"Here\" content=\"HE\" regex=\"^\\s*HE\\s+([0-9]|[1-9][0-9]|[1-9][0-9][0-9])\\s*$\" type=\"Information\">\r\n    <Description>Defines the current coordinates as the specified position.</Description>\r\n  </Command>\r\n  <Command name=\"Return\" content=\"RT\" regex=\" ^\\s*RT\\s*$\" type=\"Programming\">\r\n    <Description>Completes a subroutine and returns to the main program.</Description>\r\n  </Command>\r\n  <Command name=\"Halt\" content=\"HLT\" regex=\"^\\s*HLT\\s*$\" type=\"Programming\">\r\n    <Description>Interrupts the motion of the robot and the operation of the program.</Description>\r\n  </Command>\r\n  <Command name=\"End\" content=\"ED\" regex=\"^\\s*ED\\s*$\" type=\"Programming\">\r\n    <Description>Ends the program.</Description>\r\n  </Command>\r\n  <Command name=\"Speed\" content=\"SP\" regex=\"^\\s*SP\\s+([1-9]|[1-2][0-9]|30)\\s*$\" type=\"Programming\">\r\n    <Description>Sets the operating speed, acceleration or deceleration time and the continuous path setting.</Description>\r\n  </Command>\r\n  <Command name=\"Draw straight\" content=\"DS\" regex=\"^\\s*DS\\s+(\\d+\\s*,\\s*){2}\\d+\\s*$\" type=\"Movement\">\r\n    <Description>Moves the end of the hand to a position away from the current position by the distance specified in X, Y, and Z directions. (Joint interpolation)</Description>\r\n  </Command>  \r\n  <Command name=\"Move straight\" content=\"MS\" regex=\"^\\s*MS\\s+([1-9]|[1-9][0-9]|[1-9][0-9][0-9])\\s*,\\s*(O|C)?\" type=\"Movement\">\r\n    <Description>Moves the tip of hand to the specified position.</Description>\r\n  </Command>\r\n  <Command name=\"Move continuous\" content=\"MC\" regex=\"^\\s*MC\\s+([1-9]|[1-9][0-9]|[1-9][0-9][0-9])\\s*,\\s*([0-9]|[1-9][0-9]|[1-9][0-9][0-9])\\s*((,\\s*)(O|C))?\\s*\" type=\"Movement\">\r\n    <Description>Moves the robot continously through the predefined intermediate points between two specified position numbers.</Description>\r\n  </Command>\r\n  <Command name=\"Move R\" content=\"MR\" regex=\"^\\s*MR\\s+([0-9]\\s*,\\s*|[1-9][0-9]\\s*,\\s*|[1-9][0-9][0-9]\\s*,\\s*){2}([1-9]|[1-9][0-9]|[1-9][0-9][0-9])\\s*((,\\s*)(O|C))?\\s*$\" type=\"Movement\">\r\n    <Description> Moves the tip of hand through the predefined intermediate positions in circular interpolation.</Description>\r\n  </Command>\r\n  <Command name=\"Move RA\" content=\"MRA\" regex=\"^\\s*MRA\\s+([0-9]|[1-9][0-9]|[1-9][0-9][0-9])\\s*((,\\s*)(O|C))?\\s*$\" type=\"Movement\">\r\n    <Description>Moves to specified position in circulat interpolation.</Description>\r\n  </Command>\r\n  <Command name=\"Repeat cycle\" content=\"RC\" regex=\"^\\s*RC\\s+([1-9]|[1-9][0-9]|[1-9][0-9][0-9]|[1-9][0-9][0-9][0-9]|[1-3][0-2][0-7][0-6][0-7])\\s*$\" type=\"Programming\">\r\n    <Description>Repeats the loop specified by the NX command the specified number of times.</Description>\r\n  </Command>\r\n  <Command name=\"Next\" content=\"NX\" regex=\"^\\s*NX\\s*$\" type=\"Programming\">\r\n    <Description>Specifies the range of a loop in a program executed by the RC command.</Description>\r\n  </Command>\r\n  <Command name=\"Number\" content=\"N\" regex=\"^\\s*N\\s+(\\d+|&quot;\\w+&quot;)\\s*$\" type=\"Programming\">\r\n    <Description>Select the specified program.</Description>\r\n  </Command>\r\n  <Command name=\"Override\" content=\"OVR\" regex=\"^\\s*OVR\\s+([1-9]|[1-9][0-9]|1[0-9][0-9]|200)\\s*$\" type=\"Programming\">\r\n    <Description>Specify program override.</Description>\r\n  </Command>\r\n  <Command name=\"Run\" content=\"RN\" regex=\"^\\s*RN\\s*(\\s+([1-9]|[1-9][0-9]|[1-9][0-9]{2}|[1-9][0-9]{3}))?\\s*(,(\\s*([1-9]|[1-9][0-9]|[1-9][0-9]{2}|[1-9][0-9]{3})))?\\s*(,\\s*&quot;[\\w\\s]{1,8}&quot;\\s*)?$\" type=\"Programming\">\r\n    <Description>TODO</Description>\r\n  </Command>\r\n  <Command name=\"Shift\" content=\"SF\" regex=\"^\\s*SF\\s+([1-9]|[1-9][0-9]|[1-9][0-9]{2})\\s*,\\s*([1-9]|[1-9][0-9]|[1-9][0-9]{2})$\" type=\"Programming\">\r\n    <Description>TODO</Description>\r\n  </Command>\r\n  <Command name=\"Grip pressure\" content=\"GP\" regex=\"^\\s*GP\\s+([1-9]|[1-5][0-9]|6[0-3])\\s*,\\s*([1-9]|[1-5][0-9]|6[0-3])\\s*,\\s*([1-9]|[1-9][0-9])\\s*$\" type=\"Grip\">\r\n    <Description>TODO</Description>\r\n  </Command>\r\n  <Command name=\"Position read\" content=\"PR\" regex=\"^\\s*PR\\s*([0-9]|[1-9][0-9]|[1-9][0-9]{2}\\s*)?$\" type=\"Information\">\r\n    <Description>TODO</Description>\r\n  </Command>\r\n  <Command name=\"Version read\" content=\"VR\" regex=\"^\\s*VR\\s*$\" type=\"Information\">\r\n    <Description>TODO</Description>\r\n  </Command>\r\n  <Command name=\"Position define\" content=\"PD\" regex=\"^\\s*PD\\s+([0-9]|[1-9][0-9]|[1-9][0-9]{2})((\\s*(,\\s*\\d{1,4})?(\\.\\d{1,2})?\\s*)|\\s*,\\s*){3}((,\\s*\\d+\\s*|\\s*,\\s*){2})?(,\\s*(R|L)\\s*|\\s*,\\s*)?(,\\s*(A|B)\\s*|\\s*,\\s*)?(,\\s*(O|C)\\s*|\\s*,\\s*)?$\" type=\"Programming\">\r\n    <Description>TODO</Description>\r\n  </Command>\r\n  <Command name=\"Add\" content=\"ADD\" regex=\"^\\s*ADD\\s*\\-?\\s*(([1-9]|[1-9][0-9]|[1-9][0-9]{2}|[1-9][0-9]{3}|[1-2][0-9]{4}|3[0-1][0-9]{3}|32[0-7][0-6][0-8])|(\\&amp;([0-9]|[A-F]){4}))|(\\@([1-9]|[1-9][0-9]))$\" type=\"Programming\">\r\n    <Description>TODO</Description>\r\n  </Command>\r\n  <Command name=\"And\" content=\"AN\" regex=\"^\\s*AN\\s*\\-?\\s*(([1-9]|[1-9][0-9]|[1-9][0-9]{2}|[1-9][0-9]{3}|[1-2][0-9]{4}|3[0-1][0-9]{3}|32[0-7][0-6][0-8])|(\\&amp;([0-9]|[A-F]){4}))$\" type=\"TimersCounters\">\r\n    <Description>TODO</Description>\r\n  </Command>\r\n  <Command name=\"Counter load\" content=\"CL\" regex=\"^\\s*CL\\s*(\\$\\s*)?([1-9]|[1-9][0-9])\\s*$\" type=\"TimersCounters\">\r\n    <Description>TODO</Description>\r\n  </Command>\r\n  <Command name=\"Compare counter\" content=\"CP\" regex=\"^\\s*CP\\s*(\\$\\s*)?([1-9]|[1-9][0-9])\\s*$\" type=\"TimersCounters\">\r\n    <Description>TODO</Description>\r\n  </Command>\r\n  <Command name=\"Counter read\" content=\"CR\" regex=\"^\\s*CR\\s*(\\$\\s*)?([1-9]|[1-9][0-9])\\s*$\" type=\"TimersCounters\">\r\n    <Description>TODO</Description> \r\n  </Command>\r\n</Commands>";


        /// <summary>
        /// Performs a restart. Should be called after creating file.
        /// </summary>
        private static void Restart()
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Creates Highlighting definition (.xshd) file.
        /// </summary>
        public static void CreateHighlightingDefinitionFile()
        {
            if (MessageBox.Show("Highlighting definitions not found. Would you like to create a new definition file " +
                 "(restart will be performed)?", "No definitions", MessageBoxButton.YesNoCancel, MessageBoxImage.Information)
                 == MessageBoxResult.Yes)
            {
                File.WriteAllText("CustomHighlighting.xshd", HighlightingContent);
                Restart();
            }
        }

        /// <summary>
        /// Creates Command definition (.xml) file.
        /// </summary>
        public static void CreateCommandFile()
        {
            if (MessageBox.Show("Intellisense definitions not found. Would you like to create a new definition file " +
                 "(restart will be performed)?", "No definitions", MessageBoxButton.YesNoCancel, MessageBoxImage.Information)
                 == MessageBoxResult.Yes)
            {
                File.WriteAllText("Commands.xml", CommandsContent);
                Restart();
            }
        }

        /// <summary>
        /// Creates Highlighting (.xshd) and command (.xml) definition files.
        /// </summary>
        public static void CreateAllFiles()
        {
            if (MessageBox.Show("Highlighting and intellisense definitions not found. Would you like to create new definition files " +
                "(restart will be performed)?", "No definitions", MessageBoxButton.YesNoCancel, MessageBoxImage.Information)
                == MessageBoxResult.Yes)
            {
                File.WriteAllText("CustomHighlighting.xshd", HighlightingContent);
                File.WriteAllText("Commands.xml", CommandsContent);
                Restart();
            }
        }

        /// <summary>
        /// Checks and creates missing files.
        /// </summary>
        public static void CheckForRequiredFiles()
        {
            if (MissingFileErrorAlreadyShown)
                return;

            bool highlighting = false, commands = false;
            var files = GetFilesByExtensions(new DirectoryInfo(@".\"), new string[] { ".xml", ".xshd" });
            
            foreach(var file in files)
            {
                if (file.ToString() == "Commands.xml")
                    commands = true;
                if (file.ToString() == "CustomHighlighting.xshd")
                    highlighting = true;
            }

            MissingFileErrorAlreadyShown = true;
            if (!highlighting && !commands)
                CreateAllFiles();
            else if (!highlighting)
                CreateHighlightingDefinitionFile();
            else if (!commands)
                CreateCommandFile();
        }

        /// <summary>
        /// Returns all files with desired extension(s) from location.
        /// </summary>
        /// <param name="dirInfo">Location to search for files.</param>
        /// <param name="extensions">Array of extensions to search files with. (eg. new string[] { ".xml", ".txt" } )</param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dirInfo, params string[] extensions)
        {
            var allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);

            return dirInfo.EnumerateFiles()
                          .Where(f => allowedExtensions.Contains(f.Extension));
        }

    }
}
