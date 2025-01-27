namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _guidValueParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._guidValue> Instance { get; } = from _HEXDIG_1 in __GeneratedOdata.Parsers.Rules._HEXDIGParser.Instance.Repeat(8, 8)
from _ʺx2Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2DʺParser.Instance
from _HEXDIG_2 in __GeneratedOdata.Parsers.Rules._HEXDIGParser.Instance.Repeat(4, 4)
from _ʺx2Dʺ_2 in __GeneratedOdata.Parsers.Inners._ʺx2DʺParser.Instance
from _HEXDIG_3 in __GeneratedOdata.Parsers.Rules._HEXDIGParser.Instance.Repeat(4, 4)
from _ʺx2Dʺ_3 in __GeneratedOdata.Parsers.Inners._ʺx2DʺParser.Instance
from _HEXDIG_4 in __GeneratedOdata.Parsers.Rules._HEXDIGParser.Instance.Repeat(4, 4)
from _ʺx2Dʺ_4 in __GeneratedOdata.Parsers.Inners._ʺx2DʺParser.Instance
from _HEXDIG_5 in __GeneratedOdata.Parsers.Rules._HEXDIGParser.Instance.Repeat(12, 12)
select new __GeneratedOdata.CstNodes.Rules._guidValue(new __GeneratedOdata.CstNodes.Inners.HelperRangedExactly8<__GeneratedOdata.CstNodes.Rules._HEXDIG>(_HEXDIG_1), _ʺx2Dʺ_1, new __GeneratedOdata.CstNodes.Inners.HelperRangedExactly4<__GeneratedOdata.CstNodes.Rules._HEXDIG>(_HEXDIG_2), _ʺx2Dʺ_2, new __GeneratedOdata.CstNodes.Inners.HelperRangedExactly4<__GeneratedOdata.CstNodes.Rules._HEXDIG>(_HEXDIG_3), _ʺx2Dʺ_3, new __GeneratedOdata.CstNodes.Inners.HelperRangedExactly4<__GeneratedOdata.CstNodes.Rules._HEXDIG>(_HEXDIG_4), _ʺx2Dʺ_4, new __GeneratedOdata.CstNodes.Inners.HelperRangedExactly12<__GeneratedOdata.CstNodes.Rules._HEXDIG>(_HEXDIG_5));
    }
    
}
