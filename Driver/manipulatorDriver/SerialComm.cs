using System;
using System.IO.Ports;
using System.Windows;

namespace manipulatorDriver
{
   public abstract class SerialComm : DataSupplier
    {
        #region Constants
        private const int DEFAULT_DATA_BITS = 8;
        private const int DEFAULT_BAUDRATE = 9600;
        private const Parity DEFAULT_PARITY = Parity.Even;
        private const StopBits DEFAULT_STOP_BITS = StopBits.Two;
        #endregion

        protected readonly SerialPort port;

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

            port.DataReceived += Port_DataReceived;
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
                MessageBox.Show(ex.Message);
            }
        }

        public void ClosePort()
        {
            if (!port.IsOpen) return;
            port.Close();
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var data = port.ReadExisting();
            NotifyObservers(data);
        }

        public void Write(string data)
        {
            port.Write(data + "\r");
        }
    }
}
