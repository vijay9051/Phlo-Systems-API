using Newtonsoft.Json;
using Phlo_Systems_API.Models;
using System.Data;

namespace Phlo_Systems_API.Service
{
    public class ProductService
    {
        private string _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "products.json");

        public async Task<Root> GetProductsListAsync()
        {
            if (!File.Exists(_jsonFilePath))
                throw new FileNotFoundException("Products file not found");

            var json = await File.ReadAllTextAsync(_jsonFilePath);
            return JsonConvert.DeserializeObject<Root>(json);
        }
    }
}
