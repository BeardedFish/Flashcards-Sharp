/*
 * File Name: HelperClass.cs
 * Purpose: A class that will serve as a container for constants and methods that will help other classes in the program.
 * Coder: Darian Benam
 * GitHub: https://github.com/BeardedFish
 * Date Created: Friday, April 27, 2020
 */

namespace FlashcardsSharp.Core
{
    public static class HelperClass
    {
        /// <summary>
        /// The dialog filter for the open/save dialogs that this program uses.
        /// </summary>
        public const string DialogFilter = "JSON Files (*.json)|*.json|All files (*.*)|*.*";

        /// <summary>
        /// The JSON property name for the FlashcardsList object.
        /// </summary>
        public const string FlashcardsListJsonPropertyName = "flashcardsList";

        /// <summary>
        /// The JSON property name for the string that represents the set name.
        /// </summary>
        public const string SetNameJsonPropertyName = "setName";

        /// <summary>
        /// The JSON property name for the stirng that represents the term of a flashcard.
        /// </summary>
        public const string TermJsonPropertyName = "term";

        /// <summary>
        /// The JSON property name for the string that represents the definition of a flashcard.
        /// </summary>
        public const string DefinitionJsonPropertyName = "definition";
    }
}