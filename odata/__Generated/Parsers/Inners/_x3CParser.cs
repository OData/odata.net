namespace __Generated.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x3CParser
    {
        public static IParser<char, __Generated.CstNodes.Inners._x3C> Instance { get; } = from _x3C in Parse.Char((char)0x3C) select __Generated.CstNodes.Inners._x3C.Instance;
    }
    
}
