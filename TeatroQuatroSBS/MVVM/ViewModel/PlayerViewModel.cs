using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeatroQuatroSBS.Core;

namespace TeatroQuatroSBS.MVVM.ViewModel
{
    public class PlayerViewModel : ObservableObject
    {
        public GlobalViewModel GlobalViewModel { get; } = GlobalViewModel.Instance;
        public RelayCommand valueChangedCommand { get; set; }

        public PlayerViewModel()
        {
            valueChangedCommand = new RelayCommand(o =>
            {
                
            });
        }
    }
}
