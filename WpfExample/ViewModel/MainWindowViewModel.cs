﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExample.ViewModel;

namespace ViewModel
{
    public class MainWindowViewModel:ViewModelBase
    {

        #region Declarations
        private string _Shutter = "1/1000";
        private string _ISO = "ISO 100";
        private string _Aperture = "4.0";
        private string _Flash = "ON";
        private string _Light = "255";
        private int _Frequency = 180;

        #endregion

        #region Property
        /// <summary>
        /// 主畫面顯示目前相機快門值
        /// </summary>
        public string Shutter
        {
            get
            {
                return _Shutter;
            }
            set
            {
                _Shutter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 主畫面顯示目前增益值
        /// </summary>
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

        /// <summary>
        /// 主畫面顯示目前光圈值
        /// </summary>
        public string Aperture
        {
            get
            {
                return _Aperture;
            }
            set
            {
                _Aperture = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 主畫面顯示目前relay閃光燈是否啟動
        /// </summary>
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

        /// <summary>
        /// 目前測光電路測光值
        /// </summary>
        public string Light
        {
            get
            {
                return _Light;
            }
            set
            {
                _Light = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 測光頻率
        /// </summary>
        public int Frequency
        {
            get
            {
                return _Frequency;
            }
            set
            {
                _Frequency = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region DataModel
        #endregion

        #region Memberfunction
        public MainWindowViewModel()
        {

        }

        #endregion

        #region Converter
        #endregion


    }
}
