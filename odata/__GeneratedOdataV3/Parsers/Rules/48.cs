namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _crossjoinParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._crossjoin> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._crossjoin>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._crossjoin> Parse(IInput<char>? input)
            {
                var _ʺx24x63x72x6Fx73x73x6Ax6Fx69x6Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x63x72x6Fx73x73x6Ax6Fx69x6EʺParser.Instance.Parse(input);
if (!_ʺx24x63x72x6Fx73x73x6Ax6Fx69x6Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._crossjoin)!, input);
}

var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(_ʺx24x63x72x6Fx73x73x6Ax6Fx69x6Eʺ_1.Remainder);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._crossjoin)!, input);
}

var _entitySetName_1 = __GeneratedOdataV3.Parsers.Rules._entitySetNameParser.Instance.Parse(_OPEN_1.Remainder);
if (!_entitySetName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._crossjoin)!, input);
}

var _ⲤCOMMA_entitySetNameↃ_1 = Inners._ⲤCOMMA_entitySetNameↃParser.Instance.Many().Parse(_entitySetName_1.Remainder);
if (!_ⲤCOMMA_entitySetNameↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._crossjoin)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_ⲤCOMMA_entitySetNameↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._crossjoin)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._crossjoin(_ʺx24x63x72x6Fx73x73x6Ax6Fx69x6Eʺ_1.Parsed, _OPEN_1.Parsed, _entitySetName_1.Parsed, _ⲤCOMMA_entitySetNameↃ_1.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
