namespace __Generated.Parsers.Inners
{
    using _GeneratorV5.ManualParsers.Rules;
    using Sprache;
    
    public static class _1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡Parser
    {
        public static Parser<__Generated.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡> Instance { get; } = from _DIGIT_1 in __Generated.Parsers.Rules._DIGITParser.Instance.Many()
from _ʺx2Aʺ_ЖDIGIT_1 in __Generated.Parsers.Inners._ʺx2Aʺ_ЖDIGITParser.Instance.Optional()
select new __Generated.CstNodes.Inners._1ЖDIGIT_꘡ʺx2Aʺ_ЖDIGIT꘡(_DIGIT_1.Convert2(), _ʺx2Aʺ_ЖDIGIT_1.GetOrElse(null));
    }
    
}
