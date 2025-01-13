namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx65ʺ_꘡SIGN꘡_1ЖDIGITParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT> Instance { get; } = from _ʺx65ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx65ʺParser.Instance
from _SIGN_1 in __GeneratedOdata.Parsers.Rules._SIGNParser.Instance.Optional()
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT(_ʺx65ʺ_1, _SIGN_1.GetOrElse(null), _DIGIT_1);
    }
    
}
