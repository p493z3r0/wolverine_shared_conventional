using contracts;
using JasperFx;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using shared_service;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = new HostApplicationBuilder();

builder.UseWolverine(opts =>
{
    opts.UseRabbitMq(rabbitmq =>
    {
        rabbitmq.HostName = "shared-rabbitmq";
        rabbitmq.UserName = "guest";
        rabbitmq.Password = "guest";
        rabbitmq.VirtualHost = "/";
        rabbitmq.RequestedHeartbeat = new TimeSpan(0, 0, 10);

    }).UseConventionalRouting()
    .AutoProvision();

    Console.WriteLine("****************************************************");
    opts.DescribeHandlerMatch(typeof(HelloHandler));


});

var host =builder.Build();
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
        
    
         bus.PublishAsync(new SomeRandomMessageFromSharedToMain()).GetAwaiter().GetResult();
    }
    
    
});

host.RunJasperFxCommandsSynchronously(args);