namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName> Instance { get; } = (_selectPropertyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName>(_qualifiedActionNameParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName>(_qualifiedFunctionNameParser.Instance);
        
        public static class _selectPropertyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._selectProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._selectProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._selectProperty> Parse(IInput<char>? input)
                {
                    var _selectProperty_1 = __GeneratedOdataV3.Parsers.Rules._selectPropertyParser.Instance.Parse(input);
if (!_selectProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._selectProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._selectProperty(_selectProperty_1.Parsed), _selectProperty_1.Remainder);
                }
            }
        }
        
        public static class _qualifiedActionNameParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedActionName> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedActionName>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedActionName> Parse(IInput<char>? input)
                {
                    var _qualifiedActionName_1 = __GeneratedOdataV3.Parsers.Rules._qualifiedActionNameParser.Instance.Parse(input);
if (!_qualifiedActionName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedActionName)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedActionName(_qualifiedActionName_1.Parsed), _qualifiedActionName_1.Remainder);
                }
            }
        }
        
        public static class _qualifiedFunctionNameParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedFunctionName> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedFunctionName>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedFunctionName> Parse(IInput<char>? input)
                {
                    var _qualifiedFunctionName_1 = __GeneratedOdataV3.Parsers.Rules._qualifiedFunctionNameParser.Instance.Parse(input);
if (!_qualifiedFunctionName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedFunctionName)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName._qualifiedFunctionName(_qualifiedFunctionName_1.Parsed), _qualifiedFunctionName_1.Remainder);
                }
            }
        }
    }
    
}
