using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace WpfApp1
{
    class CommonOpenFilePicker
    {
        public List<string> musicFiles = new List<string>();

        private bool IsSupportedAudioFile(string extension)
        {
            var supportedExtensions = new List<string> { ".mp3", ".wav", ".m4a" }; // Добавьте сюда поддерживаемые расширения файлов

            return supportedExtensions.Contains(extension);
        }

        public void SelectFolder()
        {
            
            var folderDialog = new OpenFileDialog();
            folderDialog.FileName = "Select Folder";
            folderDialog.CheckFileExists = false;
            folderDialog.CheckPathExists = true;
            folderDialog.Multiselect = true;
            folderDialog.ValidateNames = false;

            if (folderDialog.ShowDialog() == true)
            {
                musicFiles.Clear();
                foreach (var file in folderDialog.FileNames)
                {
                    var fileInfo = new FileInfo(file);
                    if (IsSupportedAudioFile(fileInfo.Extension.ToLower()))
                    {
                        musicFiles.Add(file);
                    }
                }
            }
            musicFiles.Sort();
        }
    }
}
