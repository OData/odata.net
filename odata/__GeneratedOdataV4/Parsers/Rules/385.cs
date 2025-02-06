namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _schemeParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._scheme> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._scheme>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._scheme> Parse(IInput<char>? input)
            {
                var _ALPHA_1 = __GeneratedOdataV4.Parsers.Rules._ALPHAParser.Instance.Parse(input);
if (!_ALPHA_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._scheme)!, input);
}

var _ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ_1 = Inners._ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃParser.Instance.Many().Parse(_ALPHA_1.Remainder);
if (!_ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._scheme)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._scheme(_ALPHA_1.Parsed, _ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ_1.Parsed), _ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ_1.Remainder);
            }
        }
    }
    
}
