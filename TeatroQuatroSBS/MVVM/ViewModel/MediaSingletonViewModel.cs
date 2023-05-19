using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TeatroQuatroSBS.MVVM.ViewModel
{
    public class MediaSingletonViewModel
    {
        private static MediaSingletonViewModel _instance;
        private MediaPlayer _mediaPlayer;

        private MediaSingletonViewModel()
        {
            // Initialize the MediaPlayer instance here
            _mediaPlayer = new MediaPlayer();
        }

        public static MediaSingletonViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MediaSingletonViewModel();
                }

                return _instance;
            }
        }

        // Add any other properties and methods you need for the GlobalViewModel class
        // ...

        // Add a property to access the MediaPlayer instance
        public MediaPlayer MediaPlayerInstance
        {
            get { return _mediaPlayer; }
        }
    }
}
