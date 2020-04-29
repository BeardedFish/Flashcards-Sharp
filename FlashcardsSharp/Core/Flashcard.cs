/*
 * File Name: FlashcardSet.cs
 * Purpose: A container for storing a flashcard. A flashcard consists of a term and a defintion.
 * Coder: Darian Benam
 * GitHub: https://github.com/BeardedFish
 * Date Created: Friday, April 24, 2020
 */

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FlashcardsSharp.Core
{
    public class Flashcard
    {
        /// <summary>
        /// The term of the flashcard.
        /// </summary>
        [JsonPropertyName(HelperClass.TermJsonPropertyName)]
        public string Term { get; private set; }

        /// <summary>
        /// The definition of the flashcard.
        /// </summary>
        [JsonPropertyName(HelperClass.DefinitionJsonPropertyName)]
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
        /// Checks if an object is equal to the current instance of this object.
        /// </summary>
        /// <param name="obj">The object to be compared to this object.</param>
        /// <returns>A boolean that states whether the object is equal to this object or not.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Flashcard)
            {
                Flashcard objFlashcard = (Flashcard)obj;

                if (objFlashcard.Definition.Equals(this.Definition) && objFlashcard.Term.Equals(this.Term))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the unique hash code number for this flashcard.
        /// </summary>
        /// <returns>An int that represents the unique hash code of this flashcard.</returns>
        public override int GetHashCode()
        {
            return (Term, Definition).GetHashCode();

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