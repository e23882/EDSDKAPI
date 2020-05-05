using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfExample.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {

        #region Declarations
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Memberfunction
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
