namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤcomplexPropertyⳆcomplexColPropertyↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ> Parse(IInput<char>? input)
            {
                var _complexPropertyⳆcomplexColProperty_1 = __GeneratedOdataV3.Parsers.Inners._complexPropertyⳆcomplexColPropertyParser.Instance.Parse(input);
if (!_complexPropertyⳆcomplexColProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ(_complexPropertyⳆcomplexColProperty_1.Parsed), _complexPropertyⳆcomplexColProperty_1.Remainder);
            }
        }
    }
    
}
