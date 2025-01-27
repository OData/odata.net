namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _int64ValueParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._int64Value> Instance { get; } = from _SIGN_1 in __GeneratedOdata.Parsers.Rules._SIGNParser.Instance.Optional()
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Repeat(1, 19)
select new __GeneratedOdata.CstNodes.Rules._int64Value(_SIGN_1.GetOrElse(null), new __GeneratedOdata.CstNodes.Inners.HelperRangedFrom1To19<__GeneratedOdata.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
