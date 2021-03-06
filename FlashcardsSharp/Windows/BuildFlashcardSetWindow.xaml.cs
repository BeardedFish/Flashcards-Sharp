﻿/*
 * File Name: BuildFlashcardSetWindow.xaml.cs
 * Purpose: The C# source file of the window for building a flashcard set. Contains all the code to make the window functional.
 * Coder: Darian Benam
 * GitHub: https://github.com/BeardedFish
 * Date Created: Friday, April 24, 2020
 */

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using FlashcardsSharp.Core;

namespace FlashcardsSharp
{
    public partial class BuildFlashcardSetWindow : Window
    {
        /// <summary>
        /// Reference to the main window for when the application first starts.
        /// </summary>
        private MainWindow mainWindow;

        /// <summary>
        /// Main constructor for building the 'BuildFlashcardSetWindow'.
        /// </summary>
        /// <param name="mainWindow">Reference to the main window.</param>
        public BuildFlashcardSetWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            ToggleListBoxButtons();

            this.mainWindow = mainWindow;
        }

        /// <summary>
        /// Toggles the enable state of the list box buttons which overall depends on whether there are items in the list box or not.
        /// </summary>
        private void ToggleListBoxButtons()
        {
            // Use ternary operator to set the list box button enable states
            moveFlashcardDownButton.IsEnabled = moveFlashcardUpButton.IsEnabled = flashcardsSetListBox.Items.Count > 1;
            removeSelectedFlashcardButton.IsEnabled = flashcardsSetListBox.Items.Count > 0;
        }

        /// <summary>
        /// Converts the input that the user entered into the text fields to a Flashcard object. The Flashcard object that is created is added to the list box on the window.
        /// </summary>
        private void AddFlashcardToSet()
        {
            if (string.IsNullOrWhiteSpace(termText.Text) || string.IsNullOrWhiteSpace(definitionText.Text))
            {
                MessageBox.Show("One of the fields is null or only contains whitespace! Please enter a valid value in the field.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            Flashcard flashcard = new Flashcard(termText.Text, definitionText.Text);
            flashcardsSetListBox.Items.Add(flashcard);

            termText.Clear();
            definitionText.Clear();

            ToggleListBoxButtons();
        }

        /// <summary>
        /// Swaps two Flashcard objects in the list box on the window via their index numbers.
        /// </summary>
        /// <param name="index1">The index in the list box to be swapped with index #2.</param>
        /// <param name="index2">The index in the list box to be swapped with index #1.</param>
        private void SwapFlashcards(int index1, int index2)
        {
            Flashcard tempFlashcard = (Flashcard)flashcardsSetListBox.Items[index1];

            flashcardsSetListBox.Items[index1] = flashcardsSetListBox.Items[index2];
            flashcardsSetListBox.Items[index2] = tempFlashcard;
        }

        /// <summary>
        /// Event handler for when the "Add To Set" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void AddToSetButton_Click(object sender, RoutedEventArgs e)
        {
            AddFlashcardToSet();
        }

        /// <summary>
        /// Event handler for when the "Save Flashcard Set" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void SaveFlashcardSetButton_Click(object sender, RoutedEventArgs e)
        {
            if (flashcardsSetListBox.Items.Count == 0)
            {
                MessageBox.Show("You can't save an empty flashcard set!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            if (string.IsNullOrWhiteSpace(setNameText.Text))
            {
                MessageBox.Show("Invalid set name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            List<Flashcard> flashcards = new List<Flashcard>();
            foreach (Flashcard flashcard in flashcardsSetListBox.Items)
            {
                flashcards.Add(flashcard);
            }

            FlashcardSet flashcardSet = new FlashcardSet(setNameText.Text, flashcards);
            string flashcardsSetJson = JsonSerializer.Serialize(flashcardSet); // Serialize the 'flashcardSet' to JSON text

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = HelperClass.DialogFilter;

            if (saveDialog.ShowDialog() == true) // Ok button was pressed on the save dialog, save the JSON text
            {
                try
                {
                    File.WriteAllText(saveDialog.FileName, flashcardsSetJson);

                    MessageBox.Show("Flashcard set saved succesfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured while trying to save the flashcard set.\n\nException message: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Event handler for when the "Remove Selected Flashcard" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void RemoveSelectedFlashcardButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = flashcardsSetListBox.SelectedIndex;
            flashcardsSetListBox.Items.RemoveAt(selectedIndex);

            // Find a new list box index to select
            selectedIndex = (selectedIndex == 0 && flashcardsSetListBox.Items.Count >= 1) ? selectedIndex : --selectedIndex;
            if (selectedIndex >= 0)
            {
                flashcardsSetListBox.SelectedItem = flashcardsSetListBox.Items.GetItemAt(selectedIndex);
            }

            ToggleListBoxButtons();
        }

        /// <summary>
        /// Event handler for when the "Move Flashcard Up" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void MoveFlashcardUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (flashcardsSetListBox.SelectedItem == null) // Don't move the flashcard up if no item is selected in the list box
            {
                MessageBox.Show("No flashcard is selected! Please select a flashcard and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            int selectedFlashcardIndex = flashcardsSetListBox.SelectedIndex;
            int upperIndex = selectedFlashcardIndex - 1; // Subtracting one since it points to the item above the selected index

            if (upperIndex < 0)
            {
                upperIndex = flashcardsSetListBox.Items.Count - 1;
            }

            SwapFlashcards(selectedFlashcardIndex, upperIndex);

            flashcardsSetListBox.SelectedItem = flashcardsSetListBox.Items.GetItemAt(upperIndex);
        }

        /// <summary>
        /// Event handler for when the "Move Flashcard Down" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void MoveFlashcardDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (flashcardsSetListBox.SelectedItem == null) // Don't move the flashcard down if no item is selected in the list box
            {
                MessageBox.Show("No flashcard is selected! Please select a flashcard and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            int selectedFlashcardIndex = flashcardsSetListBox.SelectedIndex;
            int lowerIndex = selectedFlashcardIndex + 1; // Adding one since it points to the item below the selected index

            if (lowerIndex >= flashcardsSetListBox.Items.Count)
            {
                lowerIndex = 0;
            }

            SwapFlashcards(selectedFlashcardIndex, lowerIndex);

            flashcardsSetListBox.SelectedItem = flashcardsSetListBox.Items.GetItemAt(lowerIndex);
        }

        /// <summary>
        /// Event handler for when a key is held down on the window.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the key is held down.</param>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddFlashcardToSet();
            }
        }

        /// <summary>
        /// Event handler for when the window is closing.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the window is closing.</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (flashcardsSetListBox.Items.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to exit the window? All items in the set will be lost.", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    e.Cancel = true;

                    return;
                }
            }

            mainWindow.Show();
        }
    }
}