namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _expandParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._expand> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._expand>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._expand> Parse(IInput<char>? input)
            {
                var _Ⲥʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺↃ_1 = __GeneratedOdataV4.Parsers.Inners._Ⲥʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺↃParser.Instance.Parse(input);
if (!_Ⲥʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._expand)!, input);
}

var _EQ_1 = __GeneratedOdataV4.Parsers.Rules._EQParser.Instance.Parse(_Ⲥʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺↃ_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._expand)!, input);
}

var _expandItem_1 = __GeneratedOdataV4.Parsers.Rules._expandItemParser.Instance.Parse(_EQ_1.Remainder);
if (!_expandItem_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._expand)!, input);
}

var _ⲤCOMMA_expandItemↃ_1 = Inners._ⲤCOMMA_expandItemↃParser.Instance.Many().Parse(_expandItem_1.Remainder);
if (!_ⲤCOMMA_expandItemↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._expand)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._expand(_Ⲥʺx24x65x78x70x61x6Ex64ʺⳆʺx65x78x70x61x6Ex64ʺↃ_1.Parsed, _EQ_1.Parsed, _expandItem_1.Parsed, _ⲤCOMMA_expandItemↃ_1.Parsed), _ⲤCOMMA_expandItemↃ_1.Remainder);
            }
        }
    }
    
}
