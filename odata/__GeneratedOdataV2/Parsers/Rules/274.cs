namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _binaryValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._binaryValue> Instance { get; } = from _Ⲥ4base64charↃ_1 in Inners._Ⲥ4base64charↃParser.Instance.Many()
from _base64b16Ⳇbase64b8_1 in __GeneratedOdataV2.Parsers.Inners._base64b16Ⳇbase64b8Parser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._binaryValue(_Ⲥ4base64charↃ_1, _base64b16Ⳇbase64b8_1.GetOrElse(null));
    }
    
}
