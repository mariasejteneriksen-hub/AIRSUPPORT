namespace AIRSUPPORT.Components.Models
{
    public class CleanedCompany
    {
        public string? NavCustomerNo { get; set; }
        public string? CompanyName { get; set; }
        public string? CustomerStatus { get; set; }
        public double PriorityCustomer { get; set; }
        public double TailsOnPPS { get; set; }
        public double TailsFlightWatchTerrestrial { get; set; }
        public double TailsFlightWatchSatellite { get; set; }
        public double TailsNotamMonitoring { get; set; }
        public DateTime? CustomerSince { get; set; }
        public decimal? PriceEscalation { get; set; }
        public DateTime? Terminationdate { get; set; }
        public  double? ChurnValue { get; set; }
        public string? Avoidable { get; set; }
        public string? ChurnReason { get; set; }
        public DateTime? BlockedDate { get; set; }
        public string? BlockedReason { get; set; }
       
    }
}