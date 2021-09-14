using System;
using Construct.Core.Database.Model;
using NUnit.Framework;

namespace Construct.Core.Test.Database.Model
{
    public class PermissionTest
    {
        /// <summary>
        /// Tests the IsActive method.
        /// </summary>
        [Test]
        public void TestIsActive()
        {
            // Check that an empty permission with null start and end times is active.
            var permission = new Permission();
            Assert.IsTrue(permission.IsActive());
            
            // Check that a start date is active.
            permission.StartTime = DateTime.Now.AddSeconds(-10);
            Assert.IsTrue(permission.IsActive());
            permission.StartTime = DateTime.Now.AddSeconds(10);
            Assert.IsFalse(permission.IsActive());
            permission.StartTime = null;
            
            // Check that an end date is active.
            permission.EndTime = DateTime.Now.AddSeconds(10);
            Assert.IsTrue(permission.IsActive());
            permission.EndTime = DateTime.Now.AddSeconds(-10);
            Assert.IsFalse(permission.IsActive());
            permission.EndTime = null;
            
            // Check with both start and end dates.
            permission.StartTime = DateTime.Now.AddSeconds(-10);
            permission.EndTime = DateTime.Now.AddSeconds(10);
            Assert.IsTrue(permission.IsActive());
            permission.StartTime = DateTime.Now.AddSeconds(10);
            permission.EndTime = DateTime.Now.AddSeconds(20);
            Assert.IsFalse(permission.IsActive());
            permission.StartTime = DateTime.Now.AddSeconds(-20);
            permission.EndTime = DateTime.Now.AddSeconds(-10);
            Assert.IsFalse(permission.IsActive());
        }
    }
}