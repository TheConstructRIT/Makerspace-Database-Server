using System;

namespace Construct.Generate.Test.Random
{
    public class RandomDateTime
    {
        /// <summary>
        /// Start date to use for randomization.
        /// </summary>
        private static readonly DateTime StartDate = new DateTime(2000, 1, 1);
        
        /// <summary>
        /// Randomizer for the date value.
        /// </summary>
        private System.Random _random = new System.Random();

        /// <summary>
        /// Returns a random date time.
        /// </summary>
        /// <returns>A random date time.</returns>
        public DateTime Next()
        {
            var dayRange = (DateTime.Today - StartDate).Days;           
            return StartDate.AddDays(this._random.Next(dayRange)).AddHours(this._random.Next(0, 24))
                .AddMinutes(this._random.Next(0, 60)).AddSeconds(this._random.Next(0, 60));
        }
    }
}