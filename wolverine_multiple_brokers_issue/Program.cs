using contracts;
using JasperFx;
using JasperFx.CodeGeneration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using wolverine_multiple_brokers_issue;
using Wolverine;
using Wolverine.RabbitMQ;
using Wolverine.Runtime;
using Wolverine.Transports;

var builder = new HostApplicationBuilder();



builder.UseWolverine(opts =>
{
    
   

    opts.DescribeHandlerMatch(typeof(RandomMessageHandler));
    opts.DescribeHandlerMatch(typeof(RandomMessage2Handler));
    opts.UseRabbitMq(rabbitmq =>
        {
            rabbitmq.HostName = "customer-rabbitmq";
            rabbitmq.UserName = "guest";
            rabbitmq.Password = "guest";
            rabbitmq.VirtualHost = "/";
            rabbitmq.RequestedHeartbeat = new TimeSpan(0, 0, 10);

        }).UseConventionalRouting(NamingSource.FromMessageType) // NamingSource.FromHandlerType breaks with this
        .AutoProvision();

    opts.Policies.DisableConventionalLocalRouting();
    opts.MultipleHandlerBehavior = MultipleHandlerBehavior.Separated;
    opts.Durability.MessageIdentity = MessageIdentity.IdAndDestination;
    opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Static;

   
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