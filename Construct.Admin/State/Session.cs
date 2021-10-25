using System;
using System.Collections.Generic;
using System.Linq;
using Construct.Core.Configuration;

namespace Construct.Admin.State
{
    public class Session
    {
        /// <summary>
        /// Max sessions an identifier can have.
        /// </summary>
        public int MaxSessions { get; set; } = 5;

        /// <summary>
        /// Max duration of a session in seconds.
        /// </summary>
        public long MaxSessionDuration { get; set; } = 60 * 60;

        /// <summary>
        /// Static session instance to use.
        /// </summary>
        private static Session _staticSession;
        
        /// <summary>
        /// Active sessions for the identifiers, including their expire time.
        /// </summary>
        private readonly Dictionary<string, List<KeyValuePair<string, DateTime>>> _sessions = new Dictionary<string, List<KeyValuePair<string, DateTime>>>();

        /// <summary>
        /// Returns a static instance of the session.
        /// </summary>
        /// <returns></returns>
        public static Session GetSingleton()
        {
            return _staticSession ??= new Session()
            {
                MaxSessions = Math.Max(0, ConstructConfiguration.Configuration.Admin.MaximumUserSessions),
                MaxSessionDuration = Math.Max(0, ConstructConfiguration.Configuration.Admin.MaximumUserSessionDuration),
            };
        }

        /// <summary>
        /// Creates a new session for the given identifier.
        /// </summary>
        /// <param name="identifier">Identifier to create the session under.</param>
        /// <returns>The session string to use.</returns>
        public string CreateSession(string identifier)
        {
            // Create the identifier storage if it doesn't exist.
            if (!this._sessions.ContainsKey(identifier))
            {
                this._sessions[identifier] = new List<KeyValuePair<string, DateTime>>();
            }
            
            // Create and add the session to the end of the list.
            var newSession = Guid.NewGuid().ToString();
            var sessions = this._sessions[identifier];
            sessions.Add(new KeyValuePair<string, DateTime>(newSession, DateTime.Now.AddSeconds(this.MaxSessionDuration)));
            
            // Remove sessions until the maximum sessions is maintained.
            while (sessions.Count > this.MaxSessions)
            {
                sessions.RemoveAt(0);
            }
            
            // Return the created session.
            return newSession;
        }
        
        /// <summary>
        /// Returns the identifier for a session. Returns null if the
        /// session does not exist or is expired.
        /// </summary>
        /// <param name="sessionString">Session string to check.</param>
        /// <returns>The identifier for a session.</returns>
        public string GetIdentifier(string sessionString)
        {
            // Iterate over the identifiers.
            foreach (var (identifier, sessions) in this._sessions)
            {
                // Ignore if the session is not linked to the identifier.
                if (sessions.All(session => session.Key != sessionString)) continue;
                
                // Remove expired sessions.
                while (sessions.Count > 0 && sessions[0].Value < DateTime.Now)
                {
                    sessions.RemoveAt(0);
                }
                
                // Return if the session string still exists.
                // If the session expired, null will be returned because it was removed.
                return sessions.Any(session => session.Key == sessionString) ? identifier : null;
            }
            
            // Return null (not found).
            return null;
        }
        
        /// <summary>
        /// Returns if a session is valid (exists and isn't expired).
        /// </summary>
        /// <param name="sessionString">Session string to check.</param>
        /// <returns>Whether the session is valid.</returns>
        public bool SessionValid(string sessionString)
        {
            return this.GetIdentifier(sessionString) != null;
        }
    }
}