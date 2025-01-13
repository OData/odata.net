namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _guidValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._guidValue> Instance { get; } = from _HEXDIG_1 in __GeneratedOdata.Parsers.Rules._HEXDIGParser.Instance.Many()
from _ʺx2Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2DʺParser.Instance
from _HEXDIG_2 in __GeneratedOdata.Parsers.Rules._HEXDIGParser.Instance.Many()
from _ʺx2Dʺ_2 in __GeneratedOdata.Parsers.Inners._ʺx2DʺParser.Instance
from _HEXDIG_3 in __GeneratedOdata.Parsers.Rules._HEXDIGParser.Instance.Many()
from _ʺx2Dʺ_3 in __GeneratedOdata.Parsers.Inners._ʺx2DʺParser.Instance
from _HEXDIG_4 in __GeneratedOdata.Parsers.Rules._HEXDIGParser.Instance.Many()
from _ʺx2Dʺ_4 in __GeneratedOdata.Parsers.Inners._ʺx2DʺParser.Instance
from _HEXDIG_5 in __GeneratedOdata.Parsers.Rules._HEXDIGParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._guidValue(_HEXDIG_1, _ʺx2Dʺ_1, _HEXDIG_2, _ʺx2Dʺ_2, _HEXDIG_3, _ʺx2Dʺ_3, _HEXDIG_4, _ʺx2Dʺ_4, _HEXDIG_5);
    }
    
}
