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
            UpdateFlashcardUI();

            this.mainWindow = mainWindow;

            currentFlashcardState = FlashcardState.Term; // Make the term be the first thing the user sees for the first flashcard loaded
        }

        /// <summary>
        /// Updates the progress text below the flashcard interactive area.
        /// </summary>
        private void UpdateProgressText()
        {
            string progress = (currentFlashcardSet == null) ? "0/0" : (flashcardsListCurrentIndex + 1) + "/" + currentFlashcardSet.FlashcardsList.Count;
            progressText.Text = progress;
        }

        /// <summary>
        /// Updates the UI area where the user can interact with the flashcard set.
        /// </summary>
        private void UpdateFlashcardUI()
        {
            UpdateProgressText();
            ToggleNavigationButtons();
            ToggleUnloadCurrentSetButton();

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

            setTitle.Text = currentFlashcardSet.SetName;

            // Update the flashcard text on the GUI depending on which side is visible
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
            foreach (FlashcardSet fs in flashcardSetListBox.Items)
            {
                if (flashcardSet.Equals(fs)) // Duplicate found
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Goes to the previous flashcard in the current set (if there is one).
        /// </summary>
        private void PreviousFlashcard()
        {
            flashcardsListCurrentIndex--;
            if (flashcardsListCurrentIndex < 0)
            {
                flashcardsListCurrentIndex = 0;

                return;
            }

            currentFlashcardState = FlashcardState.Term;

            UpdateFlashcardUI();
        }

        /// <summary>
        /// Goes to the next flashcard in the current set (if there is one).
        /// </summary>
        private void NextFlashcard()
        {
            flashcardsListCurrentIndex++;
            if (flashcardsListCurrentIndex >= currentFlashcardSet.FlashcardsList.Count)
            {
                flashcardsListCurrentIndex = currentFlashcardSet.FlashcardsList.Count - 1;

                return;
            }

            currentFlashcardState = FlashcardState.Term;

            UpdateFlashcardUI();
        }

        /// <summary>
        /// Toggles the current flashcard face between the term and definition.
        /// </summary>
        private void ToggleFlashcardFace()
        {
            if (currentFlashcardSet == null)
            {
                return;
            }

            currentFlashcardState = (currentFlashcardState == FlashcardState.Definition) ? FlashcardState.Term : FlashcardState.Definition;

            UpdateFlashcardUI();
        }

        /// <summary>
        /// Event handler for when the "Load Flashcard Set" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void LoadSetButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = HelperClass.DialogFilter;

            if (openDialog.ShowDialog() == true) // Ok button was pressed on the dialog
            {
                try
                {
                    string fileText = File.ReadAllText(openDialog.FileName);

                    // Extract the information about the flashcard set from the JSON text
                    using (JsonDocument document = JsonDocument.Parse(fileText))
                    {
                        JsonElement root = document.RootElement;

                        string setName = root.GetProperty(HelperClass.SetNameJsonPropertyName).ToString();

                        // Get the flashcards in the JSON
                        List<Flashcard> flashcardsList = new List<Flashcard>();
                        foreach (JsonElement element in root.GetProperty(HelperClass.FlashcardsListJsonPropertyName).EnumerateArray())
                        {
                            string term = element.GetProperty(HelperClass.TermJsonPropertyName).ToString();
                            string definition = element.GetProperty(HelperClass.DefinitionJsonPropertyName).ToString();

                            Flashcard flashcard = new Flashcard(term, definition);
                            flashcardsList.Add(flashcard);
                        }

                        FlashcardSet set = new FlashcardSet(setName, flashcardsList);

                        if (AlreadyLoaded(set)) // If the flashcard set is already loaded, prompt the user to make sure that that want to load in the flashcard set again
                        {
                            MessageBoxResult msgResult = MessageBox.Show("Flashcard set has already been loaded into the program! Are you sure you still want to load in flashcard set?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);

                            if (msgResult == MessageBoxResult.No)
                            {
                                return;
                            }
                        }

                        flashcardSetListBox.Items.Add(set);
                        flashcardSetListBox.SelectedItem = set;
                        flashcardsListCurrentIndex = 0;

                        UpdateFlashcardUI();

                        MessageBox.Show("Flashcard set loaded succesfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
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
            flashcardContainer.Focus();

            ToggleFlashcardFace();
        }

        /// <summary>
        /// Event handler for when the "Previous Flashcard" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void PreviousFlashcardButton_Click(object sender, RoutedEventArgs e)
        {
            PreviousFlashcard();
        }

        /// <summary>
        /// Event handler for when the "Next Flashcard" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void NextFlashcardButton_Click(object sender, RoutedEventArgs e)
        {
            NextFlashcard();
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

            UpdateFlashcardUI();
        }

        /// <summary>
        /// Event handler for when the window is closing.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the window is closing.</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (currentFlashcardSet != null && currentFlashcardSet.FlashcardsList.Count > 0) // Show warning message if the user has a flashcard set currently loaded
            {
                string loadedFlashcardMessage = (currentFlashcardSet.FlashcardsList.Count > 0) ? "You have flashcard sets" : "You have a flashcard set";

                if (MessageBox.Show("Are you sure you want to exit the window? " + loadedFlashcardMessage + " loaded in the program.", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    e.Cancel = true; // Set this to true to cancel the window from closing

                    return;
                }
            }

            mainWindow.Show();
        }

        /// <summary>
        /// Event handler for when a key is held down on the flashcard container.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the key is held down.</param>
        private void FlashcardContainer_KeyDown(object sender, KeyEventArgs e)
        {
            if (currentFlashcardSet == null)
            {
                return;
            }

            if (e.Key == Key.Left || e.Key == Key.PageDown || e.Key == Key.Right || e.Key == Key.PageUp
                || e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Space)
            {
                e.Handled = true;
            }

            if (e.Key == Key.Left || e.Key == Key.PageDown) // Go to previous flashcard in the current set
            {
                PreviousFlashcard();
            }

            if (e.Key == Key.Right || e.Key == Key.PageUp) // Go to next flashcard in current set
            {
                NextFlashcard();
            }

            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Space) // Toggle term/definition face of current flashcard
            {
                ToggleFlashcardFace();
            }
        }
    }
}