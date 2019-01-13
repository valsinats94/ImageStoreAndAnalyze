using ImageStoreAndAnalyze.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Models
{
    public class FamilyRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime SendDate { get; set; }

        [Required]
        public Family RequestedFamily { get; set; }

        [Required]
        public ApplicationUser RequestByUser { get; set; }

        [Required]
        public bool IsProcessed { get; set; }

        public DateTime ProcessedDate { get; set; }

        public ProcessResultTypes ProcessResult { get; set; }

        public ApplicationUser ProcessedByUser { get; set; }

        public Guid Guid { get; set; }

        public bool IsDeleted { get; set; }
    }
}
