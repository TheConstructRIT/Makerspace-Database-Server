namespace Construct.Generate.Test.Random
{
    public class RandomBool
    {
        /// <summary>
        /// Randomizer for the bool value.
        /// </summary>
        private System.Random _random = new System.Random();

        /// <summary>
        /// Returns a random boolean.
        /// </summary>
        /// <returns>A random boolean.</returns>
        public bool Next()
        {
            return this._random.NextDouble() > 0.5;
        }
    }
}