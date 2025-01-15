namespace __Generated.Parsers.Inners
{
    using _GeneratorV5.ManualParsers.Rules;
    using Sprache;
    
    public static class _ʺx2Aʺ_ЖDIGITParser
    {
        public static Parser<__Generated.CstNodes.Inners._ʺx2Aʺ_ЖDIGIT> Instance { get; } = from _ʺx2Aʺ_1 in __Generated.Parsers.Inners._ʺx2AʺParser.Instance
from _DIGIT_1 in __Generated.Parsers.Rules._DIGITParser.Instance.Many()
select new __Generated.CstNodes.Inners._ʺx2Aʺ_ЖDIGIT(_ʺx2Aʺ_1, _DIGIT_1.Convert());
    }
    
}
