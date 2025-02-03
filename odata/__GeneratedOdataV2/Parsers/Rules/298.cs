namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _yearParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._year> Instance { get; } = from _ʺx2Dʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2DʺParser.Instance.Optional()
from _Ⲥʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITↃ_1 in __GeneratedOdataV2.Parsers.Inners._Ⲥʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITↃParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._year(_ʺx2Dʺ_1.GetOrElse(null), _Ⲥʺx30ʺ_3DIGITⳆoneToNine_3ЖDIGITↃ_1);
    }
    
}
