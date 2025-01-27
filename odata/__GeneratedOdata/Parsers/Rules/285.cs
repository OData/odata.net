namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _sbyteValueParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._sbyteValue> Instance { get; } = from _SIGN_1 in __GeneratedOdata.Parsers.Rules._SIGNParser.Instance.Optional()
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Repeat(1, 3)
select new __GeneratedOdata.CstNodes.Rules._sbyteValue(_SIGN_1.GetOrElse(null), new __GeneratedOdata.CstNodes.Inners.HelperRangedFrom1To3<__GeneratedOdata.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
