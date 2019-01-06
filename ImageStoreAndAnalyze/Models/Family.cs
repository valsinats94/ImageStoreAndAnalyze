using ImageStoreAndAnalyze.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Models
{
    public class Family : IFamily
    {
        [Key]
        public int ID { get; set; }
        public ICollection<FamilyUsers> FamilyUsers { get; set; }
    }
}
