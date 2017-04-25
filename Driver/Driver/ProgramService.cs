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
            await Task.Delay(1000);
            var errorCode = await manipulator.ErrorRead();

            if (errorCode != 0)
            {
                throw new AlarmException(errorCode);
            }

            var program = new Program(programName);
            for (uint i = 1;; i++)
            {
                var line = await manipulator.StepRead(i);
                if (line.Equals("\r"))
                    break;
                program.Content += line + "\n";
            }
            return program;
        }

        /// <summary>
        /// Receives all programs downloaded from manipulator
        /// </summary>
        /// <returns></returns>
        public async Task<List<Program>> UploadPrograms()
        {
            var infos = await ReadProgramInfo();
            for(int i = 0; i < infos.Count; i++)
            {
                infos[i] = await UploadProgram(infos[i].Name);
            }
            return infos;
        }

        /// <summary>
        /// Sends program to manipulator
        /// </summary>
        /// <param name="program"></param>
        public async void DownloadProgram(Program program)
        {
            if (!manipulator.Connected) return;
            try
            {
                manipulator.Number(program.Name);
                await Task.Delay(1000);
                manipulator.New();
                await Task.Delay(1000);

                var lines = program.GetLines();

                for (var i = 0; i < lines.Count; i++)
                {
                    await Task.Delay(500);
                    var prefix = $"{Convert.ToString(i + 1)} ";
                    manipulator.SendCustom(lines[i]);
                    Debug.WriteLine(i);
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

        public async Task<List<Program>> ReadProgramInfo()
        {
            List<Program> programList = new List<Program>();

            // Decode data
            for (int i = 1; ; i++)
            {
                if (i == 1)
                    manipulator.SendCustom("EXE0, \"Fd<*\"");
                else
                    manipulator.SendCustom($"EXE0, \"Fd{i}\"");

                await manipulator.Port.WaitForMessageAsync();
                var QoK = manipulator.Port.Read();
                if (QoK.Equals("QoK\r"))
                    break;
                programList.Add(Program.CreateFromInfoString(QoK));
            }
            return programList;
        }
    }
}
