using System;

/// <summary>
/// Namespace
/// </summary>
namespace NumberGuesser
{   // Main class
    class Program
    {   /// <summary>
    ///  Entry point
    /// </summary>
    /// <param name="args">Takes in any number of parameters</param>
        static void Main(string[] args)
        {
            GetAppInfo(); //Run GetAppInfo function for info about  the app

            GreetUser(); //Ask for  gamer's name and greet the idiot
           
            
           while (true) {  

            Random random = new Random();
            // Init correct number
            int correctNumber = random.Next(1,10);
            // Init guess var
            int guess = 0;

            Console.WriteLine("Guess a number between 1 and 10");

            // While guess is not correct
            while(guess != correctNumber)
            {
                // Get user's input
                string input = Console.ReadLine();


                // Match guess to correct number
                if (guess != correctNumber)
                {
                    if(!int.TryParse(input, out guess))
                    {
                        // Print error message
                        PrintColorMessage(ConsoleColor.Red, "Fehler!!...das ist nict ein Nummer ");

                        //keep going...
                        continue;
                    }

                    // Cast to int and put in guess
                    guess = Int32.Parse(input);

                    if (guess != correctNumber)
                    {
                            // Print error message
                            PrintColorMessage(ConsoleColor.Red, "Schade!!...das ist Falsch ");
                          

                    }
                    
                }
               
            }
                // Print success message
                PrintColorMessage(ConsoleColor.Yellow, "Geil!!Das ist ganz richtig.Danke schon");
             

                //Ask to play again
                Console.WriteLine("Wieder spielen? [Y oder N]");

                //Get answer
                string answer = Console.ReadLine().ToUpper();

                if (answer == "Y")
                {
                    continue;
                }
                else
                {
                    return;
                }

            }
        }

        static void GetAppInfo()
        {
            //Set app vars
            string appName = "NumberGuesser";
            string appVersion = "1.0.0";
            string appAuthor = "Nasser Saazi";

            // Change text color
            Console.ForegroundColor = ConsoleColor.Green;

            //Write out app info
            Console.WriteLine("{0}: Version {1} by {2}", appName, appVersion, appAuthor);

            // Reset text color
            Console.ResetColor();
        }
        static void GreetUser()
        {
            //Ask user's name
            Console.WriteLine("Was ist deine Name?");

            // Get user input
            string inputName = Console.ReadLine();

            Console.WriteLine("Halo {0}, wir durfen ein geil Game spielen...", inputName);
    

        }

        static void PrintColorMessage(ConsoleColor color ,string message)
        {
            // Change text color
            Console.ForegroundColor = color;

            //Tell user it's not a  number
            Console.WriteLine(message);

            // Reset text color
            Console.ResetColor();
        }
        //Task(count the number of times the user tries till they get it right and assess their proficiency
    }
}
