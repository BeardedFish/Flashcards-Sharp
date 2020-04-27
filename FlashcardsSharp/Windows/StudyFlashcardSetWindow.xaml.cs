using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using FlashcardsSharp.Core;
using FlashcardsSharp.Enums;

namespace FlashcardsSharp
{
    /// <summary>
    /// Interaction logic for StudyFlashcardSetWindow.xaml
    /// </summary>
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
        /// 
        /// </summary>
        private void UpdateFlashcardUI()
        {
            if (currentFlashcardSet == null || currentFlashcardSet.FlashcardsList.Count == 0)
            {
                flashcardContainer.Cursor = Cursors.No;

                return;
            }
            else
            {
                flashcardContainer.Cursor = Cursors.Hand;
            }

            UpdateProgressText();
            ToggleNavigationButtons();

            setTitle.Text = currentFlashcardSet.SetName;

            Flashcard currentFlashcard = currentFlashcardSet.FlashcardsList[flashcardsListCurrentIndex];

            flashcardText.Text = (currentFlashcardState == FlashcardState.Term) ? currentFlashcard.Term : currentFlashcard.Definition;
            flashcardText.FontWeight = (currentFlashcardState == FlashcardState.Term) ? FontWeights.Bold : FontWeights.Normal;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ToggleNavigationButtons()
        {
            previousFlashcardButton.IsEnabled = (flashcardsListCurrentIndex > 0) ? true : false;
            nextFlashcardButton.IsEnabled = (flashcardsListCurrentIndex < (currentFlashcardSet == null ? - 1 : currentFlashcardSet.FlashcardsList.Count - 1)) ? true : false;
        }

        private void Border_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            currentFlashcardState = (currentFlashcardState == FlashcardState.Definition) ? FlashcardState.Term : FlashcardState.Definition;

            UpdateFlashcardUI();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousFlashcardButton_Click(object sender, RoutedEventArgs e)
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextFlashcardButton_Click(object sender, RoutedEventArgs e)
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
        /// Event handler for the window is closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (currentFlashcardSet != null && currentFlashcardSet.FlashcardsList.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to exit the window? You have a loaded list.", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    e.Cancel = true;

                    return;
                }
            }

            mainWindow.Show();
        }

        /// <summary>
        /// Event handler for when the "Load Flashcard Set" button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                        JsonElement root = document.RootElement;

                        string setName = root.GetProperty("SetName").ToString();

                        JsonElement flashcards = root.GetProperty("FlashcardsList");


                        List<Flashcard> flashcardsList = new List<Flashcard>();
                        foreach (JsonElement element in flashcards.EnumerateArray())
                        {
                            string term = element.GetProperty("Term").ToString();
                            string definition = element.GetProperty("Definition").ToString();

                            Flashcard flashcard = new Flashcard(term, definition);

                            flashcardsList.Add(flashcard);
                        }

                        FlashcardSet set = new FlashcardSet(setName, flashcardsList);

                        if (flashcardSetListBox.Items.Contains(set))
                        {
                            MessageBox.Show("Flashcard set has already been loaded into the program!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            flashcardSetListBox.Items.Add(set);

                            currentFlashcardSet = set;

                            UpdateFlashcardUI();

                            MessageBox.Show("Flashcard set loaded succesfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlashcardSetListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            currentFlashcardSet = (FlashcardSet)flashcardSetListBox.SelectedItem;

            currentFlashcardState = FlashcardState.Term;

            UpdateFlashcardUI();
        }
    }
}