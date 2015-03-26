using System.Collections.Generic;
using System.Linq;
using A1Dal.Models;

namespace A1Dal
{
    public class Dal
    {
        private readonly Context _context;

        public Dal()
        {
            _context = new Context();
            var g = new Genre { A1Name = "Test", DvbName = "Test" }; //Create all Tables and stuff, because EntityFramework crashes if DB doesnt exist....
            _context.Genres.Add(g);
            _context.SaveChanges();
            _context.Genres.Remove(g);
            _context.SaveChanges();
        }

        private Genre AddGenre(string name)
        {
            var g = _context.Genres.FirstOrDefault(ge => ge.A1Name == name);

            if (g == null)
            {
                g = new Genre { A1Name = name, DvbName = name };

                _context.Genres.Add(g);
                _context.SaveChanges();
            }

            return g;
        }

        public void AddShow(Station s, int id, string start, string end, string name, string genre, int year, string imageUri, string copyright, string description, int stationId, string subName)
        {
            if (_context.Shows.SingleOrDefault(g => g.ShowId == id) == null)
            {
                _context.Shows.Add(new Show
                {
                    ShowId = id,
                    End = end,
                    Start = start,
                    Name = name,
                    Copyright = copyright,
                    Description = description,
                    Genre = AddGenre(genre),
                    ImageUri = imageUri,
                    Station = s,
                    SubName = subName,
                    Year = year
                });
            }
        }

        public Station AddStation(int id, string name, string imageUri)
        {
            var s = _context.Stations.SingleOrDefault(g => g.StationId == id);

            if (s == null)
            {
                s = new Station
                {
                    Name = name,
                    StationId = id,
                    StationImageUri = imageUri
                };
                _context.Stations.Add(s);
                _context.SaveChanges();

            }

            return s;
        }

        public List<Station> GetStations()
        {
            return _context.Stations.ToList();
        }


        public void SaveShows()
        {
            _context.SaveChanges();
        }
    }
}
