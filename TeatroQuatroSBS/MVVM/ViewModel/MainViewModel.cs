using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using TeatroQuatroSBS.Core;
using TeatroQuatroSBS.MVVM.Model;
using TeatroQuatroSBS.MVVM.View.Windows;

namespace TeatroQuatroSBS.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public GlobalViewModel GlobalViewModel { get; } = GlobalViewModel.Instance;
        MediaPlayer mediaPlayer = MediaSingletonViewModel.Instance.MediaPlayerInstance;
        MediaPlayer soundFXPlayer = SoundSFXSingletonViewModel.Instance.MediaPlayerInstance;
        private double _volume;

        public int maxAct { get; set; }

        private string _displayScene;

        public string displayScene
        {
            get { return _displayScene; }
            set { _displayScene = value; OnPropertyChanged(); }
        }


        private string _displayAct;

        public string displayAct
        {
            get { return _displayAct; }
            set { _displayAct = value; OnPropertyChanged(); }
        }


        private string _currentScene;

        public string currentScene
        {
            get { return _currentScene; }
            set { _currentScene = value; OnPropertyChanged(); displayScene = "Scene " + value.ToString(); }
        }


        private int _currentAct;

        public int CurrentAct
        {
            get { return _currentAct; }
            set { _currentAct = value; OnPropertyChanged(); displayAct = "ACT " + value.ToString(); }
        }

        public double VolumeMixer
        {
            get { return _volume; }
            set { _volume = value; OnPropertyChanged();
                mediaPlayer.Volume = value;
            }
        }

        private ObservableCollection<ScenesModel> _sceneCollection;

        public ObservableCollection<ScenesModel> sceneCollection
        {
            get { return _sceneCollection; }
            set { _sceneCollection = value; }
        }

        private ObservableCollection<MusicModel> _musicSceneListCollection;

        public ObservableCollection<MusicModel> musicSceneListCollection
        {
            get { return _musicSceneListCollection; }
            set { _musicSceneListCollection = value; OnPropertyChanged(); }
        }


        public RelayCommand? easterEggMessageCommand { get; set; }
        public RelayCommand? openFolderPathCommand { get; set; }
        public RelayCommand? openPlayerWindowCommand { get; set; }
        public RelayCommand? playMusicCommand { get; set; }
        public RelayCommand? pauseMusicCommand { get; set; }
        public RelayCommand? stopMusicCommand { get; set; }
        public RelayCommand? refreshAudioCommand { get; set; }
        public RelayCommand? switchSceneCommand { get; set; }
        public RelayCommand? nextActCommand { get; set; }
        public RelayCommand? backActCommand { get; set; }
        public RelayCommand? playFXCommand { get; set; }
        public RelayCommand? stopOngoingMusicCommand { get; set; }

        private DispatcherTimer timer;

        private void MessageBox_LeftButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            (sender as Wpf.Ui.Controls.MessageBox)?.Close();
        }

        private void MessageBox_RightButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            (sender as Wpf.Ui.Controls.MessageBox)?.Close();
        }
        public MainViewModel()
        {
            sceneCollection = new ObservableCollection<ScenesModel>();
            musicSceneListCollection = new ObservableCollection<MusicModel>();

            GlobalViewModel.isPlaying = false;
            GlobalViewModel.isPaused = false;
            _volume = 1;

            _currentAct = 1;
            _currentScene = "1";

            _displayAct = "ACT 1";
            _displayScene = "Scene 1";

            CheckAudio(CurrentAct, currentScene);

            stopOngoingMusicCommand = new RelayCommand(o =>
            {
                GlobalViewModel.isPlaying = false;
                GlobalViewModel.isPaused = false;

                soundFXPlayer.Stop();
                mediaPlayer.Stop();
            });

            playFXCommand = new RelayCommand(o =>
            {
                soundFXPlayer.Open(new Uri((string)o));
                soundFXPlayer.Play();
            });

            nextActCommand = new RelayCommand(o =>
            {
                if (_currentAct < 7)
                {
                    sceneCollection.Clear();
                    
                    currentScene = "1";
                    CurrentAct = CurrentAct + 1;

                    CheckAudio(CurrentAct, currentScene);
                }
            });

            backActCommand = new RelayCommand(o =>
            {
                if (_currentAct >= 2)
                {
                    sceneCollection.Clear();

                    currentScene = "1";
                    CurrentAct = CurrentAct - 1;

                    CheckAudio(CurrentAct, currentScene);
                }
            });

            switchSceneCommand = new RelayCommand(o =>
            {
                currentScene = (string)o;
                CheckAudio(CurrentAct, (string)o);
            });

            refreshAudioCommand = new RelayCommand(o =>
            {
                CheckAudio(CurrentAct, currentScene);
            });

            pauseMusicCommand = new RelayCommand(o =>
            {
                if (!GlobalViewModel.isPaused)
                {
                    mediaPlayer.Pause();
                    GlobalViewModel.isPaused = true;
                } else if (GlobalViewModel.isPaused)
                {
                    mediaPlayer.Play();
                    GlobalViewModel.isPaused = false;
                }

            });

            stopMusicCommand = new RelayCommand(o =>
            {
                mediaPlayer.Stop();

                GlobalViewModel.isPlaying = false;
                GlobalViewModel.isPaused = false;
            });

            playMusicCommand = new RelayCommand(o =>
            {
                GlobalViewModel.isPlaying = true;
                GlobalViewModel.isPaused = false;

                mediaPlayer.Open(new Uri((string)o));
                mediaPlayer.Play();

                // Start the timer to update the progress bar
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += timer_Tick;
                timer.Start();

                mediaPlayer.MediaOpened += mediaPlayer_MediaOpened;
            });

            openPlayerWindowCommand = new RelayCommand(o =>
            {
                PlayerWindow playerWindow = new PlayerWindow();
                playerWindow.Show();
            });

            openFolderPathCommand = new RelayCommand(o =>
            {
                Process.Start("explorer.exe", "Audio");
            });

            #region Hidden Eastergg

            easterEggMessageCommand = new RelayCommand(o =>
            {
                var messageBox = new Wpf.Ui.Controls.MessageBox();

                messageBox.ButtonLeftName = "Okay";
                messageBox.ButtonRightName = "Close";

                messageBox.ButtonLeftClick += MessageBox_LeftButtonClick;
                messageBox.ButtonRightClick += MessageBox_RightButtonClick;

                messageBox.Show("Teatro Quatro", "Software Developer: Dichill Tomarong\nSupport and Inspiration: Gabby\n\n2022 - 2023 SJ04");
            });
            #endregion
        }

        private void mediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            // Set the maximum value of the progress bar to the duration of the media
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                TimeSpan duration = mediaPlayer.NaturalDuration.TimeSpan;
                GlobalViewModel.progressMaximum = duration.TotalSeconds;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            // Update the progress bar based on the current position of the media
            if (mediaPlayer.NaturalDuration.HasTimeSpan && mediaPlayer.Position < mediaPlayer.NaturalDuration)
            {
                GlobalViewModel.progressValue = mediaPlayer.Position.TotalSeconds;
            }
            else
            {
                // Stop the timer when the media has finished playing
                timer.Stop();
            }
        }

        private void CheckAudio(int act, string scene)
        {
            musicSceneListCollection.Clear();

            if (Directory.Exists("Audio"))
            {
                string audioPath = "Audio/Act " + act.ToString();
                string[] sceneDirectories = Directory.GetDirectories(audioPath);
                string sceneDirectory = audioPath + "/Scene " + scene + "/" ;
                string[] audioFiles = Directory.GetFiles(sceneDirectory, "*.mp3");

                if (sceneCollection.Count == 0)
                {
                    foreach (string scdir in sceneDirectories)
                    {
                        string audioScene = scdir.Split("\\")[1];

                        string scene_num = audioScene.Split(" ")[1];
                        
                        sceneCollection.Add(new ScenesModel { scene = audioScene, scene_num = scene_num });
                    }
                }

                foreach (string audioFile in audioFiles)
                {
                    string[] audioTitle = Path.GetFileNameWithoutExtension(audioFile).Split('_');

                    musicSceneListCollection.Add(new MusicModel { title = audioTitle[0] + " - " + audioTitle[1], directory = Path.GetFullPath(audioFile), scene = displayScene });
                }
            }            
        }
    }
}
