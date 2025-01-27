namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _polygonLiteralParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._polygonLiteral> Instance { get; } = from _ʺx50x6Fx6Cx79x67x6Fx6Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx50x6Fx6Cx79x67x6Fx6EʺParser.Instance
from _polygonData_1 in __GeneratedOdata.Parsers.Rules._polygonDataParser.Instance
select new __GeneratedOdata.CstNodes.Rules._polygonLiteral(_ʺx50x6Fx6Cx79x67x6Fx6Eʺ_1, _polygonData_1);
    }
    
}
