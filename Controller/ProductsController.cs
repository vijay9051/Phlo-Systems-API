using Microsoft.AspNetCore.Mvc;
using Phlo_Systems_API.Models;
using Phlo_Systems_API.Service;
using System.Text.Json;

namespace Phlo_Systems_API.ProductsController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetdProductsFiltered([FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] string? size, [FromQuery] string highlight)
        {
            try
            {
                _logger.LogInformation("Filter request received. MinPrice: {MinPrice}, MaxPrice: {MaxPrice}, Size: {Size}, Highlight: {Highlight}", minPrice, maxPrice, size, highlight);


                var data = await _productService.GetProductsListAsync();

                _logger.LogDebug("Full response from pastebin: {pastebin}", JsonSerializer.Serialize(data, AppJsonSerializerContext.Default.Root));

                // Get the list of filtered products
                var filteredProducts = data.Products.Where(p =>
                    (!minPrice.HasValue || p.Price >= minPrice.Value) &&
                    (!maxPrice.HasValue || p.Price <= maxPrice.Value) &&
                    (string.IsNullOrEmpty(size) || p.Sizes.Contains(size, StringComparer.OrdinalIgnoreCase))).ToList();


                // Highlight words in descriptions
                if (!string.IsNullOrEmpty(highlight))
                {
                    var wordsToHighlight = highlight.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    foreach (var product in filteredProducts)
                    {
                        foreach (var word in wordsToHighlight)
                        {
                            product.Description = product.Description.Replace(word, $"<em>{word}</em>", StringComparison.OrdinalIgnoreCase);
                        }
                    }
                }

                //get the  filter product object
                var filter = new
                {
                    MinPrice = data.Products.Min(p => p.Price),
                    MaxPrice = data.Products.Max(p => p.Price),
                    Sizes = data.Products.SelectMany(p => p.Sizes).Distinct(),
                    CommonWords = GetCommonWords(data.Products.Select(p => p.Description), 10, 5)
                };

                var result = new FilterProducts
                {
                    filteredProducts = filteredProducts,
                    minPrice = minPrice,
                    maxPrice = maxPrice,
                    size = size,
                    highlight = highlight.Split(",").ToList(),
                };
                _logger.LogDebug("FilterProducts result: {FilterProducts}", result);

                var json = JsonSerializer.Serialize(result, AppJsonSerializerContext.Default.FilterProducts);
                return Ok(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the filter request.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        private static List<string> GetCommonWords(IEnumerable<string> descriptions, int limit, int skip)
        {
            var words = descriptions
                .SelectMany(desc => desc.Split(new[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(word => word.ToLowerInvariant());

            var commonWords = words
                .GroupBy(word => word)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Skip(skip)
                .Take(limit);

            return commonWords.ToList();
        }
    }
}
