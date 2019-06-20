using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace SessionDownloader2019
{
    class Program
    {
        private static char[] invalidFilenameChars = { '\\', '/', ':', '*', '?', '"', '<', '>', '|', '\n', '.' };

        public class Arguments
        {
            public string DestinationPath { get; set; }
        }

        static Arguments arguments;

        static void Main(string[] args)
        {
            arguments = ParseArgs(args);

            if (arguments == null)
            {
                return;
            }

            WebClient webClient = new WebClient();
            string sourceData = "https://api.mybuild.techcommunity.microsoft.com/api/session/all";

            Console.WriteLine("Starting Download");
            string allThatJson = webClient.DownloadString(sourceData);
            Console.WriteLine("Download Complete");

            dynamic sessions = JArray.Parse(allThatJson);

            Console.WriteLine($"{sessions.Count} talks found.");

            DownloadSessions(sessions);




            Console.WriteLine("Finished!");



            Console.ReadLine();

        }

        private static Arguments ParseArgs(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Please enter a destination path!");
                return null;
            }
            var destinationPath = args[0];

            return new Arguments()
            {
                DestinationPath = destinationPath
            };
        }

        private static void DownloadSessions(dynamic sessions)
        {
            foreach (var session in sessions)
            {

                Console.WriteLine("*****************************");
                Console.WriteLine($"Session: {session.title}");

                if (session.slideDeck != string.Empty)
                {
                    Console.WriteLine("Slide deck available.");

                    string remoteUri = session.slideDeck.ToString();

                    string scrubbedSessionTitle = ScrubSessionTitle(session.title.ToString());
                    string destinationFilename = $"{arguments.DestinationPath}{scrubbedSessionTitle}.pptx";

                    if (File.Exists(destinationFilename) == true)
                    {
                        Console.WriteLine("File exists. Skipping");
                    }
                    else
                    {
                        DownloadFile(remoteUri, destinationFilename);
                    }


                    Console.WriteLine($"{destinationFilename}");
                }

                if (session.downloadVideoLink != string.Empty)
                {
                    Console.WriteLine("Video available.");

                    string remoteUri = session.downloadVideoLink.ToString();

                    string scrubbedSessionTitle = ScrubSessionTitle(session.title.ToString());
                    string destinationFilename = $"{arguments.DestinationPath}{scrubbedSessionTitle}.mp4";

                    if (File.Exists(destinationFilename) == true)
                    {
                        Console.WriteLine("File exists. Skipping");
                    }
                    else
                    {
                        DownloadFile(remoteUri, destinationFilename);
                    }
                }



            }

        }

        private static void DownloadFile(string remoteUri, string destinationFilename)
        {
            Console.WriteLine($"\t Download started at {DateTime.Now}");

            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile(remoteUri, destinationFilename);

                Console.WriteLine($"\t Download completed at {DateTime.Now}");
            }
        }

        private static string ScrubSessionTitle(string sessionTitle)
        {
            var scrubbedString = sessionTitle;

            invalidFilenameChars.ToList().ForEach(x =>
            {
                scrubbedString = scrubbedString.Replace(x, ' ');
            });

            return scrubbedString;
        }
    }
}
