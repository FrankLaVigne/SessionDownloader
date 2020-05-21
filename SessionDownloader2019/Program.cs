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

        private static ConsoleColor defaultForegroundConsoleColor = Console.ForegroundColor;
        private static ConsoleColor defaultBackgroundConsoleColor = Console.BackgroundColor;

        enum MessageLevel
        {
            Normal,
            Highlight,
            Warning,
            Error
        }

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

            string allThatJson = DownloadSessionMetaData();

            dynamic sessions = JArray.Parse(allThatJson);

            Console.WriteLine($"{sessions.Count} talks found.");

            DownloadSessions(sessions);

            Console.WriteLine($"Finished at {DateTime.Now}");

            Console.ReadLine();

        }

        private static string DownloadSessionMetaData()
        {
            WebClient webClient = new WebClient();

            // TODO: find a more robust way to do this

            // Build 2019
            string build2019 = "https://api.mybuild.techcommunity.microsoft.com/api/session/all";
            string build2020 = "https://api.mybuild.microsoft.com/api/session/all";

            // Ignite 2019 
            string ignite2019 = "https://api-myignite.techcommunity.microsoft.com/api/session/all";

            // Build 2020
            //https://medius.studios.ms/video/asset/HIGHMP4/B20-INT152A

            string sourceData = build2020;

            WriteHighlight("Starting Metadata Download");
            string allThatJson = webClient.DownloadString(sourceData);
            WriteHighlight("Metadata Download Complete");
            return allThatJson;
        }

        private static Arguments ParseArgs(string[] args)
        {
            if (args.Length < 1)
            {
                WriteError("Please enter a destination path!");
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

                DownloadSlides(session);

                DownloadVideo(session);

            }
        }

        private static void DownloadVideo(dynamic session)
        {

            string downloadUrl = GetDownloadUrl(session);

            if (downloadUrl != string.Empty)
            {
                Console.WriteLine("Video available.");

                string remoteUri = downloadUrl;

                string scrubbedSessionTitle = ScrubSessionTitle(session.title.ToString());
                string destinationFilename = $"{arguments.DestinationPath}{session.sessionCode}-{scrubbedSessionTitle}.mp4";

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

        private static string GetDownloadUrl(dynamic session)
        {
            string downloadUrl = string.Empty;

            if (session.downloadVideoLink != string.Empty && session.downloadVideoLink != null)
            {
                downloadUrl = session.downloadVideoLink;
            }
            else
            {
                

                Console.WriteLine(session.onDemand);


                string sessionCode = session.sessionCode;

                if(session.onDemand == string.Empty || session.onDemand == null)
                {
                    downloadUrl = string.Empty;
                }
                else
                {
                    downloadUrl = $"https://medius.studios.ms/video/asset/HIGHMP4/B20-{sessionCode}";

                }

                //https://medius.studios.ms/Embed/video-nc/B20-BDL111
                //https://medius.studios.ms/video/asset/HIGHMP4/B20-BDL101


                //downloadUrl = session.onDemand;
            }

            return downloadUrl;

        }

        private static void DownloadSlides(dynamic session)
        {
            if (session.slideDeck != string.Empty)
            {
                Console.WriteLine("Slide deck available.");

                string remoteUri = session.slideDeck.ToString();

                string scrubbedSessionTitle = ScrubSessionTitle(session.title.ToString());
                string destinationFilename = $"{arguments.DestinationPath}{scrubbedSessionTitle}.pptx";

                if (File.Exists(destinationFilename) == true)
                {
                    WriteWarning("File exists. Skipping");
                }
                else
                {
                    try
                    {
                        DownloadFile(remoteUri, destinationFilename);
                    }
                    catch (Exception exception)
                    {
                        WriteError($"Error downloading {remoteUri} to {destinationFilename}");
                    }

                }

                Console.WriteLine($"{destinationFilename}");
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

        private static void ResetConsoleColors()
        {
            Console.ForegroundColor = defaultForegroundConsoleColor;
            Console.BackgroundColor = defaultBackgroundConsoleColor;
        }

        private static void WriteError(string message)
        {
            WriteMessage(message, MessageLevel.Error);
        }
        private static void WriteWarning(string message)
        {
            WriteMessage(message, MessageLevel.Warning);
        }
        private static void WriteHighlight(string message)
        {
            WriteMessage(message, MessageLevel.Highlight);
        }


        private static void WriteMessage(string message, MessageLevel messageLevel)
        {
            switch (messageLevel)
            {
                case MessageLevel.Normal:
                    break;
                case MessageLevel.Highlight:
                    InvertConsoleColors();
                    break;
                case MessageLevel.Warning:
                    WarningConsoleColors();
                    break;
                case MessageLevel.Error:
                    ErrorConsoleColors();
                    break;
                default:
                    break;
            }

            Console.WriteLine(message);

            ResetConsoleColors();


        }

        private static void InvertConsoleColors()
        {
            Console.ForegroundColor = defaultBackgroundConsoleColor;
            Console.BackgroundColor = defaultForegroundConsoleColor;
        }

        private static void ErrorConsoleColors()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = defaultBackgroundConsoleColor;
        }

        private static void WarningConsoleColors()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = defaultBackgroundConsoleColor;
        }

    }
}
