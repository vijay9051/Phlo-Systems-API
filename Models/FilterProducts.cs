using System.Text.Json.Serialization;

namespace Phlo_Systems_API.Models
{
    [JsonSerializable(typeof(FilterProducts))]
    [JsonSerializable(typeof(Product))]
    public partial class JsonContext : JsonSerializerContext { }
    public class FilterProducts
    {
        public List<Product> filteredProducts { get; set; }
        public FilterObject filterObject { get; set; }
    }

}
