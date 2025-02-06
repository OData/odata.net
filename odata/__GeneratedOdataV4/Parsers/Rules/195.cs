namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _arrayOrObjectParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._arrayOrObject> Instance { get; } = (_complexColInUriParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._arrayOrObject>(_complexInUriParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._arrayOrObject>(_rootExprColParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._arrayOrObject>(_primitiveColInUriParser.Instance);
        
        public static class _complexColInUriParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._arrayOrObject._complexColInUri> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._arrayOrObject._complexColInUri>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._arrayOrObject._complexColInUri> Parse(IInput<char>? input)
                {
                    var _complexColInUri_1 = __GeneratedOdataV4.Parsers.Rules._complexColInUriParser.Instance.Parse(input);
if (!_complexColInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._arrayOrObject._complexColInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._arrayOrObject._complexColInUri(_complexColInUri_1.Parsed), _complexColInUri_1.Remainder);
                }
            }
        }
        
        public static class _complexInUriParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._arrayOrObject._complexInUri> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._arrayOrObject._complexInUri>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._arrayOrObject._complexInUri> Parse(IInput<char>? input)
                {
                    var _complexInUri_1 = __GeneratedOdataV4.Parsers.Rules._complexInUriParser.Instance.Parse(input);
if (!_complexInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._arrayOrObject._complexInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._arrayOrObject._complexInUri(_complexInUri_1.Parsed), _complexInUri_1.Remainder);
                }
            }
        }
        
        public static class _rootExprColParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._arrayOrObject._rootExprCol> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._arrayOrObject._rootExprCol>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._arrayOrObject._rootExprCol> Parse(IInput<char>? input)
                {
                    var _rootExprCol_1 = __GeneratedOdataV4.Parsers.Rules._rootExprColParser.Instance.Parse(input);
if (!_rootExprCol_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._arrayOrObject._rootExprCol)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._arrayOrObject._rootExprCol(_rootExprCol_1.Parsed), _rootExprCol_1.Remainder);
                }
            }
        }
        
        public static class _primitiveColInUriParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._arrayOrObject._primitiveColInUri> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._arrayOrObject._primitiveColInUri>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._arrayOrObject._primitiveColInUri> Parse(IInput<char>? input)
                {
                    var _primitiveColInUri_1 = __GeneratedOdataV4.Parsers.Rules._primitiveColInUriParser.Instance.Parse(input);
if (!_primitiveColInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._arrayOrObject._primitiveColInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._arrayOrObject._primitiveColInUri(_primitiveColInUri_1.Parsed), _primitiveColInUri_1.Remainder);
                }
            }
        }
    }
    
}
