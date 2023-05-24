using System.Security.Claims;

namespace ecommerceBack.models;

    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }

    public static dynamic validarToken(ClaimsIdentity identity, EcommerceContext dbContext)
    {
        try
        {
            if(identity.Claims.Count() == 0)
            {
                return new
                {
                    success = false,
                    message = "verify if you sent a valid token",
                    result = ""
                };
            }

            var id = identity.Claims.FirstOrDefault(x => x.Type == "id").Value;

            UserWithId usuario = dbContext.Users.FirstOrDefault(p => p.UserId.ToString() == id);
            if (usuario == null)
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
                message = "success finding user",
                result = usuario
            };
        }catch (Exception ex)
        {
            return new
            {
                success = false,
                message = ex.Message,
                result = ""
            };
        }
    }
    }

