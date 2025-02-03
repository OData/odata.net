namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _stringParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._string> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._string>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._string> Parse(IInput<char>? input)
            {
                var _SQUOTE_1 = __GeneratedOdataV3.Parsers.Rules._SQUOTEParser.Instance.Parse(input);
if (!_SQUOTE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._string)!, input);
}

var _ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1 = Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃParser.Instance.Many().Parse(_SQUOTE_1.Remainder);
if (!_ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._string)!, input);
}

var _SQUOTE_2 = __GeneratedOdataV3.Parsers.Rules._SQUOTEParser.Instance.Parse(_ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1.Remainder);
if (!_SQUOTE_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._string)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._string(_SQUOTE_1.Parsed, _ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1.Parsed,  _SQUOTE_2.Parsed), _SQUOTE_2.Remainder);
            }
        }
    }
    
}
