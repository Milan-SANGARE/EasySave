﻿using System;
using System.IO;
using EasySave.NS_Model;
using EasySave.NS_ViewModel;

namespace EasySave.NS_View
{
    class View
    {
        // --- Attributes ---
        private ViewModel viewModel;
        public int Language = 1;


        // --- Constructor ---
        public View(ViewModel _viewModel)
        {
            this.viewModel = _viewModel;
        }


        // --- Methods ---
        //Display menu

        public int Menu()
        {
            ViewModel obj = new ViewModel();
            if (Language == 1)
            {
                Console.Clear();
                Console.WriteLine(
                "Menu:" +
                "\n1 - Afficher tous les fichiers" +
                "\n2 - Ajouter un fichier" +
                "\n3 - Faire une sauvegarde" +
                "\n4 - Supprimer un fichier" +
                "\n5 - Quitter" +
                "\n6 - Changer le langage");
            }

            else if (Language == 0)
            {
                Console.Clear();
                Console.WriteLine(
                "Menu:" +
                "\n1 - Show all works" +
                "\n2 - Add a work" +
                "\n3 - Make a backup" +
                "\n4 - Remove a work" +
                "\n5 - Quit" +
                "\n6 - ChangeLanguage");
            }
            

            return CheckChoiceMenu(Console.ReadLine(), 1, 6);
        }

        //Add work name
        public string AddWorkName()
        {
            Console.Clear();
            Console.WriteLine("Parameter to add a work:");
            ConsoleUpdate(2);

            Console.WriteLine("\nEnter a name (1 to 20 characters):");
            string name = Console.ReadLine();

            //Check if the name is valid
            while (!CheckName(name))
            {
                name = Console.ReadLine();
            }
            return name;
        }

        private string RectifyPath(string _path)
        {
            if (_path != "0" && _path.Length >= 1)
            {
                _path += (_path.EndsWith("/") || _path.EndsWith("\\")) ? "" : "\\";
                _path = _path.Replace("/", "\\");
            }
            return _path.ToLower();
        }

        //Add work source
        public string AddWorkSrc()
        {
            Console.WriteLine("\nEnter directory source. ");
            string src = RectifyPath(Console.ReadLine());

            //Check if the path is valid
            while (!Directory.Exists(src) && src != "0")
            {
                ConsoleUpdate(211);
                src = RectifyPath(Console.ReadLine());
            }
            return src;
        }

        //Add work destination
        public string AddWorkDst(string _src)
        {
            Console.WriteLine("\nEnter directory destination.");
            string dst = RectifyPath(Console.ReadLine());

            //Check if the path is valid
            while (!CheckWorkDst(_src, dst))
            {
                dst = RectifyPath(Console.ReadLine());
            }
            return dst;
        }

        private bool CheckWorkDst(string _src, string _dst)
        {
            if (_dst == "0")
            {
                return true;

            }
            else if (Directory.Exists(_dst))
            {
                if (_src != _dst)
                {
                    if (_dst.Length > _src.Length)
                    {
                        if (_src != _dst.Substring(0, _src.Length))
                        {
                            return true;
                        }
                        else
                        {
                            ConsoleUpdate(217);
                            return false;
                        }
                    }
                    return true;
                }
                ConsoleUpdate(212);
                return false;
            }
            ConsoleUpdate(213);
            return false;
        }

        //Add work backup type
        public int AddWorkBackupType()
        {
            Console.WriteLine(
                "\nChoose a type of Backup: " +
                "\n1.Full " +
                "\n2.Differential");
            return CheckChoiceMenu(Console.ReadLine(), 0, 2);
        }

        //Check if the name is valid and doesn't already exist
        private bool CheckName(string _name)
        {
            int length = _name.Length;

            if (length >= 1 && length <= 20)
            {
                if (!this.viewModel.model.works.Exists(work => work.name == _name))
                {
                    return true;
                }
                ConsoleUpdate(214);
                return false;
            }
            ConsoleUpdate(215);
            return false;
        }

