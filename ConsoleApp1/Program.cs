using fakedepedency;
using System;

public sealed class Program
{
    public static void Main(string[] args)
    {
        Driver.DoWork(Driver.AnInstance);
    }
}