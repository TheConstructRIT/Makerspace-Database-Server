using System;
using System.ComponentModel.DataAnnotations;

namespace Construct.Core.Database.Model
{
    public class Permission
    {
        /// <summary>
        /// Primary key used by Entity Framework. Not intended to be used for anything else.
        /// </summary>
        [Key]
        public long Key { get; set; }
        
        /// <summary>
        /// User with the permission.
        /// </summary>
        [Required]
        public User User { get; set; }
        
        /// <summary>
        /// Name of the permission.
        /// </summary>
        [Required]
        public string Name { get; set; }
        
        /// <summary>
        /// Start time of the permission.
        /// </summary>
        public DateTime? StartTime { get; set; }
        
        /// <summary>
        /// End time of the permission.
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}