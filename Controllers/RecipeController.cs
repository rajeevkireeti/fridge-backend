using Microsoft.AspNetCore.Mvc;
using FridgeAPI.Data;
using FridgeAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        private readonly FridgeContext _context;

        public RecipeController(FridgeContext context)
        {
            _context = context;
        }

        // GET: api/Recipe
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAllRecipes()
        {
            return await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();
        }

        // POST: api/Recipe
        [HttpPost]
        public async Task<ActionResult<Recipe>> AddRecipe(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRecipeById), new { id = recipe.Id }, recipe);
        }

        // PUT: api/Recipe/{id} (Update a recipe)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecipe(int id, Recipe recipe)
        {
            if (id != recipe.Id)
            {
                return BadRequest();
            }

            // Find the existing recipe
            var existingRecipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existingRecipe == null)
            {
                return NotFound();
            }

            // Update the recipe properties
            existingRecipe.Name = recipe.Name;
            existingRecipe.Instructions = recipe.Instructions;
            existingRecipe.PreparationTime = recipe.PreparationTime;

            // Clear existing ingredients and add the updated ones
            existingRecipe.RecipeIngredients.Clear();
            foreach (var ri in recipe.RecipeIngredients)
            {
                existingRecipe.RecipeIngredients.Add(ri);
            }

            // Mark the recipe as modified
            _context.Entry(existingRecipe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // GET: api/Recipe/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipeById(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return recipe;
        }

        // DELETE: api/Recipe/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Recipe/by-ingredients
        [HttpGet("by-ingredients")]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipesByIngredients([FromQuery] List<int> ingredientIds)
        {
            var recipes = await _context.Recipes
                .Where(r => ingredientIds.All(id => r.RecipeIngredients.Any(ri => ri.IngredientId == id))) // Match all selected ingredients
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();

            if (recipes.Count == 0)
            {
                return NotFound("No recipes found with the selected ingredients.");
            }

            return Ok(recipes);
        }

        // GET: api/Recipe/by-exact-ingredients
        [HttpGet("by-exact-ingredients")]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipesByExactIngredients([FromQuery] List<int> ingredientIds)
        {
            var recipes = await _context.Recipes
                .Where(r => r.RecipeIngredients.Select(ri => ri.IngredientId).OrderBy(id => id)
                             .SequenceEqual(ingredientIds.OrderBy(id => id)))
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();

            return Ok(recipes);
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }
    }
}

