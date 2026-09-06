namespace Library.IdentityService.Core.Ports
{
    public interface IEventPublisherPort
    {
        Task PublishAsync<T>(T @event, string queueName);
    }
}
