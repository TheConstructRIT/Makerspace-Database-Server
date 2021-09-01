using System;
using System.ComponentModel.DataAnnotations;

namespace Construct.Core.Database.Model
{
    public class VisitLog
    {
        /// <summary>
        /// User who visited.
        /// </summary>
        [Key]
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