namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _zeroToFiftyNineParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._zeroToFiftyNine> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._zeroToFiftyNine>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._zeroToFiftyNine> Parse(IInput<char>? input)
            {
                var _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃ_1 = __GeneratedOdataV4.Parsers.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃParser.Instance.Parse(input);
if (!_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._zeroToFiftyNine)!, input);
}

var _DIGIT_1 = __GeneratedOdataV4.Parsers.Rules._DIGITParser.Instance.Parse(_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃ_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._zeroToFiftyNine)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._zeroToFiftyNine(_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺↃ_1.Parsed, _DIGIT_1.Parsed), _DIGIT_1.Remainder);
            }
        }
    }
    
}
