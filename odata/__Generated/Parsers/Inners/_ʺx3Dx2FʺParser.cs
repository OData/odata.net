namespace __Generated.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx3Dx2FʺParser
    {
        public static IParser<char, __Generated.CstNodes.Inners._ʺx3Dx2Fʺ> Instance { get; } = from _x3D_1 in __Generated.Parsers.Inners._x3DParser.Instance
from _x2F_1 in __Generated.Parsers.Inners._x2FParser.Instance
select new __Generated.CstNodes.Inners._ʺx3Dx2Fʺ(_x3D_1, _x2F_1);
    }
    
}
