//---------------------------------------------------------------------
// <copyright file="FormatStringData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>Provides information about interesting formatting values.</summary>
    /// <remarks>See http://msdn2.microsoft.com/en-us/library/fbxft59x.aspx for more information.</remarks>
    public sealed class FormatStringData
    {
        private static readonly Type[] IntegralTypes = new Type[] {
            typeof(Byte), typeof(SByte), 
            typeof(Int16), typeof(UInt16),
            typeof(Int32), typeof(UInt32),
            typeof(Int64), typeof(UInt64),
        };

        private static readonly Type[] NumericTypes = new Type[] {
            typeof(Byte), typeof(SByte), 
            typeof(Int16), typeof(UInt16),
            typeof(Int32), typeof(UInt32),
            typeof(Int64), typeof(UInt64),
            typeof(Single),
            typeof(Double),
            typeof(Decimal)
        };

        private static FormatStringData[] values;

        /// <summary>The .NET version of the specified encoding.</summary>
        private Type[] applicableTypes;

        /// <summary>Format specifier string.</summary>
        private string formatSpecifier;

        /// <summary>The string name of the specified encoding.</summary>
        private string name;

        /// <summary>Hideden constructor.</summary>
        private FormatStringData() { }

        /// <summary>Initializes a new dummy EncodingData instance.</summary>
        /// <param name="name">Encoding name.</param>
        /// <returns>A new EncodingData instance.</returns>
        private static FormatStringData ForNumeric(string name, string formatSpecifier, Type[] applicableTypes)
        {
            FormatStringData result = new FormatStringData();
            result.applicableTypes = applicableTypes;
            result.name = name;
            result.formatSpecifier = formatSpecifier;
            return result;
        }

        public static FormatStringData Currency
        {
            get { return FindByName("Currency"); }
        }

        public static FormatStringData Decimal
        {
            get { return FindByName("Decimal"); }
        }

        public static FormatStringData General
        {
            get { return FindByName("General"); }
        }

        public static FormatStringData Hexadecimal
        {
            get { return FindByName("Hexadecimal"); }
        }

        public static FormatStringData Number
        {
            get { return FindByName("Number"); }
        }

        /// <summary>Interesting values for testing formatted types.</summary>
        public static FormatStringData[] Values
        {
            get
            {
                if (values == null)
                {
                    values = new FormatStringData[] {
                        ForNumeric("Currency", "C", NumericTypes),
                        ForNumeric("Decimal", "D", IntegralTypes),
                        ForNumeric("Scientific (exponential)", "E", NumericTypes),
                        ForNumeric("Fixed-point", "F", NumericTypes),
                        ForNumeric("General", "G", NumericTypes),
                        ForNumeric("Number", "N", NumericTypes),
                        ForNumeric("Percent", "P", NumericTypes),
                        ForNumeric("Round-trip", "R", new Type[] { typeof(Single), typeof(Double) }),
                        ForNumeric("Hexadecimal", "X", IntegralTypes),
                    };
                }
                return values;
            }
        }

        /// <summary>Finds a format string data instance by the specified friendly name.</summary>
        /// <param name="name">Name of format string to find.</param>
        /// <returns>The format string data with the specified name.</returns>
        public static FormatStringData FindByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            foreach (FormatStringData result in Values)
            {
                if (result.Name == name)
                {
                    return result;
                }
            }

            throw new ArgumentException("Unable to find format string with name " + name);
        }

        /// <summary>Returns an array of format string data applicable to the specified type.</summary>
        /// <param name="type">Type to get data for.</param>
        /// <returns>An array of format string data applicable to the specified type.</returns>
        public static FormatStringData[] GetApplicableData(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            List<FormatStringData> result = new List<FormatStringData>();
            foreach (FormatStringData data in Values)
            {
                if (data.IsApplicableToType(type))
                {
                    result.Add(data);
                }
            }

            return result.ToArray();
        }

        /// <summary>Checks whether this format string is applicable to the specified type.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if this format string is applicable, false otherwise.</returns>
        public bool IsApplicableToType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            foreach (Type applicableType in this.applicableTypes)
            {
                if (type == applicableType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Format specifier string.</summary>
        public string FormatSpecifier
        {
            get { return this.formatSpecifier; }
        }

        /// <summary>The name for the encoding.</summary>
        public string Name
        {
            get { return this.name; }
        }

        public override string ToString()
        {
            return this.name;
        }
    }
}
