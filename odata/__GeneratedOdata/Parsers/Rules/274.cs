namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _binaryValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._binaryValue> Instance { get; } = from _Ⲥ4base64charↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥ4base64charↃParser.Instance.Many()
from _base64b16Ⳇbase64b8_1 in __GeneratedOdata.Parsers.Inners._base64b16Ⳇbase64b8Parser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._binaryValue(_Ⲥ4base64charↃ_1, _base64b16Ⳇbase64b8_1.GetOrElse(null));
    }
    
}
