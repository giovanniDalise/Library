namespace Library.MailService.Core.Ports
{
    public interface IEventPublisherPort
    {
        Task PublishAsync<T>(T @event, string queueName);
    }
}
