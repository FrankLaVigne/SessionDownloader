using System;
using System.Collections.Generic;
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
                    wc.DownloadFile(remoteUrl, destinationFilename);
                    Console.WriteLine($"\t Download completed at {DateTime.Now}");

                    //long length = new System.IO.FileInfo(destinationFilename).Length;
                    //if (length == 0)
                    //{
                    //    WriteWarning("File Size is 0 bytes. :( ");
                    //}
                }
                catch (Exception)
                {
                    //WriteWarning($"Unable to download file at {remoteUri} to {destinationFilename}");
                }


            }


        }

        


    }
}
