namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _hourParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._hour> Instance { get; } = (_Ⲥʺx30ʺⳆʺx31ʺↃ_DIGITParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._hour>(_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃParser.Instance);
        
        public static class _Ⲥʺx30ʺⳆʺx31ʺↃ_DIGITParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._hour._Ⲥʺx30ʺⳆʺx31ʺↃ_DIGIT> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._hour._Ⲥʺx30ʺⳆʺx31ʺↃ_DIGIT>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._hour._Ⲥʺx30ʺⳆʺx31ʺↃ_DIGIT> Parse(IInput<char>? input)
                {
                    var _Ⲥʺx30ʺⳆʺx31ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx30ʺⳆʺx31ʺↃParser.Instance.Parse(input);
if (!_Ⲥʺx30ʺⳆʺx31ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._hour._Ⲥʺx30ʺⳆʺx31ʺↃ_DIGIT)!, input);
}

var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Parse(_Ⲥʺx30ʺⳆʺx31ʺↃ_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._hour._Ⲥʺx30ʺⳆʺx31ʺↃ_DIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._hour._Ⲥʺx30ʺⳆʺx31ʺↃ_DIGIT(_Ⲥʺx30ʺⳆʺx31ʺↃ_1.Parsed, _DIGIT_1.Parsed), _DIGIT_1.Remainder);
                }
            }
        }
        
        public static class _ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._hour._ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._hour._ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._hour._ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ> Parse(IInput<char>? input)
                {
                    var _ʺx32ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx32ʺParser.Instance.Parse(input);
if (!_ʺx32ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._hour._ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ)!, input);
}

var _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃParser.Instance.Parse(_ʺx32ʺ_1.Remainder);
if (!_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._hour._ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._hour._ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ(_ʺx32ʺ_1.Parsed, _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ_1.Parsed), _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺↃ_1.Remainder);
                }
            }
        }
    }
    
}
