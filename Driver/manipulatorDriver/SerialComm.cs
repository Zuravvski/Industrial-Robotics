using System;
using System.IO.Ports;

namespace ManipulatorDriver
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

        #region Constants
        private const int DEFAULT_DATA_BITS = 8;
        private const int DEFAULT_BAUDRATE = 9600;
        private const Parity DEFAULT_PARITY = Parity.Even;
        private const StopBits DEFAULT_STOP_BITS = StopBits.Two;
        private const Terminator DEFAULT_FRAME_TERMINATOR = Terminator.CR;
        #endregion

        private readonly SerialPort port;
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

        public SerialComm()
        {
            port = new SerialPort()
            {
                BaudRate = DEFAULT_BAUDRATE,
                DataBits = DEFAULT_DATA_BITS,
                Parity = DEFAULT_PARITY,
                StopBits = DEFAULT_STOP_BITS,
                RtsEnable = true,
                ReadTimeout = 4000,
                WriteTimeout = 6000
            };

            FrameTerminator = DEFAULT_FRAME_TERMINATOR;
            port.DataReceived += Port_DataReceived;
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            NotifyObservers(port.ReadExisting());
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

        public void Write(string data)
        {
            try
            {
                port.Write(data + GetTerminator());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
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
