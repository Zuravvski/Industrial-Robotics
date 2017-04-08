using System.IO.Ports;
using ManipulatorDriver;
using Driver;

namespace manipulatorDriver
{
    public class SerialBuilder
    {
        // TODO: Reconsider building
        private readonly SerialPort port;

        public SerialBuilder()
        {
            port = new SerialPort()
            {
                BaudRate = DriverSettings.Instance.BaudRate,
                DataBits = DriverSettings.Instance.DataBits,
                Parity = DriverSettings.Instance.Parity,
                StopBits = DriverSettings.Instance.StopBits,
                RtsEnable = DriverSettings.Instance.RtsEnable,
                ReadTimeout = DriverSettings.Instance.ReadTimeout,
                WriteTimeout = DriverSettings.Instance.WriteTimeout
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

        public SerialBuilder DataBits(int databits)
        {
            port.DataBits = databits;
            return this;
        }

        public SerialBuilder ReadTimeout(int readtimeout)
        {
            port.ReadTimeout = readtimeout;
            return this;
        }

        public SerialBuilder WriteTimeout(int writetimeout)
        {
            port.WriteTimeout = writetimeout;
            return this;
        }
    }
}
