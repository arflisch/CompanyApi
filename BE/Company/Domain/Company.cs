namespace Domain
{
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Vat { get; set; } = null!;
    }
}
