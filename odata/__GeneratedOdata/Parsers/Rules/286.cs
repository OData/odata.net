namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _int16ValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._int16Value> Instance { get; } = from _SIGN_1 in __GeneratedOdata.Parsers.Rules._SIGNParser.Instance.Optional()
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._int16Value(_SIGN_1.GetOrElse(null), _DIGIT_1);
    }
    
}
