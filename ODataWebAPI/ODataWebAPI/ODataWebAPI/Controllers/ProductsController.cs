using ODataWebAPI.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
namespace ProductService.Controllers
{
    public class ProductsController : ODataController
    {
        ProductsContext db = new ProductsContext();
        private bool ProductExists(int key)
        {
            return db.products.Any(p => p.Id == key);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        [EnableQuery]
        public IQueryable<Products> Get()
        {
            return db.products;
        }
        [EnableQuery]
        public SingleResult<Products> Get([FromODataUri] int key)
        {
            IQueryable<Products> result = db.products.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }
        public async Task<IHttpActionResult> Post(Products product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.products.Add(product);
            await db.SaveChangesAsync();
            return Created(product);
        }
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Products> product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await db.products.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            product.Patch(entity);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(entity);
        }
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Products update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.Id)
            {
                return BadRequest();
            }
            db.Entry(update).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(update);
        }
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var product = await db.products.FindAsync(key);
            if (product == null)
            {
                return NotFound();
            }
            db.products.Remove(product);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}