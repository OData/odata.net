namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _qualifiedEntityTypeNameⳆqualifiedComplexTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName> Instance { get; } = (_qualifiedEntityTypeNameParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName>(_qualifiedComplexTypeNameParser.Instance);
        
        public static class _qualifiedEntityTypeNameParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedEntityTypeName> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedEntityTypeName>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedEntityTypeName> Parse(IInput<char>? input)
                {
                    var _qualifiedEntityTypeName_1 = __GeneratedOdataV4.Parsers.Rules._qualifiedEntityTypeNameParser.Instance.Parse(input);
if (!_qualifiedEntityTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedEntityTypeName)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedEntityTypeName(_qualifiedEntityTypeName_1.Parsed), _qualifiedEntityTypeName_1.Remainder);
                }
            }
        }
        
        public static class _qualifiedComplexTypeNameParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedComplexTypeName> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedComplexTypeName>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedComplexTypeName> Parse(IInput<char>? input)
                {
                    var _qualifiedComplexTypeName_1 = __GeneratedOdataV4.Parsers.Rules._qualifiedComplexTypeNameParser.Instance.Parse(input);
if (!_qualifiedComplexTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedComplexTypeName)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._qualifiedEntityTypeNameⳆqualifiedComplexTypeName._qualifiedComplexTypeName(_qualifiedComplexTypeName_1.Parsed), _qualifiedComplexTypeName_1.Remainder);
                }
            }
        }
    }
    
}
