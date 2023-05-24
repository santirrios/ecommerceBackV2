using Microsoft.AspNetCore.Mvc;
using ecommerceBack.models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace ecommerceBack.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController:ControllerBase
    {
        [HttpGet]
        [Route("getone/{id}")]
        public dynamic GetOneProduct([FromServices] EcommerceContext dbContext, [FromRoute] Guid id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = Jwt.validarToken(identity, dbContext);

            if (!rToken.success) return rToken;

            var product = dbContext.Products.Where(p=>p.ProductId == id).Include(p => p.Images).FirstOrDefault();
            if(product == null)
            {
                return new
                {
                    success = false,
                    message = "product not found",
                    result = ""
                };
            }
            return new
            {
                success = true,
                message = "returning product",
                result = product
            };
        }

        [HttpGet]
        [Route("getall")]
        public dynamic GetAllProducts([FromServices] EcommerceContext dbContext)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = Jwt.validarToken(identity, dbContext);

            if (!rToken.success) return rToken;

            var products = dbContext.Products.Include(p => p.Images).ToList();
            if(products.Count == 0)
            {
                return new
                {
                    success = false,
                    message = "there are no products yet",
                    result = ""
                };
            }
            return new
            {
                success = true,
                message = "returning products",
                result = products
            };
        }
        [HttpPost]
        [Route("add")]
        [Authorize]
        public dynamic AddProduct([FromBody] AddProduct product, [FromServices] EcommerceContext dbContext) 
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = Jwt.validarToken(identity, dbContext);

            if (!rToken.success) return rToken;

            UserWithId usuario = rToken.result;
            if (usuario.Rol != "admin")
            {
                return new
                {
                    success = false,
                    message = "you're not admin",
                    result = ""
                };
            }
            Product newProduct = new Product();
            newProduct.ProductId = Guid.NewGuid();
            newProduct.Name = product.Name;
            newProduct.Description = product.Description;
            newProduct.Price = product.Price;
            newProduct.CategoryId = product.CategoryId;
            var existCategory = dbContext.Categories.FirstOrDefault(c => c.CategoryId == newProduct.CategoryId);
            if(existCategory == null)
            {
                return new
                {
                    success = false,
                    message = $"category with this id:{product.CategoryId} doesn't exist",
                    result = ""
                };
            }
            dbContext.Products.Add(newProduct);
            if(product.URLS.Count > 0) 
            {
                foreach (var url in product.URLS)
                {
                    var image = new Image();
                    image.ProductId = newProduct.ProductId;
                    image.ImageId = Guid.NewGuid();
                    image.URL = url;
                    dbContext.Images.Add(image);
                }
            }
            try
            {
                dbContext.SaveChanges();
            }
            catch(Exception ex)
            {
                return new
                {
                    success = false,
                    message = ex.Message,
                    reult = ""
                };
            }
            
            return new
            {
                success = true,
                message = "success creating product",
                result = new
                {
                    ProductId = newProduct.ProductId,
                    Name = newProduct.Name,
                    Description = newProduct.Description,
                    Price = newProduct.Price,
                    CategoryId = newProduct.CategoryId,
                    URLS = product.URLS,
                }
            };
        }
        [HttpPut]
        [Route("update/{id}")]
        [Authorize]
        public dynamic UpdateProduct([FromServices] EcommerceContext dbContext, [FromBody] AddProduct product, [FromRoute] Guid id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = Jwt.validarToken(identity, dbContext);

            if (!rToken.success) return rToken;

            UserWithId usuario = rToken.result;
            if (usuario.Rol != "admin")
            {
                return new
                {
                    success = false,
                    message = "you're not admin",
                    result = ""
                };
            }
            var productoActual = dbContext.Products.Find(id);
            if(productoActual == null)
            {
                return new
                {
                    success = false,
                    message = "product not found",
                    result = ""
                };
            }
            productoActual.CategoryId = product.CategoryId;
            productoActual.Name = product.Name;
            productoActual.Description = product.Description;
            productoActual.Price = product.Price;
            
            var imagenActual = dbContext.Images.Where(i => i.ProductId == id);
            if(imagenActual != null)
            {
                dbContext.RemoveRange(imagenActual);
            }
            foreach (var url in product.URLS)
            {
                var image = new Image();
                image.ProductId = id;
                image.ImageId = Guid.NewGuid();
                image.URL = url;
                dbContext.Images.Add(image);
            }
            dbContext.SaveChanges();

            return new
            {
                success = true,
                message = "updated successfully",
                result = new
                {
                    ProductId = id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                    URLS = product.URLS
                }
            };
        }
        [HttpDelete]
        [Route("delete/{id}")]
        [Authorize]
        public dynamic DeleteProduct([FromServices] EcommerceContext dbContext, [FromRoute] Guid id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = Jwt.validarToken(identity, dbContext);

            if (!rToken.success) return rToken;

            UserWithId usuario = rToken.result;
            if (usuario.Rol != "admin")
            {
                return new
                {
                    success = false,
                    message = "you're not admin",
                    result = ""
                };
            }
            var productoActual = dbContext.Products.Find(id);

            if(productoActual == null)
            {
                return new
                {
                    success = false,
                    message = "product not found",
                    result = ""
                };
            }
            dbContext.Products.Remove(productoActual);
            dbContext.SaveChanges();
            return new
            {
                success = true,
                message = "success deleting product",
                result = ""
            };
        }
    }
}
