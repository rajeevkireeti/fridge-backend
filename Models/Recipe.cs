// Models/Recipe.cs
namespace FridgeAPI.Models
{
    public class Recipe
    {
        public int Id { get; set; }  // Recipe ID
        public string Name { get; set; } = string.Empty;  // Recipe Name
        public string Instructions { get; set; } = string.Empty;  // Steps/Instructions
        public int PreparationTime { get; set; }  // Time to prepare the recipe (in minutes)

        // The collection of ingredients associated with the recipe
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; }

        // Constructor to initialize the RecipeIngredients collection
        public Recipe()
        {
            RecipeIngredients = new List<RecipeIngredient>();
        }
    }
}