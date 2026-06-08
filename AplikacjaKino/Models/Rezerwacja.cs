namespace AplikacjaKino.Models
{
    public class Rezerwacja
    {
        public int Id { get; set; }
        public int NumerRzedu { get; set; }
        public int NumerMiejsca { get; set; }
        public int SeansId { get; set; }
        public virtual Seans? Seans { get; set; }
        public string? UserId { get; set; }
    }
}