/*
 * File Name: FlashcardSet.cs
 * Purpose: A container for storing a flashcard set. A flashcard set consists of a set name and a list of Flashcard objects.
 * Coder: Darian Benam
 * GitHub: https://github.com/BeardedFish
 * Date Created: Friday, April 27, 2020
 */

using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;

namespace FlashcardsSharp.Core
{
    public class FlashcardSet
    {
        /// <summary>
        /// The name of the flashcard set.
        /// </summary>
        [JsonPropertyName(HelperClass.SetNameJsonPropertyName)]
        public string SetName { get; private set; }

        /// <summary>
        /// The list of flashcards in the set.
        /// </summary>
        [JsonPropertyName(HelperClass.FlashcardsListJsonPropertyName)]
        public List<Flashcard> FlashcardsList { get; private set; }

        /// <summary>
        /// Main constructor for creating a flashcard set.
        /// </summary>
        /// <param name="setName">The name of the flashcard set.</param>
        /// <param name="flashcardsList">The list of flashcards that will be in the flashcard set.</param>
        public FlashcardSet(string setName, List<Flashcard> flashcardsList)
        {
            this.SetName = setName;
            this.FlashcardsList = flashcardsList;
        }

        /// <summary>
        /// Checks if an object is equal to the current instance of this object.
        /// </summary>
        /// <param name="obj">The object to be compared to this object.</param>
        /// <returns>A boolean that states whether the object is equal to this object or not.</returns>
        public override bool Equals(object obj)
        {
            if (obj is FlashcardSet)
            {
                FlashcardSet objFlashcard = (FlashcardSet)obj;

                bool isListsEqual = Enumerable.SequenceEqual(objFlashcard.FlashcardsList.OrderBy(e => e), this.FlashcardsList.OrderBy(e => e));
                if (objFlashcard.SetName.Equals(this.SetName) && isListsEqual)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the unique hash code number for this flashcard set.
        /// </summary>
        /// <returns>An int that represents the unique hash code of this flashcard set.</returns>
        public override int GetHashCode()
        {
            var hashCode = 709270232;

            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SetName);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Flashcard>>.Default.GetHashCode(FlashcardsList);

            return hashCode;
        }

        /// <summary>
        /// Returns a formatted string that represents the flashcard set.
        /// </summary>
        /// <returns>A string in the format of "[TOTAL_FLASHCARDS_IN_SET_COUNT flashcard(s)]" (excluding quotes).</returns>
        public override string ToString()
        {
            return SetName + " [" + FlashcardsList.Count + " flashcard" + (FlashcardsList.Count == 1 ? "" : "s") + "]";
        }
    }
}