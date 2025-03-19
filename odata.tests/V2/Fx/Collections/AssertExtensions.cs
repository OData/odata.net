namespace V2.Fx.Collections //// TODO use the right namespace
{
    using System.Collections;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class AssertExtensions
    {
        public static void AreEqual<TEnumerableFirst, TEnumerableSecond>(TEnumerableFirst first, TEnumerableSecond second)
            where TEnumerableFirst : IEnumerable, allows ref struct 
            where TEnumerableSecond : IEnumerable, allows ref struct
            //// TODO do all of the overloads that `assert`s have
        {
            var firstEnumerator = first.GetEnumerator();
            var secondEnumerator = second.GetEnumerator();

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
            }
        }
    }
}
