
namespace MoexIntegration.Core.Application.Handling.Contracts
{
    public sealed record MoexGetDataResponse
    {
        public required double PRICE  { get; set; }
        public required int ISSUESIZE { get; set; }
    }
}
