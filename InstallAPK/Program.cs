using System;
using System.IO;
using System.Linq;

namespace InstallAPK {



    public class TimeCreatedComparer : System.Collections.Generic.IComparer<string> {
        public int Compare( string x, string y ) {
            return -File.GetLastWriteTime(x).CompareTo(File.GetLastWriteTime(y));
        }
    }

    class Program {
        static void Main( string[] args ) {





            if ( !cmdCommand("where", "adb").Contains("adb") ) {
                Console.WriteLine("ADB not found");
                Console.WriteLine("You need to add the dir of adb.exe to your PATH variables");
                return;
            }

            string path = Environment.CurrentDirectory;
            string[] apks = Directory.GetFiles(path, "*.apk", SearchOption.AllDirectories);

            if ( apks.Length == 0 ) {
                Console.WriteLine("No APKs found in " + path);
                Console.ReadKey();
                return;
            }

            Array.Sort(apks, new TimeCreatedComparer());

            Console.WriteLine("Choose APK:");
            for ( int i = 0; i < apks.Length; i++ ) {
                Console.WriteLine(i + ": " + Path.GetFileName(apks[i]));
            }

            string apk = "";
            while ( apk == "" ) {
                Console.Write("Type Number:");
                string input = Console.ReadLine();
                int n;
                if ( int.TryParse(input, out n) ) {
                    if ( n >= 0 && n < apks.Length ) {
                        apk = apks[n];
                    } else {
                        Console.WriteLine("Number not in list");
                    }
                } else {
                    Console.WriteLine("Not a number, try again");
                }
            }

            bool loop = true;
            while ( loop ) {
                Console.WriteLine("Installing: " + Path.GetFileName(apk));
                Console.WriteLine(cmdCommand("adb", $"install -g -r \"{apk}\""));

                Console.Write("Install again? (y) ");
                if ( (Console.ReadLine().ToLower() != "y") ) loop = false;
            }

        }

        static string cmdCommand( string command, string arguments ) {

            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = command;
            pProcess.StartInfo.Arguments = arguments;
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;

            pProcess.Start();

            string strOutput = pProcess.StandardOutput.ReadToEnd();

            pProcess.WaitForExit();
            return strOutput;
        }
    }
}
