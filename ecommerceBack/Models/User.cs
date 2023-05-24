using System.ComponentModel.DataAnnotations;
namespace ecommerceBack.models;


public class UserEAndP
{
    [Required]
    [MaxLength(100)]
    public string Email { get; set; }
    [Required]
    [MaxLength(100)]
    public string Password { get; set; }
}
public class User:UserEAndP
{
    public string FirstName {get;set;}
    public string LastName {get;set;}
    public string Rol { get;set;}
}
public class UserWithId:User
{
    public Guid UserId { get; set; }
}