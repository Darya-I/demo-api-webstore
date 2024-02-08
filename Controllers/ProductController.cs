using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiDemo_ML_lesson.Data;
using WebApiDemo_ML_lesson.Models;

namespace WebApiDemo_ML_lesson.Controllers
{
    [Route("api/[controller]")]                                             //api/ProductController
    [ApiController]
    public class ProductController : ControllerBase
    {

        private static AppDbContext _context;
        
        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        
        
        [HttpGet]                                                                 // имена маршрутов помогают разбираться в эндпоинтах [Route("GetExpensiveProduct")]
        public async  Task<ActionResult> Get()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);         //Метод FirstOrDefault применяется к коллекциям в C# и
                                                                            //возвращает первый элемент коллекции, удовлетворяющий заданному условию,
                                                                            //или значение по умолчанию (null для ссылочных типов), если такой элемент не найден.
            if (product == null)
            {
                return BadRequest("nepravilni id");
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();                             // сохранить изменения в бд
            return CreatedAtAction("Get", product.Id, product);            // 201 значит запись была добавлен в БД
        }


        [HttpPatch]
        public async Task<IActionResult> Patch(int id, string name)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return BadRequest("nepravilni id");

            }

            product.Name = name;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return BadRequest("nepravilni id");

            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
