namespace MoexIntegration.Core.Domain.Model.Securities
{
    public class SecurityGroup
    {
        /// <summary>
        /// Название группы
        /// </summary>
        public required string GroupeName { get; set; }

        /// <summary>
        /// Список активов РФ (тикеров) входящих в эту группу
        /// </summary>
        public List<Security> SecurityList { get; set; }
    }
}
