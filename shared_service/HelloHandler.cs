using contracts;

namespace shared_service;

public class HelloHandler
{
    public static void Handle(HelloToShared message)
    {
        Console.WriteLine("HelloHandler has been called");
    }
}