namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pointLiteralParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._pointLiteral> Instance { get; } = from _ʺx50x6Fx69x6Ex74ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx50x6Fx69x6Ex74ʺParser.Instance
from _pointData_1 in __GeneratedOdata.Parsers.Rules._pointDataParser.Instance
select new __GeneratedOdata.CstNodes.Rules._pointLiteral(_ʺx50x6Fx69x6Ex74ʺ_1, _pointData_1);
    }
    
}
