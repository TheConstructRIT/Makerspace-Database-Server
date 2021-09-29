using System;
using Construct.Core.Database.Model;
using NUnit.Framework;

namespace Construct.Core.Test.Database.Model
{
    public class PermissionTest
    {
        /// <summary>
        /// Permission to test.
        /// </summary>
        private Permission _permission;

        /// <summary>
        /// Sets up the test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this._permission = new Permission();
        }
        
        /// <summary>
        /// Tests the IsActive method with no bounds.
        /// </summary>
        [Test]
        public void TestIsActiveNoBounds()
        {
            Assert.IsTrue(this._permission.IsActive());
        }

        /// <summary>
        /// Tests the IsActive method with a start time.
        /// </summary>
        [Test]
        public void TestIsActiveStartBound()
        {
            this._permission.StartTime = DateTime.Now.AddSeconds(-10);
            Assert.IsTrue(this._permission.IsActive());
            this._permission.StartTime = DateTime.Now.AddSeconds(10);
            Assert.IsFalse(this._permission.IsActive());
        }

        /// <summary>
        /// Tests the IsActive method with an end time.
        /// </summary>
        [Test]
        public void TestIsActiveEndBound()
        {
            this._permission.EndTime = DateTime.Now.AddSeconds(10);
            Assert.IsTrue(this._permission.IsActive());
            this._permission.EndTime = DateTime.Now.AddSeconds(-10);
            Assert.IsFalse(this._permission.IsActive());
        }

        /// <summary>
        /// Tests the IsActive method with a start and end time.
        /// </summary>
        [Test]
        public void TestIsActiveStartEndBounds()
        {
            this._permission.StartTime = DateTime.Now.AddSeconds(-10);
            this._permission.EndTime = DateTime.Now.AddSeconds(10);
            Assert.IsTrue(this._permission.IsActive());
            this._permission.StartTime = DateTime.Now.AddSeconds(10);
            this._permission.EndTime = DateTime.Now.AddSeconds(20);
            Assert.IsFalse(this._permission.IsActive());
            this._permission.StartTime = DateTime.Now.AddSeconds(-20);
            this._permission.EndTime = DateTime.Now.AddSeconds(-10);
            Assert.IsFalse(this._permission.IsActive());
        }
    }
}