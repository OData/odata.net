namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _dayParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._day> Instance { get; } = (_ʺx30ʺ_oneToNineParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._day>(_Ⲥʺx31ʺⳆʺx32ʺↃ_DIGITParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._day>(_ʺx33ʺ_Ⲥʺx30ʺⳆʺx31ʺↃParser.Instance);
        
        public static class _ʺx30ʺ_oneToNineParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._day._ʺx30ʺ_oneToNine> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._day._ʺx30ʺ_oneToNine>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._day._ʺx30ʺ_oneToNine> Parse(IInput<char>? input)
                {
                    var _ʺx30ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx30ʺParser.Instance.Parse(input);
if (!_ʺx30ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._day._ʺx30ʺ_oneToNine)!, input);
}

var _oneToNine_1 = __GeneratedOdataV3.Parsers.Rules._oneToNineParser.Instance.Parse(_ʺx30ʺ_1.Remainder);
if (!_oneToNine_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._day._ʺx30ʺ_oneToNine)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._day._ʺx30ʺ_oneToNine(_ʺx30ʺ_1.Parsed,  _oneToNine_1.Parsed), _oneToNine_1.Remainder);
                }
            }
        }
        
        public static class _Ⲥʺx31ʺⳆʺx32ʺↃ_DIGITParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._day._Ⲥʺx31ʺⳆʺx32ʺↃ_DIGIT> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._day._Ⲥʺx31ʺⳆʺx32ʺↃ_DIGIT>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._day._Ⲥʺx31ʺⳆʺx32ʺↃ_DIGIT> Parse(IInput<char>? input)
                {
                    var _Ⲥʺx31ʺⳆʺx32ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx31ʺⳆʺx32ʺↃParser.Instance.Parse(input);
if (!_Ⲥʺx31ʺⳆʺx32ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._day._Ⲥʺx31ʺⳆʺx32ʺↃ_DIGIT)!, input);
}

var _DIGIT_1 = __GeneratedOdataV3.Parsers.Rules._DIGITParser.Instance.Parse(_Ⲥʺx31ʺⳆʺx32ʺↃ_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._day._Ⲥʺx31ʺⳆʺx32ʺↃ_DIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._day._Ⲥʺx31ʺⳆʺx32ʺↃ_DIGIT(_Ⲥʺx31ʺⳆʺx32ʺↃ_1.Parsed,  _DIGIT_1.Parsed), _DIGIT_1.Remainder);
                }
            }
        }
        
        public static class _ʺx33ʺ_Ⲥʺx30ʺⳆʺx31ʺↃParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._day._ʺx33ʺ_Ⲥʺx30ʺⳆʺx31ʺↃ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._day._ʺx33ʺ_Ⲥʺx30ʺⳆʺx31ʺↃ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._day._ʺx33ʺ_Ⲥʺx30ʺⳆʺx31ʺↃ> Parse(IInput<char>? input)
                {
                    var _ʺx33ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx33ʺParser.Instance.Parse(input);
if (!_ʺx33ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._day._ʺx33ʺ_Ⲥʺx30ʺⳆʺx31ʺↃ)!, input);
}

var _Ⲥʺx30ʺⳆʺx31ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx30ʺⳆʺx31ʺↃParser.Instance.Parse(_ʺx33ʺ_1.Remainder);
if (!_Ⲥʺx30ʺⳆʺx31ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._day._ʺx33ʺ_Ⲥʺx30ʺⳆʺx31ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._day._ʺx33ʺ_Ⲥʺx30ʺⳆʺx31ʺↃ(_ʺx33ʺ_1.Parsed,  _Ⲥʺx30ʺⳆʺx31ʺↃ_1.Parsed), _Ⲥʺx30ʺⳆʺx31ʺↃ_1.Remainder);
                }
            }
        }
    }
    
}
