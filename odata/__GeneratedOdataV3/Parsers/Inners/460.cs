namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _4base64charParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._4base64char> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._4base64char>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._4base64char> Parse(IInput<char>? input)
            {
                var _base64char_1 = __GeneratedOdataV3.Parsers.Rules._base64charParser.Instance.Repeat(4, 4).Parse(input);
if (!_base64char_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._4base64char)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._4base64char(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedExactly4<__GeneratedOdataV3.CstNodes.Rules._base64char>(_base64char_1.Parsed)), _base64char_1.Remainder);
            }
        }
    }
    
}
