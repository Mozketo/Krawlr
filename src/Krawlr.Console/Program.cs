using DryIoc;
using Krawlr.Core;
using Krawlr.Core.DTO;
using Krawlr.Core.Services;
using MZMemoize.Extensions;
using OpenQA.Selenium;
using ServiceStack;
using ServiceStack.Messaging;
using ServiceStack.RabbitMq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

                container.Register<IWriterService, WriterService>(Reuse.Singleton);

                container.Register<IWebDriverService, WebDriverService>();
                container.RegisterDelegate(r => r.Resolve<IWebDriverService>().Get(), Reuse.Singleton);

                //var mqServer = container.Resolve<IMessageService>();

                container.Register<IPageActionService, PageActionService>(Reuse.Singleton);
                container.Register<Page, Page>();
                //container.Register<Application, Application>(Reuse.Singleton);

                container.Register<IMessageQueueServer, MessageQueueServer>();
                container.RegisterDelegate<IMessageService>(r => r.Resolve<IMessageQueueServer>().Instance(), Reuse.Singleton);
                container.Register<IListen, NavigationListener>(Reuse.Singleton);

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

                var navigationListener = container.Resolve<IListen>();
                navigationListener.Listen();

                container.Register<IUrlQueueService, UrlQueueService>(Reuse.Singleton);
                var queueService = container.Resolve<IUrlQueueService>();
                queueService.Add(configuration.BaseUrl);
                queueService.Add(configuration.Inclusions);

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
