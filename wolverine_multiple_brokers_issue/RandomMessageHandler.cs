using contracts;

namespace wolverine_multiple_brokers_issue;

public class RandomMessageHandler
{
    public static void Handle(SomeRandomMessageFromSharedToMain message)
    {
        Console.WriteLine("RandomMessageHandler has been called");
    }
}