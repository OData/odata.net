namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃ> Parse(IInput<char>? input)
            {
                var _ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded_1 = __GeneratedOdataV3.Parsers.Inners._ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedParser.Instance.Parse(input);
if (!_ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencodedↃ(_ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded_1.Parsed), _ALPHAⳆDIGITⳆCOMMAⳆʺx2EʺⳆpctⲻencoded_1.Remainder);
            }
        }
    }
    
}
