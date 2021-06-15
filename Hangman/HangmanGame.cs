﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Hangman
{
    public class HangmanGame
    {
        private readonly WordImporter wordImporter;
        private readonly int numberOfTrys = 10;

        public HangmanGame(WordImporter wordImporter)
        {
            this.wordImporter = wordImporter;
        }

        public void StartGame()
        {
            SetUp();

            string input;

            Console.WriteLine("Welcome to Hangman! Enter a letter or word to guess!");

            do
            {
                var word = wordImporter.GetOneWord();
                var wonGame = GuessWord(word);

                if (wonGame)
                {
                    Console.WriteLine("You did it! You guessed " + word.ToUpper());
                }
                else
                {
                    Console.WriteLine("You failed!");
                }

                Console.WriteLine("Do you want to play again? Press enter to continue, type -1 to exit");
                input = Console.ReadLine();

            } while (input != "-1");

        }

        private void SetUp()
        {
            try
            {
                wordImporter.ImportWordsFromFile();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found, using default words");

                wordImporter.SetWordsManually(new string[] { "ARBETSFÖRMEDLINGEN", "FÖRBUNDSKAPTEN", "RESERVPLATS",
                    "NORDVÄSTERSJÖKUSTARTILLERIFLYGSPANINGSSIMULATORANLÄGGNINGSMATERIELUNDERHÅLLSUPPFÖLJNINGSSYSTEMDISKUSSIONSINLÄGGSFÖRBEREDELSEARBETE",
                    "STACKOVERFLOW", "HANGMAN", "VÄGGISOLERING", "GUMMIDÄCK", "STORMVIND", "DADDLAR" });
            }

        }

        private bool GuessWord(string wordToBeGuessed)
        {
            wordToBeGuessed = wordToBeGuessed.ToUpper();
            StringBuilder guessedLetters = new StringBuilder();

            int guesses = 0;
            var charArray = StringMethods.GetCharArrayWithChar(wordToBeGuessed.Length);    
            bool WordHasBeenGuessed = false;

            while (guesses < numberOfTrys && !WordHasBeenGuessed)
            {
                StringMethods.PrintCharArray(charArray);                                                // Print the word to the user
                
                Console.WriteLine("\n" + guessedLetters);                                               // Print all the already guessed letter

                var guess = GetPlayerInput();                                                           // Get the guess from the user

                if (guess.Length == 1) 
                {
                    char guessedLetter = guess[0];

                    if (StringMethods.StringBuilderContainsLetter(guessedLetters, guessedLetter))  
                    {
                        Console.WriteLine("Letter already guessed, try again");
                        continue;
                    }
                    else
                    {
                        guessedLetters.Append(guessedLetter);
                    }

                    var indexes = StringMethods.IndexesOf(wordToBeGuessed, guessedLetter);                  // Get the indexes of the letter in the word

                    if (indexes.Count > 0)                                                                  // If there are any indexes
                    {
                        StringMethods.SetLetterInIndexes(indexes, ref charArray, guessedLetter);            //Update char array      
                        WordHasBeenGuessed = StringMethods.StringMatchesCharArray(wordToBeGuessed, charArray);  
                    } 
                    else
                    {
                        guesses++;
                    }

                }
                else if (guess.Equals(wordToBeGuessed))                                                     // If player has guessed the entire word
                {
                    WordHasBeenGuessed = true;
                }
                else
                {
                    guesses++;                                                                              // Guess is wrong
                }
            }

            return guesses < numberOfTrys;
        }

        private string GetPlayerInput()
        {
            bool correctInput = false;
            string playerInput = "";

            do
            {
                try
                {
                    playerInput = Console.ReadLine().ToUpper().Trim();
                    correctInput = CheckForValidInput(playerInput);
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("Input cannot be empty.");
                }
                catch (InvalidUserInputException)
                {
                    Console.WriteLine("Invalid input, only enter letter/s");   
                }
            } while (!correctInput);

            return playerInput;
        }

        private bool CheckForValidInput(string word)
        {
            if (null == word || word.Equals(""))
                throw new ArgumentNullException();
            if (Regex.IsMatch(word, @"[\s+\d+\W+\b+]"))  // Check if word has any numbers, special characters or blank spaces in it
                throw new InvalidUserInputException();

            return true;

        }

    } 
}
