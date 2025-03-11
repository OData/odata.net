namespace Fx.Collections
{
    public static class LinkedListNodeExtensions
    {
        public static LinkedListNode<T> Append<T>(this LinkedListNode<T> source, T value, Span<byte> previousMemory) where T : allows ref struct
        {
            var span = BetterSpan.FromInstance(value);
            return source.Append(span, previousMemory);
        }
    }
}
