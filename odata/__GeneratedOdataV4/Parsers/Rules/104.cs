namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _customValueParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._customValue> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._customValue>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._customValue> Parse(IInput<char>? input)
            {
                var _ⲤqcharⲻnoⲻAMPↃ_1 = Inners._ⲤqcharⲻnoⲻAMPↃParser.Instance.Many().Parse(input);
if (!_ⲤqcharⲻnoⲻAMPↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._customValue)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._customValue(_ⲤqcharⲻnoⲻAMPↃ_1.Parsed), _ⲤqcharⲻnoⲻAMPↃ_1.Remainder);
            }
        }
    }
    
}
