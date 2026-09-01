namespace AIRSUPPORT.Components.Models
{
    public class FormerCostumer : CurrentCostumer
    {
        public DateTime? ChurnDate { get; set; }
        public int? ChurnValue { get; set; }
        public string? ChurnReason { get; set; }
        public Boolean? Avoidable { get; set; }


    }
}
