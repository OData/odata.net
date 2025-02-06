namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchWordParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._searchWord> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._searchWord>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._searchWord> Parse(IInput<char>? input)
            {
                var _ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃ_1 = __GeneratedOdataV4.Parsers.Inners._ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃParser.Instance.Repeat(1, null).Parse(input);
if (!_ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._searchWord)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._searchWord(new __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV4.CstNodes.Inners._ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃ>(_ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃ_1.Parsed)), _ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃ_1.Remainder);
            }
        }
    }
    
}
