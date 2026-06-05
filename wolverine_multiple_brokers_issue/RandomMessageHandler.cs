using contracts;
using Wolverine;

namespace wolverine_multiple_brokers_issue;

public class RandomMessageHandler : Saga 
{
    public Guid Id { get; set; }
    public  void Start(SomeRandomMessage message)
    {
        Console.WriteLine("RandomMessageHandler has been called");
        
        Id = Guid.NewGuid();
    }
}

public class RandomMessage2Handler
{
    public static void Handle(SomeRandomMessage message)
    {
        Console.WriteLine("RandomMessageHandler has been called 2");
    }
}