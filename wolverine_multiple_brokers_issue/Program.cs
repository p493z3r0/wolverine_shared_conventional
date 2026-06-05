using System.Diagnostics;
using contracts;
using JasperFx;
using JasperFx.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using wolverine_multiple_brokers_issue;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;
using Wolverine.Runtime;

var builder = new HostApplicationBuilder();

var sharedBroker = new BrokerName("shared");


builder.UseWolverine(opts =>
{
    opts.DescribeHandlerMatch(typeof(RandomMessageHandler));
    opts.UseRabbitMq(rabbitmq =>
        {
            rabbitmq.HostName = "customer-rabbitmq";
            rabbitmq.UserName = "guest";
            rabbitmq.Password = "guest";
            rabbitmq.VirtualHost = "/";
            rabbitmq.RequestedHeartbeat = new TimeSpan(0, 0, 10);

        }).UseConventionalRouting()
        .AutoProvision();
    opts.Policies.DisableConventionalLocalRouting();

   
   
});

var host = builder.Build();

var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("****************************************************");
    Console.WriteLine("DEBUG: Host is fully started and Wolverine is live!");
    Console.WriteLine("****************************************************");

    Task.Delay(TimeSpan.FromSeconds(5)).Wait();
    
    using (var scope = host.Services.CreateScope())
    {
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        bus.PublishAsync(new SomeRandomMessage()).GetAwaiter().GetResult();
        
        
        var runtime = host.Services.GetRequiredService<IWolverineRuntime>();
    
        var router = runtime.RoutingFor(typeof(SomeRandomMessage));
        foreach (var messageRoute in router.Routes)
        {
            Console.WriteLine(messageRoute);
        }
    }
    
});
    


    
host.RunJasperFxCommandsSynchronously(args);