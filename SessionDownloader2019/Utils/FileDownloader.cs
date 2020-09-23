using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SessionDownloader.Utils
{
    /// <summary>
    /// Class to manage file downloading
    /// </summary>
    public class FileDownloader
    {
        public void DownloadFile(string remoteUrl, string destinationFilename, bool checkExistingFile = true)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    Console.WriteLine($"\t Downloading {remoteUrl}");
                    wc.DownloadFile(remoteUrl, destinationFilename);
                    Console.WriteLine($"\t Download completed at {DateTime.Now}");

                    long length = new FileInfo(destinationFilename).Length;
                    if (length == 22)
                    {
                        Console.WriteLine($"\t File invalid. Deleting.");
                        File.Delete(destinationFilename);
                    }
                }
                catch (Exception)
                {
                    //WriteWarning($"Unable to download file at {remoteUri} to {destinationFilename}");
                }


            }


        }

        


    }
}
