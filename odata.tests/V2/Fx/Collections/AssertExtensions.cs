namespace V2.Fx.Collections //// TODO use the right namespace
{
    using System.Collections;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class AssertExtensions
    {
        public static void AreEqualEnumerables<TEnumerableFirst, TEnumerableSecond>(this Assert self, TEnumerableFirst expected, TEnumerableSecond actual)
            where TEnumerableFirst : IEnumerable, allows ref struct 
            where TEnumerableSecond : IEnumerable, allows ref struct
            //// TODO do all of the overloads that `assert`s have
        {
            /*var firstEnumerator = expected.GetEnumerator();
            var secondEnumerator = actual.GetEnumerator();

            while (true)
            {
                if (firstEnumerator.MoveNext())
                {
                    //// TODO this method should throw its own exceptions
                    Assert.IsTrue(secondEnumerator.MoveNext());
                    Assert.AreEqual(firstEnumerator.Current, secondEnumerator.Current);
                }
                else
                {
                    //// TODO this method should throw its own exceptions
                    Assert.IsFalse(secondEnumerator.MoveNext());
                }
            }*/

            int expectedIndex = 0;
            foreach (var expectedElement in actual)
            {
            }
        }
    }
}
