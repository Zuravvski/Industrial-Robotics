using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;

namespace Driver
{
   public class SerialComm : DataSupplier
    {
        public enum Terminator
        {
            NONE,
            CR,     // Carriage return
            LF,     // Line feed
            CRLF    // Both
        };
        private const Terminator DEFAULT_FRAME_TERMINATOR = SerialComm.Terminator.CR;

        private readonly SerialPort port;
        private readonly HashSet<IObserver<string>> observers;
        private string incompleteData;

        /// <summary>
        /// Enables adjusting data frame terminator 
        /// </summary>
        public Terminator FrameTerminator { get; set; }

        #region Properties
        public int BaudRate
        {
            get { return port.BaudRate; }
            set { port.BaudRate = value; }
        }

        public int DataBits
        {
            get { return port.DataBits; }
            set { port.DataBits = value; }
        }

        public Parity Parity
        {
            get { return port.Parity; }
            set { port.Parity = value; }
        }

        public StopBits StopBits
        {
            get { return port.StopBits; }
            set { port.StopBits = value; }
        }

        public bool RtsEnable
        {
            get { return port.RtsEnable; }
            set { port.RtsEnable = value; }
        }

        #endregion

        public SerialComm(SerialPort port)
        {
            this.port = port;
            FrameTerminator = DEFAULT_FRAME_TERMINATOR;
            observers = new HashSet<IObserver<string>>();
            
        }

        private async void Read()
        {
            try
            {
                var buffer = new byte[1024];
                await port.BaseStream.ReadAsync(buffer, 0, buffer.Length);
                var decoded = Encoding.ASCII.GetString(buffer).Split('\r');

                foreach (var line in decoded)
                {
                    NotifyObservers(line);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public void OpenPort(string portName)
        {
            if (port.IsOpen) return;
            port.PortName = portName;
            try
            {
                port.Open();
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public void ClosePort()
        {
            if (!port.IsOpen) return;
            port.Close();
        }

        public async void Write(string data)
        {
            try
            {
                var processedData = Encoding.ASCII.GetBytes(data + GetTerminator());
                await port.BaseStream.WriteAsync(processedData, 0, processedData.Length);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Debug.WriteLine(e.Message);
            }
        }

        private string GetTerminator()
        {
            var terminator = "";
            switch (FrameTerminator)
            {
                case Terminator.CR:
                    terminator = "\r";
                    break;

                case Terminator.LF:
                    terminator = "\n";
                    break;

                case Terminator.CRLF:
                    terminator = "\r\n";
                    break;
            }
            return terminator;
        }
    }
}
