namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥ4base64charↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥ4base64charↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥ4base64charↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥ4base64charↃ> Parse(IInput<char>? input)
            {
                var _4base64char_1 = __GeneratedOdataV3.Parsers.Inners._4base64charParser.Instance.Parse(input);
if (!_4base64char_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥ4base64charↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥ4base64charↃ(_4base64char_1.Parsed), _4base64char_1.Remainder);
            }
        }
    }
    
}
