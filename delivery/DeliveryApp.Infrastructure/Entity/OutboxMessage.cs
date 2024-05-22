#nullable enable
namespace DeliveryApp.Infrastructure.Entities
{
    /// <summary>
    /// Outbox
    /// </summary>
    public sealed class OutboxMessage
    {
        /// <summary>
        /// Уникальный идентификатор сообщения
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Тип сообщения
        /// </summary>
        public string Type { get; set; } = String.Empty;

        /// <summary>
        /// Тело сообщения (полезная информация)
        /// </summary>
        public string Content { get; set; } = String.Empty;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime OccuredOnUtc { get; set; }

        /// <summary>
        /// Дата публикации
        /// </summary>
        public DateTime? ProcessedOnUtc { get; set; }

        /// <summary>
        /// Ошибка, если есть
        /// </summary>
        public string? Error { get; set; }
    }
}