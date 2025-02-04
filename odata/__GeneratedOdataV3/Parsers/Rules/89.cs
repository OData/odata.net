namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectPropertyParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectProperty> Instance { get; } = (_primitivePropertyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectProperty>(_primitiveColProperty_꘡OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectProperty>(_navigationPropertyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectProperty>(_selectPath_꘡OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty꘡Parser.Instance);
        
        public static class _primitivePropertyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectProperty._primitiveProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectProperty._primitiveProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectProperty._primitiveProperty> Parse(IInput<char>? input)
                {
                    var _primitiveProperty_1 = __GeneratedOdataV3.Parsers.Rules._primitivePropertyParser.Instance.Parse(input);
if (!_primitiveProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectProperty._primitiveProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectProperty._primitiveProperty(_primitiveProperty_1.Parsed), _primitiveProperty_1.Remainder);
                }
            }
        }
        
        public static class _primitiveColProperty_꘡OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectProperty._primitiveColProperty_꘡OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectProperty._primitiveColProperty_꘡OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectProperty._primitiveColProperty_꘡OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE꘡> Parse(IInput<char>? input)
                {
                    var _primitiveColProperty_1 = __GeneratedOdataV3.Parsers.Rules._primitiveColPropertyParser.Instance.Parse(input);
if (!_primitiveColProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectProperty._primitiveColProperty_꘡OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE꘡)!, input);
}

var _OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE_1 = __GeneratedOdataV3.Parsers.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSEParser.Instance.Optional().Parse(_primitiveColProperty_1.Remainder);
if (!_OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectProperty._primitiveColProperty_꘡OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectProperty._primitiveColProperty_꘡OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE꘡(_primitiveColProperty_1.Parsed, _OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE_1.Parsed.GetOrElse(null)), _OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE_1.Remainder);
                }
            }
        }
        
        public static class _navigationPropertyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectProperty._navigationProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectProperty._navigationProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectProperty._navigationProperty> Parse(IInput<char>? input)
                {
                    var _navigationProperty_1 = __GeneratedOdataV3.Parsers.Rules._navigationPropertyParser.Instance.Parse(input);
if (!_navigationProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectProperty._navigationProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectProperty._navigationProperty(_navigationProperty_1.Parsed), _navigationProperty_1.Remainder);
                }
            }
        }
        
        public static class _selectPath_꘡OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectProperty._selectPath_꘡OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectProperty._selectPath_꘡OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectProperty._selectPath_꘡OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty꘡> Parse(IInput<char>? input)
                {
                    var _selectPath_1 = __GeneratedOdataV3.Parsers.Rules._selectPathParser.Instance.Parse(input);
if (!_selectPath_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectProperty._selectPath_꘡OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty꘡)!, input);
}

var _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty_1 = __GeneratedOdataV3.Parsers.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectPropertyParser.Instance.Optional().Parse(_selectPath_1.Remainder);
if (!_OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectProperty._selectPath_꘡OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectProperty._selectPath_꘡OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty꘡(_selectPath_1.Parsed, _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty_1.Parsed.GetOrElse(null)), _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty_1.Remainder);
                }
            }
        }
    }
    
}
