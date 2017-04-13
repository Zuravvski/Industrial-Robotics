using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IDE.Common.Models
{
    public class RegexMatching
    {



        public static bool InputMatching(string line)
        {
            List<Regex> regexes = new List<Regex>();

            //patterns
            regexes.Add(new Regex(@"^GO\s+$"));    //GO
            regexes.Add(new Regex(@"^GC\s+$"));    //GC
            regexes.Add(new Regex(@"^WH\s+$"));    //WH
            regexes.Add(new Regex(@"^'[\w+\s+]$"));  //comment  POPRAW WIELE SPACJI
            regexes.Add(new Regex(@"^TI\s+\d+\s+$"));    //TI  POPRAW ZAKRES
            regexes.Add(new Regex(@"^GS\s+\d+\s+$"));    //GS  POPRAW ZAKRES
            regexes.Add(new Regex(@"^HE\s+\d+\s+$"));    //HE  POPRAW ZAKRES
            regexes.Add(new Regex(@"^RT\s+$"));    //RT
            regexes.Add(new Regex(@"^HLT\s+$"));    //HLT
            regexes.Add(new Regex(@"^ED\s+$"));    //ED
            regexes.Add(new Regex(@"^SP\s+[1-2][0-9]\s+$"));    //SP    //POPRAW    
            regexes.Add(new Regex(@"^SD\s+[1-2][0-9]\s+$"));    //SD    //POPRAW


            foreach (Regex regex in regexes)
            {
                if (regex.IsMatch(line))
                    return true;
            }

            //if there was no match
            return false;
        }
    }
}
