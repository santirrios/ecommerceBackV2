using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ecommerceBack.models;

public class Product
{
    public Guid ProductId {get; set;}
    public string Name {get; set;}
    public string Description {get; set;}
    public double Price {get;set;}

    public Guid CategoryId { get; set; }
    [JsonIgnore]
    public virtual Category Category { get; set;}


    public List<Image> Images { get; set;}
}
public class AddProduct
{
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public Guid CategoryId { get; set; }
    public List<string> URLS { get; set;}
}