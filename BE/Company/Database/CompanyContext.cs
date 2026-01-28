using Arc4u.MongoDB.Configuration;
using Domain;
using MongoDB.Bson.Serialization;
using DbContext = Arc4u.MongoDB.DbContext;

namespace Database
{
    public partial class CompanyContext : DbContext
    {
        protected override void OnConfiguring(DbContextBuilder modelBuilder)
        {
            BsonClassMap.RegisterClassMap<Company>(cm =>
            {
                cm.MapIdMember(c => c.Id).SetElementName("_id").SetIsRequired(true);
                cm.MapProperty(c => c.Name).SetElementName("Name").SetIsRequired(true);
                cm.MapProperty(c => c.Vat).SetElementName("Vat").SetIsRequired(true);
            });
            
            modelBuilder.MapCollection("companies").With<Company>();

            
        }
    }
}
