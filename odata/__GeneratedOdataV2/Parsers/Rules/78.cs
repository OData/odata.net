namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _inlinecountParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._inlinecount> Instance { get; } = from _Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃParser.Instance
from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _booleanValue_1 in __GeneratedOdataV2.Parsers.Rules._booleanValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._inlinecount(_Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃ_1, _EQ_1, _booleanValue_1);
    }
    
}
