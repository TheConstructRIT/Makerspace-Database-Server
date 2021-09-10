using System.ComponentModel.DataAnnotations;

namespace Construct.Core.Database.Model
{
    public class Student
    {
        /// <summary>
        /// Primary key used by Entity Framework. Not intended to be used for anything else.
        /// User is not able to be the primary key.
        /// </summary>
        [Key]
        public long Key { get; set; }
        
        /// <summary>
        /// User with the student information.
        /// </summary>
        [Required]
        public User User { get; set; }
        
        /// <summary>
        /// College the user is part of.
        /// </summary>
        public string College { get; set; }
        
        /// <summary>
        /// Year of the user.
        /// </summary>
        public string Year { get; set; }
    }
}