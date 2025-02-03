namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _multiPointLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._multiPointLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._multiPointLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._multiPointLiteral> Parse(IInput<char>? input)
            {
                var _ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74x28ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74x28ʺParser.Instance.Parse(input);
if (!_ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74x28ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._multiPointLiteral)!, input);
}

var _pointData_ЖⲤCOMMA_pointDataↃ_1 = __GeneratedOdataV3.Parsers.Inners._pointData_ЖⲤCOMMA_pointDataↃParser.Instance.Optional().Parse(_ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74x28ʺ_1.Remainder);
if (!_pointData_ЖⲤCOMMA_pointDataↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._multiPointLiteral)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_pointData_ЖⲤCOMMA_pointDataↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._multiPointLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._multiPointLiteral(_ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74x28ʺ_1.Parsed, _pointData_ЖⲤCOMMA_pointDataↃ_1.Parsed.GetOrElse(null),  _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
