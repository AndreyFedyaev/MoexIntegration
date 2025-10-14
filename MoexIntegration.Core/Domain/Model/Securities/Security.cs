namespace MoexIntegration.Core.Domain.Model.Securities
{
    public class Security
    {
        public required string Ticker { get; set; }           // SECID
        public required string ShortName { get; set; }        // SHORTNAME
        public required string ISIN { get; set; }             // ISIN
    }
}
