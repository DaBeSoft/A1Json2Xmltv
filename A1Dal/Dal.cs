using System.Collections.Generic;
using System.Linq;
using A1Dal.Models;
using System.Data.Entity;

namespace A1Dal
{
    public class Dal
    {
        private readonly Context _context;

        public Dal()
        {
            _context = new Context();

            //Create all Tables and stuff, because EntityFramework crashes if DB doesnt exist....
            var g = new Genre { A1Name = "Test", DvbName = "Test" };
            _context.Genres.Add(g);
            _context.SaveChanges();
            _context.Genres.Remove(g);
            _context.SaveChanges();
        }

        private Genre AddGenre(string name)
        {
            var g = _context.Genres.Local.SingleOrDefault(ge => ge.A1Name == name) ??
                    _context.Genres.SingleOrDefault(ge => ge.A1Name == name);


            if (g == null)
            {
                g = new Genre { A1Name = name, DvbName = name };
                _context.Genres.Add(g);
            }

            return g;
        }

        public Show AddShow(Station s, int id, string start, string end, string name, string genre, int year, string imageUri, string copyright, string description, int stationId, string subName)
        {
            var show = _context.Shows.Local.SingleOrDefault(g => g.ShowId == id) ??
                       _context.Shows.SingleOrDefault(g => g.ShowId == id);

            if (show == null)
            {
                show = new Show
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
                };
                _context.Shows.Add(show);
            }

            return show;
        }

        public Station AddStation(int id, string name, string imageUri)
        {
            var s = _context.Stations.Local.SingleOrDefault(g => g.StationId == id) ??
                    _context.Stations.SingleOrDefault(g => g.StationId == id);

            if (s == null)
            {
                s = new Station
                {
                    Name = name,
                    StationId = id,
                    StationImageUri = imageUri
                };
                _context.Stations.Add(s);

            }

            return s;
        }

        public List<Station> GetStations()
        {
            return _context.Stations.Include(i => i.Shows.Select(t => t.Genre)).ToList();
        }


        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
