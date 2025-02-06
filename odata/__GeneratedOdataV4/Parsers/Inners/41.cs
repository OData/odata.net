namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _primitiveKeyPropertyⳆkeyPropertyAliasParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias> Instance { get; } = (_primitiveKeyPropertyParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias>(_keyPropertyAliasParser.Instance);
        
        public static class _primitiveKeyPropertyParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias._primitiveKeyProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias._primitiveKeyProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias._primitiveKeyProperty> Parse(IInput<char>? input)
                {
                    var _primitiveKeyProperty_1 = __GeneratedOdataV4.Parsers.Rules._primitiveKeyPropertyParser.Instance.Parse(input);
if (!_primitiveKeyProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias._primitiveKeyProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias._primitiveKeyProperty(_primitiveKeyProperty_1.Parsed), _primitiveKeyProperty_1.Remainder);
                }
            }
        }
        
        public static class _keyPropertyAliasParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias._keyPropertyAlias> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias._keyPropertyAlias>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias._keyPropertyAlias> Parse(IInput<char>? input)
                {
                    var _keyPropertyAlias_1 = __GeneratedOdataV4.Parsers.Rules._keyPropertyAliasParser.Instance.Parse(input);
if (!_keyPropertyAlias_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias._keyPropertyAlias)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._primitiveKeyPropertyⳆkeyPropertyAlias._keyPropertyAlias(_keyPropertyAlias_1.Parsed), _keyPropertyAlias_1.Remainder);
                }
            }
        }
    }
    
}