        //Check if the input is an integer
        private static bool CheckInt(string _input)
        {
            try
            {
                int.Parse(_input);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Display all works 
        private void LoadWorks(int _shift)
        {
            var works = this.viewModel.model.works;

            for (int i = 0; i < works.Count; i++)
            {
                Console.WriteLine(
                    "\n" + (i + _shift) + " - " + "Name: " + works[i].name
                    + "\n    Source: " + works[i].src
                    + "\n    Destination: " + works[i].dst
                    + "\n    Type: " + works[i].backupType);
            }
        }

        public void DisplayWorks()
        {
            Console.Clear();
            Console.WriteLine("Work list :");

            //Display all works 
            LoadWorks(1);
            ConsoleUpdate(1);
        }

        //Choose the work to save
        public int LaunchBackupChoice()
        {
            Console.Clear();
            Console.WriteLine(
                "Choose the work to save : " +
                "\n\n1 - all");

            //Display all works 
            LoadWorks(2);
            ConsoleUpdate(2);

            //Check if the user's input is a valid integer
            return CheckChoiceMenu(Console.ReadLine(), 0, this.viewModel.model.works.Count + 1);
        }

        //Choose the work to remove
        public int RemoveWorkChoice()
        {
            Console.Clear();
            Console.WriteLine("Choose the work to remove :");

            //Display all works 
            LoadWorks(1);
            ConsoleUpdate(2);

            //Check if the user's input is a valid integer
            return CheckChoiceMenu(Console.ReadLine(), 0, this.viewModel.model.works.Count);
        }

        //Check if the input is a integer and in the good range
        private int CheckChoiceMenu(string _inputUser, int _minEntry, int _maxEntry)
        {
            while (!(CheckInt(_inputUser) && (Int32.Parse(_inputUser) >= _minEntry && Int32.Parse(_inputUser) <= _maxEntry)))
            {
                ConsoleUpdate(206);
                _inputUser = Console.ReadLine();
            }
            return Int32.Parse(_inputUser);
        }

        public void DisplayCurrentState(string _name, int _fileLeft, long _leftSize, long _curSize, int _pourcent)
        {
            Console.Clear();
            Console.WriteLine(
                "Current backup : " + _name
                + "\nSize of the current file : " + DiplaySize(_curSize)
                + "\nNumber of files left : " + _fileLeft
                + "\nSize of the files left : " + DiplaySize(_leftSize) + "\n");
            DisplayProgressBar(_pourcent);
        }

        public void DisplayBackupRecap(string _name, double _transferTime)
        {
            Console.WriteLine("\n\n" +
                "Backup : " + _name + " finished\n"
                + "\nTime taken : " + _transferTime + " ms\n");
            DisplayProgressBar(100);
        }

        public void DisplayFiledError(string _name)
        {
            Console.WriteLine("File named " + _name + " failed.");
        }

        private void DisplayProgressBar(int _pourcent)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("Progress: [ " + _pourcent + "%]");
            Console.ResetColor();

            Console.Write(" [");
            for (int i = 0; i < 100; i += 5)
            {
                if (_pourcent > i)
                {
                    Console.Write("#");
                }
                else
                {
                    Console.Write(".");
                }
            }
            Console.Write("]\n\n");
        }

        private string DiplaySize(long _octet)
        {
            if (_octet > 1000000000000)
            {
                return Math.Round((decimal)_octet / 1000000000000, 2) + " To";
            }
            else if (_octet > 1000000000)
            {
                return Math.Round((decimal)_octet / 1000000000, 2) + " Go";
            }
            else if (_octet > 1000000)
            {
                return Math.Round((decimal)_octet / 1000000, 2) + " Mo";
            }
            else if (_octet > 1000)
            {
                return Math.Round((decimal)_octet / 1000, 2) + " ko";
            }
            else
            {
                return _octet + " o";
            }
        }

        //Display message on the console
        public void ConsoleUpdate(int _id) 
        {
            if (_id < 100)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                switch (_id)
                {
                    //Information message
                    case 1:                       
                        Console.WriteLine("\nPress Enter key to display menu . . .");
                        Console.ReadLine();
                        break;

                    case 2:
                        Console.WriteLine("\n(Enter 0 to return to the menu)");
                        break;

                    case 3:
                        Console.Clear();
                        Console.WriteLine("\nBackup information :");
                        break;

                    case 4:
                        Console.WriteLine("\nPress Enter key to show more . . .");
                        Console.ReadLine();
                        break;
                }
            }
            else if (_id < 200)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                switch (_id)
                {
                    // Success message from 100 to 199
                    case 100:
                        Console.WriteLine("\n----- WELCOME ON EASYSAVE -----");
                        ConsoleUpdate(1);
                        break;

                    case 101:
                        Console.WriteLine("\nThe work was added with success!");
                        ConsoleUpdate(1);
                        break;

                    case 102:
                        Console.WriteLine("\nThe work was saved with success!");
                        break;

                    case 103:
                        Console.WriteLine("\nThe work was removed with success!");
                        ConsoleUpdate(1);
                        break;

                    case 104:
                        Console.WriteLine("\nBackup success !");
                        break;

                    case 105:
                        Console.WriteLine("\nNo modification since the last full backup!\n");
                        break;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                switch (_id)
                {
                    // Error message from 200 to 299
                    case 200:
                        Console.WriteLine("\nPlease restore your JSON backup file.");
                        ConsoleUpdate(1);
                        break;

                    case 201:
                        Console.WriteLine("\nFailed to add work.");
                        ConsoleUpdate(1);
                        break;

                    case 202:
                        Console.WriteLine("\nFailed to saved work.");
                        ConsoleUpdate(1);
                        break;

                    case 203:
                        Console.WriteLine("\nFailed to removed work.");
                        ConsoleUpdate(1);
                        break;

                    case 204:
                        Console.WriteLine("\nWork List is empty.");
                        ConsoleUpdate(1);
                        break;

                    case 205:
                        Console.WriteLine("\nWork List is full.");
                        ConsoleUpdate(1);
                        break;

                    case 206:
                        Console.WriteLine("\nPlease enter a valid option");
                        break;

                    case 207:
                        Console.WriteLine("\nFailed to move a file, destination or source file do not exists.");
                        break;

                    case 208:
                        Console.WriteLine("\nSelected backup type doesn't exists.");
                        break;

                    case 209:
                        Console.WriteLine("\nFailed to copy file.");
                        ConsoleUpdate(1);
                        break;

                    case 210:
                        Console.WriteLine("\nFailed to create the backup folder.");
                        ConsoleUpdate(1);
                        break;
                    case 211:
                        Console.WriteLine("\nDirectory doesn't exist. Please enter a valid directory source. ");
                        break;

                    case 212:
                        Console.WriteLine("\nChoose a different path from the source. ");
                        break;

                    case 213:
                        Console.WriteLine("\nDirectory doesn't exist. Please enter a valid directory direction. ");
                        break;

                    case 214:
                        Console.WriteLine("\nWorkName already taken. Please enter an other name.");
                        break;

                    case 215:
                        Console.WriteLine("\nEnter a VALID name (1 to 20 characters):");
                        break;

                    case 216:
                        Console.WriteLine("\nBackup finished with error.");
                        break;

                    case 217:
                        Console.WriteLine("\nDestination directory cannot be inside the source directory.");
                        break;

                    default:
                        Console.WriteLine("\nFailed : Error Unknown.");
                        ConsoleUpdate(1);
                        break;
                }
            }
            Console.ResetColor();
        }
    }
}
