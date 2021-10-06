using System;
using System.ComponentModel.DataAnnotations;

namespace Construct.Core.Database.Model
{
    public class PrintLog
    {
        /// <summary>
        /// Primary key used by Entity Framework. Not intended to be used for anything else.
        /// </summary>
        [Key]
        public long Key { get; set; }
        
        /// <summary>
        /// User who printed.
        /// </summary>
        [Required]
        public User User { get; set; }
        
        /// <summary>
        /// Time of the print.
        /// </summary>
        [Required]
        public DateTime Time { get; set; }
        
        /// <summary>
        /// Name of the file that was printed.
        /// </summary>
        [Required]
        public string FileName { get; set; }
        
        /// <summary>
        /// Material that was printed with.
        /// </summary>
        [Required]
        public PrintMaterial Material { get; set; }
        
        /// <summary>
        /// Weight of the print in grams.
        /// </summary>
        [Required]
        public float WeightGrams { get; set; }
        
        /// <summary>
        /// Purpose of the print.
        /// </summary>
        [Required]
        public string Purpose { get; set; }
        
        /// <summary>
        /// Id to build to. At RIT, this would be MSD / Senior Design.
        /// </summary>
        public string BillTo { get; set; }
        
        /// <summary>
        /// Cost of the print. Separate variable in price of the material changes.
        /// </summary>
        [Required]
        public float Cost { get; set; }
        
        /// <summary>
        /// Whether the print is owed.
        /// </summary>
        [Required]
        public bool Owed { get; set; }
    }
}