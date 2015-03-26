using System;

namespace A1Dal.Models
{
    public class Show
    {
        public int ShowId { get; set; }

        //public int StationId { get; set; }
        public Station Station { get; set; }

        //public int GenreId { get; set; }
        public Genre Genre { get; set; }

        public string Start { get; set; }
        public string End { get; set; }

        public string Name { get; set; }
        public string SubName { get; set; }

        public int Year { get; set; }

        public string ImageUri { get; set; }

        public string Copyright { get; set; }

        public string Description { get; set; }

    }
}
