namespace AplikacjaKino.Models
{
    public class RezerwacjaWieleModel
    {
        public int SeansId { get; set; }
        public string? UserId { get; set; }
        public List<MiejsceModel> Miejsca { get; set; } = new List<MiejsceModel>();
    }

    public class MiejsceModel
    {
        public int NumerRzedu { get; set; }
        public int NumerMiejsca { get; set; }
    }
}