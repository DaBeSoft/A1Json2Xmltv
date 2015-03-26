using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1Dal.Models
{
    public class Station
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StationId { get; set; }
        public string Name { get; set; }

        public virtual List<Show> Shows { get; set; }

        public string StationImageUri { get; set; }
    }
}
