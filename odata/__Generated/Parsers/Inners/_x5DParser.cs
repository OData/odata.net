namespace __Generated.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x5DParser
    {
        public static IParser<char, __Generated.CstNodes.Inners._x5D> Instance { get; } = from _x5D in Parse.Char((char)0x5D) select __Generated.CstNodes.Inners._x5D.Instance;
    }
    
}
