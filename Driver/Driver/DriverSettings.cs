using System;
using System.IO;
using System.IO.Ports;
using System.Xml;

namespace Driver
{
    public class DriverSettings : IXMLObject
    {
        #region Constants
        private const int DEFAULT_DATA_BITS = 8;
        private const int DEFAULT_BAUDRATE = 9600;
        private const Parity DEFAULT_PARITY = System.IO.Ports.Parity.Even;
        private const StopBits DEFAULT_STOP_BITS = System.IO.Ports.StopBits.Two;
        private const bool DEFAULT_RTSENABLE = true;
        private const int DEFAULT_READTIMEOUT = 4000;
        private const int DEFAULT_WRITETIMEOUT = 4000;
        #endregion

        #region Node 
        private const string BaudRateToken = "BaudRate";
        private const string DataBitsToken = "DataBits";
        private const string ParityToken = "Parity";
        private const string StopBitsToken = "StopBits";
        private const string RtsEnableToken = "RtsEnable";
        private const string ReadTimeoutToken = "ReadTimeout";
        private const string WriteTimeoutToken = "WriteTimeout";
        #endregion

        public int BaudRate { get; private set; }
        public int DataBits { get; private set; }
        public Parity Parity { get; private set; }
        public StopBits StopBits { get; private set; }
        public bool RtsEnable { get; private set; }
        public int ReadTimeout { get; private set; }
        public int WriteTimeout { get; private set; }

        private static readonly Lazy<DriverSettings> instance = new Lazy<DriverSettings>(() => new DriverSettings());

        public static DriverSettings Instance => instance.Value;


        protected DriverSettings()
        {
            FromXML();
        }

        public void ToXML()
        {
            var doc = new XmlDocument();
            var node = doc.AppendChild(doc.CreateElement("DriverSettings"));

            var temp = doc.CreateElement(BaudRateToken);
            temp.InnerText = Convert.ToString(BaudRate);
            node.AppendChild(temp);

            temp = doc.CreateElement(DataBitsToken);
            temp.InnerText = Convert.ToString(DataBits);
            node.AppendChild(temp);

            temp = doc.CreateElement(ParityToken);
            temp.InnerText = Convert.ToString(Parity);
            node.AppendChild(temp);

            temp = doc.CreateElement(StopBitsToken);
            temp.InnerText = Convert.ToString(StopBits);
            node.AppendChild(temp);

            temp = doc.CreateElement(RtsEnableToken);
            temp.InnerText = Convert.ToString(RtsEnable);
            node.AppendChild(temp);

            temp = doc.CreateElement(ReadTimeoutToken);
            temp.InnerText = Convert.ToString(ReadTimeout);
            node.AppendChild(temp);

            temp = doc.CreateElement(WriteTimeoutToken);
            temp.InnerText = Convert.ToString(WriteTimeout);
            node.AppendChild(temp);

            doc.Save("DriverSettings.xml");
        }

        public void FromXML()
        {
            var doc = new XmlDocument();
            var path = "..\\Debug\\DriverSettings.xml";

            if (!File.Exists(path))
            {
                BaudRate = DEFAULT_BAUDRATE;
                DataBits = DEFAULT_DATA_BITS;
                Parity = DEFAULT_PARITY;
                StopBits = DEFAULT_STOP_BITS;
                RtsEnable = DEFAULT_RTSENABLE;
                ReadTimeout = DEFAULT_READTIMEOUT;
                WriteTimeout = DEFAULT_WRITETIMEOUT;
                ToXML();
                return;
            }

            doc.Load("..\\Debug\\DriverSettings.xml");

            var node = doc.DocumentElement?.SelectSingleNode("/DriverSettings");

            if (node == null) return;
            // BAUDRATE

            var baudrateNode = node.SelectSingleNode(BaudRateToken);
            if (baudrateNode != null)
            {
                int baudrate;
                if (!int.TryParse(baudrateNode.InnerText, out baudrate))
                {
                    baudrate = DEFAULT_BAUDRATE;
                }
                BaudRate = baudrate;
            }
            else
            {
                BaudRate = DEFAULT_BAUDRATE;
            }

            // DATABITS    

            var databitsNode = node.SelectSingleNode(DataBitsToken);
            if (databitsNode != null)
            {
                int databits;
                if (!int.TryParse(databitsNode.InnerText, out databits))
                {
                    databits = DEFAULT_DATA_BITS;
                }
                DataBits = databits;
            }
            else
            {
                DataBits = DEFAULT_DATA_BITS;
            }

            // PARITY

            var parityNode = node.SelectSingleNode(ParityToken);
            if (parityNode != null)
            {
                int parity;
                if (!int.TryParse(parityNode.InnerText, out parity))
                {

                    parity = (int)DEFAULT_PARITY;
                }
                Parity = (Parity)parity;
            }
            else
            {
                Parity = DEFAULT_PARITY;
            }

            // STOPBITS

            var stopbitsNode = node.SelectSingleNode(StopBitsToken);
            if (stopbitsNode != null)
            {
                int stopbits;
                if (!int.TryParse(stopbitsNode.InnerText, out stopbits))
                {
                    stopbits = (int)DEFAULT_STOP_BITS;
                }
                StopBits = (StopBits)stopbits;
            }
            else
            {
                StopBits = DEFAULT_STOP_BITS;
            }

            // RTSENABLE

            var rtsenableNode = node.SelectSingleNode(RtsEnableToken);
            if (rtsenableNode != null)
            {
                bool rtsenable;
                if (!bool.TryParse(rtsenableNode.InnerText, out rtsenable))
                {
                    rtsenable = DEFAULT_RTSENABLE;
                }
                RtsEnable = rtsenable;
            }
            else
            {
                RtsEnable = DEFAULT_RTSENABLE;
            }

            // READTIMEOUT

            var readtimeoutNode = node.SelectSingleNode(ReadTimeoutToken);
            if (readtimeoutNode != null)
            {
                int readtimeout;
                if (!int.TryParse(readtimeoutNode.InnerText, out readtimeout))
                {
                    readtimeout = DEFAULT_READTIMEOUT;
                }
                ReadTimeout = readtimeout;
            }
            else
            {
                ReadTimeout = DEFAULT_READTIMEOUT;
            }

            // WRITETIMEOUT

            var writetimeoutNode = node.SelectSingleNode(WriteTimeoutToken);
            if (writetimeoutNode != null)
            {
                int writetimeout;
                if (!int.TryParse(writetimeoutNode.InnerText, out writetimeout))
                {
                    writetimeout = DEFAULT_WRITETIMEOUT;
                }
                WriteTimeout = writetimeout;
            }
            else
            {
                WriteTimeout = DEFAULT_WRITETIMEOUT;
            }
        }
    }
}    


