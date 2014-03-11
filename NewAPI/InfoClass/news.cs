using System.Collections.Generic;

namespace NewAPI.InfoClass
{
    class news
    {
        public List<Video> Video = new List<Video> { };
        public List<Image> img = new List<Image> { };
        public string source_id { get; set; } //Откуда взялась новость
        public string post_id { get; set; } //Откуда взялась новость
        public int Date { get; set; }
        public string Text { get; set; }
        public string Audio { get; set; }
        public string Poll { get; set; }
    }
}
