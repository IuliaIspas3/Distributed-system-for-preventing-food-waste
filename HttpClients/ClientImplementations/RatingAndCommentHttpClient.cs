﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Domain.DTOs;
using HttpClients.ClientInterfaces;

namespace HttpClients.ClientImplementations;

public class RatingAndCommentHttpClient : IRatingAndCommentService
{
    private readonly HttpClient _client;
    private readonly IAuthService _authService;

    public RatingAndCommentHttpClient(HttpClient client, IAuthService authService)
    {
        _client = client;
        _authService = authService;
    }

    public async Task CreateRatingAsync(RatingBasicDTO dto)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",_authService.token);
        _client.DefaultRequestHeaders.Add("MustBeCustomer", "customer");
        HttpResponseMessage message = await _client.PostAsJsonAsync("/Ratings", dto);
        if (!message.IsSuccessStatusCode)
        {
            string content = await message.Content.ReadAsStringAsync();
            throw new Exception(content);
        }    
    }

    public async Task CreateCommentAsync(CommentBasicDTO dto)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",_authService.token);
        _client.DefaultRequestHeaders.Add("MustBeCustomer", "customer");
        HttpResponseMessage message = await _client.PostAsJsonAsync("/Comments", dto);
        if (!message.IsSuccessStatusCode)
        {
            string content = await message.Content.ReadAsStringAsync();
            throw new Exception(content);
        } 
    }

    public async Task UpdateRatingAsync(RatingBasicDTO dto)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",_authService.token);
        _client.DefaultRequestHeaders.Add("MustBeCustomer", "customer");
        HttpResponseMessage message = await _client.PatchAsJsonAsync("/Ratings", dto);
        if (!message.IsSuccessStatusCode)
        {
            string content = await message.Content.ReadAsStringAsync();
            throw new Exception(content);
        } 
    }

    public async Task DeleteCommentAsync(int commentId)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",_authService.token);
        _client.DefaultRequestHeaders.Add("MustBeCustomer", "customer");
        HttpResponseMessage message = await _client.DeleteAsync($"/Comments/{commentId}");
        if (!message.IsSuccessStatusCode)
        {
            string content = await message.Content.ReadAsStringAsync();
            throw new Exception(content);
        } 
    }

    public async Task<List<ReadCommentDTO>> ReadCommentsByFoodSellerIdAsync(int foodSellerId)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",_authService.token);
        HttpResponseMessage response = await _client.GetAsync($"/Comments/{foodSellerId}");
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        List<ReadCommentDTO> comments = JsonSerializer.Deserialize<List<ReadCommentDTO>>(content, new JsonSerializerOptions{PropertyNameCaseInsensitive = true})!;
        return comments;
    }

    public async Task<double> ReadAverageRatingByFoodSellerIdAsync(int foodSellerId)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",_authService.token);
        HttpResponseMessage response = await _client.GetAsync($"/Ratings/{foodSellerId}");
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }
        
        if (double.TryParse(content, out double averageRating))
        {
            return averageRating;
        }
        throw new InvalidOperationException("Unable to parse the average rating.");

    }

    public async Task<int> ReadRatingAsync(ReadRatingDTO dto)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",_authService.token);
        HttpResponseMessage response = await _client.PostAsJsonAsync("/Ratings", dto);
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        if (int.TryParse(content, out int rating))
        {
            return rating;
        }
        throw new InvalidOperationException("Unable to parse the rating.");
    }
}