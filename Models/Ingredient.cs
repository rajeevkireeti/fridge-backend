namespace FridgeAPI.Models
{
    public class Ingredient
    {
        public int Id { get; set; }

        // Ensure that the Name property is not nullable by assigning a default value
        public string Name { get; set; } = string.Empty;

        // Optional: You can add any additional properties or relationships here, for example:

        // Navigation property if each ingredient can be part of many recipes
        // public ICollection<RecipeIngredient> RecipeIngredients { get; set; }

        // Optional: Any other attributes of the ingredient like quantity, unit, etc.
        // public string Unit { get; set; } = string.Empty;  // Example: kg, g, tsp, etc.
    }
}
