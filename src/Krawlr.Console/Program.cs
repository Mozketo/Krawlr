using DryIoc;
using Krawlr.Core;
using Krawlr.Core.Services;
using Krawlr.Core.Extensions;
using ServiceStack;
using ServiceStack.Messaging;
using System.Linq;

namespace Krawlr.Console
{
    class Program
    {
        enum ExitCode
        {
            Success = 0,
            InvalidArgs,
        }

        static int Main(string[] args)
        {
            using (var container = new Container())
            {
                container.Register<ILog, Log>(Reuse.Singleton);
                var log = container.Resolve<ILog>();

                container.RegisterDelegate<IConfiguration>(r => new ConsoleConfiguration(args), Reuse.Singleton);
                var configuration = container.Resolve<IConfiguration>();
                if (configuration.HasError)
                    return (int)ExitCode.InvalidArgs;
                log.Info($"Starting Krawlr with URL {configuration.BaseUrl}");

                container.Register<IWriterService, CsvWriter>(Reuse.Singleton);

                // If running as a client then register the client components.
                if (configuration.DistributionMode.In(DistributionMode.ClientServer, DistributionMode.Client))
                {
                    container.Register<IWebDriverService, WebDriverService>();
                    container.RegisterDelegate(r => r.Resolve<IWebDriverService>().Get(), Reuse.Singleton);
                    container.Register<IPageActionService, PageActionService>(Reuse.Singleton);
                    container.Register<Page, Page>();
                }

                container.Register<IMessageQueueServer, MessageQueueServer>();
                container.RegisterDelegate<IMessageService>(r => r.Resolve<IMessageQueueServer>().Instance(), Reuse.Singleton);

                if (configuration.DistributionMode.In(DistributionMode.ClientServer, DistributionMode.Client))
                {
                    container.Register<IListen, NavigationListener>(Reuse.Singleton);
                    var navigationListener = container.Resolve<IListen>();
                    navigationListener.Listen();
                }

                //var mqServer = container.Resolve<IMessageService>();
                //log.Warn("Starting MQ server");
                //// Client - MQ Service Impl:
                //mqServer.RegisterHandler<NavigateToUrl>(m =>
                //{
                //    log.Warn("Received: " + m.GetBody().Path + ". Processing for 10s");
                //    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
                //    return "wha?";
                //});
                //mqServer.Start();
                //const string queueName = "x";
                ////Producer - Start publishing messages:
                //using (var mqClient = mqServer.CreateMessageQueueClient())
                //{
                //    for (int i = 0; i < 3; i++)
                //    {
                //        log.Debug("Publishing...");
                //        //mqClient.Publish(new NavigateToUrl { Path = $"ServiceStack {i.ToString()}" });
                //        mqClient.Publish(new Message<NavigateToUrl>(new NavigateToUrl { Path = $"ServiceStack {i.ToString()}" })
                //        {
                //            ReplyTo = queueName
                //        });
                //        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                //    }
                //}
                //log.Warn("Done with MQ server");
                //using (var mqClient = mqServer.CreateMessageQueueClient())
                //{
                //    var response = mqClient.Get<NavigateToUrl>(queueName);
                //    mqClient.Ack(response);
                //}
                //log.Warn("Done with MQ client");
                //while (true)
                //{
                //    System.Threading.Thread.Sleep(1000);
                //    log.Debug($"looping {DateTime.Now}");
                //}

                if (configuration.DistributionMode.In(DistributionMode.ClientServer, DistributionMode.Server))
                {
                    container.Register<IUrlQueueService, UrlQueueService>(Reuse.Singleton);
                    var queueService = container.Resolve<IUrlQueueService>();

                    queueService.Progress += (sender, a) =>
                    {
                        log.Warn($"{a.Remaining} pages remaining. {a.Count} parsed.");
                    };

                    queueService.Add(configuration.BaseUrl);
                    queueService.Add(configuration.Inclusions);

                    while (queueService.Peek)
                    {
                        System.Threading.Thread.Sleep(200);
                    }

                    return 0;
                }

                //container.Resolve<Application>().Start();
                while (true)
                {
                    System.Threading.Thread.Sleep(200);
                }
            }

            return (int)ExitCode.Success;
        }
    }
}
