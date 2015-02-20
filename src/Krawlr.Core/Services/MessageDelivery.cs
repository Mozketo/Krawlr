using ServiceStack.Messaging;
using ServiceStack.RabbitMq;

namespace Krawlr.Core.Services
{
    public interface IMessageQueueServer
    {
        IMessageService Instance(int retryCount = 1);
    }

    public class MessageQueueServer : IMessageQueueServer
    {
        protected IConfiguration _configuration;

        public MessageQueueServer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IMessageService Instance(int retryCount = 1)
        {
            if (_configuration.DistributionMode == DistributionMode.Server || _configuration.DistributionMode == DistributionMode.Client)
                return new RabbitMqServer { RetryCount = retryCount };
            return new InMemoryTransientMessageService { RetryCount = retryCount };
        }
    }
}
