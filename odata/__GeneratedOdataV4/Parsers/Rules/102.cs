namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _customQueryOptionParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._customQueryOption> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._customQueryOption>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._customQueryOption> Parse(IInput<char>? input)
            {
                var _customName_1 = __GeneratedOdataV4.Parsers.Rules._customNameParser.Instance.Parse(input);
if (!_customName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._customQueryOption)!, input);
}

var _EQ_customValue_1 = __GeneratedOdataV4.Parsers.Inners._EQ_customValueParser.Instance.Optional().Parse(_customName_1.Remainder);
if (!_EQ_customValue_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._customQueryOption)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._customQueryOption(_customName_1.Parsed, _EQ_customValue_1.Parsed.GetOrElse(null)), _EQ_customValue_1.Remainder);
            }
        }
    }
    
}
