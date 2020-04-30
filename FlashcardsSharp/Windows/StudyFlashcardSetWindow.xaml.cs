/*
 * File Name: BuildFlashcardSetWindow.xaml.cs
 * Purpose: The C# source file of the window for studying flashcard sets. Contains all the code to make the window functional.
 * Coder: Darian Benam
 * GitHub: https://github.com/BeardedFish
 * Date Created: Friday, April 24, 2020
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using FlashcardsSharp.Core;
using FlashcardsSharp.Enums;

namespace FlashcardsSharp
{
    public partial class StudyFlashcardSetWindow : Window
    {
        /// <summary>
        /// Reference to the main window for when the application first starts.
        /// </summary>
        private MainWindow mainWindow;

        /// <summary>
        /// The current flashcard set the user is studying.
        /// </summary>
        private FlashcardSet currentFlashcardSet;

        /// <summary>
        /// The current flashcard index the user is looking at in the set.
        /// </summary>
        private int flashcardsListCurrentIndex = 0;

        /// <summary>
        /// States the flashcard face that the user is currently looking at (either term of definition).
        /// </summary>
        private FlashcardState currentFlashcardState;

        /// <summary>
        /// Main constructor for creating the window for studying flashcards.
        /// </summary>
        public StudyFlashcardSetWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            ToggleUnloadCurrentSetButton();
            ToggleNavigationButtons();

            this.mainWindow = mainWindow;

            // Make the term be the first thing that shows up on the GUI
            currentFlashcardState = FlashcardState.Term;
        }

        /// <summary>
        /// Updates the progress text below the flashcard interactive area.
        /// </summary>
        private void UpdateProgressText()
        {
            progressText.Text = (flashcardsListCurrentIndex + 1) + "/" + currentFlashcardSet.FlashcardsList.Count;
        }

        /// <summary>
        /// Updates the UI area where the user can interact with the flashcard set.
        /// </summary>
        private void UpdateFlashcardUI()
        {
            if (currentFlashcardSet == null || currentFlashcardSet.FlashcardsList.Count == 0) // If no flashcard is loaded, set the cursor for the flashcard container to the "No" cursor
            {
                flashcardContainer.Cursor = Cursors.No;
                setTitle.Text = "Load a set to start studying...";
                flashcardText.Text = "";

                return;
            }
            else // Set the cursor to "Hand" if a flashcard set is loaded
            {
                flashcardContainer.Cursor = Cursors.Hand;
            }

            // Update the flashcard set title
            setTitle.Text = currentFlashcardSet.SetName;

            UpdateProgressText();
            ToggleNavigationButtons();

            // Update the flashcard text depending on which side is visible
            Flashcard currentFlashcard = currentFlashcardSet.FlashcardsList[flashcardsListCurrentIndex];
            flashcardText.Text = (currentFlashcardState == FlashcardState.Term) ? currentFlashcard.Term : currentFlashcard.Definition;
            flashcardText.FontWeight = (currentFlashcardState == FlashcardState.Term) ? FontWeights.Bold : FontWeights.Normal; // Definition is bold while term is normal font weight
        }

        /// <summary>
        /// Toggles the enable/disable status of the buttons for navigating through the flashcard set.
        /// </summary>
        private void ToggleNavigationButtons()
        {
            previousFlashcardButton.IsEnabled = (flashcardsListCurrentIndex > 0);
            nextFlashcardButton.IsEnabled = (flashcardsListCurrentIndex < (currentFlashcardSet == null ? -1 : currentFlashcardSet.FlashcardsList.Count - 1));
        }

        /// <summary>
        /// Toggles the enable/disable status of the "Unload Current Set" button.
        /// </summary>
        private void ToggleUnloadCurrentSetButton()
        {
            unloadCurrentSetButton.IsEnabled = (flashcardSetListBox.Items.Count > 0);
        }

        /// <summary>
        /// Checks whether a flashcard set is already present in the flashcard set list box on the window.
        /// </summary>
        /// <param name="flashcardSet">The flashcard set to be checked for whether it is already loaded or not.</param>
        /// <returns>A boolean that states whether the flashcard set is loaded in the program or not.</returns>
        private bool AlreadyLoaded(FlashcardSet flashcardSet)
        {
            // Iterate through each flashcard set in the list box and see if there is a duplicate, if there is, that means that the flashcard set is already loaded
            foreach (FlashcardSet fs in flashcardSetListBox.Items)
            {
                if (flashcardSet.Equals(fs))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Event handler for when the "Load Flashcard Set" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void LoadSetButton_Click(object sender, RoutedEventArgs e)
        {
            // Save it to a specified location that the user chooses
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = HelperClass.DialogFilter;

            if (openDialog.ShowDialog() == true) // Ok button was pressed on the dialog
            {
                // String for storing the text contents of the file to be loaded in (which should be JSON)
                string fileText = "";

                try
                {
                    fileText = File.ReadAllText(openDialog.FileName);

                    // Try and deserialize the JSON into flashcard objects
                    using (JsonDocument document = JsonDocument.Parse(fileText))
                    {
                        // Get the root element in the text file
                        JsonElement root = document.RootElement;

                        // Get the set name and store it in a string
                        string setName = root.GetProperty(HelperClass.SetNameJsonPropertyName).ToString();

                        // Declare a list for storing flashcards that were read in from the JSON file
                        List<Flashcard> flashcardsList = new List<Flashcard>();

                        // Extract every flashcard from the list and add it to the list
                        JsonElement flashcards = root.GetProperty(HelperClass.FlashcardsListJsonPropertyName);
                        foreach (JsonElement element in flashcards.EnumerateArray())
                        {
                            // Extract the term and definition and store them in a string variables
                            string term = element.GetProperty(HelperClass.TermJsonPropertyName).ToString();
                            string definition = element.GetProperty(HelperClass.DefinitionJsonPropertyName).ToString();

                            // Create flashcard object and add it to the list
                            Flashcard flashcard = new Flashcard(term, definition);
                            flashcardsList.Add(flashcard);
                        }

                        // Convert the items loaded from the file to a FlashcardSet object
                        FlashcardSet set = new FlashcardSet(setName, flashcardsList);

                        // If the flashcard set is already loaded, prompt the user to make sure that that want to load in the flashcard set again
                        if (AlreadyLoaded(set))
                        {
                            MessageBoxResult msgResult = MessageBox.Show("Flashcard set has already been loaded into the program! Are you sure you still want to load in flashcard set?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);

                            if (msgResult == MessageBoxResult.No) // User does not want to load in the flashcard set again that is already loaded in, therefore, return from the method
                            {
                                return;
                            }
                        }

                        // Add the FlashcardSet object into the list
                        flashcardSetListBox.Items.Add(set);

                        // Set it as the current flashcard the user can interact with
                        currentFlashcardSet = set;

                        // Update the UI to show the changes visually
                        ToggleUnloadCurrentSetButton();
                        UpdateFlashcardUI();

                        // Let the user know the flashcard set was loaded succesfully
                        MessageBox.Show("Flashcard set loaded succesfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);    
                    }
                }
                catch (Exception ex)
                {
                    // Show message to the user
                    MessageBox.Show("An error occured while trying to parse the file '" + openDialog.FileName + "' which resulted in the flashcard set not being loaded into the program.\n\nException message: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Event handler for when the "Unload Current Set" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void UnloadCurrentSetButton_Click(object sender, RoutedEventArgs e)
        {
            if (flashcardSetListBox.Items.Count > 0)
            {
                int selectedIndex = flashcardSetListBox.Items.IndexOf(currentFlashcardSet);

                flashcardSetListBox.Items.RemoveAt(selectedIndex);

                if (flashcardSetListBox.Items.Count > 0)
                {
                    flashcardSetListBox.SelectedIndex = 0;
                }
                else
                {
                    currentFlashcardSet = null;
                }

                currentFlashcardState = FlashcardState.Term;
                flashcardsListCurrentIndex = 0;

                ToggleUnloadCurrentSetButton();
                UpdateFlashcardUI();
            }
        }

        /// <summary>
        /// Event handler for when the user clicks on the flashcard container with the left mouse button.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for flashcard container is clicked.</param>
        private void FlashcardContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Don't do anything if no flashcard is currently loaded for studying
            if (currentFlashcardSet == null)
            {
                return;
            }

            // Toggle the visibility state of the term and definition of the flashcard
            currentFlashcardState = (currentFlashcardState == FlashcardState.Definition) ? FlashcardState.Term : FlashcardState.Definition;

            UpdateFlashcardUI();
        }

        /// <summary>
        /// Event handler for when the "Previous Flashcard" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void PreviousFlashcardButton_Click(object sender, RoutedEventArgs e)
        {
            // Go to the previous index in the flashcard list (if there is one)
            flashcardsListCurrentIndex--;

            // Prevent out of bounds if the user is already at the first flashcard in the set
            if (flashcardsListCurrentIndex < 0)
            {
                flashcardsListCurrentIndex = 0;

                return;
            }

            // Show the term of the flashcard and update the UI to show the changes
            currentFlashcardState = FlashcardState.Term;
            UpdateFlashcardUI();
        }

        /// <summary>
        /// Event handler for when the "Next Flashcard" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void NextFlashcardButton_Click(object sender, RoutedEventArgs e)
        {
            // Go to the next index in the flashcard list (if there is one)
            flashcardsListCurrentIndex++;

            // Prevent out of bounds if there is no more flashcards in the set
            if (flashcardsListCurrentIndex >= currentFlashcardSet.FlashcardsList.Count)
            {
                flashcardsListCurrentIndex = currentFlashcardSet.FlashcardsList.Count - 1;

                return;
            }

            // Show the term of the flashcard and update the UI to show the changes
            currentFlashcardState = FlashcardState.Term;
            UpdateFlashcardUI();
        }

        /// <summary>
        /// Event handler for when the item selection changes in the flashcard set list box.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the selection is changed.</param>
        private void FlashcardSetListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update the current flashcard to the one selected
            currentFlashcardSet = (FlashcardSet)flashcardSetListBox.SelectedItem;
            currentFlashcardState = FlashcardState.Term;
            flashcardsListCurrentIndex = 0;

            // Update the UI to show the change
            UpdateFlashcardUI();
        }

        /// <summary>
        /// Event handler for the window is closing.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (currentFlashcardSet != null && currentFlashcardSet.FlashcardsList.Count > 0) // Show warning message if the user has a flashcard set currently loaded
            {
                string loadedFlashcardMessage = (currentFlashcardSet.FlashcardsList.Count > 0) ? "You have flashcard sets" : "You have a flashcard set";

                if (MessageBox.Show("Are you sure you want to exit the window? " + loadedFlashcardMessage + " loaded in the program.", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    // Set this to true to cancel the window from closing
                    e.Cancel = true;

                    return;
                }
            }

            mainWindow.Show();
        }
    }
}