using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LightAPI
{
    public class Light
    {
        #region Declaration
        bool Console_receiving = true;
        SerialPort connection;
        public delegate void GetDataEvent(object sender, EventArgs e);
        public event GetDataEvent ReceiveData;
        #endregion

        #region Property
        public int LightValue { get; set; }
        #endregion

        #region Memberfunction
        /// <summary>
        /// 建構子，建立COM Port連線
        /// </summary>
        /// <param name="comPort"></param>
        /// <param name="baudRate"></param>
        /// <param name="dataBits"></param>
        public Light(string comPort, int baudRate, int dataBits)
        {
            connection = new SerialPort();
            if (connection.IsOpen)
                connection.Close();
            connection.PortName = comPort;
            connection.BaudRate = baudRate;
            connection.DataBits = dataBits;
            //connection.PortName = "COM3";
            //connection.BaudRate = 9600;
            //connection.DataBits = 8;
            connection.StopBits = StopBits.One;
            if (!connection.IsOpen)
            {
                //開啟 Serial Port
                connection.Open();
                Console_receiving = true;

                //開啟執行續做接收動作
                Thread t = new Thread(DoReceive);
                t.IsBackground = true;
                t.Start();
            }
        }

        /// <summary>
        /// 透過COM Port傳送資料
        /// </summary>
        /// <param name="sendBuffer"></param>
        public void SendData(Object sendBuffer)
        {
            if (sendBuffer != null)
            {
                Byte[] buffer = sendBuffer as Byte[];

                try
                {
                    connection.Write(buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {
                    CloseComport();
                }
            }
        }

        /// <summary>
        /// 關閉COM Port連線
        /// </summary>
        private void CloseComport()
        {
            try
            {
                connection.Close();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 接收訊息
        /// </summary>
        private void DoReceive()
        {
            Byte[] buffer = new Byte[1024];

            try
            {
                while (Console_receiving)
                {
                    if (connection.BytesToRead > 0)
                    {
                        Int32 length = connection.Read(buffer, 0, buffer.Length);

                        string buf = Encoding.ASCII.GetString(buffer);
                        this.LightValue = int.Parse(buf.Substring(buf.IndexOf(",") + 1, 3));
                        ReceiveData?.Invoke(this, new EventArgs());
                    }

                    Thread.Sleep(20);
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
