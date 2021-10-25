using System.Threading;
using Construct.Admin.State;
using NUnit.Framework;

namespace Construct.Admin.Test.State
{
    public class SessionTest
    {
        /// <summary>
        /// Session under test.
        /// </summary>
        private Session _session;

        /// <summary>
        /// Creates the test Session.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this._session = new Session()
            {
                MaxSessions = 3,
                MaxSessionDuration = 1,
            };
        }

        /// <summary>
        /// Tests getting a single instance.
        /// </summary>
        [Test]
        public void TestSingleton()
        {
            Assert.AreEqual(Session.GetSingleton(), Session.GetSingleton());
        }
        
        /// <summary>
        /// Tests creating the maximum amount of sessions.
        /// </summary>
        [Test]
        public void TestMaxSessions()
        {
            var initialSession = this._session.CreateSession("test");
            Assert.AreNotEqual(initialSession, this._session.CreateSession("test"));
            Assert.IsTrue(this._session.SessionValid(initialSession));
            Assert.AreEqual("test", this._session.GetIdentifier(initialSession));
            Assert.AreNotEqual(initialSession, this._session.CreateSession("test"));
            Assert.IsTrue(this._session.SessionValid(initialSession));
            Assert.AreEqual("test", this._session.GetIdentifier(initialSession));
            Assert.AreNotEqual(initialSession, this._session.CreateSession("test"));
            Assert.IsFalse(this._session.SessionValid(initialSession));
            Assert.AreNotEqual("test", this._session.GetIdentifier(initialSession));
        }
        
        /// <summary>
        /// Tests expiring sessions.
        /// </summary>
        [Test]
        public void TestExpiringSessions()
        {
            var initialSession = this._session.CreateSession("test");
            Assert.IsTrue(this._session.SessionValid(initialSession));
            Assert.AreEqual("test", this._session.GetIdentifier(initialSession));
            Thread.Sleep(500);
            Assert.IsTrue(this._session.SessionValid(initialSession));
            Assert.AreEqual("test", this._session.GetIdentifier(initialSession));
            Thread.Sleep(750);
            Assert.IsFalse(this._session.SessionValid(initialSession));
            Assert.IsNull(this._session.GetIdentifier(initialSession));
        }
        
        /// <summary>
        /// Tests getting the identifiers of sessions.
        /// </summary>
        [Test]
        public void TestGetIdentifier()
        {
            Assert.IsNull(this._session.GetIdentifier("unknown"));
            Assert.AreEqual("test", this._session.GetIdentifier(this._session.CreateSession("test")));
            Assert.AreEqual("test2", this._session.GetIdentifier(this._session.CreateSession("test2")));
        }
    }
}