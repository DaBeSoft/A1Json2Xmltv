using System.Data.Entity;

namespace A1Dal.Models
{
    class Context : DbContext
    {
        public Context()
            : base("Data Source=localhost;Integrated Security=True;Initial Catalog=A1Epg;")
        {

        }

        public DbSet<Show> Shows { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Station> Stations { get; set; } 
    }
}
