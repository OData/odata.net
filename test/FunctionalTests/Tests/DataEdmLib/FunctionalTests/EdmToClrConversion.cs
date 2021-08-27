//---------------------------------------------------------------------
// <copyright file="EdmToClrConversion.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EdmToClrConversion : EdmLibTestCaseBase
    {
        [TestMethod]
        public void ConvertPrimitiveTypes()
        {
            EdmToClrConverter c = new EdmToClrConverter();

            byte[] b = { 1, 2, 3 };
            Assert.AreEqual(b, c.AsClrValue(new EdmBinaryConstant(b), typeof(byte[])), "binary 1");
            Assert.AreEqual(b, c.AsClrValue<byte[]>(new EdmBinaryConstant(b)), "binary2");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(byte[])), "Null is null even when null is bool");

            Assert.AreEqual("qwerty", c.AsClrValue(new EdmStringConstant("qwerty"), typeof(string)), "string 1");
            Assert.AreEqual("qwerty", c.AsClrValue<string>(new EdmStringConstant("qwerty")), "string 2");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(string)), "Null is null even when null is string");

            Assert.AreEqual(true, c.AsClrValue(new EdmBooleanConstant(true), typeof(bool)), "bool 1");
            Assert.AreEqual(true, c.AsClrValue<bool>(new EdmBooleanConstant(true)), "bool 2");

            bool? nullableBool = true;
            Assert.AreEqual(nullableBool, c.AsClrValue(new EdmBooleanConstant(true), typeof(bool?)), "nullable bool");
            Assert.AreEqual(typeof(bool), c.AsClrValue(new EdmBooleanConstant(true), typeof(bool?)).GetType(), "nullable bool correct type");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(bool?)), "Null is null even when null is bool");

            Assert.AreEqual('q', c.AsClrValue(new EdmIntegerConstant('q'), typeof(char)), "char 1");
            Assert.AreEqual('q', c.AsClrValue<char>(new EdmIntegerConstant('q')), "char 2");

            char? nullableChar = 'q';
            Assert.AreEqual(nullableChar, c.AsClrValue(new EdmIntegerConstant('q'), typeof(char?)), "nullable char");
            Assert.AreEqual(typeof(char), c.AsClrValue(new EdmIntegerConstant('q'), typeof(char?)).GetType(), "nullable char is correct type");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(char?)), "Null is null even when null is char");

            Assert.AreEqual((sbyte)(-12), c.AsClrValue(new EdmIntegerConstant(-12), typeof(sbyte)), "sbyte 1");
            Assert.AreEqual((sbyte)(-12), c.AsClrValue<sbyte>(new EdmIntegerConstant(-12)), "sbyte 2");

            sbyte? nullableSByte = -12;
            Assert.AreEqual(nullableSByte, ((sbyte?)c.AsClrValue(new EdmIntegerConstant(-12), typeof(sbyte?))).Value, "nullable sbyte");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(sbyte?)), "Null is null even when null is sbyte");

            Assert.AreEqual((byte)12, c.AsClrValue(new EdmIntegerConstant(12), typeof(byte)), "byte 1");
            Assert.AreEqual((byte)12, c.AsClrValue<byte>(new EdmIntegerConstant(12)), "byte 2");
            try
            {
                c.AsClrValue<byte>(new EdmIntegerConstant(257));
                Assert.Fail("OverflowException expected");
            }
            catch (OverflowException)
            {
            }

            byte? nullableByte = 12;
            Assert.AreEqual(nullableByte, ((byte?)c.AsClrValue(new EdmIntegerConstant(12), typeof(byte?))).Value, "nullable byte");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(byte?)), "Null is null even when null is byte");
            try
            {
                c.AsClrValue(new EdmIntegerConstant(257), typeof(byte?));
                Assert.Fail("OverflowException expected");
            }
            catch (OverflowException)
            {
            }

            Assert.AreEqual((Int16)(-12), c.AsClrValue(new EdmIntegerConstant(-12), typeof(Int16)), "Int16 1");
            Assert.AreEqual((Int16)(-12), c.AsClrValue<Int16>(new EdmIntegerConstant(-12)), "Int16 2");

            Int16? nullableInt16 = -12;
            Assert.AreEqual(nullableInt16, ((Int16?)c.AsClrValue(new EdmIntegerConstant(-12), typeof(Int16?))).Value, "nullable Int16");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(Int16?)), "Null is null even when null is Int16");

            Assert.AreEqual((UInt16)12, c.AsClrValue(new EdmIntegerConstant(12), typeof(UInt16)), "UInt16 1");
            Assert.AreEqual((UInt16)12, c.AsClrValue<UInt16>(new EdmIntegerConstant(12)), "UInt16 2");

            Assert.AreEqual((UInt16)12, ((UInt16?)c.AsClrValue(new EdmIntegerConstant(12), typeof(UInt16?))).Value, "nullable UInt16");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(UInt16?)), "Null is null even when null is UInt16");

            Assert.AreEqual((Int32)(-12), c.AsClrValue(new EdmIntegerConstant(-12), typeof(Int32)), "Int32 1");
            Assert.AreEqual((Int32)(-12), c.AsClrValue<Int32>(new EdmIntegerConstant(-12)), "Int32 2");

            Assert.AreEqual((Int32)(-12), ((Int32?)c.AsClrValue(new EdmIntegerConstant(-12), typeof(Int32?))).Value, "nullable Int32");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(Int32?)), "Null is null even when null is Int32");

            Assert.AreEqual((UInt32)12, c.AsClrValue(new EdmIntegerConstant(12), typeof(UInt32)), "UInt32 1");
            Assert.AreEqual((UInt32)12, c.AsClrValue<UInt32>(new EdmIntegerConstant(12)), "UInt32 2");

            Assert.AreEqual((UInt32)12, ((UInt32?)c.AsClrValue(new EdmIntegerConstant(12), typeof(UInt32?))).Value, "nullable UInt32");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(UInt32?)), "Null is null even when null is UInt32");

            Assert.AreEqual((Int64)(-12), c.AsClrValue(new EdmIntegerConstant(-12), typeof(Int64)), "Int64 1");
            Assert.AreEqual((Int64)(-12), c.AsClrValue<Int64>(new EdmIntegerConstant(-12)), "Int64 2");

            Assert.AreEqual((Int64)(-12), ((Int64?)c.AsClrValue(new EdmIntegerConstant(-12), typeof(Int64?))).Value, "nullable Int64");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(Int64?)), "Null is null even when null is Int64");

            Assert.AreEqual((UInt64)12, c.AsClrValue(new EdmIntegerConstant(12), typeof(UInt64)), "UInt64 1");
            Assert.AreEqual((UInt64)12, c.AsClrValue<UInt64>(new EdmIntegerConstant(12)), "UInt64 2");

            Assert.AreEqual((UInt64)12, ((UInt64?)c.AsClrValue(new EdmIntegerConstant(12), typeof(UInt64?))).Value, "nullable UInt64");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(UInt64?)), "Null is null even when null is UInt64");

            Assert.AreEqual((double)1.2, c.AsClrValue(new EdmFloatingConstant(1.2), typeof(double)), "double 1");
            Assert.AreEqual((double)1.2, c.AsClrValue<double>(new EdmFloatingConstant(1.2)), "double 2");

            Assert.AreEqual((double)1.2, ((double?)c.AsClrValue(new EdmFloatingConstant(1.2), typeof(double?))).Value, "nullable double");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(double?)), "Null is null even when null is double");

            Assert.AreEqual((Single)1.2, c.AsClrValue(new EdmFloatingConstant(1.2), typeof(Single)), "Single 1");
            Assert.AreEqual((Single)1.2, c.AsClrValue<Single>(new EdmFloatingConstant(1.2)), "Single 2");

            Assert.AreEqual((Single)1.2, ((Single?)c.AsClrValue(new EdmFloatingConstant(1.2), typeof(Single?))).Value, "nullable Single");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(Single?)), "Null is null even when null is Single");

            Assert.AreEqual((decimal)1.2, c.AsClrValue(new EdmDecimalConstant(1.2m), typeof(decimal)), "decimal 1");
            Assert.AreEqual((decimal)1.2, c.AsClrValue<decimal>(new EdmDecimalConstant(1.2m)), "decimal 2");

            Assert.AreEqual((decimal)1.2, ((decimal?)c.AsClrValue(new EdmDecimalConstant(1.2m), typeof(decimal?))).Value, "nullable decimal");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(decimal?)), "Null is null even when null is decimal");

            DateTime dt = new DateTime(2011, 10, 18, 14, 48, 16);

            DateTimeOffset dto = new DateTimeOffset(dt, new TimeSpan(-8, 0, 0));
            Assert.AreEqual(dto, c.AsClrValue(new EdmDateTimeOffsetConstant(dto), typeof(DateTimeOffset)), "DateTimeOffset 1");
            Assert.AreEqual(dto, c.AsClrValue<DateTimeOffset>(new EdmDateTimeOffsetConstant(dto)), "DateTimeOffset 2");

            Assert.AreEqual(dto, ((DateTimeOffset?)c.AsClrValue(new EdmDateTimeOffsetConstant(dto), typeof(DateTimeOffset?))).Value, "nullable DateTimeOffset");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(DateTimeOffset?)), "Null is null even when null is DateTimeOffset");

            Date date = new Date(2014, 8, 8);

            Assert.AreEqual(date, c.AsClrValue(new EdmDateConstant(date), typeof(Date)), "Date 1");
            Assert.AreEqual(date, c.AsClrValue<Date>(new EdmDateConstant(date)), "Date 2");
            Assert.AreEqual(date, ((Date?)c.AsClrValue(new EdmDateConstant(date), typeof(Date?))).Value, "nullable Date");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(Date?)), "Null is null even when null is Date");

            TimeOfDay time = new TimeOfDay(12, 5, 30, 900);
            Assert.AreEqual(time, c.AsClrValue(new EdmTimeOfDayConstant(time), typeof(TimeOfDay)), "Time 1");
            Assert.AreEqual(time, c.AsClrValue<TimeOfDay>(new EdmTimeOfDayConstant(time)), "Time 2");
            Assert.AreEqual(time, ((TimeOfDay?)c.AsClrValue(new EdmTimeOfDayConstant(time), typeof(TimeOfDay?))).Value, "nullable Time");
            Assert.IsNull(c.AsClrValue(EdmNullExpression.Instance, typeof(TimeOfDay?)), "Null is null even when null is Time");
        }

        [TestMethod]
        public void ConvertCollections()
        {
            string[] elements = { "ab", "cd", "ef", null };
            EdmCollectionValue c = new EdmCollectionValue(
                EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)),
                elements.Select(e => e == null ? (IEdmDelayedValue)EdmNullExpression.Instance : (IEdmDelayedValue)new EdmStringConstant(e)));

            EdmToClrConverter cv = new EdmToClrConverter();

            string[] converted = ((IEnumerable<string>)cv.AsClrValue(c, typeof(IEnumerable<string>))).ToArray();
            Assert.AreEqual(elements.Length, converted.Length, "Length 1");
            for (int i = 0; i < elements.Length; ++i)
            {
                Assert.AreEqual(elements[i], converted[i], "elements[i] 1");
            }

            converted = cv.AsClrValue<IEnumerable<string>>(c).ToArray();
            Assert.AreEqual(elements.Length, converted.Length, "Length 2");
            for (int i = 0; i < elements.Length; ++i)
            {
                Assert.AreEqual(elements[i], converted[i], "elements[i] 2");
            }
        }

        public enum RedBlue : int
        {
            Red = 10,
            Blue = 20
        }

        public enum RedBlueByte : byte
        {
            Red = 10,
            Blue = 20
        }

        public enum RedBlueLong : long
        {
            Red = 10,
            Blue = 20
        }

        [TestMethod]
        public void ConvertEnums()
        {
            EdmToClrConverter c = new EdmToClrConverter();

            EdmEnumType e32 = new EdmEnumType("", "");
            var r32 = e32.AddMember("Red", new EdmEnumMemberValue(10));
            e32.AddMember("Blue", new EdmEnumMemberValue(20));

            EdmEnumValue evRed = new EdmEnumValue(new EdmEnumTypeReference(e32, false), r32);
            EdmEnumValue ev20 = new EdmEnumValue(new EdmEnumTypeReference(e32, false), new EdmEnumMemberValue(20));
            EdmEnumValue ev30 = new EdmEnumValue(new EdmEnumTypeReference(e32, false), new EdmEnumMemberValue(30));

            Assert.AreEqual(RedBlue.Red, c.AsClrValue<RedBlue>(evRed), "c.AsClrValue<RedBlue>(evRed)");
            Assert.AreEqual(RedBlue.Blue, c.AsClrValue<RedBlue>(ev20), "c.AsClrValue<RedBlue>(ev20)");
            Assert.AreEqual((RedBlue)30, c.AsClrValue<RedBlue>(ev30), "c.AsClrValue<RedBlue>(ev30)");

            Assert.AreEqual(null, c.AsClrValue<RedBlue?>(EdmNullExpression.Instance), "Null Enum is null");

            Assert.AreEqual(RedBlueByte.Red, c.AsClrValue<RedBlueByte>(evRed), "c.AsClrValue<RedBlueByte>(evRed)");
            Assert.AreEqual(RedBlueByte.Blue, c.AsClrValue<RedBlueByte>(ev20), "c.AsClrValue<RedBlueByte>(ev20)");
            Assert.AreEqual((RedBlueByte)30, c.AsClrValue<RedBlueByte>(ev30), "c.AsClrValue<RedBlueByte>(ev30)");

            Assert.AreEqual(RedBlueLong.Red, c.AsClrValue<RedBlueLong>(evRed), "c.AsClrValue<RedBlueLong>(evRed)");
            Assert.AreEqual(RedBlueLong.Blue, c.AsClrValue<RedBlueLong>(ev20), "c.AsClrValue<RedBlueLong>(ev20)");
            Assert.AreEqual((RedBlueLong)30, c.AsClrValue<RedBlueLong>(ev30), "c.AsClrValue<RedBlueLong>(ev30)");

            Assert.AreEqual(RedBlueLong.Red, c.AsClrValue(evRed, typeof(RedBlueLong)), "c.AsClrValue(evRed, typeof(RedBlueLong))");
            Assert.AreEqual(RedBlueLong.Blue, c.AsClrValue(ev20, typeof(RedBlueLong)), "c.AsClrValue(ev20, typeof(RedBlueLong))");
            Assert.AreEqual((RedBlueLong)30, c.AsClrValue(ev30, typeof(RedBlueLong)), "c.AsClrValue(ev30, typeof(RedBlueLong))");
        }

        public abstract class C1Base
        {
            public string P1 { get; set; }
        }

        public class C1 : C1Base
        {
            public C2 P2 { get; set; }
            public int P3 { get; set; }
        }

        public class C2
        {
            public C2()
            {
            }

            public string Q1 { get; set; }
            public C1 Q2 { get; set; }
            public int? Q3 { get; set; }
        }

        [TestMethod]
        public void ConvertStructuredTypes()
        {
            IEdmStructuredValue c1_inner = new EdmStructuredValue(null, new IEdmPropertyValue[]
            {
                new EdmPropertyValue("P1", new EdmStringConstant("c1ip1")),
                new EdmPropertyValue("P3", new EdmIntegerConstant(110))
            });

            IEdmStructuredValue c2 = new EdmStructuredValue(null, new IEdmPropertyValue[]
            {
                new EdmPropertyValue("Q1", new EdmStringConstant("c2q1")),
                new EdmPropertyValue("Q2", c1_inner),
                new EdmPropertyValue("Q3", new EdmIntegerConstant(22))
            });

            IEdmStructuredValue c1 = new EdmStructuredValue(null, new IEdmPropertyValue[]
            {
                new EdmPropertyValue("P1", new EdmStringConstant("c1p1")),
                new EdmPropertyValue("P2", c2),
                new EdmPropertyValue("P3", new EdmIntegerConstant(11))
            });

            EdmToClrConverter cv = new EdmToClrConverter();

            C1[] clrC1s = { (C1)cv.AsClrValue(c1, typeof(C1)), cv.AsClrValue<C1>(c1) };

            for (int i = 0; i < clrC1s.Length; ++i)
            {
                C1 clrC1 = clrC1s[i];

                // C1
                Assert.IsNotNull(clrC1, "c1 " + i);
                Assert.AreEqual("c1p1", clrC1.P1, "c1.P1 " + i);
                Assert.AreEqual(11, clrC1.P3, "c1.P3 " + i);

                // C1.P2
                Assert.IsNotNull(clrC1.P2, "c1.P2 " + i);
                Assert.AreEqual("c2q1", clrC1.P2.Q1, "c1.P2.Q1 " + i);
                Assert.AreEqual(22, clrC1.P2.Q3, "c1.P2.Q3 " + i);

                // C1.P2.Q2
                Assert.IsNotNull(clrC1.P2.Q2, "c1.P2.Q2 " + i);
                Assert.AreEqual("c1ip1", clrC1.P2.Q2.P1, "c1.P2.Q2.P1 " + i);
                Assert.AreEqual(110, clrC1.P2.Q2.P3, "c1.P2.Q2.P3 " + i);

                // C1.P2.Q2.P2
                Assert.IsNull(clrC1.P2.Q2.P2, "c1.P2.Q2.P2 " + i);
            }
        }

        [TestMethod]
        public void ConvertGraphWithLoops()
        {
            var c1_inner_loopProperty = new EdmPropertyValue("P2");

            IEdmStructuredValue c1_inner = new EdmStructuredValue(null, new IEdmPropertyValue[]
            {
                new EdmPropertyValue("P1", new EdmStringConstant("c1ip1")),
                c1_inner_loopProperty,
                new EdmPropertyValue("P3", new EdmIntegerConstant(110)),
                new EdmPropertyValue("NoClrCounterpart", new EdmIntegerConstant(110)) // conversion should ignore this and not choke
            });

            IEdmStructuredValue c2 = new EdmStructuredValue(null, new IEdmPropertyValue[]
            {
                new EdmPropertyValue("Q1", new EdmStringConstant("c2q1")),
                new EdmPropertyValue("Q2", c1_inner),
                new EdmPropertyValue("Q3", EdmNullExpression.Instance)
            });

            c1_inner_loopProperty.Value = c2;

            IEdmStructuredValue c1 = new EdmStructuredValue(null, new IEdmPropertyValue[]
            {
                new EdmPropertyValue("P1", new EdmStringConstant("c1p1")),
                new EdmPropertyValue("P2", c2),
                new EdmPropertyValue("P3", new EdmIntegerConstant(11))
            });

            EdmToClrConverter cv = new EdmToClrConverter();

            C1[] clrC1s = { (C1)cv.AsClrValue(c1, typeof(C1)), cv.AsClrValue<C1>(c1) };

            for (int i = 0; i < clrC1s.Length; ++i)
            {
                C1 clrC1 = clrC1s[i];

                // C1
                Assert.IsNotNull(clrC1, "c1 " + i);
                Assert.AreEqual("c1p1", clrC1.P1, "c1.P1 " + i);
                Assert.AreEqual(11, clrC1.P3, "c1.P3 " + i);

                // C1.P2
                Assert.IsNotNull(clrC1.P2, "c1.P2 " + i);
                Assert.AreEqual("c2q1", clrC1.P2.Q1, "c1.P2.Q1 " + i);
                Assert.AreEqual(null, clrC1.P2.Q3, "c1.P2.Q3 " + i);

                // C1.P2.Q2
                Assert.IsNotNull(clrC1.P2.Q2, "c1.P2.Q2 " + i);
                Assert.AreEqual("c1ip1", clrC1.P2.Q2.P1, "c1.P2.Q2.P1 " + i);
                Assert.AreEqual(110, clrC1.P2.Q2.P3, "c1.P2.Q2.P3 " + i);

                // C1.P2.Q2.P2
                Assert.IsTrue(Object.ReferenceEquals(clrC1.P2.Q2.P2, clrC1.P2), "c1.P2.Q2.P2 " + i);
            }
        }

        public class Product
        {
            public string Name { get; set; }
        }

        public class Derived : Product
        {
            public string Name2 { get; set; }
        }

        public class Derived2 : Derived
        {
            public Derived2(string name, string name2)
            {
                base.Name = name;
                base.Name2 = name2;
            }

            new public string Name { get { return base.Name2; } }
            new public string Name2 { get { return base.Name; } }
        }

        public class MyList<T> : List<T>, IList<int>
        {
            #region IList<int>
            int IList<int>.IndexOf(int item)
            {
                throw new NotImplementedException();
            }

            void IList<int>.Insert(int index, int item)
            {
                throw new NotImplementedException();
            }

            void IList<int>.RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            int IList<int>.this[int index]
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            void ICollection<int>.Add(int item)
            {
                throw new NotImplementedException();
            }

            void ICollection<int>.Clear()
            {
                throw new NotImplementedException();
            }

            bool ICollection<int>.Contains(int item)
            {
                throw new NotImplementedException();
            }

            void ICollection<int>.CopyTo(int[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            int ICollection<int>.Count
            {
                get { throw new NotImplementedException(); }
            }

            bool ICollection<int>.IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            bool ICollection<int>.Remove(int item)
            {
                throw new NotImplementedException();
            }

            IEnumerator<int> IEnumerable<int>.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
            #endregion
        }

        public class ProductsList : List<Product>, ICollection<int>
        {
            #region ICollection<int>
            void ICollection<int>.Add(int item)
            {
                throw new NotImplementedException();
            }

            void ICollection<int>.Clear()
            {
                throw new NotImplementedException();
            }

            bool ICollection<int>.Contains(int item)
            {
                throw new NotImplementedException();
            }

            void ICollection<int>.CopyTo(int[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            int ICollection<int>.Count
            {
                get { throw new NotImplementedException(); }
            }

            bool ICollection<int>.IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            bool ICollection<int>.Remove(int item)
            {
                throw new NotImplementedException();
            }

            IEnumerator<int> IEnumerable<int>.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
            #endregion
        }

        public class MyProductsList : MyList<Product>, IList<int>
        {
            #region IList<int>
            int IList<int>.IndexOf(int item)
            {
                throw new NotImplementedException();
            }

            void IList<int>.Insert(int index, int item)
            {
                throw new NotImplementedException();
            }

            void IList<int>.RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            int IList<int>.this[int index]
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            void ICollection<int>.Add(int item)
            {
                throw new NotImplementedException();
            }

            void ICollection<int>.Clear()
            {
                throw new NotImplementedException();
            }

            bool ICollection<int>.Contains(int item)
            {
                throw new NotImplementedException();
            }

            void ICollection<int>.CopyTo(int[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            int ICollection<int>.Count
            {
                get { throw new NotImplementedException(); }
            }

            bool ICollection<int>.IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            bool ICollection<int>.Remove(int item)
            {
                throw new NotImplementedException();
            }

            IEnumerator<int> IEnumerable<int>.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
            #endregion
        }

        public class Category
        {
            public Category()
            {
                Products3.Add(new Product() { Name = "sentinel3" });
                Products4.Add(new Product() { Name = "sentinel4" });
                Products5.Add(new Product() { Name = "sentinel5" });
                Products6.Add(new Product() { Name = "sentinel6" });
                Products7.Add(new Product() { Name = "sentinel7" });
            }

            public string Name { get; set; }

            public IList<Product> Products1 { get; set; }

            public IList<Product> Products11 { get; set; }

            public ICollection<Product> Products2 { get; set; }

            public IList<Product> Products3
            {
                get { return products3; }
            }
            private List<Product> products3 = new List<Product>();

            public ICollection<Product> Products4
            {
                get { return products4; }
            }
            private Collection<Product> products4 = new Collection<Product>();

            public IList<Product> Products5
            {
                get { return (IList<Product>)products5; }
            }
            private MyList<Product> products5 = new MyList<Product>();

            public IList<Product> Products6
            {
                get { return products6; }
            }
            private ProductsList products6 = new ProductsList();

            public IList<Product> Products7
            {
                get { return (IList<Product>)products7; }
            }
            private MyProductsList products7 = new MyProductsList();

            public IList<int> Ints { get; set; }

            public IEnumerable<int> IntsEnum { get; set; }

            public IList<Product> ProductsPoly { get; set; }
        }

        [TestMethod]
        public void ConvertCollectionProperties()
        {
            string[] products1 = { "p11", "p12" };
            string[] products2 = { null, "p22" };
            string[] products3 = { "p31", "p32" };
            string[] products4 = { "p41", null };
            string[] products5 = { "p51", "p52" };
            string[] products6 = { "p61", "p62" };
            string[] products7 = { "p71", "p72" };
            int[] ints = { 1, 2, 3 };
            string[] productsPoly = { "p1", "d1", "p2", "d2", "D3" };

            EdmEntityType productType = new EdmEntityType("", "Product");
            productType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            IEdmEntityTypeReference productTypeRef = new EdmEntityTypeReference(productType, true);

            EdmEntityType derivedProductType = new EdmEntityType("", "Derived", productType);
            derivedProductType.AddStructuralProperty("Name2", EdmCoreModel.Instance.GetString(false));
            IEdmEntityTypeReference derivedProductTypeRef = new EdmEntityTypeReference(derivedProductType, true);

            EdmEntityType derived2ProductType = new EdmEntityType("", "Derived2", derivedProductType);
            IEdmEntityTypeReference derived2ProductTypeRef = new EdmEntityTypeReference(derived2ProductType, true);

            var edmProducts1 = products1.Select(p => new EdmStructuredValue(productTypeRef, new IEdmPropertyValue[] { new EdmPropertyValue("Name", new EdmStringConstant(p)) })).Cast<IEdmDelayedValue>().ToList();

            IEdmStructuredValue category = new EdmStructuredValue(null, new IEdmPropertyValue[] 
            {
                new EdmPropertyValue("Name", new EdmStringConstant("SomeCategoryX")),
                new EdmPropertyValue("Products1", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef), edmProducts1)),
                new EdmPropertyValue("Products11", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef), edmProducts1)),
                new EdmPropertyValue("Products2", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef),
                    products2.Select(p => (p == null) ? (IEdmDelayedValue)EdmNullExpression.Instance : (IEdmDelayedValue)new EdmStructuredValue(productTypeRef, new IEdmPropertyValue[] { new EdmPropertyValue("Name", new EdmStringConstant(p)) })))),
                new EdmPropertyValue("Products3", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef),
                    products3.Select(p => (p == null) ? (IEdmDelayedValue)EdmNullExpression.Instance : (IEdmDelayedValue)new EdmStructuredValue(productTypeRef, new IEdmPropertyValue[] { new EdmPropertyValue("Name", new EdmStringConstant(p)) })))),
                new EdmPropertyValue("Products4", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef),
                    products4.Select(p => (p == null) ? (IEdmDelayedValue)EdmNullExpression.Instance : (IEdmDelayedValue)new EdmStructuredValue(productTypeRef, new IEdmPropertyValue[] { new EdmPropertyValue("Name", new EdmStringConstant(p)) })))),
                new EdmPropertyValue("Products5", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef),
                    products5.Select(p => (p == null) ? (IEdmDelayedValue)EdmNullExpression.Instance : (IEdmDelayedValue)new EdmStructuredValue(productTypeRef, new IEdmPropertyValue[] { new EdmPropertyValue("Name", new EdmStringConstant(p)) })))),
                new EdmPropertyValue("Products6", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef),
                    products6.Select(p => (p == null) ? (IEdmDelayedValue)EdmNullExpression.Instance : (IEdmDelayedValue)new EdmStructuredValue(productTypeRef, new IEdmPropertyValue[] { new EdmPropertyValue("Name", new EdmStringConstant(p)) })))),
                new EdmPropertyValue("Products7", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef),
                    products7.Select(p => (p == null) ? (IEdmDelayedValue)EdmNullExpression.Instance : (IEdmDelayedValue)new EdmStructuredValue(productTypeRef, new IEdmPropertyValue[] { new EdmPropertyValue("Name", new EdmStringConstant(p)) })))),
                new EdmPropertyValue("ProductsNull", EdmNullExpression.Instance),
                new EdmPropertyValue("Ints", new EdmCollectionValue(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true)),
                    ints.Select(p => new EdmIntegerConstant(p)).Cast<IEdmDelayedValue>())),
                new EdmPropertyValue("IntsEnum", new EdmCollectionValue(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true)),
                    ints.Select(p => new EdmIntegerConstant(p*10)).Cast<IEdmDelayedValue>())),
                new EdmPropertyValue("ProductsPoly", new EdmCollectionValue(EdmCoreModel.GetCollection(productTypeRef),
                    productsPoly.Select(p =>
                        {
                            if (p.StartsWith("p"))
                                return new EdmStructuredValue(productTypeRef, new IEdmPropertyValue[] { new EdmPropertyValue("Name", new EdmStringConstant(p)) });
                            else if (p.StartsWith("d"))
                                return new EdmStructuredValue(derivedProductTypeRef, new IEdmPropertyValue[]
                                {
                                    new EdmPropertyValue("Name", new EdmStringConstant(p)),
                                    new EdmPropertyValue("Name2", new EdmStringConstant(p + "***")),
                                });
                            else
                                return new EdmStructuredValue(derived2ProductTypeRef, new IEdmPropertyValue[]
                                {
                                    new EdmPropertyValue("Name", new EdmStringConstant(p)),
                                    new EdmPropertyValue("Name2", new EdmStringConstant(p + "***")),
                                });
                        }).Cast<IEdmDelayedValue>())),
            });

            // Test polymorphic collection creation.
            EdmToClrConverter cv = new EdmToClrConverter(
                (IEdmStructuredValue edmValue, Type clrType, EdmToClrConverter converter, out object objectInstance, out bool objectInstanceInitialized) =>
                {
                    objectInstance = null;
                    objectInstanceInitialized = false;
                    if (edmValue.Type != null && edmValue.Type.Definition.IsEquivalentTo(derivedProductType))
                    {
                        objectInstance = new Derived();
                    }
                    else if (edmValue.Type != null && edmValue.Type.Definition.IsEquivalentTo(derived2ProductType))
                    {
                        EdmToClrConverter clrConverter = new EdmToClrConverter();
                        objectInstance = new Derived2(clrConverter.AsClrValue<string>(edmValue.FindPropertyValue("Name").Value),
                                                      clrConverter.AsClrValue<string>(edmValue.FindPropertyValue("Name2").Value));
                        objectInstanceInitialized = true;
                    }
                    return objectInstance != null;
                });


            Category clrCategory = cv.AsClrValue<Category>(category);
            Assert.IsNotNull(clrCategory, "clrCategory");
            Assert.AreEqual("SomeCategoryX", clrCategory.Name, "clrCategory.Name");

            Assert.AreEqual(2, clrCategory.Products1.Count, "clrCategory.Products1.Count");
            Assert.AreEqual("p11", clrCategory.Products1[0].Name, "clrCategory.Products1[0].Name");
            Assert.AreEqual("p12", clrCategory.Products1[1].Name, "clrCategory.Products1[1].Name");

            Assert.AreEqual(clrCategory.Products1.Count, clrCategory.Products11.Count, "clrCategory.Products1.Count");
            Assert.AreSame(clrCategory.Products1[0], clrCategory.Products11[0], "clrCategory.Products11[0]");
            Assert.AreSame(clrCategory.Products1[1], clrCategory.Products11[1], "clrCategory.Products11[1]");

            Assert.AreEqual(2, clrCategory.Products2.Count, "clrCategory.Products2.Count");
            Assert.AreEqual(null, clrCategory.Products2.ElementAt(0), "clrCategory.Products2[0] is null");
            Assert.AreEqual("p22", clrCategory.Products2.ElementAt(1).Name, "clrCategory.Products2[1].Name");

            Assert.AreEqual(3, clrCategory.Products3.Count, "clrCategory.Products3.Count");
            Assert.AreEqual("sentinel3", clrCategory.Products3.ElementAt(0).Name, "clrCategory.Products3[0].Name");
            Assert.AreEqual("p31", clrCategory.Products3.ElementAt(1).Name, "clrCategory.Products3[1].Name");
            Assert.AreEqual("p32", clrCategory.Products3.ElementAt(2).Name, "clrCategory.Products3[2].Name");

            Assert.AreEqual(3, clrCategory.Products4.Count, "clrCategory.Products4.Count");
            Assert.AreEqual("sentinel4", clrCategory.Products4.ElementAt(0).Name, "clrCategory.Products4[0].Name");
            Assert.AreEqual("p41", clrCategory.Products4.ElementAt(1).Name, "clrCategory.Products4[1].Name");
            Assert.AreEqual(null, clrCategory.Products4.ElementAt(2), "clrCategory.Products4[2] is null");

            Assert.AreEqual(3, clrCategory.Products5.Count, "clrCategory.Products5.Count");
            Assert.AreEqual("sentinel5", clrCategory.Products5.ElementAt(0).Name, "clrCategory.Products5[0].Name");
            Assert.AreEqual("p51", clrCategory.Products5.ElementAt(1).Name, "clrCategory.Products5[1].Name");
            Assert.AreEqual("p52", clrCategory.Products5.ElementAt(2).Name, "clrCategory.Products5[2].Name");

            Assert.AreEqual(3, clrCategory.Products6.Count, "clrCategory.Products6.Count");
            Assert.AreEqual("sentinel6", clrCategory.Products6.ElementAt(0).Name, "clrCategory.Products6[0].Name");
            Assert.AreEqual("p61", clrCategory.Products6.ElementAt(1).Name, "clrCategory.Products6[1].Name");
            Assert.AreEqual("p62", clrCategory.Products6.ElementAt(2).Name, "clrCategory.Products6[2].Name");

            Assert.AreEqual(3, clrCategory.Products7.Count, "clrCategory.Products7.Count");
            Assert.AreEqual("sentinel7", clrCategory.Products7.ElementAt(0).Name, "clrCategory.Products7[0].Name");
            Assert.AreEqual("p71", clrCategory.Products7.ElementAt(1).Name, "clrCategory.Products7[1].Name");
            Assert.AreEqual("p72", clrCategory.Products7.ElementAt(2).Name, "clrCategory.Products7[2].Name");

            Assert.AreEqual(3, clrCategory.Ints.Count, "clrCategory.Ints.Count");
            Assert.AreEqual(1, clrCategory.Ints.ElementAt(0), "clrCategory.Ints[0]");
            Assert.AreEqual(2, clrCategory.Ints.ElementAt(1), "clrCategory.Ints[1]");
            Assert.AreEqual(3, clrCategory.Ints.ElementAt(2), "clrCategory.Ints[2]");

            Assert.AreEqual(3, clrCategory.IntsEnum.Count(), "clrCategory.IntsEnum.Count");
            Assert.AreEqual(10, clrCategory.IntsEnum.ElementAt(0), "clrCategory.IntsEnum[0]");
            Assert.AreEqual(20, clrCategory.IntsEnum.ElementAt(1), "clrCategory.IntsEnum[1]");
            Assert.AreEqual(30, clrCategory.IntsEnum.ElementAt(2), "clrCategory.IntsEnum[2]");

            Assert.AreEqual(5, clrCategory.ProductsPoly.Count, "clrCategory.ProductsPoly.Count");
            Assert.AreEqual("p1", clrCategory.ProductsPoly.ElementAt(0).Name, "clrCategory.ProductsPoly[0].Name");
            Assert.AreEqual("d1", clrCategory.ProductsPoly.ElementAt(1).Name, "clrCategory.ProductsPoly[1].Name");
            Assert.AreEqual("d1***", ((Derived)clrCategory.ProductsPoly.ElementAt(1)).Name2, "clrCategory.ProductsPoly[1].Name2");
            Assert.AreEqual("p2", clrCategory.ProductsPoly.ElementAt(2).Name, "clrCategory.ProductsPoly[2].Name");
            Assert.AreEqual("d2", clrCategory.ProductsPoly.ElementAt(3).Name, "clrCategory.ProductsPoly[3].Name");
            Assert.AreEqual("d2***", ((Derived)clrCategory.ProductsPoly.ElementAt(3)).Name2, "clrCategory.ProductsPoly[3].Name2");
            Assert.AreEqual("D3", clrCategory.ProductsPoly.ElementAt(4).Name, "clrCategory.ProductsPoly[4].Name");
            Assert.AreEqual("D3***", ((Derived2)clrCategory.ProductsPoly.ElementAt(4)).Name, "clrCategory.ProductsPoly[4].(Derived2)Name");
            Assert.AreEqual("D3", ((Derived2)clrCategory.ProductsPoly.ElementAt(4)).Name2, "clrCategory.ProductsPoly[4].Name2");
        }

        public class Cc1
        {
            public string P1 { get; set; }
        }

        public class Cc2 : Cc1
        {
            new public int P1 { get; set; }
        }

        [TestMethod]
        public void ConvertToClassWithNewProps()
        {
            IEdmStructuredValue cc2 = new EdmStructuredValue(null, new IEdmPropertyValue[]
            {
                new EdmPropertyValue("P1", new EdmIntegerConstant(20)),
            });

            EdmToClrConverter cv = new EdmToClrConverter();

            Cc2 cc2Clr = cv.AsClrValue<Cc2>(cc2);

            Assert.AreEqual(((Cc1)cc2Clr).P1, (string)null, "Cc1.P1");
            Assert.AreEqual(cc2Clr.P1, 20, "Cc2.P1");
        }

        [TestMethod]
        public void ConvertColOfCollections()
        {
            var innerColType = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true));
            var innerCol1 = new EdmCollectionValue(innerColType,
                new IEdmDelayedValue[] { new EdmIntegerConstant(1), new EdmIntegerConstant(2), new EdmIntegerConstant(3) });
            var innerCol2 = new EdmCollectionValue(innerColType,
                new IEdmDelayedValue[] { new EdmIntegerConstant(4), new EdmIntegerConstant(5), new EdmIntegerConstant(6) });

            var outerCol1 = new EdmCollectionValue(EdmCoreModel.GetCollection(innerColType),
                new IEdmDelayedValue[] { innerCol1, innerCol2 });

            EdmToClrConverter cv = new EdmToClrConverter();

            ICollection<ICollection<int>> colOfCol = cv.AsClrValue<ICollection<ICollection<int>>>(outerCol1);
            Assert.AreEqual(2, colOfCol.Count(), "colOfCol.Count()");
            Assert.AreEqual(3, colOfCol.ElementAt(0).Count(), "colOfCol.ElementAt(0).Count()");
            Assert.AreEqual(1, colOfCol.ElementAt(0).ElementAt(0), "colOfCol.ElementAt(0).ElementAt(0)");
            Assert.AreEqual(2, colOfCol.ElementAt(0).ElementAt(1), "colOfCol.ElementAt(0).ElementAt(1)");
            Assert.AreEqual(3, colOfCol.ElementAt(0).ElementAt(2), "colOfCol.ElementAt(0).ElementAt(2)");
            Assert.AreEqual(4, colOfCol.ElementAt(1).ElementAt(0), "colOfCol.ElementAt(1).ElementAt(0)");
            Assert.AreEqual(5, colOfCol.ElementAt(1).ElementAt(1), "colOfCol.ElementAt(1).ElementAt(1)");
            Assert.AreEqual(6, colOfCol.ElementAt(1).ElementAt(2), "colOfCol.ElementAt(1).ElementAt(2)");

            IList<IList<int>> listOfList = cv.AsClrValue<IList<IList<int>>>(outerCol1);
            Assert.AreEqual(2, listOfList.Count(), "listOfList.Count()");
            Assert.AreEqual(3, listOfList.ElementAt(0).Count(), "listOfList.ElementAt(0).Count()");
            Assert.AreEqual(1, listOfList.ElementAt(0).ElementAt(0), "listOfList.ElementAt(0).ElementAt(0)");
            Assert.AreEqual(2, listOfList.ElementAt(0).ElementAt(1), "listOfList.ElementAt(0).ElementAt(1)");
            Assert.AreEqual(3, listOfList.ElementAt(0).ElementAt(2), "listOfList.ElementAt(0).ElementAt(2)");
            Assert.AreEqual(4, listOfList.ElementAt(1).ElementAt(0), "listOfList.ElementAt(1).ElementAt(0)");
            Assert.AreEqual(5, listOfList.ElementAt(1).ElementAt(1), "listOfList.ElementAt(1).ElementAt(1)");
            Assert.AreEqual(6, listOfList.ElementAt(1).ElementAt(2), "listOfList.ElementAt(1).ElementAt(2)");

            IEnumerable<IEnumerable<int>> enumOfEnum = cv.AsClrValue<IEnumerable<IEnumerable<int>>>(outerCol1);
            Assert.AreEqual(2, enumOfEnum.Count(), "enumOfEnum.Count()");
            Assert.AreEqual(3, enumOfEnum.ElementAt(0).Count(), "enumOfEnum.ElementAt(0).Count()");
            Assert.AreEqual(1, enumOfEnum.ElementAt(0).ElementAt(0), "enumOfEnum.ElementAt(0).ElementAt(0)");
            Assert.AreEqual(2, enumOfEnum.ElementAt(0).ElementAt(1), "enumOfEnum.ElementAt(0).ElementAt(1)");
            Assert.AreEqual(3, enumOfEnum.ElementAt(0).ElementAt(2), "enumOfEnum.ElementAt(0).ElementAt(2)");
            Assert.AreEqual(4, enumOfEnum.ElementAt(1).ElementAt(0), "enumOfEnum.ElementAt(1).ElementAt(0)");
            Assert.AreEqual(5, enumOfEnum.ElementAt(1).ElementAt(1), "enumOfEnum.ElementAt(1).ElementAt(1)");
            Assert.AreEqual(6, enumOfEnum.ElementAt(1).ElementAt(2), "enumOfEnum.ElementAt(1).ElementAt(2)");
        }

        public class ColListEnumProp
        {
            public ICollection<ICollection<int>> ColCol { get; set; }
            public ICollection<ICollection<int>> ColCol2 { get { return this.colCol2; } }
            private List<ICollection<int>> colCol2 = new List<ICollection<int>>();

            public ICollection<IEnumerable<int>> ColEnum { get; set; }
            public ICollection<IEnumerable<int>> ColEnum2 { get { return this.colEnum2; } }
            private List<IEnumerable<int>> colEnum2 = new List<IEnumerable<int>>();

            public ICollection<IList<int>> ColList { get; set; }
            public ICollection<IList<int>> ColList2 { get { return this.colList2; } }
            private List<IList<int>> colList2 = new List<IList<int>>();

            public IList<IEnumerable<int>> ListEnum { get; set; }
            public IList<IEnumerable<int>> ListEnum2 { get { return this.listEnum2; } }
            private List<IEnumerable<int>> listEnum2 = new List<IEnumerable<int>>();

            public IEnumerable<IList<int>> EnumList { get; set; }

            public IEnumerable<IEnumerable<int>> EnumEnum { get; set; }
        }

        [TestMethod]
        public void ConvertColOfCollectionsProps()
        {
            int i = 0;
            Func<IEdmValue> createColOfColValue = () =>
                {
                    var innerColType = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true));
                    var innerCol1 = new EdmCollectionValue(innerColType,
                        new IEdmDelayedValue[] { new EdmIntegerConstant(++i), new EdmIntegerConstant(++i), new EdmIntegerConstant(++i) });
                    var innerCol2 = new EdmCollectionValue(innerColType,
                        new IEdmDelayedValue[] { new EdmIntegerConstant(++i), new EdmIntegerConstant(++i), new EdmIntegerConstant(++i) });
                    var outerCol = new EdmCollectionValue(EdmCoreModel.GetCollection(innerColType),
                        new IEdmDelayedValue[] { innerCol1, innerCol2 });
                    return outerCol;
                };

            IEdmStructuredValue colListEnumProp = new EdmStructuredValue(null, new IEdmPropertyValue[] 
            {
                new EdmPropertyValue("ColCol", createColOfColValue()),
                new EdmPropertyValue("ColCol2", createColOfColValue()),
                new EdmPropertyValue("ColEnum", createColOfColValue()),
                new EdmPropertyValue("ColEnum2", createColOfColValue()),
                new EdmPropertyValue("ColList", createColOfColValue()),
                new EdmPropertyValue("ColList2", createColOfColValue()),
                new EdmPropertyValue("ListEnum", createColOfColValue()),
                new EdmPropertyValue("ListEnum2", createColOfColValue()),
                new EdmPropertyValue("EnumList", createColOfColValue()),
                new EdmPropertyValue("EnumEnum", createColOfColValue()),
            });

            EdmToClrConverter cv = new EdmToClrConverter();

            i = 0;
            Action<string, IEnumerable> checkColOfColValue = (name, value) =>
                {
                    IEnumerable<IEnumerable<int>> enumOfEnum = value.Cast<IEnumerable<int>>();
                    Assert.AreEqual(2, enumOfEnum.Count(), name + ".Count()");
                    Assert.AreEqual(3, enumOfEnum.ElementAt(0).Count(), name + ".ElementAt(0).Count()");
                    Assert.AreEqual(++i, enumOfEnum.ElementAt(0).ElementAt(0), name + ".ElementAt(0).ElementAt(0)");
                    Assert.AreEqual(++i, enumOfEnum.ElementAt(0).ElementAt(1), name + ".ElementAt(0).ElementAt(1)");
                    Assert.AreEqual(++i, enumOfEnum.ElementAt(0).ElementAt(2), name + ".ElementAt(0).ElementAt(2)");
                    Assert.AreEqual(++i, enumOfEnum.ElementAt(1).ElementAt(0), name + ".ElementAt(1).ElementAt(0)");
                    Assert.AreEqual(++i, enumOfEnum.ElementAt(1).ElementAt(1), name + ".ElementAt(1).ElementAt(1)");
                    Assert.AreEqual(++i, enumOfEnum.ElementAt(1).ElementAt(2), name + ".ElementAt(1).ElementAt(2)");
                };

            ColListEnumProp colListEnumPropClr = cv.AsClrValue<ColListEnumProp>(colListEnumProp);
            checkColOfColValue("ColCol", colListEnumPropClr.ColCol);
            checkColOfColValue("ColCol2", colListEnumPropClr.ColCol2);
            checkColOfColValue("ColEnum", colListEnumPropClr.ColEnum);
            checkColOfColValue("ColEnum2", colListEnumPropClr.ColEnum2);
            checkColOfColValue("ColList", colListEnumPropClr.ColList);
            checkColOfColValue("ColList2", colListEnumPropClr.ColList2);
            checkColOfColValue("ListEnum", colListEnumPropClr.ListEnum);
            checkColOfColValue("ListEnum2", colListEnumPropClr.ListEnum2);
            checkColOfColValue("EnumList", colListEnumPropClr.EnumList);
            checkColOfColValue("EnumEnum", colListEnumPropClr.EnumEnum);
        }

        [TestMethod]
        public void TestConversionOfDuplicatePropValues()
        {
            IEdmStructuredValue cc1 = new EdmStructuredValue(null, new IEdmPropertyValue[]
            {
                new EdmPropertyValue("P1", new EdmStringConstant("1")),
                new EdmPropertyValue("P1", new EdmStringConstant("2")),
            });

            EdmToClrConverter cv = new EdmToClrConverter();

            try
            {
                cv.AsClrValue<Cc2>(cc1);
                Assert.Fail("InvalidCastException expected");
            }
            catch (InvalidCastException)
            {
            }
        }

        public class NotSupportedCollectionProps
        {
            public int[] Prop { get; set; }
        }

        public class NotSupportedCollectionProps2
        {
            public IEnumerable Prop { get; set; }
        }

        public class NotSupportedCollectionProps3
        {
            public NotSupportedCollectionProps Prop2 { get; set; }
        }

        [TestMethod]
        public void ConvertNotSupportedCollectionProps()
        {
            var innerColType = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(true));
            IEdmStructuredValue colStruct = new EdmStructuredValue(null, new IEdmPropertyValue[] 
            {
                new EdmPropertyValue("Prop", new EdmCollectionValue(innerColType,
                        new IEdmDelayedValue[] { new EdmIntegerConstant(1), new EdmIntegerConstant(2), new EdmIntegerConstant(3) })),
                new EdmPropertyValue("Prop2", new EdmIntegerConstant(1)),
            });

            EdmToClrConverter cv = new EdmToClrConverter();

            try
            {
                cv.AsClrValue<NotSupportedCollectionProps>(colStruct);
                Assert.Fail("InvalidCastException expected");
            }
            catch (InvalidCastException e)
            {
                Assert.IsTrue(e.Message.Contains("System.Int32[]"), "System.Int32[]");
                Assert.IsTrue(e.Message.Contains("System.Collections.Generic.IEnumerable<T>"), "System.Collections.Generic.IEnumerable<T>");
                Assert.IsTrue(e.Message.Contains("System.Collections.Generic.IList<T>"), "System.Collections.Generic.IList<T>");
                Assert.IsTrue(e.Message.Contains("System.Collections.Generic.ICollection<T>"), "System.Collections.Generic.ICollection<T>");
            }

            cv = new EdmToClrConverter();
            try
            {
                cv.AsClrValue<NotSupportedCollectionProps2>(colStruct);
                Assert.Fail("InvalidCastException expected");
            }
            catch (InvalidCastException e)
            {
                Assert.IsTrue(e.Message.Contains("System.Collections.IEnumerable"), "System.Collections.IEnumerable");
                Assert.IsTrue(e.Message.Contains("System.Collections.Generic.IEnumerable<T>"), "System.Collections.Generic.IEnumerable<T>");
                Assert.IsTrue(e.Message.Contains("System.Collections.Generic.IList<T>"), "System.Collections.Generic.IList<T>");
                Assert.IsTrue(e.Message.Contains("System.Collections.Generic.ICollection<T>"), "System.Collections.Generic.ICollection<T>");
            }

            cv = new EdmToClrConverter();
            try
            {
                cv.AsClrValue<NotSupportedCollectionProps3>(colStruct);
                Assert.Fail("InvalidCastException expected");
            }
            catch (InvalidCastException e)
            {
                Assert.IsTrue(e.Message.Contains("IEdmIntegerConstantExpression"), "IEdmIntegerConstantExpression");
                Assert.IsTrue(e.Message.Contains("EdmLibTests.FunctionalTests.EdmToClrConversion+NotSupportedCollectionProps"), "EdmLibTests.FunctionalTests.EdmToClrConversion+NotSupportedCollectionProps");
            }
        }

        [TestMethod]
        public void TryCreateObjectInstanceReturnsWrongObject()
        {
            IEdmStructuredValue colStruct = new EdmStructuredValue(null, new IEdmPropertyValue[] 
            {
                new EdmPropertyValue("Prop", new EdmIntegerConstant(1))
            });

            EdmToClrConverter cv = new EdmToClrConverter((IEdmStructuredValue edmValue, Type clrType, EdmToClrConverter converter, out object objectInstance, out bool objectInstanceInitialized) =>
            {
                objectInstance = new object();
                objectInstanceInitialized = false;
                return true;
            });

            try
            {
                cv.AsClrValue<NotSupportedCollectionProps2>(colStruct);
                Assert.Fail("InvalidCastException expected");
            }
            catch (InvalidCastException e)
            {
                Assert.IsTrue(e.Message.Contains("System.Object"), "System.Object");
                Assert.IsTrue(e.Message.Contains("TryCreateObjectInstance"), "TryCreateObjectInstance");
                Assert.IsTrue(e.Message.Contains("EdmLibTests.FunctionalTests.EdmToClrConversion+NotSupportedCollectionProps2"), "EdmLibTests.FunctionalTests.EdmToClrConversion+NotSupportedCollectionProps2");
            }
        }
    }
}
