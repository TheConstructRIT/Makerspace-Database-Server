using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Construct.User.Data.Correction
{
    public class EmailCorrection
    {
        /// <summary>
        /// Emails that are valid.
        /// </summary>
        public List<string> ValidEmails { get; set; }= new List<string>();

        /// <summary>
        /// Changes to make to emails to make them valid.
        /// </summary>
        public Dictionary<string, string> Corrections { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Corrects an email to a valid email. Throws an FormatException if the email is invalid.
        /// </summary>
        /// <param name="email">Email to correct.</param>
        /// <returns>Corrected email to use.</returns>
        public string CorrectEmail(string email)
        {
            // Correct the emails until no corrects can be made.
            var address = new MailAddress(email.ToLower());
            while (true)
            {
                // Replace the end of the email.
                var changeMade = false;
                foreach (var (originalEmail, newEmail) in this.Corrections)
                {
                    if (address.Host != originalEmail.ToLower()) continue;
                    address = new MailAddress(address.User + "@" + newEmail.ToLower());
                    changeMade = true;
                    break;
                }
                
                // Break the loop if no change was made.
                if (!changeMade) break;
            }
            
            // Return if no valid emails are specified.
            if (this.ValidEmails.Count == 0)
            {
                return address.Address;
            }
            
            // Return the email if one is valid.
            foreach (var validEmail in this.ValidEmails)
            {
                if (address.Host != validEmail.ToLower()) continue;
                return address.Address;
            }
            
            // Throw an exception (no valid email).
            throw new FormatException("Email does not end with a valid email.");
        }
    }
}