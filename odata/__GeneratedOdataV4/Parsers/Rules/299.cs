namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _monthParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._month> Instance { get; } = (_ʺx30ʺ_oneToNineParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._month>(_ʺx31ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃParser.Instance);
        
        public static class _ʺx30ʺ_oneToNineParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._month._ʺx30ʺ_oneToNine> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._month._ʺx30ʺ_oneToNine>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._month._ʺx30ʺ_oneToNine> Parse(IInput<char>? input)
                {
                    var _ʺx30ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx30ʺParser.Instance.Parse(input);
if (!_ʺx30ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._month._ʺx30ʺ_oneToNine)!, input);
}

var _oneToNine_1 = __GeneratedOdataV4.Parsers.Rules._oneToNineParser.Instance.Parse(_ʺx30ʺ_1.Remainder);
if (!_oneToNine_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._month._ʺx30ʺ_oneToNine)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._month._ʺx30ʺ_oneToNine(_ʺx30ʺ_1.Parsed, _oneToNine_1.Parsed), _oneToNine_1.Remainder);
                }
            }
        }
        
        public static class _ʺx31ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._month._ʺx31ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._month._ʺx31ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._month._ʺx31ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃ> Parse(IInput<char>? input)
                {
                    var _ʺx31ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx31ʺParser.Instance.Parse(input);
if (!_ʺx31ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._month._ʺx31ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃ)!, input);
}

var _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃ_1 = __GeneratedOdataV4.Parsers.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃParser.Instance.Parse(_ʺx31ʺ_1.Remainder);
if (!_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._month._ʺx31ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._month._ʺx31ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃ(_ʺx31ʺ_1.Parsed, _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃ_1.Parsed), _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺↃ_1.Remainder);
                }
            }
        }
    }
    
}
