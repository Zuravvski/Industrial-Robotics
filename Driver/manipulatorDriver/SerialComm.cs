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
        private const SerialComm.Terminator DEFAULT_FRAME_TERMINATOR = SerialComm.Terminator.CR;

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
            port = new SerialPort();

            FrameTerminator = DEFAULT_FRAME_TERMINATOR;
            port.DataReceived += Port_DataReceived;
        }

        public SerialComm(SerialPort port)
        {
            this.port = port;
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
