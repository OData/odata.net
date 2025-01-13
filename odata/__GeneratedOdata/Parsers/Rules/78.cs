namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _inlinecountParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._inlinecount> Instance { get; } = from _Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _booleanValue_1 in __GeneratedOdata.Parsers.Rules._booleanValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._inlinecount(_Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃ_1, _EQ_1, _booleanValue_1);
    }
    
}
