namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ> Parse(IInput<char>? input)
            {
                var _ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺParser.Instance.Parse(input);
if (!_ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ(_ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ_1.Parsed), _ALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2Eʺ_1.Remainder);
            }
        }
    }
    
}
