/*
 * File Name: Flashcard.cs
 * Purpose: 
 * Coder: Darian Benam
 * GitHub: https://github.com/BeardedFish
 * Date Created: Friday, April 24, 2020
 */

namespace FlashcardsSharp.Core
{
    public class Flashcard
    {
        /// <summary>
        /// The term of the flashcard.
        /// </summary>
        public string Term { get; private set; }

        /// <summary>
        /// The definition of the flashcard.
        /// </summary>
        public string Definition { get; private set; }

        /// <summary>
        /// Main constructors for creating a Flashcard object.
        /// </summary>
        /// <param name="term">The term of the flashcard.</param>
        /// <param name="definition">The defintion of the flashcard.</param>
        public Flashcard(string term, string definition)
        {
            this.Term = term;
            this.Definition = definition;
        }

        /// <summary>
        /// Returns a formatted string that represents the flashcard.
        /// </summary>
        /// <returns>A string in the format of "[TERM] : [DEFINITION]" (excluding quotes).</returns>
        public override string ToString()
        {
            return Term + " : " + Definition;
        }
    }
}