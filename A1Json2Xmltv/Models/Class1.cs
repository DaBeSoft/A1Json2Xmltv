namespace A1Json2Xmltv.Models
{
    public class Rootobject<T>
    {
        public Head head { get; set; }
        public T[] data { get; set; }
    }

    public class Head
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }
        public string MTime { get; set; }
    }


    public class StationData
    {
        public Station Station { get; set; }
    }

    public class Station
    {
        public int UID { get; set; }
        public string MobileUID { get; set; }
        public string DisplayName { get; set; }
        public string[] MapIAP { get; set; }
        public object[] Channel { get; set; }
        public string[] Flags { get; set; }
        public string[] Products { get; set; }
        public Logo Logo { get; set; }
    }

    public class Logo
    {
        public string Color { get; set; }
        public string URI { get; set; }
    }

    public class ProgramDetail
    {
        public Event Event { get; set; }
    }

    public class Event
    {
        public string ID { get; set; }
        public object StationUID { get; set; }
        public string DisplayName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Genre { get; set; }
        public object[] Year { get; set; }
        public string Image { get; set; }
        public string Copyright { get; set; }
        public string[][] Person { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }

    }



}
