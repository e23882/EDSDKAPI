using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace Demo.View
{
    /// <summary>
    /// SettingDetail.xaml 的互動邏輯
    /// </summary>
    public partial class SettingDetail : Window
    {
        #region Declarations
        SettingDetailViewModel viewModel;
        #endregion

        #region Memberfunction
        public SettingDetail(string lightValue, string tv, string iso, string av, string flash, string title)
        {
            InitializeComponent();
            viewModel = new SettingDetailViewModel();
            this.DataContext = viewModel;
            viewModel.Title = title;
            viewModel.AV = av;
            switch (av)
            {
                case "4.0":
                    viewModel.AVIndex = 0;
                    break;
                case "5.6":
                    viewModel.AVIndex = 1;
                    break;
                case "6.3":
                    viewModel.AVIndex = 2;
                    break;
                case "7.1":
                    viewModel.AVIndex = 3;
                    break;
                case "8.0":
                    viewModel.AVIndex = 4;
                    break;
                case "9.0":
                    viewModel.AVIndex = 5;
                    break;
            }
            viewModel.Flash = flash;
            switch (flash)
            {
                case "ON":
                    viewModel.FlashIndex = 0;
                    break;
                case "OFF":
                    viewModel.FlashIndex = 1;
                    break;
            }
            viewModel.ISO = iso;
            switch (iso)
            {
                case "ISO 100":
                    viewModel.ISOIndex = 0;
                    break;
                case "ISO 200":
                    viewModel.ISOIndex = 1;
                    break;
                case "ISO 400":
                    viewModel.ISOIndex = 2;
                    break;
                case "ISO 800":
                    viewModel.ISOIndex = 3;
                    break;
                case "ISO 1600":
                    viewModel.ISOIndex = 4;
                    break;
            }
            viewModel.TV = tv;
            switch (tv)
            {
                case "1/1000":
                    viewModel.TVIndex = 0;
                    break;
                case "1/800":
                    viewModel.TVIndex = 1;
                    break;
                case "1/640":
                    viewModel.TVIndex = 2;
                    break;
                case "1/320":
                    viewModel.TVIndex = 3;
                    break;
                case "1/250":
                    viewModel.TVIndex = 4;
                    break;
                case "1/200":
                    viewModel.TVIndex = 5;
                    break;
            }
            viewModel.LightValue = lightValue;
           
        }

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            var dt = viewModel.Title.Replace("段修改", "");
            var targetLV = 0;
            switch (dt)
            {
                case "一":
                    targetLV = 1;
                    break;
                case "二":
                    targetLV = 2;
                    break;
                case "三":
                    targetLV = 3;
                    break;
                case "四":
                    targetLV = 4;
                    break;
                case "五":
                    targetLV = 5;
                    break;
                case "六":
                    targetLV = 6;
                    break;
                case "七":
                    targetLV = 7;
                    break;
                case "八":
                    targetLV = 8;
                    break;
                case "九":
                    targetLV = 9;
                    break;
                case "十":
                    targetLV = 10;
                    break;
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Setting.xml");//載入xml檔
            var data = xmlDoc.SelectNodes("Setting/LV");
            
            foreach (var item in data)
            {
                var LvValue = int.Parse(((XmlElement)item).GetAttribute("LV"));
                if (targetLV== LvValue)
                {
                    switch (viewModel.FlashIndex)
                    {
                        case 0:
                            ((XmlElement)item).SetAttribute("Flash", "ON");
                            break;
                        case 1:
                            ((XmlElement)item).SetAttribute("Flash", "OFF");
                            break;
                    }
                    switch (viewModel.AVIndex)
                    {
                        case 0:
                            ((XmlElement)item).SetAttribute("AV", "4.0");
                            break;
                        case 1:
                            ((XmlElement)item).SetAttribute("AV", "5.6");
                            break;
                        case 2:
                            ((XmlElement)item).SetAttribute("AV", "6.3");
                            break;
                        case 3:
                            ((XmlElement)item).SetAttribute("AV", "7.1");
                            break;
                        case 4:
                            ((XmlElement)item).SetAttribute("AV", "8.0");
                            break;
                        case 5:
                            ((XmlElement)item).SetAttribute("AV", "9.0");
                            break;
                    }
                    switch (viewModel.TVIndex)
                    {
                        case 0:
                            ((XmlElement)item).SetAttribute("TV", "1/1000");
                            break;
                        case 1:
                            ((XmlElement)item).SetAttribute("TV", "1/800");
                            break;
                        case 2:
                            ((XmlElement)item).SetAttribute("TV", "1/640");
                            break;
                        case 3:
                            ((XmlElement)item).SetAttribute("TV", "1/320");
                            break;
                        case 4:
                            ((XmlElement)item).SetAttribute("TV", "1/250");
                            break;
                        case 5:
                            ((XmlElement)item).SetAttribute("TV", "1/200");
                            break;
                    }
                    switch (viewModel.ISOIndex)
                    {
                        case 0:
                            ((XmlElement)item).SetAttribute("ISO", "ISO 100");
                            break;
                        case 1:
                            ((XmlElement)item).SetAttribute("ISO", "ISO 200");
                            break;
                        case 2:
                            ((XmlElement)item).SetAttribute("ISO", "ISO 400");
                            break;
                        case 3:
                            ((XmlElement)item).SetAttribute("ISO", "ISO 800");
                            break;
                        case 4:
                            ((XmlElement)item).SetAttribute("ISO", "1600");
                            break;
                    }
                    ((XmlElement)item).SetAttribute("Value", viewModel.LightValue);
                }
            }
            xmlDoc.Save("Setting.xml");
            this.Close();
        }

        private void BtCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region ViewModel
        public class SettingDetailViewModel : ViewModel.ViewModelBase
        {
            #region Declarations
            private string _Title = string.Empty;
            private string _LightValue = string.Empty;
            private string _AV = string.Empty;
            private string _Flash = string.Empty;
            private string _TV = string.Empty;
            private string _ISO = string.Empty;
            private int _TVIndex = 0;
            private int _ISOIndex = 0;
            private int _AVIndex = 0;
            private int _FlashIndex = 0;

            #endregion

            #region Property
            public string Title
            {
                get
                {
                    return _Title;
                }
                set
                {
                    _Title = value;
                    OnPropertyChanged();
                }
            }

            public string LightValue
            {
                get
                {
                    return _LightValue;
                }
                set
                {
                    _LightValue = value;
                    OnPropertyChanged();
                }
            }

            public string TV
            {
                get
                {
                    return _TV;
                }
                set
                {
                    _TV = value;
                    OnPropertyChanged();
                }
            }

            public int TVIndex
            {
                get
                {
                    return _TVIndex;
                }
                set
                {
                    _TVIndex = value;
                    OnPropertyChanged();
                }
            }

            public string ISO
            {
                get
                {
                    return _ISO;
                }
                set
                {
                    _ISO = value;
                    OnPropertyChanged();
                }
            }

            public int ISOIndex
            {
                get
                {
                    return _ISOIndex;
                }
                set
                {
                    _ISOIndex = value;
                    OnPropertyChanged();
                }
            }

            public string AV
            {
                get
                {
                    return _AV;
                }
                set
                {
                    _AV = value;
                    OnPropertyChanged();
                }
            }

            public int AVIndex
            {
                get
                {
                    return _AVIndex;
                }
                set
                {
                    _AVIndex = value;
                    OnPropertyChanged();
                }
            }

            public string Flash
            {
                get
                {
                    return _Flash;
                }
                set
                {
                    _Flash = value;
                    OnPropertyChanged();
                }
            }

            public int FlashIndex
            {
                get
                {
                    return _FlashIndex;
                }
                set
                {
                    _FlashIndex = value;
                    OnPropertyChanged();
                }
            }
            #endregion
        }
        #endregion
    }
}
