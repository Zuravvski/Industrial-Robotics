using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Driver.Exceptions;

namespace Driver
{
    public class ProgramService
    {
        private readonly E3JManipulator manipulator;

        public ProgramService(E3JManipulator manipulator)
        {
            this.manipulator = manipulator;
        }

        /// <summary>
        /// Uploads program from manipulator to PC memory
        /// </summary>
        /// <param name="programName">Name of program on manipulator</param>
        /// <returns>Requested program or null when program with given name does not exist</returns>
        public async Task<Program> UploadProgram(string programName)
        {
            // TODO: Test if this works
            manipulator.Number(programName);
            await Task.Delay(500);
            var errorCode = await manipulator.ErrorRead();

            if (errorCode != 0)
            {
                throw new AlarmException(errorCode);
            }

            var program = new Program(programName);
            for (uint i = 1;; i++)
            {
                var line = await manipulator.StepRead(i);
                if (string.IsNullOrEmpty(line))
                    break;
                program.Content += line + manipulator.Port.FrameTerminator;
                await Task.Delay(300);
            }
            return program;
        }

        /// <summary>
        /// Gets all programs downloaded to manipulator
        /// </summary>
        /// <returns></returns>
        public List<Program> UploadPrograms()
        {
            var infos = ReadProgramInfo();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sends program to manipulator
        /// </summary>
        /// <param name="program"></param>
        public void DownloadProgram(Program program)
        {
            if (!manipulator.Connected) return;
            try
            {
                var lines = program.GetLines();

                for (var i = 0; i < lines.Count; i++)
                {
                    Thread.Sleep(300);
                    var prefix = $"{Convert.ToString(i + 1)} ";
                    manipulator.SendCustom(prefix + lines[i]);
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

        private async Task<List<Program>> ReadProgramInfo()
        {
            manipulator.SendCustom("EXE0, \"Fd < *.RE2\"");
            await manipulator.Port.WaitForMessageAsync();
            manipulator.Port.Read();
            // Decode data
            throw new NotImplementedException();
            return new List<Program>();
        }
    }
}
