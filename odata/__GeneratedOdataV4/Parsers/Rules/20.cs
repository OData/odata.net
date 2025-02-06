namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexPathParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._complexPath> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._complexPath>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._complexPath> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_qualifiedComplexTypeName_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameParser.Instance.Optional().Parse(input);
if (!_ʺx2Fʺ_qualifiedComplexTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._complexPath)!, input);
}

var _ʺx2Fʺ_propertyPathⳆboundOperation_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2Fʺ_propertyPathⳆboundOperationParser.Instance.Optional().Parse(_ʺx2Fʺ_qualifiedComplexTypeName_1.Remainder);
if (!_ʺx2Fʺ_propertyPathⳆboundOperation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._complexPath)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._complexPath(_ʺx2Fʺ_qualifiedComplexTypeName_1.Parsed.GetOrElse(null), _ʺx2Fʺ_propertyPathⳆboundOperation_1.Parsed.GetOrElse(null)), _ʺx2Fʺ_propertyPathⳆboundOperation_1.Remainder);
            }
        }
    }
    
}
