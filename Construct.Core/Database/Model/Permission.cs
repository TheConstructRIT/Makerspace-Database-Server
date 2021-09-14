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

        /// <summary>
        /// Returns if the permission is active.
        /// </summary>
        /// <returns>Whether the permission is active.</returns>
        public bool IsActive()
        {
            // Return if the permission hasn't started or the permission has expired.
            if ((this.StartTime.HasValue && this.StartTime > DateTime.Now) || (this.EndTime.HasValue && this.EndTime < DateTime.Now))
            {
                return false;
            }
            
            // Return true (permission active).
            return true;
        }
    }
}