namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _multiPointLiteralParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._multiPointLiteral> Instance { get; } = from _ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74x28ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74x28ʺParser.Instance
from _pointData_ЖⲤCOMMA_pointDataↃ_1 in __GeneratedOdataV2.Parsers.Inners._pointData_ЖⲤCOMMA_pointDataↃParser.Instance.Optional()
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._multiPointLiteral(_ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74x28ʺ_1, _pointData_ЖⲤCOMMA_pointDataↃ_1.GetOrElse(null), _CLOSE_1);
    }
    
}
