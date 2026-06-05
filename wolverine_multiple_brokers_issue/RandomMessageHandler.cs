using contracts;

namespace wolverine_multiple_brokers_issue;

public class RandomMessageHandler
{
    public static void Handle(SomeRandomMessage message)
    {
        Console.WriteLine("RandomMessageHandler has been called");
    }
}

public class RandomMessageHandler2
{
    public static void Handle(SomeRandomMessage message)
    {
        Console.WriteLine("RandomMessageHandler has been called 2");
    }
}