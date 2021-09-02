using System;
using System.ComponentModel.DataAnnotations;

namespace Construct.Core.Database.Model
{
    public class VisitLog
    {
        /// <summary>
        /// Primary key used by Entity Framework. Not intended to be used for anything else.
        /// </summary>
        [Key]
        public long Key { get; set; }
        
        /// <summary>
        /// User who visited.
        /// </summary>
        [Required]
        public User User { get; set; }
        
        /// <summary>
        /// Source of the visit entry.
        /// </summary>
        [Required]
        public string Source { get; set; }
        
        /// <summary>
        /// Time of the visit.
        /// </summary>
        [Required]
        public DateTime Time { get; set; }
    }
}