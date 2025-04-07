namespace CombinatorParsingV3
{
    /// <summary>
    /// NOTE: you considered having a class variant of this for cases where the caller needs to avoid boxing, but based on nullabletests.test4 there is basically no different in perforamnce
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct Optional<T>
    {
        private readonly T value;

        private readonly bool hasValue;

        public Optional(T value)
        {
            this.value = value;
            this.hasValue = true;
        }

        public bool TryGetValue([System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out T value)
        {
            value = this.value;
            return this.hasValue;
        }

        public static implicit operator Optional<T>(T value)
        {
            return new Optional<T>(value);
        }
    }
}
