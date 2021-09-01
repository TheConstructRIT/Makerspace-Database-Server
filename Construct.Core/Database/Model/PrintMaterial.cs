using System.ComponentModel.DataAnnotations;

namespace Construct.Core.Database.Model
{
    public class PrintMaterial
    {
        /// <summary>
        /// Name of the material.
        /// </summary>
        [Key]
        [Required]
        public string Name { get; set; }
        
        /// <summary>
        /// Cost of the material per gram.
        /// </summary>
        [Required]
        public float CostPerGram { get; set; }
    }
}