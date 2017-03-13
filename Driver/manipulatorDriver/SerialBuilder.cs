using System.IO.Ports;
using ManipulatorDriver;

namespace manipulatorDriver
{
    public class SerialBuilder
    {
        #region Constants
        private const int DEFAULT_DATA_BITS = 8;
        private const int DEFAULT_BAUDRATE = 9600;
        private const Parity DEFAULT_PARITY = System.IO.Ports.Parity.Even;
        private const StopBits DEFAULT_STOP_BITS = System.IO.Ports.StopBits.Two;

        #endregion

        // TODO: Reconsider building
        private readonly SerialPort port;

        public SerialBuilder()
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
        }

        public SerialComm Build()
        {
            return new SerialComm(port);
        }

        public SerialBuilder BaudRate(int baudrate)
        {
            port.BaudRate = baudrate;
            return this;
        }

        public SerialBuilder Parity(Parity parity)
        {
            port.Parity = parity;
            return this;
        }

        public SerialBuilder StopBits(StopBits stopBits)
        {
            port.StopBits = stopBits;
            return this;
        }

        public SerialBuilder RtsEnable(bool state)
        {
            port.RtsEnable = state;
            return this;
        }
    }
}
