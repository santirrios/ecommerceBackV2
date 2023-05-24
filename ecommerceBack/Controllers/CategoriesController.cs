using ecommerceBack.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ecommerceBack.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController: ControllerBase
    {
        [HttpPost]
        [Route("add")]
        [Authorize]
        public dynamic AddCategory([FromServices] EcommerceContext dbContext, [FromBody] AddCategory category)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = Jwt.validarToken(identity, dbContext);

            if (!rToken.success) return rToken;

            UserWithId usuario = rToken.result;
            if(usuario.Rol != "admin")
            {
                return new
                {
                    success = false,
                    message = "you're not admin",
                    result = ""
                };
            }
            Category newCategory = new Category()
            {
                CategoryId = Guid.NewGuid(),
                Name = category.Name,
                Description = category.Description
            };
            dbContext.Categories.Add(newCategory);
            dbContext.SaveChanges();

            return new
            {
                success = true,
                message = "success creating category",
                result = new
                {
                    CategoryId = newCategory.CategoryId,
                    Name = newCategory.Name,
                    Description = newCategory.Description
                }
            };
        }
        [HttpGet]
        [Route("getall")]
        public dynamic GetAllCategory([FromServices] EcommerceContext dbContext)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = Jwt.validarToken(identity, dbContext);

            if (!rToken.success) return rToken;

            var categories = dbContext.Categories
                .Include(c => c.Products)
                    .ThenInclude(p => p.Images)
                .ToList();

            if (categories.Count == 0)
            {
                return new
                {
                    success = false,
                    message = "there are no categories yet",
                    result = ""
                };
            }
            return new
            {
                success = true,
                message = "returning categories and products",
                result = categories
            };
        }
        [HttpGet]
        [Route("getone/{id}")]
        public dynamic GetOneCategory([FromServices] EcommerceContext dbContext, [FromRoute] Guid id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = Jwt.validarToken(identity, dbContext);

            if (!rToken.success) return rToken;
            
            var category = dbContext.Categories
                .Where(c=> c.CategoryId == id)
                .Include(c => c.Products)
                .ThenInclude(p => p.Images).FirstOrDefault();
            if(category == null)
            {
                return new
                {
                    success = false,
                    message = "category not found",
                    result = ""
                };
            }
            return new
            {
                success = true,
                message = "returning category with products",
                result = category
            };
        }
        [HttpPut]
        [Route("update/{id}")]
        [Authorize]
        public dynamic UpdateCategory([FromServices] EcommerceContext dbContext,[FromBody]AddCategory category, [FromRoute] Guid id)
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
            var currentCategory = dbContext.Categories.Find(id);
            if(currentCategory == null)
            {
                return new
                {
                    success = false,
                    message = "category not found",
                    result = ""
                };
            }
            currentCategory.Name = category.Name;
            currentCategory.Description = category.Description;
            dbContext.SaveChanges();
            return new
            {
                success = true,
                message = "category changed",
                result = new
                {
                    CategoryId=id,
                    Name = category.Name,
                    Description = category.Description
                }
            };
        }
        [HttpDelete]
        [Route("delete/{id}")]
        [Authorize]
        public dynamic DeleteCategory([FromServices] EcommerceContext dbContext, [FromRoute] Guid id)
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
            var Category = dbContext.Categories.Find(id);
            if(Category == null)
            {
                return new
                {
                    success = false,
                    message = "category not found",
                    result = ""
                };
            }
            dbContext.Categories.Remove(Category);
            dbContext.SaveChanges();
            return new
            {
                success = true,
                message = "category and products inside of this category deleted",
                result = ""
            };
        }
    }
}
