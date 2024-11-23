﻿using System.Text.Json.Serialization;

namespace Phlo_Systems_API.Models
{
    [JsonSerializable(typeof(FilterProducts))]
    [JsonSerializable(typeof(Product))]
    public partial class JsonContext : JsonSerializerContext { }
    public class FilterProducts
    {
        public List<Product> filteredProducts { get; set; }
        public decimal? minPrice { get; set; }
        public decimal? maxPrice { get; set; }
        public List<string> highlight { get; set; }
        public string size { get; set; }
    }
}