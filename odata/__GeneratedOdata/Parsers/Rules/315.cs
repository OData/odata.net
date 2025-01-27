namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _lineStringLiteralParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._lineStringLiteral> Instance { get; } = from _ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺParser.Instance
from _lineStringData_1 in __GeneratedOdata.Parsers.Rules._lineStringDataParser.Instance
select new __GeneratedOdata.CstNodes.Rules._lineStringLiteral(_ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺ_1, _lineStringData_1);
    }
    
}
