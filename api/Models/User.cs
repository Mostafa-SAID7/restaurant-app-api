using System;
using System.ComponentModel.DataAnnotations;

namespace RestuarantAPI.Models;

public class User
{  
    public string UserEmail{get;set;}
    public string Password{get;set;}
    [Key]
    public string Usercode{get;set;}

}
