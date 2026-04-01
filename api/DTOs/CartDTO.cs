using System.ComponentModel.DataAnnotations;

namespace RestuarantAPI.Models;

public class getcartDTO
{
    public List<Cart> cartitems { get; set; }
    public decimal GrandTotal { get; set; }
}

public class CartDTO
{
    public string UserID { get; set; }
    public int ItemID { get; set; }
    public string ItemName { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; }
    
    public decimal ItemPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class setcart
{
    public Item item { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; }
}