using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.ModelViews
{
    public class UserChatViewModel : INotifyPropertyChanged
    {
        public static  UserChatViewModel Instance { get; } = new UserChatViewModel();
        private UserChatViewModel() { } 
        public event PropertyChangedEventHandler? PropertyChanged;


    }
}
