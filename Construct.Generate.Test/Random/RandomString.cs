using System.Collections.Generic;
using System.Text;
using System.Text.Unicode;

namespace Construct.Generate.Test.Random
{
    public class RandomString
    {
        /// <summary>
        /// Range of the length of the random strings.
        /// </summary>
        public IntRange LengthRange { get; set; }
        
        /// <summary>
        /// Random number generator used for getting random numbers.
        /// </summary>
        private readonly System.Random _random = new System.Random();

        /// <summary>
        /// Character sets for only ASCII characters.
        /// </summary>
        private List<IntRange> _characterSetsAscii = new List<IntRange>()
        {
            new IntRange(48, 57), // Numbers
            new IntRange(65, 90), // Latin uppercase
            new IntRange(97, 122), // Latin lowercase
        };

        /// <summary>
        /// Character sets for ASCII and extended characters.
        /// </summary>
        private List<IntRange> _characterSetsExtended = new List<IntRange>()
        {
            new IntRange(48, 57), // Numbers
            new IntRange(65, 90), // Latin uppercase
            new IntRange(97, 122), // Latin lowercase
            new IntRange(192, 214), // Extended Latin uppercase
            new IntRange(216, 222),
            new IntRange(223, 246), // Extended Latin uppercase
            new IntRange(248, 255),
            new IntRange(256, 328), // European Latin
            new IntRange(330, 383),
            new IntRange(384, 591), // Non-European Latin and Historic
            new IntRange(880, 1023), // Greek
        };

        /// <summary>
        /// Creates the random string generator.
        /// </summary>
        /// <param name="lengthRange"></param>
        public RandomString(IntRange lengthRange)
        {
            this.LengthRange = lengthRange;
        }
        
        /// <summary>
        /// Returns a random string with a list of character ranges.
        /// </summary>
        /// <param name="characterRanges">Character ranges to use.</param>
        /// <returns>A random string.</returns>
        private string Next(List<IntRange> characterRanges)
        {
            var characters = new StringBuilder();
            for (var i = 0; i < this.LengthRange.Next(); i++)
            {
                var characterSet = characterRanges[this._random.Next(0, characterRanges.Count)];
                characters.Append((char) characterSet.Next());
            }
            return characters.ToString();
        }

        /// <summary>
        /// Returns a random string with ASCII and non-ASCII characters.
        /// </summary>
        /// <returns>A random string.</returns>
        public string Next() => this.Next(this._characterSetsExtended);
        
        /// <summary>
        /// Returns a random string with only ASCII characters.
        /// </summary>
        /// <returns>A random string.</returns>
        public string NextAscii() => this.Next(this._characterSetsAscii);
    }
}