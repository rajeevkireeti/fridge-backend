using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class OpenAIController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAIController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");  // Fetch the API key from environment variable
    }

    public class RecipeRequest
    {
        public string[] IngredientNames { get; set; }
        public string SelectedCuisine { get; set; }
    }

    [HttpPost("recipes")]
    public async Task<IActionResult> GetRecipes([FromBody] RecipeRequest recipeRequest)
    {
        if (recipeRequest?.IngredientNames == null || recipeRequest.IngredientNames.Length == 0)
        {
            return BadRequest("The ingredients field is required.");
        }

        var ingredientList = string.Join(", ", recipeRequest.IngredientNames);
        var cuisinePrompt = string.IsNullOrWhiteSpace(recipeRequest.SelectedCuisine)
            ? ""
            : $" in {recipeRequest.SelectedCuisine} cuisine";

        var prompt = $"Please provide 5 recipes using the following ingredients: {ingredientList}{cuisinePrompt}. For each recipe, include the following fields clearly labeled: \n1. Title \n2. Ingredients \n3. Instructions \n4. Cook time in minutes.";

        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = "You are a helpful assistant that provides recipes." },
                new { role = "user", content = prompt }
            },
            max_tokens = 800,
            temperature = 0.7
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        var responseText = await response.Content.ReadAsStringAsync();
        return Ok(responseText);
    }
}
