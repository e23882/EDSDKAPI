using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using EOSDigital.API;
using EOSDigital.SDK;
using LightAPI;

namespace Demo
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Declarations
        ViewModel.MainWindowViewModel main = new ViewModel.MainWindowViewModel();
        CanonAPI APIHandler;
        Camera MainCamera;
        CameraValue[] AvList;
        CameraValue[] TvList;
        CameraValue[] ISOList;
        List<Camera> CamList;
        bool IsInit = false;
        int BulbTime = 30;
        ImageBrush bgbrush = new ImageBrush();
        Action<BitmapImage> SetImageAction;
        int ErrCount;
        object ErrLock = new object();
        Thread checkThread;
        private bool IsRunning = true;
        Dictionary<int, SettingData> TotalSetting = new Dictionary<int, SettingData>();
        Light lightInstance;
        #endregion

        #region DataModel
        public class SettingData
        {
            public int LV { get; set; }
            public int LightValue { get; set; }
            public string TV { get; set; }
            public string ISO { get; set; }
            public string AV { get; set; }
            public string Flash { get; set; }

        }
        #endregion

        #region Memberfunction
        public MainWindow()
        {
            //初始化元件
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            try
            {
                //檢查設定檔(Setting.xml)是否存在，是 > 讀取設定 否 > 建立預設設定檔 
                if (!File.Exists($"{System.Environment.CurrentDirectory}\\setting.xml"))
                {
                    CreatDefaultSetting();
                    ReadSetting();
                }
                else
                    ReadSetting();
                //設定UI DataBinding
                this.DataContext = main;
                //初始化相機API相關物件
                if (!InitAPI())
                    MessageBox.Show("Error");
                //
                lightInstance = new Light(main.COM, 9600, 8);
                lightInstance.ReceiveData += Lt_ReceiveData;
                //初始化測光執行續
                checkThread = new Thread(GetLightValue);
                checkThread.Start();
            }
            catch (Exception ie)
            {
                MessageBox.Show($"程式初始化發生例外{ie.Message}\r\n{ie.StackTrace}");
                if(checkThread != null)
                    checkThread.Suspend();
                btStop.Content = "啟動";
                ReadSetting();
            }
        }

        private void Lt_ReceiveData(object sender, EventArgs e)
        {
            try
            {
                //依照測光值設定相機
                SetCamera((sender as Light).LightValue);
                Thread.Sleep(main.Frequency * 1000);

            }
            catch (Exception ie)
            {
            }
        }

        /// <summary>
        /// 讀取測光值
        /// </summary>
        private void GetLightValue()
        {
            try
            {
                while (true)
                {
                    lightInstance.SendData(Encoding.ASCII.GetBytes("m@G02#"));
                    Thread.Sleep(main.Frequency * 1000);
                }
            }
            catch (Exception ie)
            {
            }
        }

        /// <summary>
        /// 依照測光值設定相機AV、TV、ISO
        /// </summary>
        /// <param name="lightValue"></param>
        public void SetCamera(int lightValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Setting.xml");//載入xml檔

            var root = xmlDoc.SelectSingleNode("Setting");
            main.SavePath = ((XmlElement)root).GetAttribute("Path");

            var data = xmlDoc.SelectNodes("Setting/LV");
            TotalSetting.Clear();
            foreach (var item in data)
            {
                var lv = ((XmlElement)item).GetAttribute("LV");
                var value = ((XmlElement)item).GetAttribute("Value");
                var tv = ((XmlElement)item).GetAttribute("TV");
                var iso = ((XmlElement)item).GetAttribute("ISO");
                var av = ((XmlElement)item).GetAttribute("AV");
                var flash = ((XmlElement)item).GetAttribute("Flash");
                TotalSetting.Add(int.Parse(value), new SettingData()
                {
                    AV = av,
                    Flash = flash,
                    ISO = iso,
                    LightValue = int.Parse(value),
                    LV = int.Parse(lv),
                    TV = tv
                });
            }
            //如果設定資料中有完全符合的測光值設定
            if (TotalSetting.ContainsKey(lightValue))
            {
                var dt = TotalSetting[lightValue];
                main.Flash = dt.Flash;
                main.ISO = dt.ISO;
                main.Light = lightValue.ToString();
                main.Shutter = dt.TV;
                main.Aperture = dt.AV;
            }
            //沒完全符合，抓最接近
            else
            {
                var minLight = TotalSetting.Where(x => x.Key > lightValue).Min(x => x.Key);
                var dt = TotalSetting[minLight];
                main.Flash = dt.Flash;
                main.ISO = dt.ISO;
                main.Light = lightValue.ToString();
                main.Shutter = dt.TV;
                main.Aperture = dt.AV;
            }

            //Set Camera
            SettingAV(main.Aperture);
            SettingISO(main.ISO);
            SettingTV(main.Shutter);
            if (main.Flash.Equals("ON"))
                SettingRelayFlash(true);
            else
                SettingRelayFlash(false);


        }

        /// <summary>
        /// 讀取設定(Setting.xml)
        /// </summary>
        public void ReadSetting()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Setting.xml");//載入xml檔
            var settingNode = xmlDoc.SelectSingleNode("Setting");
            var frequency = ((XmlElement)settingNode).GetAttribute("Frequency");
            main.COM = ((XmlElement)settingNode).GetAttribute("COM");
            if(lightInstance != null)
                lightInstance.SetCOM(main.COM);
            main.Frequency = int.Parse(frequency);
        }

        /// <summary>
        /// 建立預設設定檔
        /// </summary>
        public void CreatDefaultSetting()
        {
            XElement xElement = new XElement(
                new XElement("Setting",
                new XAttribute("COM", "COM4"),
                new XAttribute("Frequency", "180"),
                    new XAttribute("Path", "C:\\"),
                        new XElement("LV",
                            new XAttribute("LV", "1"),
                            new XAttribute("Value", "3"),
                            new XAttribute("TV", "1/320"),
                            new XAttribute("ISO", "ISO 800"),
                            new XAttribute("AV", "4"),
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
                            new XAttribute("AV", "8"),
                            new XAttribute("Flash", "OFF")
                            ),
                         new XElement("LV",
                            new XAttribute("LV", "9"),
                            new XAttribute("Value", "160"),
                            new XAttribute("TV", "1/1000"),
                            new XAttribute("ISO", "ISO 400"),
                            new XAttribute("AV", "9"),
                            new XAttribute("Flash", "OFF")
                            ),
                          new XElement("LV",
                            new XAttribute("LV", "10"),
                            new XAttribute("Value", "255"),
                            new XAttribute("TV", "1/1000"),
                            new XAttribute("ISO", "ISO 800"),
                            new XAttribute("AV", "9"),
                            new XAttribute("Flash", "ON")
                            )
                        )
                );

            //需要指定編碼格式，否則在讀取時會拋：根級別上的資料無效。 第 1 行 位置 1異常
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UTF8Encoding(false);
            settings.Indent = true;
            XmlWriter xw = XmlWriter.Create(System.Environment.CurrentDirectory + "\\setting.xml", settings);
            xElement.Save(xw);
            //寫入檔案
            xw.Flush();
            xw.Close();
        }

        //與連接相機建立Session
        public bool InitAPI()
        {
            try
            {
                APIHandler = new CanonAPI();
                CamList = APIHandler.GetCameraList();
                APIHandler.CameraAdded += APIHandler_CameraAdded;
                ErrorHandler.SevereErrorHappened += ErrorHandler_SevereErrorHappened;
                ErrorHandler.NonSevereErrorHappened += ErrorHandler_NonSevereErrorHappened;
                SetImageAction = (BitmapImage img) => { bgbrush.ImageSource = img; };
                IsInit = true;
                OpenSession();
                SettingSaveMode();
                return true;
            }
            catch (DllNotFoundException de)
            {
                MessageBox.Show($"Canon DLLs not found!:\r\n{de.Message}\r\n{de.StackTrace}");
                return false;
            }
            catch (Exception ie)
            {
                MessageBox.Show($"初始化相機API發生例外:\r\n{ie.Message}\r\n{ie.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 開啟相機執行續
        /// </summary>
        private void OpenSession()
        {
            try
            {
                MainCamera = CamList[0];
                MainCamera.OpenSession();
                MainCamera.LiveViewUpdated += MainCamera_LiveViewUpdated;
                MainCamera.ProgressChanged += MainCamera_ProgressChanged;
                MainCamera.StateChanged += MainCamera_StateChanged;
                MainCamera.DownloadReady += MainCamera_DownloadReady;
                main.DeviceName = MainCamera.DeviceName;
                AvList = MainCamera.GetSettingsList(PropertyID.Av);
                TvList = MainCamera.GetSettingsList(PropertyID.Tv);
                ISOList = MainCamera.GetSettingsList(PropertyID.ISO);
            }
            catch (Exception ie)
            {
                MessageBox.Show($"建立相機Session發生例外\r\n{ie.Message}\r\n{ie.StackTrace}");
            }
        }

        /// <summary>
        /// 關閉相機Session
        /// </summary>
        private void CloseSession()
        {
            MainCamera.CloseSession();
        }

        private void BtSetting_Click(object sender, RoutedEventArgs e)
        {
            //停止測光執行續
            if(checkThread != null)
                checkThread.Suspend();
            btStop.Content = "啟動";
            IsRunning = false;
            //顯示登入視窗
            View.Login login = new View.Login();
            login.Topmost = true;
            login.Show();
        }

        /// <summary>
        /// 偵測光線停止/起動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtStop_Click(object sender, RoutedEventArgs e)
        {
            if (IsRunning)
            {
                checkThread.Suspend();
                btStop.Content = "啟動";
            }
            else
            {
                if (checkThread is null)
                    Init();
                else
                {
                    checkThread.Resume();
                    btStop.Content = "停止";
                    ReadSetting();
                }
            }
            IsRunning = !IsRunning;
        }

        /// <summary>
        /// 關閉視窗，處理中斷測光執行續、停止相機Session
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            //中止測光執行續
            if (checkThread != null)
                checkThread.Interrupt();
            //關閉相機Session
            CloseSession();
            System.Environment.Exit(System.Environment.ExitCode);
        }

        /// <summary>
        /// 設定相機AV
        /// </summary>
        /// <param name="value"></param>
        public void SettingAV(string value)
        {
            try
            {
                MainCamera.SetSetting(
                    PropertyID.Av,
                    AvValues.GetValue(value).IntValue
                    );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定相機光圈發生例外\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 設定相機TV
        /// </summary>
        /// <param name="value"></param>
        public void SettingTV(string value)
        {
            try
            {
                MainCamera.SetSetting(
                    PropertyID.Tv,
                    TvValues.GetValue(value).IntValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定相機快門發生例外\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 設定相機ISO
        /// </summary>
        /// <param name="value"></param>
        public void SettingISO(string value)
        {
            try
            {
                MainCamera.SetSetting(
                    PropertyID.ISO,
                    ISOValues.GetValue(value).IntValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定相機ISO發生例外\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 設定相機儲存置電腦
        /// </summary>
        public void SettingSaveMode()
        {
            try
            {
                if (IsInit)
                {
                    MainCamera.SetSetting(PropertyID.SaveTo, (int)SaveTo.Host);
                    MainCamera.SetCapacity(4096, int.MaxValue);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定相機儲存模式時發生例外\r\n{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 設定閃光燈
        /// </summary>
        public void SettingRelayFlash(bool isTurnOnFlash)
        {
            if(isTurnOnFlash)
                lightInstance.SendData(Encoding.ASCII.GetBytes("m@M01,21#"));
            else
                lightInstance.SendData(Encoding.ASCII.GetBytes("m@M01,20#"));
        }

        /// <summary>
        /// 試拍
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSnapshot_Click(object sender, RoutedEventArgs e)
        {
            main.IsTest = true;
            MainCamera.TakePhotoAsync();
        }

        #region 相機相關事件
        private void ErrorHandler_SevereErrorHappened(object sender, Exception ex)
        {
        }

        private void ErrorHandler_NonSevereErrorHappened(object sender, ErrorCode ex)
        {
        }

        private void APIHandler_CameraAdded(CanonAPI sender)
        {
        }


        private void MainCamera_ProgressChanged(object sender, int progress)
        {
        }

        private void MainCamera_StateChanged(Camera sender, StateEventID eventID, int parameter)
        {
            try
            {
                if (eventID == StateEventID.Shutdown && IsInit)
                {
                    Dispatcher.Invoke((Action)delegate { CloseSession(); });
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void MainCamera_DownloadReady(Camera sender, DownloadInfo Info)
        {
            try
            {
                //試拍
                if (main.IsTest == true)
                {
                    var testSavePath = $"{System.AppDomain.CurrentDomain.BaseDirectory}\\Test";
                    if (!Directory.Exists(testSavePath))
                    {
                        Directory.CreateDirectory(testSavePath);
                        sender.DownloadFile(Info, testSavePath);
                    }
                    sender.DownloadFile(Info, testSavePath);

                    FileTimeInfo file = GetLatestFileTimeInfo(testSavePath, ".JPG");
                    Dispatcher.BeginInvoke(new Action(delegate
                    {
                        Window wd = new Window();
                        wd.Width = 300;
                        wd.Height = 300;
                        ImageBrush myBrush = new ImageBrush();
                        myBrush.ImageSource =
                            new BitmapImage(new Uri(file.FileName, UriKind.Absolute));
                        wd.Background = myBrush;
                        wd.Show();
                    }));

                    main.IsTest = false;
                }
                //一般模式
                else
                    sender.DownloadFile(Info, main.SavePath);
            }
            catch (Exception oe)
            {

            }
        }

        public class FileTimeInfo
        {
            public string FileName;  //檔名
            public DateTime FileCreateTime; //建立時間
        }

        private FileTimeInfo GetLatestFileTimeInfo(string dir, string ext)
        {
            List<FileTimeInfo> list = new List<FileTimeInfo>();
            DirectoryInfo d = new DirectoryInfo(dir);
            foreach (FileInfo file in d.GetFiles())
            {
                if (file.Extension.ToUpper() == ext.ToUpper())
                {
                    list.Add(new FileTimeInfo()
                    {
                        FileName = file.FullName,
                        FileCreateTime = file.CreationTime
                    });
                }
            }
            var f = from x in list
                    orderby x.FileCreateTime
                    select x;
            return f.LastOrDefault();
        }

        private void MainCamera_LiveViewUpdated(Camera sender, Stream img)
        {
        }

        #endregion

        #endregion
    }
}
