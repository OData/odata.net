namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _computeParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._compute> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._compute>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._compute> Parse(IInput<char>? input)
            {
                var _Ⲥʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺↃParser.Instance.Parse(input);
if (!_Ⲥʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._compute)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_Ⲥʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺↃ_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._compute)!, input);
}

var _computeItem_1 = __GeneratedOdataV3.Parsers.Rules._computeItemParser.Instance.Parse(_EQ_1.Remainder);
if (!_computeItem_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._compute)!, input);
}

var _ⲤCOMMA_computeItemↃ_1 = Inners._ⲤCOMMA_computeItemↃParser.Instance.Many().Parse(_computeItem_1.Remainder);
if (!_ⲤCOMMA_computeItemↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._compute)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._compute(_Ⲥʺx24x63x6Fx6Dx70x75x74x65ʺⳆʺx63x6Fx6Dx70x75x74x65ʺↃ_1.Parsed, _EQ_1.Parsed, _computeItem_1.Parsed, _ⲤCOMMA_computeItemↃ_1.Parsed), _ⲤCOMMA_computeItemↃ_1.Remainder);
            }
        }
    }
    
}
