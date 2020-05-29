using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SessionDownloader.Utils
{
    public class FileSystem
    {
        private static List<char> invalidFileNameList = new List<char>() { '\\', '/', ':', '*', '?', '"', '<', '>', '|', '\n', '.' };

        public static string ScrubFileName(string inputFileName)
        {
            var outputFileName = inputFileName;

            invalidFileNameList.ForEach(x =>
            {
                outputFileName = outputFileName.Replace(x, ' ');
            });

            return outputFileName;
        }


    }
}
