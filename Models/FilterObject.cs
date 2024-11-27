namespace Phlo_Systems_API.Models
{
    public class FilterObject
    {
        public decimal? minPrice { get; set; }
        public decimal? maxPrice { get; set; }
        public List<string> highlight { get; set; }
        public List<string>mostCommonWords { get; set; }
        public string size { get; set; }
    }
}
