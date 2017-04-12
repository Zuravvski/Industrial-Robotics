using System;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

namespace Driver
{
   public class SerialComm
    {
        private readonly SerialPort port;

        #region Enums and data structures
        public enum Terminator
        {
            NONE,
            CR,     // Carriage return
            LF,     // Line feed
            CRLF    // Both
        };
        private const Terminator DEFAULT_FRAME_TERMINATOR = SerialComm.Terminator.CR;
        public static readonly int DEFAULT_BUFFER_SIZE = 1024;
        #endregion

        #region Events

        public delegate void ReceiveData(string data);

        public event ReceiveData DataReceived;

        #endregion

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

        public int BufferSize { get; set; }

        public Terminator FrameTerminator { get; set; }

        public bool Opened => port.IsOpen;

        #endregion

        // TODO: Builder instead of constructors
        public SerialComm()
        {
            port = new SerialPort();
            FrameTerminator = DEFAULT_FRAME_TERMINATOR;
            BufferSize = DEFAULT_BUFFER_SIZE;
            port.DataReceived += Port_DataReceived;
        }

        public SerialComm(SerialPort port)
        {
            this.port = port;
            FrameTerminator = DEFAULT_FRAME_TERMINATOR;
            BufferSize = DEFAULT_BUFFER_SIZE;
            port.DataReceived += Port_DataReceived;
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var read = "";
            while(!read.Contains("\r"))
            {
                read += port.ReadExisting();
            }
            DataReceived?.Invoke(read);
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

        public async Task WriteAsync(string data)
        {
            try
            {
                var buffer = Encoding.ASCII.GetBytes(data);
                await port.BaseStream.WriteAsync(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        public async Task<string> ReadAsync()
        {
            var buffer = new byte[1024];
            try
            {
                var bytesRead = await port.BaseStream.ReadAsync(buffer, 0, buffer.Length);
                Array.Resize(ref buffer, bytesRead);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }

            return Encoding.ASCII.GetString(buffer);
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
