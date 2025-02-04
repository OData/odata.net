namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤVCHARⳆobsⲻtextↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤVCHARⳆobsⲻtextↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤVCHARⳆobsⲻtextↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤVCHARⳆobsⲻtextↃ> Parse(IInput<char>? input)
            {
                var _VCHARⳆobsⲻtext_1 = __GeneratedOdataV3.Parsers.Inners._VCHARⳆobsⲻtextParser.Instance.Parse(input);
if (!_VCHARⳆobsⲻtext_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤVCHARⳆobsⲻtextↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤVCHARⳆobsⲻtextↃ(_VCHARⳆobsⲻtext_1.Parsed), _VCHARⳆobsⲻtext_1.Remainder);
            }
        }
    }
    
}
