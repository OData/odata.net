namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x3DParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._x3D> Instance { get; } = from _x3D in Parse.Char((char)0x3D) select __GeneratedOdataV2.CstNodes.Inners._x3D.Instance;
    }
    
}
