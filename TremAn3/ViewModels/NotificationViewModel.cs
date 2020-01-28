using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TremAn3.ViewModels
{
  public  class NotificationViewModel:ViewModelBase
    {

      
        public event Action<string> NotificationHandler;

        internal void SimpleNotification(string v)
        {
            NotificationHandler.Invoke(v);
        }
    }
}
