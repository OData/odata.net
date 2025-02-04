namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _nameⲻseparatorParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._nameⲻseparator> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._nameⲻseparator>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._nameⲻseparator> Parse(IInput<char>? input)
            {
                var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(input);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._nameⲻseparator)!, input);
}

var _COLON_1 = __GeneratedOdataV3.Parsers.Rules._COLONParser.Instance.Parse(_BWS_1.Remainder);
if (!_COLON_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._nameⲻseparator)!, input);
}

var _BWS_2 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_COLON_1.Remainder);
if (!_BWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._nameⲻseparator)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._nameⲻseparator(_BWS_1.Parsed, _COLON_1.Parsed, _BWS_2.Parsed), _BWS_2.Remainder);
            }
        }
    }
    
}
