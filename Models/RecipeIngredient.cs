// Models/RecipeIngredient.cs
namespace FridgeAPI.Models
{
    public class RecipeIngredient
    {
        public int RecipeId { get; set; }
        public int IngredientId { get; set; }
        public Recipe Recipe { get; set; }  // Reference to Recipe
        public Ingredient Ingredient { get; set; }  // Reference to Ingredient
        // Other properties...
    }
}