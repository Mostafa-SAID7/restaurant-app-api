namespace RestuarantAPI.Models;

public class GetItems
{
    public int ItemID { get; set; }
    public string ItemName { get; set; }
    public string ItemDescription { get; set; }
    public decimal ItemPrice { get; set; }
    public string RestaurantName { get; set; }
    public int RestaurantID { get; set; }
    public string ImageUrl { get; set; }
}