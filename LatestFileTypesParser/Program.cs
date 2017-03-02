using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using BatchFramework;

// batchFileProcessor
namespace LatestFileTypesParser
{
    class Program
    {
        static long sTotaTextFilesLength = 0;
        static FileInfo latestFile = null;

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
            string directoryPath;
            string[] fileTypes;

            fileTypes = ChooseFileTypes();//.Split(' ');

            directoryPath = ChooseDirectory();

            FileAccessLogic logic = new FileAccessLogic();

            logic.Recursive = true;
            logic.Verbose = true;
            logic.FilePattern = "*.*";
            logic.OnProcess += new FileAccessProcessEventHandler(OnProcessSimpleList);
            logic.OnNotify += new FileAccessNotifyEventHandler(OnNotify);

            Console.WriteLine("");
            Console.WriteLine("Processing file or folder " + directoryPath);
            Console.WriteLine("Press any key to start:");

            Console.ReadKey();

            Console.WriteLine("");
            Console.WriteLine("******************************");
            Console.WriteLine("");

            foreach (string extension in fileTypes)
            {
                logic.FilePattern = "*." + extension;

                logic.Execute(directoryPath);

                if (latestFile != null && logic.Verbose)
                {
                    AccumulateLength(logic, latestFile);

                    Console.WriteLine("Full Path: {0},", latestFile.FullName);
                    Console.WriteLine("Size: {0}, Last Modified: {1}", latestFile.Length, latestFile.LastWriteTime);
                    Console.WriteLine();
                }

                latestFile = null;
            }

            Console.WriteLine("******************************");
            Console.WriteLine("");
            Console.WriteLine("Total length of latest files is {0}", sTotaTextFilesLength);

            Console.ReadKey();
        }

        private static void OnProcessSimpleList(object sender, ProcessEventArgs e)
        {
            if (e.Logic.Cancelled)
            {
                return;
            }

            e.Logic.Notify(string.Format("Listing {0}", e.FileInfo.Name));

            //AccumulateLength(e.Logic, e.FileInfo);

            // update the latest file
            FileInfo fi = e.FileInfo;

            if (latestFile == null)
            {
                latestFile = fi;
            }
            else if (fi.LastWriteTime < latestFile.LastWriteTime)
            {
                latestFile = fi;
            }
        }

        private static void OnNotify(object sender, NotifyEventArgs e)
        {
            //Console.WriteLine(e.Message);
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

        private static string[] ChooseFileTypes()
        {
            bool validChoice = false;
            string[] typeChoices = null;

            while (validChoice == false)
            {
                Console.WriteLine("Please, specify the file types that you are interested in:");

                typeChoices = Console.ReadLine().Trim().Replace("\"", "").Split(' ');

                if (typeChoices.Length == 1 && typeChoices[0] == "")
                {
                    typeChoices[0] = "*";
                    validChoice = true;
                }
                // checks if all types are unique
                else if (typeChoices.Distinct<string>().Count() == typeChoices.Length)
                {
                    validChoice = true;
                }
                else
                {
                    Console.WriteLine("{0} {1}", typeChoices.Distinct<string>().Count(), typeChoices.Length);
                    Console.WriteLine("Repeated types. Try again!");
                    Console.WriteLine("");
                }
            }

            return typeChoices;
        }
    }
}
