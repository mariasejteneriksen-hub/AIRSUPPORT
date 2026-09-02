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
    }
}