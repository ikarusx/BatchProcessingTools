using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using BatchFramework;

// batcgFileProcessor
namespace TotalTextFilesLength
{
    class Program
    {
        static long sTotaTextFilesLength = 0;

        static void testNotify(string msg)
        {
            Console.WriteLine(msg);
        }

        static void AccumulateLength(IFileAccessLogic lo, FileInfo fi)
        {
            sTotaTextFilesLength += fi.Length;
        }

        static void Main(string[] args)
        {
            //bool recursive = false,
            //    skip = false,
            //    forcewritable = false,


            //for (int i = 0; i < args.Length; ++i)
            //{

            //}

            string directoryPath;

            //if (args.Length < 1)
            //{
            //    //Console.WriteLine("Missing file or folder path:" +
            //    //    "\nUsage: TotalTextFilesLength filePath\\folderPath");

            //    //return;

            //    directoryPath = ChooseDirectory();
            //}
            //else
            //{
            //    directoryPath = args[0];
            //}

            //directoryPath = args[0];
            directoryPath = ChooseDirectory();

            FileAccessLogic logic = new FileAccessLogic();

            logic.Recursive = true;
            logic.Verbose = true;
            logic.FilePattern = "*.txt";
            logic.OnProcess += new FileAccessProcessEventHandler(OnProcessSimpleList);
            logic.OnNotify += new FileAccessNotifyEventHandler(OnNotify);

            Console.WriteLine("");
            Console.WriteLine("Processing file or folder " + directoryPath);
            Console.WriteLine("Press any key to start:");

            Console.ReadKey();
            
            Console.WriteLine("******************************");

            logic.Execute(directoryPath);

            Console.WriteLine("******************************");
            Console.WriteLine("");
            Console.WriteLine("Total length of all text files is {0}", sTotaTextFilesLength);

            Console.ReadKey();
        }

        private static void OnProcessSimpleList(object sender, ProcessEventArgs e)
        {
            if (e.Logic.Cancelled)
            {
                return;
            }

            e.Logic.Notify(string.Format("Listing {0}", e.FileInfo.Name));

            AccumulateLength(e.Logic, e.FileInfo);
        }

        private static void OnNotify(object sender, NotifyEventArgs e)
        {
            Console.WriteLine(e.Message);
            //testNotify(e.Message);
        }

        private static string ChooseDirectory()
        {
            bool validChoice = false;
            string directoryChoice = "";

            while (validChoice == false)
            {
                Console.WriteLine("Please, specify top level directory in full:");

                directoryChoice = Console.ReadLine().Trim().Replace("\"", "");

                if (Directory.Exists(directoryChoice))
                {
                    validChoice = true;
                }
                else
                {
                    Console.WriteLine("Invalid directory. Try again!");
                    Console.WriteLine("");
                }
            }

            return directoryChoice;
        }
    }
}
