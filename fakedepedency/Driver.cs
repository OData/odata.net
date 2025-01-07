using fakeodl;
using System;

namespace fakedepedency
{
    public sealed class Implementaiton : ISomeInterface
    {
        public string Something => "the first value";
    }

    public static class Driver
    {
        public static ISomeInterface AnInstance { get; } = new Implementaiton();

        public static void DoWork(ISomeInterface someInterface)
        {
            Console.WriteLine(someInterface.Something);
        }
    }
}
