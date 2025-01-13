namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _expandParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._expand> Instance { get; } = from _Ⲥʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺↃParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _expandItem_1 in __GeneratedOdata.Parsers.Rules._expandItemParser.Instance
from _ⲤCOMMA_expandItemↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤCOMMA_expandItemↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._expand(_Ⲥʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺↃ_1, _EQ_1, _expandItem_1, _ⲤCOMMA_expandItemↃ_1);
    }
    
}
