namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _singleQualifiedTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName> Instance { get; } = (_qualifiedEntityTypeNameParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName>(_qualifiedComplexTypeNameParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName>(_qualifiedTypeDefinitionNameParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName>(_qualifiedEnumTypeNameParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName>(_primitiveTypeNameParser.Instance);
        
        public static class _qualifiedEntityTypeNameParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedEntityTypeName> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedEntityTypeName>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedEntityTypeName> Parse(IInput<char>? input)
                {
                    var _qualifiedEntityTypeName_1 = __GeneratedOdataV3.Parsers.Rules._qualifiedEntityTypeNameParser.Instance.Parse(input);
if (!_qualifiedEntityTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedEntityTypeName)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedEntityTypeName(_qualifiedEntityTypeName_1.Parsed), _qualifiedEntityTypeName_1.Remainder);
                }
            }
        }
        
        public static class _qualifiedComplexTypeNameParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedComplexTypeName> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedComplexTypeName>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedComplexTypeName> Parse(IInput<char>? input)
                {
                    var _qualifiedComplexTypeName_1 = __GeneratedOdataV3.Parsers.Rules._qualifiedComplexTypeNameParser.Instance.Parse(input);
if (!_qualifiedComplexTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedComplexTypeName)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedComplexTypeName(_qualifiedComplexTypeName_1.Parsed), _qualifiedComplexTypeName_1.Remainder);
                }
            }
        }
        
        public static class _qualifiedTypeDefinitionNameParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedTypeDefinitionName> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedTypeDefinitionName>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedTypeDefinitionName> Parse(IInput<char>? input)
                {
                    var _qualifiedTypeDefinitionName_1 = __GeneratedOdataV3.Parsers.Rules._qualifiedTypeDefinitionNameParser.Instance.Parse(input);
if (!_qualifiedTypeDefinitionName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedTypeDefinitionName)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedTypeDefinitionName(_qualifiedTypeDefinitionName_1.Parsed), _qualifiedTypeDefinitionName_1.Remainder);
                }
            }
        }
        
        public static class _qualifiedEnumTypeNameParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedEnumTypeName> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedEnumTypeName>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedEnumTypeName> Parse(IInput<char>? input)
                {
                    var _qualifiedEnumTypeName_1 = __GeneratedOdataV3.Parsers.Rules._qualifiedEnumTypeNameParser.Instance.Parse(input);
if (!_qualifiedEnumTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedEnumTypeName)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._qualifiedEnumTypeName(_qualifiedEnumTypeName_1.Parsed), _qualifiedEnumTypeName_1.Remainder);
                }
            }
        }
        
        public static class _primitiveTypeNameParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._primitiveTypeName> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._primitiveTypeName>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._primitiveTypeName> Parse(IInput<char>? input)
                {
                    var _primitiveTypeName_1 = __GeneratedOdataV3.Parsers.Rules._primitiveTypeNameParser.Instance.Parse(input);
if (!_primitiveTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._primitiveTypeName)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._singleQualifiedTypeName._primitiveTypeName(_primitiveTypeName_1.Parsed), _primitiveTypeName_1.Remainder);
                }
            }
        }
    }
    
}
