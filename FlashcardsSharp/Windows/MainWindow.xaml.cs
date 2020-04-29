/*
 * File Name: BuildFlashcardSetWindow.xaml.cs
 * Purpose: The C# source file of the window for navigating through the program. Contains all the code to make the window functional.
 * Coder: Darian Benam
 * GitHub: https://github.com/BeardedFish
 * Date Created: Friday, April 24, 2020
 */

using System.Windows;

namespace FlashcardsSharp
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Main constructor for creating the main window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event handler for when the "Create Flashcard Set" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void CreateFlashcardsSetButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide the main window
            this.Hide();

            // Create and show the window
            BuildFlashcardSetWindow buildSetWindow = new BuildFlashcardSetWindow(this);
            buildSetWindow.Show();
        }

        /// <summary>
        /// Event handler for when the "Study Flashcard Set" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void StudyFlashcardSetButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide the main window
            this.Hide();

            // Create and show the window
            StudyFlashcardSetWindow studySetWindow = new StudyFlashcardSetWindow(this);
            studySetWindow.Show();
        }

        /// <summary>
        /// Event handler for when the "Quit Application" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void QuitApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
