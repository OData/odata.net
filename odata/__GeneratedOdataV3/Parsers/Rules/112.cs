namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectListPropertyParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectListProperty> Instance { get; } = (_primitivePropertyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectListProperty>(_primitiveColPropertyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectListProperty>(_navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectListProperty>(_selectPath_꘡ʺx2Fʺ_selectListProperty꘡Parser.Instance);
        
        public static class _primitivePropertyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectListProperty._primitiveProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectListProperty._primitiveProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectListProperty._primitiveProperty> Parse(IInput<char>? input)
                {
                    var _primitiveProperty_1 = __GeneratedOdataV3.Parsers.Rules._primitivePropertyParser.Instance.Parse(input);
if (!_primitiveProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectListProperty._primitiveProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectListProperty._primitiveProperty(_primitiveProperty_1.Parsed), _primitiveProperty_1.Remainder);
                }
            }
        }
        
        public static class _primitiveColPropertyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectListProperty._primitiveColProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectListProperty._primitiveColProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectListProperty._primitiveColProperty> Parse(IInput<char>? input)
                {
                    var _primitiveColProperty_1 = __GeneratedOdataV3.Parsers.Rules._primitiveColPropertyParser.Instance.Parse(input);
if (!_primitiveColProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectListProperty._primitiveColProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectListProperty._primitiveColProperty(_primitiveColProperty_1.Parsed), _primitiveColProperty_1.Remainder);
                }
            }
        }
        
        public static class _navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectListProperty._navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectListProperty._navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectListProperty._navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡> Parse(IInput<char>? input)
                {
                    var _navigationProperty_1 = __GeneratedOdataV3.Parsers.Rules._navigationPropertyParser.Instance.Parse(input);
if (!_navigationProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectListProperty._navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡)!, input);
}

var _ʺx2Bʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2BʺParser.Instance.Optional().Parse(_navigationProperty_1.Remainder);
if (!_ʺx2Bʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectListProperty._navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡)!, input);
}

var _selectList_1 = __GeneratedOdataV3.Parsers.Rules._selectListParser.Instance.Optional().Parse(_ʺx2Bʺ_1.Remainder);
if (!_selectList_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectListProperty._navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectListProperty._navigationProperty_꘡ʺx2Bʺ꘡_꘡selectList꘡(_navigationProperty_1.Parsed, _ʺx2Bʺ_1.Parsed.GetOrElse(null), _selectList_1.Parsed.GetOrElse(null)), _selectList_1.Remainder);
                }
            }
        }
        
        public static class _selectPath_꘡ʺx2Fʺ_selectListProperty꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectListProperty._selectPath_꘡ʺx2Fʺ_selectListProperty꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectListProperty._selectPath_꘡ʺx2Fʺ_selectListProperty꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectListProperty._selectPath_꘡ʺx2Fʺ_selectListProperty꘡> Parse(IInput<char>? input)
                {
                    var _selectPath_1 = __GeneratedOdataV3.Parsers.Rules._selectPathParser.Instance.Parse(input);
if (!_selectPath_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectListProperty._selectPath_꘡ʺx2Fʺ_selectListProperty꘡)!, input);
}

var _ʺx2Fʺ_selectListProperty_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fʺ_selectListPropertyParser.Instance.Optional().Parse(_selectPath_1.Remainder);
if (!_ʺx2Fʺ_selectListProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectListProperty._selectPath_꘡ʺx2Fʺ_selectListProperty꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectListProperty._selectPath_꘡ʺx2Fʺ_selectListProperty꘡(_selectPath_1.Parsed, _ʺx2Fʺ_selectListProperty_1.Parsed.GetOrElse(null)), _ʺx2Fʺ_selectListProperty_1.Remainder);
                }
            }
        }
    }
    
}
