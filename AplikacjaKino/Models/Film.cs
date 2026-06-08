using System.ComponentModel.DataAnnotations;

namespace AplikacjaKino.Models
{
    public class Film
    {
        public int Id { get; set; }

        [Required]
        public string Tytul { get; set; }

        [Required]
        public string Opis { get; set; }

        [Range(1, 600, ErrorMessage = "Czas trwania musi byæ miêdzy 1 a 600 minut.")]
        public int CzasTrwaniaMin { get; set; }

        public string? Okladka { get; set; }
    }
}