using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Driver
{
    public class ProgramService
    {
        public E3JManipulator Manipulator { get; private set; }

        public ProgramService(E3JManipulator manipulator)
        {
            Manipulator = manipulator;
        }

        /// <summary>
        /// Upload program from manipulator to local memory
        /// </summary>
        /// <param name="programName">Name of program on manipulator</param>
        /// <returns>Requested program or null when program with given name does not exist</returns>
        public Program UploadProgram(string programName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all programs downloaded to manipulator
        /// </summary>
        /// <returns></returns>
        public List<Program> UploadPrograms()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends program to manipulator
        /// </summary>
        /// <param name="program"></param>
        public void DownloadProgram(Program program)
        {
            if (!Manipulator.Connected) return;
            try
            {
                var lines = program.GetLines();

                for (var i = 0; i < lines.Count; i++)
                {
                    Thread.Sleep(300);
                    var prefix = $"{Convert.ToString(i + 1)} ";
                    Manipulator.SendCustom(prefix + lines[i]);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Deletes program from manipulator memory
        /// </summary>
        /// <param name="programName">Deleted program name</param>
        public void DeleteProgram(string programName)
        {
            throw new NotImplementedException();
        }
    }
}
