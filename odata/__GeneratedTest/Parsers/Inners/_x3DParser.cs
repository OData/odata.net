namespace __GeneratedTest.Parsers.Inners
{
    using Sprache;
    
    public static class _x3DParser
    {
        public static Parser<__Generated.CstNodes.Inners._x3D> Instance { get; } = from _x3D in Parse.Char((char)0x3D) select __Generated.CstNodes.Inners._x3D.Instance;
    }
    
}
