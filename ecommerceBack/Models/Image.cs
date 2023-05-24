using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ecommerceBack.models;

public class Image
{
    public Guid ImageId {get; set;}
    public string URL {get;set;}

    public Guid ProductId { get; set; }
    [JsonIgnore]
    public Product Product { get; set; }

}