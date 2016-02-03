using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace photo.Models
{
    public class Purchase
    {
        public string PaymentMethod { get; set; }
        public Decimal Price { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string postalCode { get; set; }
        public string photoName { get; set; }
        public string file { get; set; }
        public string size { get; set; }
        public string cost { get; set; }
        public string PayerID { get; set; }
        public string PaymentId { get; set; }
    }



    public class ImageInfo
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public string FileLocation { get; set; }
    }
}