using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;

namespace Domain
{
    public class Company
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; } = null!;
        public string Vat { get; set; } = null!;
    }
}
