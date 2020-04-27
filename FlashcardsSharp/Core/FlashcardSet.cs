/*
 * File Name: FlashcardSet.cs
 * Purpose: 
 * Coder: Darian Benam
 * GitHub: https://github.com/BeardedFish
 * Date Created: Friday, April 27, 2020
 */

using System.Collections.Generic;

namespace FlashcardsSharp.Core
{
    public class FlashcardSet
    {
        /// <summary>
        /// 
        /// </summary>
        public string SetName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Flashcard> FlashcardsList { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="flashcardsList"></param>
        public FlashcardSet(string setName, List<Flashcard> flashcardsList)
        {
            this.SetName = setName;
            this.FlashcardsList = flashcardsList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SetName + " [" + FlashcardsList.Count + " flashcard" + (FlashcardsList.Count == 1 ? "" : "s") + "]";
        }
    }
}