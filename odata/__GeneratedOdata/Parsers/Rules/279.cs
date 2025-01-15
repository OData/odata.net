namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _decimalValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._decimalValue> Instance { get; } = (_꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡Parser.Instance).Or<__GeneratedOdata.CstNodes.Rules._decimalValue>(_nanInfinityParser.Instance);
        
        public static class _꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡Parser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._decimalValue._꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡> Instance { get; } = from _SIGN_1 in __GeneratedOdata.Parsers.Rules._SIGNParser.Instance.Optional()
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
from _ʺx2Eʺ_1ЖDIGIT_1 in __GeneratedOdata.Parsers.Inners._ʺx2Eʺ_1ЖDIGITParser.Instance.Optional()
from _ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT_1 in __GeneratedOdata.Parsers.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGITParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._decimalValue._꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡(_SIGN_1.GetOrElse(null), new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Rules._DIGIT>(_DIGIT_1), _ʺx2Eʺ_1ЖDIGIT_1.GetOrElse(null), _ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT_1.GetOrElse(null));
        }
        
        public static class _nanInfinityParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._decimalValue._nanInfinity> Instance { get; } = from _nanInfinity_1 in __GeneratedOdata.Parsers.Rules._nanInfinityParser.Instance
select new __GeneratedOdata.CstNodes.Rules._decimalValue._nanInfinity(_nanInfinity_1);
        }
    }
    
}
