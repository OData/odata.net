namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_propertyPathⳆboundOperationParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation> Instance { get; } = (_ʺx2Fʺ_propertyPathParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation>(_boundOperationParser.Instance);
        
        public static class _ʺx2Fʺ_propertyPathParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._ʺx2Fʺ_propertyPath> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._ʺx2Fʺ_propertyPath>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._ʺx2Fʺ_propertyPath> Parse(IInput<char>? input)
                {
                    var _ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._ʺx2Fʺ_propertyPath)!, input);
}

var _propertyPath_1 = __GeneratedOdataV3.Parsers.Rules._propertyPathParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_propertyPath_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._ʺx2Fʺ_propertyPath)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._ʺx2Fʺ_propertyPath(_ʺx2Fʺ_1.Parsed,  _propertyPath_1.Parsed), _propertyPath_1.Remainder);
                }
            }
        }
        
        public static class _boundOperationParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._boundOperation> Instance { get; } = from _boundOperation_1 in __GeneratedOdataV3.Parsers.Rules._boundOperationParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperation._boundOperation(_boundOperation_1);
        }
    }
    
}
