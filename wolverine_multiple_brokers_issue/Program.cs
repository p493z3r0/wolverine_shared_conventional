using contracts;
using JasperFx;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using wolverine_multiple_brokers_issue;
using Wolverine;
using Wolverine.RabbitMQ;

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

    opts.AddNamedRabbitMqBroker(sharedBroker, rabbitmq =>
    {
        rabbitmq.HostName = "shared-rabbitmq";
        rabbitmq.UserName = "guest";
        rabbitmq.Password = "guest";
        rabbitmq.VirtualHost = "/";
        rabbitmq.RequestedHeartbeat = new TimeSpan(0, 0, 10);
    }).UseConventionalRouting().AutoProvision();

   var types = typeof(ISharedMessage).Assembly.GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(ISharedMessage))).ToList();
   
   foreach (var type in types)
   {
       opts
           .PublishMessage(type)
           .ToRabbitExchangeOnNamedBroker(sharedBroker, type.FullName!);
       Console.WriteLine($"DEBUG: {type.FullName} is published to {sharedBroker.Name} exchange");
   }
   
});

var app = builder.Build();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("****************************************************");
    Console.WriteLine("DEBUG: Host is fully started and Wolverine is live!");
    Console.WriteLine("****************************************************");

    Task.Delay(TimeSpan.FromSeconds(5)).Wait();
    
    using (var scope = app.Services.CreateScope())
    {
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        bus.PublishAsync(new HelloToShared()).GetAwaiter().GetResult();
    }
    
});
    
    
app.RunJasperFxCommandsSynchronously(args);