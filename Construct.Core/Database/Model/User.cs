using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Construct.Core.Database.Model
{
    public class User
    {
        /// <summary>
        /// Hashed (University) id of the user.
        /// </summary>
        [Key]
        [Required]
        public string HashedId { get; set; }
        
        /// <summary>
        /// Name of the user.
        /// </summary>
        [Required]
        public string Name { get; set; }
        
        /// <summary>
        /// Email of the user.
        /// </summary>
        [Required]
        public string Email { get; set; }
        
        /// <summary>
        /// Permissions of the user.
        /// </summary>
        public List<Permission> Permissions { get; set; }
        
        /// <summary>
        /// Sign up time of the user.
        /// </summary>
        public DateTime? SignUpTime { get; set; }
    }
}