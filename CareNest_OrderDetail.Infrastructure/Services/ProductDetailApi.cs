using CareNest_OrderDetail.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace CareNest_OrderDetail.Infrastructure.Services
{
    public class ProductDetailApi : IProductDetailApi
    {
        private readonly HttpClient _httpClient;

        public ProductDetailApi(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            var baseUrl = configuration["ProductDetailApi:BaseUrl"] ?? "http://localhost:8016";
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<ProductDetailDto?> GetByIdAsync(string productDetailId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"/api/ProductDetails/{productDetailId}", cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();
            var dto = await response.Content.ReadFromJsonAsync<ProductDetailDto>(cancellationToken: cancellationToken);
            return dto;
        }

        public async Task<bool> UpdateAsync(string productDetailId, ProductDetailUpdateDto request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/ProductDetails/{productDetailId}", request, cancellationToken);
            return response.IsSuccessStatusCode;
        }
    }
}


