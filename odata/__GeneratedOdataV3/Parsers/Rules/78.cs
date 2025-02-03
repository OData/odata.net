namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _inlinecountParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._inlinecount> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._inlinecount>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._inlinecount> Parse(IInput<char>? input)
            {
                var _Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃParser.Instance.Parse(input);
if (!_Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._inlinecount)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃ_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._inlinecount)!, input);
}

var _booleanValue_1 = __GeneratedOdataV3.Parsers.Rules._booleanValueParser.Instance.Parse(_EQ_1.Remainder);
if (!_booleanValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._inlinecount)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._inlinecount(_Ⲥʺx24x63x6Fx75x6Ex74ʺⳆʺx63x6Fx75x6Ex74ʺↃ_1.Parsed, _EQ_1.Parsed,  _booleanValue_1.Parsed), _booleanValue_1.Remainder);
            }
        }
    }
    
}
