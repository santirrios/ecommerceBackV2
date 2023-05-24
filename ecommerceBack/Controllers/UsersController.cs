using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ecommerceBack.models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace ecommerceBack.Controllers;

[ApiController]
[Route("[controller]")]

public class UsersController : ControllerBase
{
    public IConfiguration _configuration;
    public UsersController(IConfiguration configuration) 
    { 
        this._configuration = configuration;
    }
    [HttpGet]
    [Route("createdb")]
    public IResult EnsureCreated([FromServices] EcommerceContext dbContext)
    {
        dbContext.Database.EnsureCreated();
        return Results.Ok("Base de datos creada");
    }



    [HttpPost]
    [Route("login")]
    public dynamic IniciarSesion([FromBody] UserEAndP Data, [FromServices] EcommerceContext dbContext)
    {

        string email = Data.Email;
        string password = Data.Password;

        UserWithId usuario = dbContext.Users.FirstOrDefault( p => p.Email == email && p.Password == password );

        if(usuario == null)
        {
            return new
            {
                success = false,
                message = "Wrong credentials",
                result = ""
            };
        }
        var jwt = _configuration.GetSection("Jwt").Get<Jwt>();

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
            new Claim("id",usuario.UserId.ToString())
        };

        var key = new  SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
        var sigIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            jwt.Issuer,
            jwt.Audience,
            claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: sigIn
            );
        return new
        {
            success = true,
            message = "success creating the token",
            result = new JwtSecurityTokenHandler().WriteToken(token)
        };

    }
    [HttpPost]
    [Route("logup")]
    public dynamic Registrarse([FromBody] User Data, [FromServices] EcommerceContext dbContext)
    {
        var id = new Guid();
        
        if(Data.FirstName == null || Data.LastName == null || Data.Email == null || Data.Password == null || Data.Rol == null)
        {
            return new
            {
                success = false,
                message = "you no fil all of parameters",
                result = ""
            };
        }
        if(Data.Rol == "admin")
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = Jwt.validarToken(identity, dbContext);

            if (!rToken.success) return rToken;

            UserWithId usuarioo = rToken.result;
            if (usuarioo.Rol != "admin")
            {
                return new
                {
                    success = false,
                    message = "you gotta be an admin to create an admin acoount",
                    result = ""
                };
            }
        }
        var usuario = new UserWithId
        {
            UserId = id,
            FirstName = Data.FirstName,
            LastName = Data.LastName,
            Email = Data.Email,
            Password = Data.Password,
            Rol = Data.Rol
        };
        dbContext.Users.Add(usuario);
        dbContext.SaveChanges();
        return new
        {
            success = true,
            message = "success creating user",
            result = ""
        };
    }
    [HttpDelete]
    [Route("delete/{id}")]
    [Authorize]
    public dynamic DeleteAccount([FromServices] EcommerceContext dbContext, [FromRoute] Guid id)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var rToken = Jwt.validarToken(identity, dbContext);

        if (!rToken.success) return rToken;

        UserWithId usuarioo = rToken.result;
        if (usuarioo.UserId != id)
        {
            if (usuarioo.Rol != "admin")
            {
                return new
                {
                    success = false,
                    message = "it's not your account",
                    result = ""
                };
            }
        }
        var user = dbContext.Users.Find(id);
        if(user == null)
        {
            return new
            {
                success = false,
                message = "user not found",
                result = ""
            };
        }
        dbContext.Users.Remove(user);
        dbContext.SaveChanges();
        return new
        {
            success = true,
            message = "user deleted successfully",
            result = ""
        };
    }
    [HttpGet]
    [Route("getall")]
    [Authorize]
    public dynamic GetAllUsers([FromServices] EcommerceContext dbContext)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var rToken = Jwt.validarToken(identity, dbContext);

        if (!rToken.success) return rToken;

        UserWithId usuarioo = rToken.result;
        if (usuarioo.Rol != "admin")
        {
            return new
            {
                success = false,
                message = "you gotta be an admin to create an admin acoount",
                result = ""
            };
        }
        var users = dbContext.Users;
        if(users == null)
        {
            return new
            {
                success = false,
                message = "there are no users yet",
                result = ""
            };
        }
        return new
        {
            success = true,
            message = "returning users",
            result = users
        };
    }
    [HttpGet]
    [Route("getone/{id}")]
    [Authorize]
    public dynamic GetOneUser([FromServices] EcommerceContext dbContext, [FromRoute] Guid id)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var rToken = Jwt.validarToken(identity, dbContext);

        if (!rToken.success) return rToken;

        UserWithId usuarioo = rToken.result;
        if (usuarioo.UserId != id)
        {
            if (usuarioo.Rol != "admin")
            {
                return new
                {
                    success = false,
                    message = "it's not your account",
                    result = ""
                };
            }
        }
        var user = dbContext.Users.Find(id);
        if(user == null)
        {
            return new
            {
                success = false,
                message = "user not found",
                result = ""
            };
        }
        return new
        {
            success = true,
            message = "returning user",
            result = user
        };
    }
    [HttpPut]
    [Route("update/{id}")]
    [Authorize]
    public dynamic UpdateUser([FromServices] EcommerceContext dbContext, [FromBody] User user, [FromRoute] Guid id)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var rToken = Jwt.validarToken(identity, dbContext);

        if (!rToken.success) return rToken;

        UserWithId usuarioo = rToken.result;
        if (usuarioo.UserId != id)
        {
            if(usuarioo.Rol != "admin")
            {
                return new
                {
                    success = false,
                    message = "it's not your account",
                    result = ""
                };
            }
        }
        if(user.Rol == "admin" && usuarioo.Rol != "admin")
        {
            return new
            {
                success = false,
                message = "only admin accounts are able to create or update to admin account",
                result = ""
            };
        }

        var currentUser = dbContext.Users.Find(id);
        if (currentUser == null)
        {
            return new
            {
                success = false,
                message = "user not found",
                result = ""
            };
        }
        currentUser.Email = user.Email;
        currentUser.FirstName = user.FirstName;
        currentUser.LastName = user.LastName;
        currentUser.Password = user.Password;
        currentUser.Rol = user.Rol;
        dbContext.SaveChanges();
        return new
        {
            success = true,
            message = "account updated successfully",
            result = currentUser
        };
    }
    [HttpGet]
    [Route("getuser")]
    public dynamic GetUser([FromServices] EcommerceContext dbContext)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var rToken = Jwt.validarToken(identity, dbContext);

        if (!rToken.success) return rToken;

        UserWithId usuarioo = rToken.result;
        return new
        {
            success = true,
            message = "returning user",
            result = usuarioo
        };
    }
    [HttpGet]
    [Route("verifymail/{mail}")]
    public dynamic VerifyMail([FromServices] EcommerceContext dbContext, [FromRoute] string mail)
    {
        if (string.IsNullOrEmpty(mail))
        {
            return new
            {
                success = false,
                message = "invalid mail",
                result = ""
            };
        }
        var user = dbContext.Users.Where(u => u.Email == mail).FirstOrDefault();
        if (user == null)
        {
            return new
            {
                success = true,
                message = "there is no an account with this email",
                result = ""
            };
        }
        else
        {
            return new
            {
                success = false,
                message = "there is an account with this mail",
                result = ""
            };
        }
    }
}