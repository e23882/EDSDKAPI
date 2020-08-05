using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace Demo.View
{
    /// <summary>
    /// Setting.xaml 的互動邏輯
    /// </summary>
    public partial class Setting : Window
    {
        public Setting()
        {
            InitializeComponent();
            ReadCurrentCOMPort();

            //如果沒有設定檔產生設定檔
            if (!File.Exists($"{System.Environment.CurrentDirectory}\\setting.xml"))
            {
                CreatDefaultSetting();
                ReadSetting();
            }
            else
                ReadSetting();
        }
        public void CreatDefaultSetting()
        {
            XElement xElement = new XElement(
                new XElement("Setting",
                    new XAttribute("Frequency", "180"),
                    new XAttribute("Path", "C:\\"),
                    new XAttribute("COM", "COM4"),
                        new XElement("LV",
                            new XAttribute("LV", "1"),
                            new XAttribute("Value", "3"),
                            new XAttribute("TV", "1/320"),
                            new XAttribute("ISO", "ISO 800"),
                            new XAttribute("AV", "4.0"),
                            new XAttribute("Flash", "ON")
                            ),
                        new XElement("LV",
                            new XAttribute("LV", "2"),
                            new XAttribute("Value", "20"),
                            new XAttribute("TV", "1/800"),
                            new XAttribute("ISO", "ISO 800"),
                            new XAttribute("AV", "5.6"),
                            new XAttribute("Flash", "ON")
                            ),
                        new XElement("LV",
                            new XAttribute("LV", "3"),
                            new XAttribute("Value", "30"),
                            new XAttribute("TV", "1/1000"),
                            new XAttribute("ISO", "ISO 800"),
                            new XAttribute("AV", "5.6"),
                            new XAttribute("Flash", "OFF")
                            ),
                        new XElement("LV",
                            new XAttribute("LV", "4"),
                            new XAttribute("Value", "40"),
                            new XAttribute("TV", "1/1000"),
                            new XAttribute("ISO", "ISO 800"),
                            new XAttribute("AV", "5.6"),
                            new XAttribute("Flash", "OFF")
                            ),
                        new XElement("LV",
                            new XAttribute("LV", "5"),
                            new XAttribute("Value", "70"),
                            new XAttribute("TV", "1/1000"),
                            new XAttribute("ISO", "ISO 400"),
                            new XAttribute("AV", "5.6"),
                            new XAttribute("Flash", "OFF")
                            ),
                        new XElement("LV",
                            new XAttribute("LV", "6"),
                            new XAttribute("Value", "100"),
                            new XAttribute("TV", "1/1000"),
                            new XAttribute("ISO", "ISO 400"),
                            new XAttribute("AV", "6.3"),
                            new XAttribute("Flash", "OFF")
                            ),
                        new XElement("LV",
                            new XAttribute("LV", "7"),
                            new XAttribute("Value", "120"),
                            new XAttribute("TV", "1/1000"),
                            new XAttribute("ISO", "ISO 200"),
                            new XAttribute("AV", "7.1"),
                            new XAttribute("Flash", "OFF")
                            ),
                        new XElement("LV",
                            new XAttribute("LV", "8"),
                            new XAttribute("Value", "140"),
                            new XAttribute("TV", "1/1000"),
                            new XAttribute("ISO", "ISO 400"),
                            new XAttribute("AV", "8.0"),
                            new XAttribute("Flash", "OFF")
                            ),
                         new XElement("LV",
                            new XAttribute("LV", "9"),
                            new XAttribute("Value", "160"),
                            new XAttribute("TV", "1/1000"),
                            new XAttribute("ISO", "ISO 400"),
                            new XAttribute("AV", "9.0"),
                            new XAttribute("Flash", "OFF")
                            ),
                          new XElement("LV",
                            new XAttribute("LV", "10"),
                            new XAttribute("Value", "255"),
                            new XAttribute("TV", "1/1000"),
                            new XAttribute("ISO", "ISO 800"),
                            new XAttribute("AV", "9.0"),
                            new XAttribute("Flash", "ON")
                            )
                        )
                );

            //需要指定編碼格式，否則在讀取時會拋：根級別上的資料無效。 第 1 行 位置 1異常
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UTF8Encoding(false);
            settings.Indent = true;
            XmlWriter xw = XmlWriter.Create(System.Environment.CurrentDirectory+"\\setting.xml", settings);
            xElement.Save(xw);
            //寫入檔案
            xw.Flush();
            xw.Close();
        }

        public void ReadCurrentCOMPort()
        {
            string[] ports2 = SerialPort.GetPortNames();
            cbComPort.ItemsSource = null;
            List<ComboboxData> list = new List<ComboboxData>();
            foreach (var item in ports2)
            {
                list.Add(new ComboboxData()
                {
                    Name = item,
                    Value = item
                });
            }
            cbComPort.ItemsSource = list;
            cbComPort.DisplayMemberPath = "Name";
            cbComPort.SelectedValuePath = "Value";
        }
        public class ComboboxData
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }


        public void ReadSetting()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Setting.xml");//載入xml檔

            var root = xmlDoc.SelectNodes("Setting");
            foreach(var item in root)
            {
                var frequency = ((XmlElement)item).GetAttribute("Frequency");
                var path = ((XmlElement)item).GetAttribute("Path");
                tbPath.Text = path;
                tbFrequency.Text = frequency;
                cbComPort.SelectedValue = ((XmlElement)item).GetAttribute("COM");
                //cbComPort.SelectedItem = ((XmlElement)item).GetAttribute("COM");

                //tbCOM.Text = ((XmlElement)item).GetAttribute("COM");

            }
            var data = xmlDoc.SelectNodes("Setting/LV");
            
            foreach (var item in data)
            {
                var LV = ((XmlElement)item).GetAttribute("LV");
                var Value = ((XmlElement)item).GetAttribute("Value");
                var TV = ((XmlElement)item).GetAttribute("TV");
                var ISO = ((XmlElement)item).GetAttribute("ISO");
                var AV = ((XmlElement)item).GetAttribute("AV");
                var Flash = ((XmlElement)item).GetAttribute("Flash");
                switch (LV)
                {
                    case "1":
                        tbLight1.Text = Value;
                        tbTV1.Text = TV;
                        tbAV1.Text = AV;
                        tbISO1.Text = ISO;
                        tbFlash1.Text = Flash;
                        break;
                    case "2":
                        tbLight2.Text = Value;
                        tbTV2.Text = TV;
                        tbAV2.Text = AV;
                        tbISO2.Text = ISO;
                        tbFlash2.Text = Flash;
                        break;
                    case "3":
                        tbLight3.Text = Value;
                        tbTV3.Text = TV;
                        tbAV3.Text = AV;
                        tbISO3.Text = ISO;
                        tbFlash3.Text = Flash;
                        break;
                    case "4":
                        tbLight4.Text = Value;
                        tbTV4.Text = TV;
                        tbAV4.Text = AV;
                        tbISO4.Text = ISO;
                        tbFlash4.Text = Flash;
                        break;
                    case "5":
                        tbLight5.Text = Value;
                        tbTV5.Text = TV;
                        tbAV5.Text = AV;
                        tbISO5.Text = ISO;
                        tbFlash5.Text = Flash;
                        break;
                    case "6":
                        tbLight6.Text = Value;
                        tbTV6.Text = TV;
                        tbAV6.Text = AV;
                        tbISO6.Text = ISO;
                        tbFlash6.Text = Flash;
                        break;
                    case "7":
                        tbLight7.Text = Value;
                        tbTV7.Text = TV;
                        tbAV7.Text = AV;
                        tbISO7.Text = ISO;
                        tbFlash7.Text = Flash;
                        break;
                    case "8":
                        tbLight8.Text = Value;
                        tbTV8.Text = TV;
                        tbAV8.Text = AV;
                        tbISO8.Text = ISO;
                        tbFlash8.Text = Flash;
                        break;
                    case "9":
                        tbLight9.Text = Value;
                        tbTV9.Text = TV;
                        tbAV9.Text = AV;
                        tbISO9.Text = ISO;
                        tbFlash9.Text = Flash;
                        break;
                    case "10":
                        tbLight10.Text = Value;
                        tbTV10.Text = TV;
                        tbAV10.Text = AV;
                        tbISO10.Text = ISO;
                        tbFlash10.Text = Flash;
                        break;
                }
                
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if((sender as System.Windows.Controls.Button) is null)
                return;
            SettingDetail sd;
            var source = (sender as System.Windows.Controls.Button).Content.ToString().Replace("段修改", "");
            switch (source)
            {
                case "一":
                    sd = new SettingDetail(tbLight1.Text, tbTV1.Text, tbISO1.Text, tbAV1.Text, tbFlash1.Text, "一段修改");
                    sd.Topmost = true;
                    sd.Closed += Sd_Closed;
                    sd.Show();
                    break;
                case "二":
                    sd = new SettingDetail(tbLight2.Text, tbTV2.Text, tbISO2.Text, tbAV2.Text, tbFlash2.Text, "二段修改");
                    sd.Topmost = true;
                    sd.Closed += Sd_Closed;
                    sd.Show();
                    break;
                case "三":
                    sd = new SettingDetail(tbLight3.Text, tbTV3.Text, tbISO3.Text, tbAV3.Text, tbFlash3.Text, "三段修改");
                    sd.Topmost = true;
                    sd.Closed += Sd_Closed;
                    sd.Show();
                    break;
                case "四":
                    sd = new SettingDetail(tbLight4.Text, tbTV4.Text, tbISO4.Text, tbAV4.Text, tbFlash4.Text, "四段修改");
                    sd.Topmost = true;
                    sd.Closed += Sd_Closed;
                    sd.Show();
                    break;
                case "五":
                    sd = new SettingDetail(tbLight5.Text, tbTV5.Text, tbISO5.Text, tbAV5.Text, tbFlash5.Text, "五段修改");
                    sd.Topmost = true;
                    sd.Closed += Sd_Closed;
                    sd.Show();
                    break;
                case "六":
                    sd = new SettingDetail(tbLight6.Text, tbTV6.Text, tbISO6.Text, tbAV6.Text, tbFlash6.Text, "六段修改");
                    sd.Topmost = true;
                    sd.Closed += Sd_Closed;
                    sd.Show();
                    break;
                case "七":
                    sd = new SettingDetail(tbLight7.Text, tbTV7.Text, tbISO7.Text, tbAV7.Text, tbFlash7.Text, "七段修改");
                    sd.Topmost = true;
                    sd.Closed += Sd_Closed;
                    sd.Show();
                    break;
                case "八":
                    sd = new SettingDetail(tbLight8.Text, tbTV8.Text, tbISO8.Text, tbAV8.Text, tbFlash8.Text, "八段修改");
                    sd.Topmost = true;
                    sd.Closed += Sd_Closed;
                    sd.Show();
                    break;
                case "九":
                    sd = new SettingDetail(tbLight9.Text, tbTV9.Text, tbISO9.Text, tbAV9.Text, tbFlash9.Text, "九段修改");
                    sd.Topmost = true;
                    sd.Closed += Sd_Closed;
                    sd.Show();
                    break;
                case "十":
                    sd = new SettingDetail(tbLight10.Text, tbTV10.Text, tbISO10.Text, tbAV10.Text, tbFlash10.Text, "十段修改");
                    sd.Topmost = true;
                    sd.Closed += Sd_Closed;
                    sd.Show();
                    break;
            }
        }

        private void Sd_Closed(object sender, EventArgs e)
        {
            ReadSetting();
        }

        private void BtSaveFrequency_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newSetting = int.Parse(tbFrequency.Text);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("Setting.xml");//載入xml檔
                var data = xmlDoc.SelectSingleNode("Setting");

              
                var frequencyValue = int.Parse(((XmlElement)data).GetAttribute("Frequency"));
                ((XmlElement)data).SetAttribute("Frequency", tbFrequency.Text);
                
                xmlDoc.Save("Setting.xml");
                System.Windows.MessageBox.Show("儲存成功");
            }
            catch(Exception ie)
            {
                System.Windows.MessageBox.Show($"儲存測光值發生例外 \r\n{ie.Message}\r\n{ie.StackTrace}");
            }
        }

        private void BtSettingPAth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog path = new FolderBrowserDialog();
                path.ShowDialog();
          
                var newSetting = int.Parse(tbFrequency.Text);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("Setting.xml");//載入xml檔
                var data = xmlDoc.SelectSingleNode("Setting");
                ((XmlElement)data).SetAttribute("Path", path.SelectedPath);
                xmlDoc.Save("Setting.xml");
                System.Windows.MessageBox.Show("儲存路徑成功");
                ReadSetting();
            }
            catch (Exception ie)
            {
                System.Windows.MessageBox.Show($"更新設定檔路徑發生例外: \r\n{ie.Message}\r\n{ie.StackTrace}");
            }

        }

        private void BtCOM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("Setting.xml");//載入xml檔
                var data = xmlDoc.SelectSingleNode("Setting");
                //((XmlElement)data).SetAttribute("COM", tbCOM.Text);
                ((XmlElement)data).SetAttribute("COM", cbComPort.SelectedValue.ToString());
                xmlDoc.Save("Setting.xml");
                System.Windows.MessageBox.Show("儲存路徑成功");

                ReadSetting();
            }
            catch (Exception ie)
            {
                System.Windows.MessageBox.Show($"更新COM PORT設定發生例外: \r\n{ie.Message}\r\n{ie.StackTrace}");
            }
        }
    }
}
