using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace PR18.Services
{
    public static class Mp3Player // Делаем класс статическим для удобства
    {
        private static MediaPlayer _player = new MediaPlayer();
        private static bool _isInitialized = false;

        public static void PlayMp3FromProject(string mp3FileName)
        {
            if (string.IsNullOrEmpty(mp3FileName)) return;

            try
            {
                if (!_isInitialized)
                {
                    _player.MediaOpened += (s, e) => _player.Play();
                    _isInitialized = true;
                }

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                string projectDir = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\"));

                string fullPath = Path.Combine(projectDir, "Data", "Sounds", mp3FileName);

                if (!File.Exists(fullPath))
                {
                    projectDir = Path.GetFullPath(Path.Combine(baseDir, @"..\..\"));
                    fullPath = Path.Combine(projectDir, "Data", "Sounds", mp3FileName);
                }

                if (File.Exists(fullPath))
                {
                    _player.Stop();

                    string formattedPath = "file:///" + fullPath.Replace('\\', '/');
                    _player.Open(new Uri(formattedPath, UriKind.Absolute));
                }
                else
                {
                    MessageBox.Show($"Файл звука не найден по пути:\n{fullPath}", "Ошибка поиска файла");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка воспроизведения звука: " + ex.Message, "Ошибка");
            }
        }


        public static void Stop()
        {
            _player.Stop();
        }
    }
}
