namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _int32ValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._int32Value> Instance { get; } = from _SIGN_1 in __GeneratedOdataV2.Parsers.Rules._SIGNParser.Instance.Optional()
from _DIGIT_1 in __GeneratedOdataV2.Parsers.Rules._DIGITParser.Instance.Repeat(1, 10)
select new __GeneratedOdataV2.CstNodes.Rules._int32Value(_SIGN_1.GetOrElse(null), new __GeneratedOdataV2.CstNodes.Inners.HelperRangedFrom1To10<__GeneratedOdataV2.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
