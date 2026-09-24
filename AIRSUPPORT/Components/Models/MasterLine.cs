namespace AIRSUPPORT.Components.Models
{
    public class MasterLine
    {
        public string? CustomerNr { get; set; }
        public string? SellToCustomerNo { get; set; }
        public string? BillToCustomerNo { get; set; }
        public string? DocumentNo { get; set; }
        public int LineNo { get; set; }
        public int Type { get; set; }
        public string? ItemNr { get; set; }
        public string? Description { get; set; }
        public string? Description2 { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double UnitPriceLCY { get; set; }
        public double Amount { get; set; }
        public string? Currency { get; set; }
        public double CurrencyFactor { get; set; }
        public double AmountLCY { get; set; }
        public double AmountInclVAT { get; set; }
        public double VATPercent { get; set; }
        public string? PostingGroupUpdated { get; set; }
        public string? GenProdPostingGroup { get; set; }
        public string? VATBusPostingGroup { get; set; }
        public string? SalesType { get; set; }
        public DateTime? PostingDate { get; set; }
        public string? NavCustomerNo { get; set; }
    }
}
