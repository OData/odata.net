namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤqcharⲻnoⲻAMPⲻEQↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤqcharⲻnoⲻAMPⲻEQↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤqcharⲻnoⲻAMPⲻEQↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤqcharⲻnoⲻAMPⲻEQↃ> Parse(IInput<char>? input)
            {
                var _qcharⲻnoⲻAMPⲻEQ_1 = __GeneratedOdataV3.Parsers.Rules._qcharⲻnoⲻAMPⲻEQParser.Instance.Parse(input);
if (!_qcharⲻnoⲻAMPⲻEQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤqcharⲻnoⲻAMPⲻEQↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤqcharⲻnoⲻAMPⲻEQↃ(_qcharⲻnoⲻAMPⲻEQ_1.Parsed), _qcharⲻnoⲻAMPⲻEQ_1.Remainder);
            }
        }
    }
    
}
