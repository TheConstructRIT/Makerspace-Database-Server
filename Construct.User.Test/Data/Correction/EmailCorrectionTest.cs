using System;
using System.Collections.Generic;
using Construct.User.Data.Correction;
using NUnit.Framework;

namespace Construct.User.Test.Data.Correction
{
    public class EmailCorrectionTest
    {
        /// <summary>
        /// Email correction to test.
        /// </summary>
        private EmailCorrection _correction;

        /// <summary>
        /// Sets up the test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this._correction = new EmailCorrection()
            {
                ValidEmails = new List<string>()
                {
                    "email1.test",
                    "email2.test",
                },
                Corrections = new Dictionary<string, string>()
                {
                    { "email3.test", "EMAIL1.test" },
                    { "EMAIL4.test", "email3.test" },
                    { "email5.test", "unknown.email1.test" },
                },
            };
        }
        
        /// <summary>
        /// Tests CorrectEmail with no corrections required.
        /// </summary>
        [Test]
        public void TestCorrectEmailNoCorrections()
        {
            Assert.AreEqual("test@email1.test", this._correction.CorrectEmail("test@email1.test"));
            Assert.AreEqual("test@email2.test", this._correction.CorrectEmail("test@email2.test"));
            Assert.AreEqual("test@email1.test", this._correction.CorrectEmail("TEST@email1.test"));
            Assert.AreEqual("test@email1.test", this._correction.CorrectEmail("test@EMAIL1.test"));
        }
        
        /// <summary>
        /// Tests CorrectEmail with corrections required.
        /// </summary>
        [Test]
        public void TestCorrectEmailCorrections()
        {
            Assert.AreEqual("test@email1.test", this._correction.CorrectEmail("test@email3.test"));
            Assert.AreEqual("test@email1.test", this._correction.CorrectEmail("test@email4.test"));
        }
        
        /// <summary>
        /// Tests CorrectEmail with invalid emails.
        /// </summary>
        [Test]
        public void TestCorrectEmailInvalid()
        {
            Assert.Throws<FormatException>(() =>
            {
                this._correction.CorrectEmail("test@email6.test");
            });
            Assert.Throws<FormatException>(() =>
            {
                this._correction.CorrectEmail("test@email5.test");
            });
            Assert.Throws<FormatException>(() =>
            {
                this._correction.CorrectEmail("not_an_email");
            });
        }
        
        /// <summary>
        /// Tests CorrectEmail with no valid emails specified (all valid).
        /// </summary>
        [Test]
        public void TestCorrectEmailAllValid()
        {
            this._correction.ValidEmails = new List<string>();
            Assert.AreEqual("test@email1.test", this._correction.CorrectEmail("test@email1.test"));
            Assert.AreEqual("test@email2.test", this._correction.CorrectEmail("test@email2.test"));
            Assert.AreEqual("test@email1.test", this._correction.CorrectEmail("test@email3.test"));
            Assert.AreEqual("test@email1.test", this._correction.CorrectEmail("test@email4.test"));
            Assert.AreEqual("test@unknown.email1.test", this._correction.CorrectEmail("test@email5.test"));
            Assert.AreEqual("test@email6.test", this._correction.CorrectEmail("test@email6.test"));
        }
    }
}