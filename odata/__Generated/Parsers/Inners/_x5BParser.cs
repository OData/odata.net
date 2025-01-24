namespace __Generated.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x5BParser
    {
        public static IParser<char, __Generated.CstNodes.Inners._x5B> Instance { get; } = from _x5B in Parse.Char((char)0x5B) select __Generated.CstNodes.Inners._x5B.Instance;
    }
    
}
