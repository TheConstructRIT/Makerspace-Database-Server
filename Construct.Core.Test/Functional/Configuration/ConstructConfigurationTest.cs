using Construct.Core.Configuration;
using NUnit.Framework;

namespace Construct.Core.Test.Functional.Configuration
{
    public class ConstructConfigurationTest
    {
        /// <summary>
        /// "Test" for writing a clean configuration to the file system.
        /// This is done for the Config.py helper script, which requires a source
        /// for a configuration file with no modifications.
        /// </summary>
        [Test]
        public void TestSaveAsync()
        {
            new ConstructConfiguration().SaveAsync();
        }
    }
}