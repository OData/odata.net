namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _customNameParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._customName> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._customName>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._customName> Parse(IInput<char>? input)
            {
                var _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1 = __GeneratedOdataV4.Parsers.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLARParser.Instance.Parse(input);
if (!_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._customName)!, input);
}

var _ⲤqcharⲻnoⲻAMPⲻEQↃ_1 = Inners._ⲤqcharⲻnoⲻAMPⲻEQↃParser.Instance.Many().Parse(_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1.Remainder);
if (!_ⲤqcharⲻnoⲻAMPⲻEQↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._customName)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._customName(_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1.Parsed, _ⲤqcharⲻnoⲻAMPⲻEQↃ_1.Parsed), _ⲤqcharⲻnoⲻAMPⲻEQↃ_1.Remainder);
            }
        }
    }
    
}
