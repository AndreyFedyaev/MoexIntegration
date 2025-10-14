using MoexIntegration.Core.Domain.Model.Securities;

namespace MoexIntegration.Core.Abstractions
{
    public interface ICacheService
    {
        Task UpdateSecurity(List<Security> data);
    }
}
