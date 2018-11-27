using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Friends
    {
        [Key, ForeignKey("Package")]
        public int FriendsId { get; set; }

        public Package Package { get; set; }

        public string FirstNumber { get; set; }
        public string SecondNumber { get; set; }
        public string ThirdNumber { get; set; }
    }
}
