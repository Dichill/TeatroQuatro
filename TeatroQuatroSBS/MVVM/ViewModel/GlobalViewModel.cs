using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using TeatroQuatroSBS.Core;
using TeatroQuatroSBS.MVVM.Model;

namespace TeatroQuatroSBS.MVVM.ViewModel
{
    public class GlobalViewModel : ObservableObject
    {
        public static GlobalViewModel Instance { get; } = new GlobalViewModel();
        MediaPlayer mediaPlayer = MediaSingletonViewModel.Instance.MediaPlayerInstance;
        private bool _isPaused;

        public bool isPaused
        {
            get { return _isPaused; }
            set { _isPaused = value; OnPropertyChanged(); }
        }


        private bool _isPlaying;

        public bool isPlaying
        {
            get { return _isPlaying; }
            set { _isPlaying = value; OnPropertyChanged(); }
        }

        private double _progressValue;

        public double progressValue
        {
            get { return _progressValue; }
            set { _progressValue = value; OnPropertyChanged();
                mediaPlayer.Position = (System.TimeSpan.FromSeconds(progressValue));
                
            }
        }

        private double _progressMaximum;

        public double progressMaximum
        {
            get { return _progressMaximum; }
            set { _progressMaximum = value; OnPropertyChanged(); }
        }

        private ObservableCollection<MusicModel>? _currentSceneMusicCollection;

        public ObservableCollection<MusicModel>? currentSceneMusicCollection
        {
            get { return _currentSceneMusicCollection; }
            set { _currentSceneMusicCollection = value; OnPropertyChanged(); }
        }
    }
}
