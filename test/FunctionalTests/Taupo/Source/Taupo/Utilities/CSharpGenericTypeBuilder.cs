//---------------------------------------------------------------------
// <copyright file="CSharpGenericTypeBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
#if WINDOWS_PHONE || WIN8
    using System;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
   
    /// <summary>
    /// Creates a generic type which is required as the projected type of an anonymous type projection
    /// </summary>
    [ImplementationName(typeof(IAnonymousTypeBuilder), "CSharpGeneric", HelpText = "C# generic types created by using open generic type definitions")]
    public class CSharpGenericTypeBuilder : IAnonymousTypeBuilder
    {
        /// <summary>
        /// Returns an anonymous type that contains members with given names. Returned type is actually a generic type definition.
        /// </summary>
        /// <param name="memberNames">Names of members of the anonymous type.</param>
        /// <returns>Anonymous type containing given members.</returns>
        public Type GetAnonymousType(params string[] memberNames)
        {
            int countOfTypeParameters = memberNames.Length;
            ExceptionUtilities.Assert(memberNames.Any(), "There should  be at least one member being projected");
            ExceptionUtilities.Assert(memberNames.Length <= 13, "We currently support only 13 members being projected into an anonymous type");

            // NETCF does not support Open generics with reflection,
            // hence Type.GetType("Generic`1") will throw a NotSupportedException
            // We have to use this if condition to figure out which type to return
            Type result = typeof(GenericType<>);
            if (countOfTypeParameters == 1)
            {
                return typeof(GenericType<>);
            }
            else if (countOfTypeParameters == 2)
            {
                return typeof(GenericType<,>);
            }
            else if (countOfTypeParameters == 3)
            {
                return typeof(GenericType<,,>);
            }
            else if (countOfTypeParameters == 4)
            {
                return typeof(GenericType<,,,>);
            }
            else if (countOfTypeParameters == 5)
            {
                return typeof(GenericType<,,,,>);
            }
            else if (countOfTypeParameters == 6)
            {
                return typeof(GenericType<,,,,,>);
            }
            else if (countOfTypeParameters == 7)
            {
                return typeof(GenericType<,,,,,,>);
            }
            else if (countOfTypeParameters == 8)
            {
                return typeof(GenericType<,,,,,,,>);
            }
            else if (countOfTypeParameters == 9)
            {
                return typeof(GenericType<,,,,,,,,>);
            }
            else if (countOfTypeParameters == 10)
            {
                return typeof(GenericType<,,,,,,,,,>);
            }
            else if (countOfTypeParameters == 11)
            {
                return typeof(GenericType<,,,,,,,,,,>);
            }
            else if (countOfTypeParameters == 12)
            {
                return typeof(GenericType<,,,,,,,,,,,>);
            }
            else if (countOfTypeParameters == 13)
            {
                return typeof(GenericType<,,,,,,,,,,,,>);
            }

            return result;
        }

        /// <summary>
        /// Generic Type definition which accepts 1 type parameter(s)
        /// </summary> 
        /// <typeparam name="T1">The type of the property Field1</typeparam>
        public class GenericType<T1>
        {
            /// <summary>
            /// Initializes a new instance of the GenericType class.
            /// </summary> 
            /// <param name="field1">The value of the property Field1</param>
            public GenericType(T1 field1)
            {
                this.Field1 = field1;
            }

            /// <summary>
            /// Gets or sets a value for Field1
            /// </summary>
            public T1 Field1 { get; set; }
        }

        /// <summary>
        /// Generic Type definition which accepts 2 type parameter(s)
        /// </summary> 
        /// <typeparam name="T1">The type of the property Field1</typeparam> 
        /// <typeparam name="T2">The type of the property Field2</typeparam>
        public class GenericType<T1, T2>
        {
            /// <summary>
            /// Initializes a new instance of the GenericType class.
            /// </summary> 
            /// <param name="field1">The value of the property Field1</param> 
            /// <param name="field2">The value of the property Field2</param>
            public GenericType(T1 field1, T2 field2)
            {
                this.Field1 = field1;
                this.Field2 = field2;
            }

            /// <summary>
            /// Gets or sets a value for Field1
            /// </summary>
            public T1 Field1 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field2
            /// </summary>
            public T2 Field2 { get; set; }
        }

        /// <summary>
        /// Generic Type definition which accepts 3 type parameter(s)
        /// </summary> 
        /// <typeparam name="T1">The type of the property Field1</typeparam> 
        /// <typeparam name="T2">The type of the property Field2</typeparam> 
        /// <typeparam name="T3">The type of the property Field3</typeparam>
        public class GenericType<T1, T2, T3>
        {
            /// <summary>
            /// Initializes a new instance of the GenericType class.
            /// </summary> 
            /// <param name="field1">The value of the property Field1</param> 
            /// <param name="field2">The value of the property Field2</param> 
            /// <param name="field3">The value of the property Field3</param>
            public GenericType(T1 field1, T2 field2, T3 field3)
            {
                this.Field1 = field1;
                this.Field2 = field2;
                this.Field3 = field3;
            }

            /// <summary>
            /// Gets or sets a value for Field1
            /// </summary>
            public T1 Field1 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field2
            /// </summary>
            public T2 Field2 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field3
            /// </summary>
            public T3 Field3 { get; set; }
        }

        /// <summary>
        /// Generic Type definition which accepts 4 type parameter(s)
        /// </summary> 
        /// <typeparam name="T1">The type of the property Field1</typeparam> 
        /// <typeparam name="T2">The type of the property Field2</typeparam> 
        /// <typeparam name="T3">The type of the property Field3</typeparam> 
        /// <typeparam name="T4">The type of the property Field4</typeparam>
        public class GenericType<T1, T2, T3, T4>
        {
            /// <summary>
            /// Initializes a new instance of the GenericType class.
            /// </summary> 
            /// <param name="field1">The value of the property Field1</param> 
            /// <param name="field2">The value of the property Field2</param> 
            /// <param name="field3">The value of the property Field3</param> 
            /// <param name="field4">The value of the property Field4</param>
            public GenericType(T1 field1, T2 field2, T3 field3, T4 field4)
            {
                this.Field1 = field1;
                this.Field2 = field2;
                this.Field3 = field3;
                this.Field4 = field4;
            }

            /// <summary>
            /// Gets or sets a value for Field1
            /// </summary>
            public T1 Field1 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field2
            /// </summary>
            public T2 Field2 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field3
            /// </summary>
            public T3 Field3 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field4
            /// </summary>
            public T4 Field4 { get; set; }
        }

        /// <summary>
        /// Generic Type definition which accepts 5 type parameter(s)
        /// </summary> 
        /// <typeparam name="T1">The type of the property Field1</typeparam> 
        /// <typeparam name="T2">The type of the property Field2</typeparam> 
        /// <typeparam name="T3">The type of the property Field3</typeparam> 
        /// <typeparam name="T4">The type of the property Field4</typeparam> 
        /// <typeparam name="T5">The type of the property Field5</typeparam>
        public class GenericType<T1, T2, T3, T4, T5>
        {
            /// <summary>
            /// Initializes a new instance of the GenericType class.
            /// </summary> 
            /// <param name="field1">The value of the property Field1</param> 
            /// <param name="field2">The value of the property Field2</param> 
            /// <param name="field3">The value of the property Field3</param> 
            /// <param name="field4">The value of the property Field4</param> 
            /// <param name="field5">The value of the property Field5</param>
            public GenericType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5)
            {
                this.Field1 = field1;
                this.Field2 = field2;
                this.Field3 = field3;
                this.Field4 = field4;
                this.Field5 = field5;
            }

            /// <summary>
            /// Gets or sets a value for Field1
            /// </summary>
            public T1 Field1 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field2
            /// </summary>
            public T2 Field2 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field3
            /// </summary>
            public T3 Field3 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field4
            /// </summary>
            public T4 Field4 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field5
            /// </summary>
            public T5 Field5 { get; set; }
        }

        /// <summary>
        /// Generic Type definition which accepts 6 type parameter(s)
        /// </summary> 
        /// <typeparam name="T1">The type of the property Field1</typeparam> 
        /// <typeparam name="T2">The type of the property Field2</typeparam> 
        /// <typeparam name="T3">The type of the property Field3</typeparam> 
        /// <typeparam name="T4">The type of the property Field4</typeparam> 
        /// <typeparam name="T5">The type of the property Field5</typeparam> 
        /// <typeparam name="T6">The type of the property Field6</typeparam>
        public class GenericType<T1, T2, T3, T4, T5, T6>
        {
            /// <summary>
            /// Initializes a new instance of the GenericType class.
            /// </summary> 
            /// <param name="field1">The value of the property Field1</param> 
            /// <param name="field2">The value of the property Field2</param> 
            /// <param name="field3">The value of the property Field3</param> 
            /// <param name="field4">The value of the property Field4</param> 
            /// <param name="field5">The value of the property Field5</param> 
            /// <param name="field6">The value of the property Field6</param>
            public GenericType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6)
            {
                this.Field1 = field1;
                this.Field2 = field2;
                this.Field3 = field3;
                this.Field4 = field4;
                this.Field5 = field5;
                this.Field6 = field6;
            }

            /// <summary>
            /// Gets or sets a value for Field1
            /// </summary>
            public T1 Field1 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field2
            /// </summary>
            public T2 Field2 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field3
            /// </summary>
            public T3 Field3 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field4
            /// </summary>
            public T4 Field4 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field5
            /// </summary>
            public T5 Field5 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field6
            /// </summary>
            public T6 Field6 { get; set; }
        }

        /// <summary>
        /// Generic Type definition which accepts 7 type parameter(s)
        /// </summary> 
        /// <typeparam name="T1">The type of the property Field1</typeparam> 
        /// <typeparam name="T2">The type of the property Field2</typeparam> 
        /// <typeparam name="T3">The type of the property Field3</typeparam> 
        /// <typeparam name="T4">The type of the property Field4</typeparam> 
        /// <typeparam name="T5">The type of the property Field5</typeparam> 
        /// <typeparam name="T6">The type of the property Field6</typeparam> 
        /// <typeparam name="T7">The type of the property Field7</typeparam>
        public class GenericType<T1, T2, T3, T4, T5, T6, T7>
        {
            /// <summary>
            /// Initializes a new instance of the GenericType class.
            /// </summary> 
            /// <param name="field1">The value of the property Field1</param> 
            /// <param name="field2">The value of the property Field2</param> 
            /// <param name="field3">The value of the property Field3</param> 
            /// <param name="field4">The value of the property Field4</param> 
            /// <param name="field5">The value of the property Field5</param> 
            /// <param name="field6">The value of the property Field6</param> 
            /// <param name="field7">The value of the property Field7</param>
            public GenericType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7)
            {
                this.Field1 = field1;
                this.Field2 = field2;
                this.Field3 = field3;
                this.Field4 = field4;
                this.Field5 = field5;
                this.Field6 = field6;
                this.Field7 = field7;
            }

            /// <summary>
            /// Gets or sets a value for Field1
            /// </summary>
            public T1 Field1 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field2
            /// </summary>
            public T2 Field2 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field3
            /// </summary>
            public T3 Field3 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field4
            /// </summary>
            public T4 Field4 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field5
            /// </summary>
            public T5 Field5 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field6
            /// </summary>
            public T6 Field6 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field7
            /// </summary>
            public T7 Field7 { get; set; }
        }

        /// <summary>
        /// Generic Type definition which accepts 8 type parameter(s)
        /// </summary> 
        /// <typeparam name="T1">The type of the property Field1</typeparam> 
        /// <typeparam name="T2">The type of the property Field2</typeparam> 
        /// <typeparam name="T3">The type of the property Field3</typeparam> 
        /// <typeparam name="T4">The type of the property Field4</typeparam> 
        /// <typeparam name="T5">The type of the property Field5</typeparam> 
        /// <typeparam name="T6">The type of the property Field6</typeparam> 
        /// <typeparam name="T7">The type of the property Field7</typeparam> 
        /// <typeparam name="T8">The type of the property Field8</typeparam>
        public class GenericType<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            /// <summary>
            /// Initializes a new instance of the GenericType class.
            /// </summary> 
            /// <param name="field1">The value of the property Field1</param> 
            /// <param name="field2">The value of the property Field2</param> 
            /// <param name="field3">The value of the property Field3</param> 
            /// <param name="field4">The value of the property Field4</param> 
            /// <param name="field5">The value of the property Field5</param> 
            /// <param name="field6">The value of the property Field6</param> 
            /// <param name="field7">The value of the property Field7</param> 
            /// <param name="field8">The value of the property Field8</param>
            public GenericType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8)
            {
                this.Field1 = field1;
                this.Field2 = field2;
                this.Field3 = field3;
                this.Field4 = field4;
                this.Field5 = field5;
                this.Field6 = field6;
                this.Field7 = field7;
                this.Field8 = field8;
            }

            /// <summary>
            /// Gets or sets a value for Field1
            /// </summary>
            public T1 Field1 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field2
            /// </summary>
            public T2 Field2 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field3
            /// </summary>
            public T3 Field3 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field4
            /// </summary>
            public T4 Field4 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field5
            /// </summary>
            public T5 Field5 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field6
            /// </summary>
            public T6 Field6 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field7
            /// </summary>
            public T7 Field7 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field8
            /// </summary>
            public T8 Field8 { get; set; }
        }

        /// <summary>
        /// Generic Type definition which accepts 9 type parameter(s)
        /// </summary> 
        /// <typeparam name="T1">The type of the property Field1</typeparam> 
        /// <typeparam name="T2">The type of the property Field2</typeparam> 
        /// <typeparam name="T3">The type of the property Field3</typeparam> 
        /// <typeparam name="T4">The type of the property Field4</typeparam> 
        /// <typeparam name="T5">The type of the property Field5</typeparam> 
        /// <typeparam name="T6">The type of the property Field6</typeparam> 
        /// <typeparam name="T7">The type of the property Field7</typeparam> 
        /// <typeparam name="T8">The type of the property Field8</typeparam> 
        /// <typeparam name="T9">The type of the property Field9</typeparam>
        public class GenericType<T1, T2, T3, T4, T5, T6, T7, T8, T9>
        {
            /// <summary>
            /// Initializes a new instance of the GenericType class.
            /// </summary> 
            /// <param name="field1">The value of the property Field1</param> 
            /// <param name="field2">The value of the property Field2</param> 
            /// <param name="field3">The value of the property Field3</param> 
            /// <param name="field4">The value of the property Field4</param> 
            /// <param name="field5">The value of the property Field5</param> 
            /// <param name="field6">The value of the property Field6</param> 
            /// <param name="field7">The value of the property Field7</param> 
            /// <param name="field8">The value of the property Field8</param> 
            /// <param name="field9">The value of the property Field9</param>
            public GenericType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9)
            {
                this.Field1 = field1;
                this.Field2 = field2;
                this.Field3 = field3;
                this.Field4 = field4;
                this.Field5 = field5;
                this.Field6 = field6;
                this.Field7 = field7;
                this.Field8 = field8;
                this.Field9 = field9;
            }

            /// <summary>
            /// Gets or sets a value for Field1
            /// </summary>
            public T1 Field1 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field2
            /// </summary>
            public T2 Field2 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field3
            /// </summary>
            public T3 Field3 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field4
            /// </summary>
            public T4 Field4 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field5
            /// </summary>
            public T5 Field5 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field6
            /// </summary>
            public T6 Field6 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field7
            /// </summary>
            public T7 Field7 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field8
            /// </summary>
            public T8 Field8 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field9
            /// </summary>
            public T9 Field9 { get; set; }
        }

        /// <summary>
        /// Generic Type definition which accepts 10 type parameter(s)
        /// </summary> 
        /// <typeparam name="T1">The type of the property Field1</typeparam> 
        /// <typeparam name="T2">The type of the property Field2</typeparam> 
        /// <typeparam name="T3">The type of the property Field3</typeparam> 
        /// <typeparam name="T4">The type of the property Field4</typeparam> 
        /// <typeparam name="T5">The type of the property Field5</typeparam> 
        /// <typeparam name="T6">The type of the property Field6</typeparam> 
        /// <typeparam name="T7">The type of the property Field7</typeparam> 
        /// <typeparam name="T8">The type of the property Field8</typeparam> 
        /// <typeparam name="T9">The type of the property Field9</typeparam> 
        /// <typeparam name="T10">The type of the property Field10</typeparam>
        public class GenericType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
        {
            /// <summary>
            /// Initializes a new instance of the GenericType class.
            /// </summary> 
            /// <param name="field1">The value of the property Field1</param> 
            /// <param name="field2">The value of the property Field2</param> 
            /// <param name="field3">The value of the property Field3</param> 
            /// <param name="field4">The value of the property Field4</param> 
            /// <param name="field5">The value of the property Field5</param> 
            /// <param name="field6">The value of the property Field6</param> 
            /// <param name="field7">The value of the property Field7</param> 
            /// <param name="field8">The value of the property Field8</param> 
            /// <param name="field9">The value of the property Field9</param> 
            /// <param name="field10">The value of the property Field10</param>
            public GenericType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10)
            {
                this.Field1 = field1;
                this.Field2 = field2;
                this.Field3 = field3;
                this.Field4 = field4;
                this.Field5 = field5;
                this.Field6 = field6;
                this.Field7 = field7;
                this.Field8 = field8;
                this.Field9 = field9;
                this.Field10 = field10;
            }

            /// <summary>
            /// Gets or sets a value for Field1
            /// </summary>
            public T1 Field1 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field2
            /// </summary>
            public T2 Field2 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field3
            /// </summary>
            public T3 Field3 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field4
            /// </summary>
            public T4 Field4 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field5
            /// </summary>
            public T5 Field5 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field6
            /// </summary>
            public T6 Field6 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field7
            /// </summary>
            public T7 Field7 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field8
            /// </summary>
            public T8 Field8 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field9
            /// </summary>
            public T9 Field9 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field10
            /// </summary>
            public T10 Field10 { get; set; }
        }

        /// <summary>
        /// Generic Type definition which accepts 11 type parameter(s)
        /// </summary> 
        /// <typeparam name="T1">The type of the property Field1</typeparam> 
        /// <typeparam name="T2">The type of the property Field2</typeparam> 
        /// <typeparam name="T3">The type of the property Field3</typeparam> 
        /// <typeparam name="T4">The type of the property Field4</typeparam> 
        /// <typeparam name="T5">The type of the property Field5</typeparam> 
        /// <typeparam name="T6">The type of the property Field6</typeparam> 
        /// <typeparam name="T7">The type of the property Field7</typeparam> 
        /// <typeparam name="T8">The type of the property Field8</typeparam> 
        /// <typeparam name="T9">The type of the property Field9</typeparam> 
        /// <typeparam name="T10">The type of the property Field10</typeparam> 
        /// <typeparam name="T11">The type of the property Field11</typeparam>
        public class GenericType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
        {
            /// <summary>
            /// Initializes a new instance of the GenericType class.
            /// </summary> 
            /// <param name="field1">The value of the property Field1</param> 
            /// <param name="field2">The value of the property Field2</param> 
            /// <param name="field3">The value of the property Field3</param> 
            /// <param name="field4">The value of the property Field4</param> 
            /// <param name="field5">The value of the property Field5</param> 
            /// <param name="field6">The value of the property Field6</param> 
            /// <param name="field7">The value of the property Field7</param> 
            /// <param name="field8">The value of the property Field8</param> 
            /// <param name="field9">The value of the property Field9</param> 
            /// <param name="field10">The value of the property Field10</param> 
            /// <param name="field11">The value of the property Field11</param>
            public GenericType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11)
            {
                this.Field1 = field1;
                this.Field2 = field2;
                this.Field3 = field3;
                this.Field4 = field4;
                this.Field5 = field5;
                this.Field6 = field6;
                this.Field7 = field7;
                this.Field8 = field8;
                this.Field9 = field9;
                this.Field10 = field10;
                this.Field11 = field11;
            }

            /// <summary>
            /// Gets or sets a value for Field1
            /// </summary>
            public T1 Field1 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field2
            /// </summary>
            public T2 Field2 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field3
            /// </summary>
            public T3 Field3 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field4
            /// </summary>
            public T4 Field4 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field5
            /// </summary>
            public T5 Field5 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field6
            /// </summary>
            public T6 Field6 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field7
            /// </summary>
            public T7 Field7 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field8
            /// </summary>
            public T8 Field8 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field9
            /// </summary>
            public T9 Field9 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field10
            /// </summary>
            public T10 Field10 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field11
            /// </summary>
            public T11 Field11 { get; set; }
        }

        /// <summary>
        /// Generic Type definition which accepts 12 type parameter(s)
        /// </summary> 
        /// <typeparam name="T1">The type of the property Field1</typeparam> 
        /// <typeparam name="T2">The type of the property Field2</typeparam> 
        /// <typeparam name="T3">The type of the property Field3</typeparam> 
        /// <typeparam name="T4">The type of the property Field4</typeparam> 
        /// <typeparam name="T5">The type of the property Field5</typeparam> 
        /// <typeparam name="T6">The type of the property Field6</typeparam> 
        /// <typeparam name="T7">The type of the property Field7</typeparam> 
        /// <typeparam name="T8">The type of the property Field8</typeparam> 
        /// <typeparam name="T9">The type of the property Field9</typeparam> 
        /// <typeparam name="T10">The type of the property Field10</typeparam> 
        /// <typeparam name="T11">The type of the property Field11</typeparam> 
        /// <typeparam name="T12">The type of the property Field12</typeparam>
        public class GenericType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
        {
            /// <summary>
            /// Initializes a new instance of the GenericType class.
            /// </summary> 
            /// <param name="field1">The value of the property Field1</param> 
            /// <param name="field2">The value of the property Field2</param> 
            /// <param name="field3">The value of the property Field3</param> 
            /// <param name="field4">The value of the property Field4</param> 
            /// <param name="field5">The value of the property Field5</param> 
            /// <param name="field6">The value of the property Field6</param> 
            /// <param name="field7">The value of the property Field7</param> 
            /// <param name="field8">The value of the property Field8</param> 
            /// <param name="field9">The value of the property Field9</param> 
            /// <param name="field10">The value of the property Field10</param> 
            /// <param name="field11">The value of the property Field11</param> 
            /// <param name="field12">The value of the property Field12</param>
            public GenericType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12)
            {
                this.Field1 = field1;
                this.Field2 = field2;
                this.Field3 = field3;
                this.Field4 = field4;
                this.Field5 = field5;
                this.Field6 = field6;
                this.Field7 = field7;
                this.Field8 = field8;
                this.Field9 = field9;
                this.Field10 = field10;
                this.Field11 = field11;
                this.Field12 = field12;
            }

            /// <summary>
            /// Gets or sets a value for Field1
            /// </summary>
            public T1 Field1 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field2
            /// </summary>
            public T2 Field2 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field3
            /// </summary>
            public T3 Field3 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field4
            /// </summary>
            public T4 Field4 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field5
            /// </summary>
            public T5 Field5 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field6
            /// </summary>
            public T6 Field6 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field7
            /// </summary>
            public T7 Field7 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field8
            /// </summary>
            public T8 Field8 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field9
            /// </summary>
            public T9 Field9 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field10
            /// </summary>
            public T10 Field10 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field11
            /// </summary>
            public T11 Field11 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field12
            /// </summary>
            public T12 Field12 { get; set; }
        }

        /// <summary>
        /// Generic Type definition which accepts 13 type parameter(s)
        /// </summary> 
        /// <typeparam name="T1">The type of the property Field1</typeparam> 
        /// <typeparam name="T2">The type of the property Field2</typeparam> 
        /// <typeparam name="T3">The type of the property Field3</typeparam> 
        /// <typeparam name="T4">The type of the property Field4</typeparam> 
        /// <typeparam name="T5">The type of the property Field5</typeparam> 
        /// <typeparam name="T6">The type of the property Field6</typeparam> 
        /// <typeparam name="T7">The type of the property Field7</typeparam> 
        /// <typeparam name="T8">The type of the property Field8</typeparam> 
        /// <typeparam name="T9">The type of the property Field9</typeparam> 
        /// <typeparam name="T10">The type of the property Field10</typeparam> 
        /// <typeparam name="T11">The type of the property Field11</typeparam> 
        /// <typeparam name="T12">The type of the property Field12</typeparam> 
        /// <typeparam name="T13">The type of the property Field13</typeparam>
        public class GenericType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
        {
            /// <summary>
            /// Initializes a new instance of the GenericType class.
            /// </summary> 
            /// <param name="field1">The value of the property Field1</param> 
            /// <param name="field2">The value of the property Field2</param> 
            /// <param name="field3">The value of the property Field3</param> 
            /// <param name="field4">The value of the property Field4</param> 
            /// <param name="field5">The value of the property Field5</param> 
            /// <param name="field6">The value of the property Field6</param> 
            /// <param name="field7">The value of the property Field7</param> 
            /// <param name="field8">The value of the property Field8</param> 
            /// <param name="field9">The value of the property Field9</param> 
            /// <param name="field10">The value of the property Field10</param> 
            /// <param name="field11">The value of the property Field11</param> 
            /// <param name="field12">The value of the property Field12</param> 
            /// <param name="field13">The value of the property Field13</param>
            public GenericType(T1 field1, T2 field2, T3 field3, T4 field4, T5 field5, T6 field6, T7 field7, T8 field8, T9 field9, T10 field10, T11 field11, T12 field12, T13 field13)
            {
                this.Field1 = field1;
                this.Field2 = field2;
                this.Field3 = field3;
                this.Field4 = field4;
                this.Field5 = field5;
                this.Field6 = field6;
                this.Field7 = field7;
                this.Field8 = field8;
                this.Field9 = field9;
                this.Field10 = field10;
                this.Field11 = field11;
                this.Field12 = field12;
                this.Field13 = field13;
            }

            /// <summary>
            /// Gets or sets a value for Field1
            /// </summary>
            public T1 Field1 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field2
            /// </summary>
            public T2 Field2 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field3
            /// </summary>
            public T3 Field3 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field4
            /// </summary>
            public T4 Field4 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field5
            /// </summary>
            public T5 Field5 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field6
            /// </summary>
            public T6 Field6 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field7
            /// </summary>
            public T7 Field7 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field8
            /// </summary>
            public T8 Field8 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field9
            /// </summary>
            public T9 Field9 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field10
            /// </summary>
            public T10 Field10 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field11
            /// </summary>
            public T11 Field11 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field12
            /// </summary>
            public T12 Field12 { get; set; }

            /// <summary>
            /// Gets or sets a value for Field13
            /// </summary>
            public T13 Field13 { get; set; }
        }
    }
#else
    /// <summary>
    /// Stub anonymous type builder for the Windows Phone, this type is only available on the Windows Phone platform
    /// </summary>
    public class CSharpGenericTypeBuilder
    {
    }
#endif
}