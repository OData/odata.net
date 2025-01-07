using fakedepedency;
using fakeodl;
using System;

public sealed class Program
{
    public static void Main(string[] args)
    {
        Driver.DoWork(Driver.AnInstance);
        UseTheInterface(Driver.AnInstance);
    }

    public static void UseTheInterface(ISomeInterface someInterface)
    {
        Console.WriteLine(someInterface.AnotherThing());
    }
}