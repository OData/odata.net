namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _int64ValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._int64Value> Instance { get; } = from _SIGN_1 in __GeneratedOdataV2.Parsers.Rules._SIGNParser.Instance.Optional()
from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance.Repeat(1, 19)
select new __GeneratedOdataV2.CstNodes.Rules._int64Value(_SIGN_1.GetOrElse(null), new __GeneratedOdataV2.CstNodes.Inners.HelperRangedFrom1To19<__GeneratedOdataV2.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
