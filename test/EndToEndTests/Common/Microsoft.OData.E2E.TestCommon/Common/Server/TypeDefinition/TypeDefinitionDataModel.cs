//---------------------------------------------------------------------
// <copyright file="TypeDefinitionDataModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.TypeDefinition;

public class Address
{
    public string? Road { get; set; }
    public string? City { get; set; }
}

public class Person
{
    public int PersonId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public Height? Height { get; set; }

    public Address? Address { get; set; }

    public Collection<string>? Descriptions { get; set; }
}

public class NumberCombo
{
    public UInt16 Small { get; set; }
    public UInt32 Middle { get; set; }
    public UInt64 Large { get; set; }
}

public class Product
{
    public UInt16 ProductId { get; set; }

    public UInt32 Quantity { get; set; }

    public UInt32? NullableUInt32 { get; set; }

    public UInt64 LifeTimeInSeconds { get; set; }

    public NumberCombo? TheCombo { get; set; }

    public Temperature? Temperature { get; set; }

    public Collection<UInt64>? LargeNumbers { get; set; }
}

// TypeDefinition for Height
public class Height
{
    public enum DistanceUnit
    {
        M,
        CM,
        FT,
        IN,
    }

    public Height()
    {
    }

    public Height(string value)
    {
        int index = 0;
        for (; index < value.Length; index++)
        {
            if (!char.IsDigit(value[index]) && value[index] != '.')
            {
                break;
            }
        }

        string left = value.Substring(0, index);
        string right = value.Substring(index);

        Value = double.Parse(left);
        Unit = Enum.Parse<DistanceUnit>(right.ToUpper());
    }

    public double Value { get; set; }

    public DistanceUnit Unit { get; set; }

    public override string ToString()
    {
        return $"{Value}{Unit.ToString().ToLowerInvariant()}";
    }
}

// TypeDefinition for Temperature
public class Temperature
{
    public Temperature(string temp)
    {
        if (temp.EndsWith("℃"))
        {
            Kind = TemperatureKind.Celsius;

            _ = double.TryParse(temp.AsSpan(0, temp.Length - 1), out double value);
            Celsius = value;
            Fahrenheit = ToFahrenheit(Celsius);
        }
        else if (temp.EndsWith("℉"))
        {
            Kind = TemperatureKind.Fahrenheit;

            _ = double.TryParse(temp.AsSpan(0, temp.Length - 1), out double value);
            Fahrenheit = value;
            Celsius = ToCelsius(Fahrenheit);
        }
        else
        {
            throw new Exception("Unknown format for temperature");
        }
    }

    public Temperature(double temp, TemperatureKind kind)
    {
        Kind = kind;

        if (kind == TemperatureKind.Celsius)
        {
            Celsius = temp;
            Fahrenheit = ToFahrenheit(temp);
        }
        else
        {
            Celsius = ToCelsius(temp);
            Fahrenheit = temp;
        }
    }

    public TemperatureKind Kind { get; }

    public double Celsius { get; }

    public double Fahrenheit { get; }

    private static double ToCelsius(double fahrenheit)
    {
        return (fahrenheit - 32) * 5 / 9;
    }

    private static double ToFahrenheit(double celsius)
    {
        return celsius * 9 / 5 + 32;
    }

    public override string ToString()
    {
        if (Kind == TemperatureKind.Celsius)
        {
            return $"{Celsius:0.00}\x2103";
        }
        else
        {
            return $"{Fahrenheit:0.00}\x2109";
        }
    }
}

public enum TemperatureKind
{
    Celsius,
    Fahrenheit
}
