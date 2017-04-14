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
            regexes.Add(new Regex(@"^\s*GO\s*$"));                                                                                                      //GO
            regexes.Add(new Regex(@"^\s*GC\s*$"));                                                                                                      //GC
            regexes.Add(new Regex(@"^\s*WH\s*$"));                                                                                                      //WH
            regexes.Add(new Regex(@"^\s*'[\w\s]+$"));                                                                                                   //comment
            regexes.Add(new Regex(@"^\s*TI\s+([1-9]|[1-9][0-9]|[1-9][0-9][0-9]|[1-9][0-9][0-9][0-9]|[1-3][0-2][0-7][0-6][0-7])\s*$"));                  //TI (1-32767)
            regexes.Add(new Regex(@"^\s*GS\s+([1-9]|[1-9][0-9]|[1-9][0-9][0-9]|[1-9][0-9][0-9][0-9])\s*$"));                                            //GS  
            regexes.Add(new Regex(@"^\s*HE\s+([0-9]|[1-9][0-9]|[1-9][0-9][0-9])\s*$"));                                                                 //HE (0-999)
            regexes.Add(new Regex(@"^\s*RT\s*$"));                                                                                                      //RT
            regexes.Add(new Regex(@"^\s*HLT\s*$"));                                                                                                     //HLT
            regexes.Add(new Regex(@"^\s*ED\s*$"));                                                                                                      //ED
            regexes.Add(new Regex(@"^\s*SP\s+([1-9]|[1-2][0-9]|30)\s*$"));                                                                              //SP (1-30)
            regexes.Add(new Regex(@"^\s*SD\s+([1-9]|[1-2][0-9]|30)\s*$"));                                                                              //SD 
            regexes.Add(new Regex(@"^\s*DS\s+(\d+\s*,\s*){2}\d+\s*$"));                                                                                 //DS
            regexes.Add(new Regex(@"^\s*MS\s+([1-9]|[1-9][0-9]|[1-9][0-9][0-9])\s*,\s*(O|C)?"));                                                        //MS (1-999)
            regexes.Add(new Regex(@"^\s*MC\s+([1-9]|[1-9][0-9]|[1-9][0-9][0-9])\s*,\s*([0-9]|[1-9][0-9]|[1-9][0-9][0-9])\s*((,\s*)(O|C))?\s*"));        //MC
            regexes.Add(new Regex(@"^\s*MR\s+([0-9]\s*,\s*|[1-9][0-9]\s*,\s*|[1-9][0-9][0-9]\s*,\s*){2}([1-9]|[1-9][0-9]|[1-9][0-9][0-9])\s*((,\s*)(O|C))?\s*$"));    //MR
            regexes.Add(new Regex(@"^\s*MRA\s+([0-9]|[1-9][0-9]|[1-9][0-9][0-9])\s*((,\s*)(O|C))?\s*$"));                                                //MRA
            regexes.Add(new Regex(@"^\s*RC\s+([1-9]|[1-9][0-9]|[1-9][0-9][0-9]|[1-9][0-9][0-9][0-9]|[1-3][0-2][0-7][0-6][0-7])\s*$"));                  //RC
            regexes.Add(new Regex(@"^\s*NX\s*$"));                                                                                                      //NX
            regexes.Add(new Regex("^\\s*N\\s+(\\d+|\"\\w+\")\\s*$"));                                                                                   //N
            regexes.Add(new Regex(@"^\s*OVR\s+([1-9]|[1-9][0-9]|1[0-9][0-9]|200)\s*$"));                                                                //OVR (1-200)


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
