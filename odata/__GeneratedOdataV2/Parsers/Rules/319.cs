namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _multiLineStringLiteralParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._multiLineStringLiteral> Instance { get; } = from _ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67x28ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67x28ʺParser.Instance
from _lineStringData_ЖⲤCOMMA_lineStringDataↃ_1 in __GeneratedOdataV2.Parsers.Inners._lineStringData_ЖⲤCOMMA_lineStringDataↃParser.Instance.Optional()
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._multiLineStringLiteral(_ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67x28ʺ_1, _lineStringData_ЖⲤCOMMA_lineStringDataↃ_1.GetOrElse(null), _CLOSE_1);
    }
    
}
