namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _COMMA_entitySetNameParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._COMMA_entitySetName> Instance { get; } = from _COMMA_1 in __GeneratedOdataV2.Parsers.Rules._COMMAParser.Instance
from _entitySetName_1 in __GeneratedOdataV2.Parsers.Rules._entitySetNameParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._COMMA_entitySetName(_COMMA_1, _entitySetName_1);
    }
    
}
