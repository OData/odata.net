namespace __GeneratedTest.Parsers.Inners
{
    using Sprache;
    
    public static class _x3CParser
    {
        public static Parser<__Generated.CstNodes.Inners._x3C> Instance { get; } = from _x3C in Parse.Char((char)0x3C) select __Generated.CstNodes.Inners._x3C.Instance;
    }
    
}
