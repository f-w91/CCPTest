using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CCP.DTO;
using CCP.Enums;
using CCP.Helpers;

namespace CCP
{
    //Some comments
    //Its a bit buggy, there's a few ways you can break it
    //It should take any csv and convert into any xml, json, vice versa
    //It can only go one layer deep... I think
    //I wish I had more time to finish this off but I think my missus is getting annoyed at me!
    //Time spent.... maybe 5 hours? But it's been pretty broken up so maybe more like 4. Enjoy!

    public class Program
    {
        static void Main(string[] args)
        {
            new ETL().Start();
        }
    }

    public class ETL
    {
        public ImportType ImportType { get; private set; }
        public ImportType ExportType { get; private set; }

        public string ImportFilePath { get; set; }
        public string ExportFilePath { get; set; }

        public IEnumerable<ImportType> AvailableTypes =>
            Enum.GetValues(typeof(ImportType))
            .Cast<ImportType>()
            .Where(x => x != ImportType && x != ImportType.None);

        public void Start()
        {
            if (Entry())
            {
                ChooseExportType();
                ChooseOutputFileName();
                Execute();
            }
        }

        public void Execute()
        {
            CCPDataSet set = new CCPDataSet();

            switch (ImportType)
            {
                case ImportType.Csv:
                    set = CsvUtils.Read(ImportFilePath);
                    break;
                case ImportType.Xml:
                    set = XmlUtils.Read(ImportFilePath);
                    break;
                case ImportType.Json:
                    set = JsonUtils.Read(ImportFilePath);
                    break;
            }

            switch (ExportType)
            {
                case ImportType.Csv:
                    CsvUtils.Convert(set, ExportFilePath);
                    break;
                case ImportType.Xml:
                    XmlUtils.Convert(set, ExportFilePath);
                    break;
                case ImportType.Json:
                    JsonUtils.Convert(set, ExportFilePath);
                    break;
            }
        }
        
        public void ChooseOutputFileName()
        {
            Console.Clear();
            Console.WriteLine("Please enter a filename for the Export.");

            var fileName = Console.ReadLine();

            var ext = Path.GetExtension(fileName);
            string expectedExtension = ExportType.GetExtension();

            if (ext == string.Empty)
                fileName += expectedExtension;

            var path = Path.GetDirectoryName(this.ImportFilePath);

            bool exportToFilePath = Confirm($"Save new file to {path}?");

            if (exportToFilePath)
            {
                ExportFilePath = Path.Combine(path, fileName);
            }
            else
            {
                Console.WriteLine("Enter a new path:");
                var newPath = Console.ReadLine();

                ExportFilePath = Path.Combine(newPath, fileName);
            }
        }

        public void ChooseExportType()
        {
            bool validChoice = false;
            bool firstPass = true;

            while (!validChoice)
            {
                Console.Clear();
                Console.WriteLine($"Please select a file format to convert your {ImportType.ToString()} to:");

                foreach(var type in AvailableTypes)
                {
                    Console.WriteLine($"{(int)type}: {type.ToString()}");
                }

                if (!firstPass)
                    Console.WriteLine("Invalid selection");

                Console.WriteLine();

                var choice = Console.ReadKey();

                CheckForEscapeOption(choice);

                if(Enum.TryParse<ImportType>(choice.KeyChar.ToString(), out ImportType selectedType))
                {
                    ExportType = selectedType;
                    validChoice = true;
                }
            }
        }

        public bool Entry()
        {
            //Used to indicate whether we have a valid choice and can proceed or not
            bool validChoice = false;
            //If we pass through again, the user must've done something wrong, use this to provide additional context or help
            bool firstPass = true;

            string previousFilePath = "";

            while (!validChoice)
            {
                Console.Clear();
                Console.WriteLine("Welcome to Felix's magical technical test, we hope to take you on a journey where I obsess way too long over this.");
                if (!firstPass)
                    Console.WriteLine($"The file path '{previousFilePath}' was either incomplete, not found, not a valid type, or access was denied.");
                firstPass = false;
                Console.WriteLine("Please enter a filepath, including the extension of the file. Press enter when you are done.");


                var filePath = Console.ReadLine();
                previousFilePath = filePath;

                if (File.Exists(filePath))
                {
                    var extension = Path.GetExtension(filePath);

                    switch (extension)
                    {
                        case ".csv":
                            ImportType = ImportType.Csv;
                            break;
                        case ".xml":
                            ImportType = ImportType.Xml;
                            break;
                        case ".json":
                            ImportType = ImportType.Json;
                            break;
                        default:
                            ImportType = ImportType.None;
                            break;
                    }

                    if (ImportType != ImportType.None)
                    {
                        if (Confirm($"This file looks to be a {extension} type. Proceed?"))
                        {
                            ImportFilePath = filePath;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        //Generic method to get yes/no results from simple decisions
        public static bool Confirm(string message)
        {
            Console.Clear();
            Console.WriteLine($"{message} Y/N");
            bool validChoice = false;

            while (!validChoice)
            {
                var choice = Console.ReadKey();

                if (char.ToLower(choice.KeyChar) == 'y')
                {
                    return true;
                }
                else if (char.ToLower(choice.KeyChar) == 'n')
                {
                    return false;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid entry!");
                    Console.WriteLine($"{message} Y/N");
                }
            }

            return false;
        }

        //Extension for ConsoleKeyInfo on any reads we do to escape at any time
        public static void CheckForEscapeOption(ConsoleKeyInfo choice)
        {
            if (choice.Key == ConsoleKey.Escape)
            {
                if (Confirm("Close?"))
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}
