using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FakeRestuarantAPI.Models;

public class OrderDTO
{
    [Required]
    public string ItemName { get; set; }
    
    [Required]
    public int Quantity { get; set; }
}

public class setorderDTO
{
    public string UserID { get; set; }
    public string ItemName { get; set; }
    public int Quantity { get; set; }

    [Precision(10, 2)]
    public decimal ItemPrice { get; set; }

    [Precision(10, 2)]
    public decimal TotalPrice { get; set; }

    public int MasterID { get; set; }
}

public class FullOrderDTO
{
    public List<Order> fullorder { get; set; }
    public decimal GrandTotal { get; set; }
}

public class MasterOrderDTO
{
    public int MasterID { get; set; }
    public string UserID { get; set; }
    public int RestaurantID { get; set; }
    public decimal GrandTotal { get; set; }
}

public class masterorders
{
    public List<Order> orders { get; set; }
}