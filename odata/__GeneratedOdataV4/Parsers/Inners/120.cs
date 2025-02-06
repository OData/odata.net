namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _complexPropertyⳆcomplexColPropertyParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._complexPropertyⳆcomplexColProperty> Instance { get; } = (_complexPropertyParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._complexPropertyⳆcomplexColProperty>(_complexColPropertyParser.Instance);
        
        public static class _complexPropertyParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexProperty> Parse(IInput<char>? input)
                {
                    var _complexProperty_1 = __GeneratedOdataV4.Parsers.Rules._complexPropertyParser.Instance.Parse(input);
if (!_complexProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexProperty(_complexProperty_1.Parsed), _complexProperty_1.Remainder);
                }
            }
        }
        
        public static class _complexColPropertyParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexColProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexColProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexColProperty> Parse(IInput<char>? input)
                {
                    var _complexColProperty_1 = __GeneratedOdataV4.Parsers.Rules._complexColPropertyParser.Instance.Parse(input);
if (!_complexColProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexColProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexColProperty(_complexColProperty_1.Parsed), _complexColProperty_1.Remainder);
                }
            }
        }
    }
    
}
