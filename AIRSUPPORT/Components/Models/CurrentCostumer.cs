namespace AIRSUPPORT.Components.Models
{
    public class CurrentCostumer
    {
        public string? ID { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }//overvej at slette, fordi de burde jo være current 
        public string? Country { get; set; }
        public DateTime? CostumerSince { get; set; }
        public string? CancelTerms { get; set; }//tjek om der er mange naan-værdier
        public int? PriceEscalation { get; set; }//tjek om der er mange naan-værdier


    }

    }
