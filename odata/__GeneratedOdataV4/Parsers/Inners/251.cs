namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _qualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty> Instance { get; } = (_qualifiedActionNameParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty>(_qualifiedFunctionNameParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty>(_selectListPropertyParser.Instance);
        
        public static class _qualifiedActionNameParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedActionName> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedActionName>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedActionName> Parse(IInput<char>? input)
                {
                    var _qualifiedActionName_1 = __GeneratedOdataV4.Parsers.Rules._qualifiedActionNameParser.Instance.Parse(input);
if (!_qualifiedActionName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedActionName)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedActionName(_qualifiedActionName_1.Parsed), _qualifiedActionName_1.Remainder);
                }
            }
        }
        
        public static class _qualifiedFunctionNameParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedFunctionName> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedFunctionName>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedFunctionName> Parse(IInput<char>? input)
                {
                    var _qualifiedFunctionName_1 = __GeneratedOdataV4.Parsers.Rules._qualifiedFunctionNameParser.Instance.Parse(input);
if (!_qualifiedFunctionName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedFunctionName)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._qualifiedFunctionName(_qualifiedFunctionName_1.Parsed), _qualifiedFunctionName_1.Remainder);
                }
            }
        }
        
        public static class _selectListPropertyParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._selectListProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._selectListProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._selectListProperty> Parse(IInput<char>? input)
                {
                    var _selectListProperty_1 = __GeneratedOdataV4.Parsers.Rules._selectListPropertyParser.Instance.Parse(input);
if (!_selectListProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._selectListProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty._selectListProperty(_selectListProperty_1.Parsed), _selectListProperty_1.Remainder);
                }
            }
        }
    }
    
}
