namespace AplikacjaKino.Models
{
    public class Seans
    {
        public int Id { get; set; }
        public DateTime DataGodzina { get; set; }

        public int FilmId { get; set; }
        public virtual Film? Film { get; set; }

        public int SalaId { get; set; }
        public virtual Sala? Sala { get; set; }

        public virtual ICollection<Rezerwacja> Rezerwacje { get; set; } = new List<Rezerwacja>();
    }
}