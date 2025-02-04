namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤqcharⲻnoⲻAMPↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤqcharⲻnoⲻAMPↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤqcharⲻnoⲻAMPↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤqcharⲻnoⲻAMPↃ> Parse(IInput<char>? input)
            {
                var _qcharⲻnoⲻAMP_1 = __GeneratedOdataV3.Parsers.Rules._qcharⲻnoⲻAMPParser.Instance.Parse(input);
if (!_qcharⲻnoⲻAMP_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤqcharⲻnoⲻAMPↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤqcharⲻnoⲻAMPↃ(_qcharⲻnoⲻAMP_1.Parsed), _qcharⲻnoⲻAMP_1.Remainder);
            }
        }
    }
    
}
