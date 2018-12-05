using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Customer.WebSite.Models
{
    public class CustomerLoginDTO
    {
        [Required(ErrorMessage = "This field is required")]
        [MaxLength(20)]
        [DisplayName("Identity Card")]
        public string IdentityCard { get; set; }
    }
}