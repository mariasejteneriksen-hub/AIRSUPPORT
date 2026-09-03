namespace AIRSUPPORT.Components.Models
{
    public class CleanedCompany
    {
        public string? NavCustomerNo { get; set; }
        public string? CompanyName { get; set; }
        public string? CustomerStatus { get; set; }
        public double PriorityCustomer { get; set; }
        public double PPS { get; set; }
        public double OcFwTerr { get; set; }
        public double OcFwSat { get; set; }
        public double OcNotam { get; set; }
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