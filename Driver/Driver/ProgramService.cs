using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Driver.Exceptions;
using Microsoft.Win32;
using System.IO;

namespace Driver
{
    public class ProgramService
    {
        private readonly E3JManipulator manipulator;

        public ProgramService(E3JManipulator manipulator)
        {
            this.manipulator = manipulator;
        }

        public async void RunProgram(RemoteProgram remoteProgram)
        {
            if (!manipulator.Connected) return;
            try
            {
                manipulator.Number(remoteProgram.Name);
                await Task.Delay(1000);
                manipulator.Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Uploads program from manipulator to PC memory
        /// </summary>
        /// <param name="programName">Name of program on manipulator</param>
        /// <returns>Requested program or null when program with given name does not exist</returns>
        public async Task<Program> UploadProgram(RemoteProgram remoteProgram)
        {
            // TODO: Test if this works
            manipulator.Number(remoteProgram.Name);
            await Task.Delay(1000);
            var errorCode = await manipulator.ErrorRead();

            if (errorCode != 0)
            {
                throw new AlarmException(errorCode);
            }

            var content = string.Empty;
            for (uint i = 1;; i++)
            {
                var line = await manipulator.StepRead(i);
                if (line.Equals("\r"))
                    break;
                content += line + "\n";
            }
            return Program.CreateFromRemoteProgram(remoteProgram, content);
        }

        /// <summary>
        /// Receives all programs downloaded from manipulator
        /// </summary>
        /// <returns></returns>
        public async Task<List<Program>> UploadPrograms(List<RemoteProgram> remotePrograms)
        {
            var programs = new List<Program>();
            for(int i = 0; i < remotePrograms.Count; i++)
            {
                programs.Add(await UploadProgram(remotePrograms[i]));
            }
            return programs;
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
                    //var prefix = $"{Convert.ToString(i + 1)} ";
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
        /// Sends program to manipulator
        /// </summary>
        /// <param name="program"></param>
        public async void DownloadProgram()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "txt files (.txt)|*.txt"
            };
            // Default file name
            // Default file extension
            // Filter files by extension

            if (dialog.ShowDialog() == false)
            {
                return;
            }

            var path = dialog.FileName;
            var name = Path.GetFileNameWithoutExtension(path);
            var lines = File.ReadAllLines($"{dialog.FileName}");

            if (!manipulator.Connected) return;
            try
            {
                manipulator.Number(name);
                await Task.Delay(1000);
                manipulator.New();
                await Task.Delay(1000);

                for (var i = 0; i < lines.Length; i++)
                {
                    await Task.Delay(500);
                    //var prefix = $"{Convert.ToString(i + 1)} ";
                    manipulator.SendCustom(lines[i]);
                    Debug.WriteLine($"{i}/{lines.Length}, {lines[i]}");
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

        public async Task<List<RemoteProgram>> ReadProgramInfo()
        {
            List<RemoteProgram> remoteProgramList = new List<RemoteProgram>();

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
                remoteProgramList.Add(RemoteProgram.Create(QoK));
            }
            return remoteProgramList;
        }
    }
}
