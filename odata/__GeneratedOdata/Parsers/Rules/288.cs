namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _int64ValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._int64Value> Instance { get; } = from _SIGN_1 in __GeneratedOdata.Parsers.Rules._SIGNParser.Instance.Optional()
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._int64Value(_SIGN_1.GetOrElse(null), _DIGIT_1);
    }
    
}
