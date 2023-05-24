using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ecommerceBack.models;

public class Category
{
    public Guid CategoryId {get; set;}
    public string Name {get; set;}
    public string Description {get;set;}

    public List<Product> Products { get; set;}
}
public class AddCategory
{
    public string Name { get; set;}
    public string Description { get; set;}
}