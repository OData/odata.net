namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _IRIⲻinⲻheaderParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IRIⲻinⲻheader> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._IRIⲻinⲻheader>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._IRIⲻinⲻheader> Parse(IInput<char>? input)
            {
                var _ⲤVCHARⳆobsⲻtextↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤVCHARⳆobsⲻtextↃParser.Instance.Repeat(1, null).Parse(input);
if (!_ⲤVCHARⳆobsⲻtextↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._IRIⲻinⲻheader)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._IRIⲻinⲻheader(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Inners._ⲤVCHARⳆobsⲻtextↃ>(_ⲤVCHARⳆobsⲻtextↃ_1.Parsed)), _ⲤVCHARⳆobsⲻtextↃ_1.Remainder);
            }
        }
    }
    
}
