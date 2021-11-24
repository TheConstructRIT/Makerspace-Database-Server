using System;

namespace Construct.Generate.Test.Random
{
    public class IntRange
    {
        /// <summary>
        /// Minimum value to randomize.
        /// </summary>
        public int Minimum { get; set; }
        
        /// <summary>
        /// Maximum value to randomize.
        /// </summary>
        public int Maximum { get; set; }

        /// <summary>
        /// Random number generator used for getting random numbers.
        /// </summary>
        private readonly System.Random _random = new System.Random();

        /// <summary>
        /// Creates the integer range object.
        /// </summary>
        /// <param name="min">Minimum value to randomize.</param>
        /// <param name="max">Maximum value to randomize.</param>
        public IntRange(int min, int max)
        {
            this.Minimum = min;
            this.Maximum = max;
        }

        /// <summary>
        /// Returns a random integer in the range.
        /// </summary>
        /// <returns>A random integer in the range.</returns>
        public int Next()
        {
            return this._random.Next(this.Minimum, this.Maximum + 1);
        }
    }
}