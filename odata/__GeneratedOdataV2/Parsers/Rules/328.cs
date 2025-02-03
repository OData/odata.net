namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _sridLiteralParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._sridLiteral> Instance { get; } = from _ʺx53x52x49x44ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx53x52x49x44ʺParser.Instance
from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance.Repeat(1, 5)
from _SEMI_1 in __GeneratedOdataV2.Parsers.Rules._SEMIParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._sridLiteral(_ʺx53x52x49x44ʺ_1, _EQ_1, new __GeneratedOdataV2.CstNodes.Inners.HelperRangedFrom1To5<__GeneratedOdataV2.CstNodes.Rules._DIGIT>(_DIGIT_1), _SEMI_1);
    }
    
}
