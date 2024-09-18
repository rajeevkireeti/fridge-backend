using Microsoft.AspNetCore.Mvc;
using FridgeAPI.Data;
using FridgeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FridgeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientController : ControllerBase
    {
        private readonly FridgeContext _context;

        public IngredientController(FridgeContext context)
        {
            _context = context;
        }

        // GET: api/Ingredient
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetAllIngredients()
        {
            return await _context.Ingredients.ToListAsync();
        }

        // POST: api/Ingredient
        [HttpPost]
        public async Task<ActionResult<Ingredient>> AddIngredient(Ingredient ingredient)
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetIngredientById), new { id = ingredient.Id }, ingredient);
        }

        // GET: api/Ingredient/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Ingredient>> GetIngredientById(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            return ingredient;
        }

        // DELETE: api/Ingredient/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PUT: api/Ingredient/{id} -- Update Ingredient
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIngredient(int id, Ingredient ingredient)
        {
            if (id != ingredient.Id)
            {
                return BadRequest("ID mismatch");
            }

            _context.Entry(ingredient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredientExists(id))
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

        private bool IngredientExists(int id)
        {
            return _context.Ingredients.Any(e => e.Id == id);
        }
    }
}
