using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using NAudio.Wave;

namespace HauntedCastle
{
    class Program
    {
        static volatile bool isPaused = false;
        static volatile bool speedUp = false;

        static void TypeWrite(string text, int normalDelay = 60, int fastDelay = 10)
        {
            Thread inputThread = new Thread(HandleKeyPress);
            inputThread.Start();

            foreach (char c in text)
            {
                while (isPaused)
                {
                    Thread.Sleep(90);
                }

                Console.Write(c);
                Thread.Sleep(speedUp ? fastDelay : normalDelay);
            }

            inputThread.Abort(); 
        }

        static void HandleKeyPress()
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Spacebar)
                    {
                        isPaused = !isPaused;
                    }
                    else if (key == ConsoleKey.Enter)
                    {
                        speedUp = true;
                    }
                    else
                    {
                        speedUp = false;
                    }
                }
                Thread.Sleep(50);
            }
        }

        static void PlayMusic(string filePath)
        {
            try
            {
                using (Mp3FileReader reader = new Mp3FileReader(filePath))
                using (WaveOutEvent waveOut = new WaveOutEvent())
                {
                    waveOut.Init(reader);
                    waveOut.Play();
                    while (waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error playing music: " + ex.Message);
            }
        }

        static void Main()
        {
            string musicUrl = "https://www.fesliyanstudios.com/download-link.php?src=i&id=186";
            string filePath = "haunted.mp3";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                    byte[] audioBytes = client.GetByteArrayAsync(musicUrl).Result;
                    File.WriteAllBytes(filePath, audioBytes);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                return;
            }

            Thread musicThread = new Thread(() => PlayMusic(filePath));
            musicThread.Start();

            Console.CursorVisible = false;
            bool isWhite = false;
            int count = 8, change = 100, blink = 0;

            while (count > 0)
            {
                Console.BackgroundColor = isWhite ? ConsoleColor.Black : ConsoleColor.White;
                Console.ForegroundColor = isWhite ? ConsoleColor.White : ConsoleColor.Black;
                Console.Clear();

                Console.SetCursorPosition(61, 14);
                Console.WriteLine("** H A U N T E D   C A S T L E **");
                Console.SetCursorPosition(59, 20);
                Console.WriteLine($"  The game will start in {count} seconds...");

                Thread.Sleep(change);
                isWhite = !isWhite;
                blink++;

                if (blink >= 8)
                {
                    count--;
                    blink = 0;
                }
            }

            Console.Clear();
            Console.SetCursorPosition(58, 20);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" Welcome to the Haunted Castle Game!");
            Console.SetCursorPosition(63, 38);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("press any key to continue..");
            Console.ReadKey();
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(0, 3);
            Console.WriteLine("STORY:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0, 10);

            TypeWrite("You step into a forest that everyone is afraid of.\nThey say no one has ever walked through it and come out alive.\nThe wind blowing through the dry branches sounds like strange, whispering voices in your ears.\nYou keep walking, and suddenly the sound of rustling leaves makes your heart beat faster.\nYou feel like someone is following you, but when you turn around, no one is there.\nYour flashlight is getting weak, and the darkness is growing. Your eyes catch a cold, blue glow in the distance.\nAs you get closer, you find an old wooden cabin with a faint light coming from inside.\nYou open the door and hear slow, heavy breathing.\nInside, there's a wooden table with a large, ancient book on it.\nOn the cover, it says:\n\n\n                                                              *Secrets of the Forbidden Forest*\n\n\nYou touch the book, and a whispering voice says :\n\n\n\n                       (If you're looking for the truth, open this book... but be careful! each page holds a riddle you must solve...)");

            musicThread.Join();
        }
    }
}
