using MoexIntegration.Core.Domain.Model.Securities;

namespace MoexIntegration.Core.Abstractions
{
    public interface ICacheService
    {
        Task WriteSecurities(List<Security> data);
        Task<List<Security>> GetSecurities();
    }
}
