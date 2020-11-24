using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CryptoSolver.Properties;


//Cmdline arguments to check:
//How many chars
//Include numbers
namespace CryptoSolver
{
    class Program
    {
        static void Main(string[] args)
        {

            Menu menu = new Menu();
            menu.OpenMenu();
            //test
        }

    }

    class Menu
    {
        public void OpenMenu()
        {
            Console.Clear();
            
            //Logo & Console markup
            string intro =  @"  _____              __         ____     __" + "\n" +
                            @" / ___/_____ _____  / /____    / __/__  / /  _____ ____" + "\n" +
                            @"/ /__/ __/ // / _ \/ __/ _ \  _\ \/ _ \/ / |/ / -_) __/" + "\n" +
                            @"\___/_/  \_, / .__/\__/\___/ /___/\___/_/|___/\__/_/" + "\n" +
                            @"        /___/_/";

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WindowHeight = 60;
            Console.WindowWidth = 60;
            Console.WriteLine(intro.PadLeft(5));
            Console.ForegroundColor = ConsoleColor.White;
            
            //Intro
            Console.WriteLine("\nWhat Cryptogram do you want to solve?\n");
            Console.WriteLine("Formatting: B--N -> BEAN, BORN, BEEN etc.\n");
            
            //Input by user
            string UserInput = Console.ReadLine();
            
            //Load files
            Sol sol = new Sol();
            sol.RCheck(sol.RWordFinder(UserInput), sol.Solve(UserInput.Length, "/Resources/index_list"));

            //Restart
            Console.WriteLine("\nSearch a new solution? (y/n)\n");
            UserInput = Console.ReadLine();
            if (UserInput == "y" || UserInput == "Y")
            {
                OpenMenu();
            }
        }

    }
    class Sol
    {
        //Read a dictonary and organize into a list depending on letters in user input
        public List<string> Solve(int stringlength, string filepath)
        {
            List<string> CandList = new List<string>();
            foreach (var item in File.ReadLines(filepath))
            {
                if (item.StartsWith(stringlength.ToString()))
                {
                    CandList.Add(item.Split(";").Last<string>());
                }
            }

            return CandList;
        }

        //Create a regex
        public string RWordFinder(string input)
        {
            int DashCounter = 0;
            string RWord = "^"; //Beginning of word
            
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] != '-')
                {
                    if (DashCounter > 0) //Before placing a letter, check if there are no placeholders waiting and add them in \w{x}
                    {
                        RWord += "\\w{" + DashCounter + "}";
                    }
                    RWord += input[i]; //Add the letter
                    DashCounter = 0;
                }
                else
                {
                    DashCounter += 1;

                }
            }

            if (DashCounter>0) //Add the word end $, but also all final placeholders
            {
                RWord += "\\w{"+DashCounter+"}$";
            }
            else
            {
                RWord += "$";
            }
            

            return RWord;
        }

        //Run the regex over the possible candidates for a match
        public void RCheck(string RWord, List<string> CandList)
        {
            
            Console.WriteLine("\nResults:\n");
            int i = 1;
            Regex rgx = new Regex(RWord);
            foreach (var item in CandList)
            {
                if (rgx.IsMatch(item))
                {
                    
                    Console.WriteLine(" [{0}] {1}",i,item);
                    i++;
                }
               
            }
           
            Console.WriteLine("\nFound {0} matches out of {1} records.", i-1, CandList.Count);
        }
    }

    //Helper program to extract dictionary from Wikipedia
    class Extractor
    {
        public void Extract(string filepath)
        {
            List<string> titles = new List<string>();
            string cap;
            XmlReader reader = XmlReader.Create(filepath);

            while (reader.Read())
            {
                reader.ReadToFollowing("title");
                {
                    cap = reader.ReadString();
                    bool containsInt = cap.Any(char.IsDigit); //Check if there are any numbers in the string
                    if (cap.Contains(":") || containsInt || cap.Length > 20 || cap.Length < 3) //Remove Users:, Categories:, long words or short
                    {
                        continue;
                    }
                    else
                    {
                        titles.Add(cap.Length + ";" + cap);
                    }

                }
            }
            titles.Sort();
            System.IO.File.WriteAllLines("SavedLists_nl_indexed.txt", titles);
        }


        public void Index(string filepath)
        {
            List<string> titles = new List<string>();
            string[] capArray =  System.IO.File.ReadAllText(filepath).Split("\r\n");


            for (int i = 0; i < capArray.Length; i++)
            {
                Console.SetCursorPosition(0, 0);
                capArray[i] = capArray[i].Length + ";" + capArray[i];
                Console.WriteLine(i);
            }

            System.IO.File.WriteAllLines("SavedLists_nl_indexed_opentaal.txt", capArray);
            Console.ReadLine();

        }
    }
}
