namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _parameterValueParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._parameterValue> Instance { get; } = (_arrayOrObjectParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._parameterValue>(_commonExprParser.Instance);
        
        public static class _arrayOrObjectParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._parameterValue._arrayOrObject> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._parameterValue._arrayOrObject>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._parameterValue._arrayOrObject> Parse(IInput<char>? input)
                {
                    var _arrayOrObject_1 = __GeneratedOdataV4.Parsers.Rules._arrayOrObjectParser.Instance.Parse(input);
if (!_arrayOrObject_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._parameterValue._arrayOrObject)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._parameterValue._arrayOrObject(_arrayOrObject_1.Parsed), _arrayOrObject_1.Remainder);
                }
            }
        }
        
        public static class _commonExprParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._parameterValue._commonExpr> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._parameterValue._commonExpr>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._parameterValue._commonExpr> Parse(IInput<char>? input)
                {
                    var _commonExpr_1 = __GeneratedOdataV4.Parsers.Rules._commonExprParser.Instance.Parse(input);
if (!_commonExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._parameterValue._commonExpr)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._parameterValue._commonExpr(_commonExpr_1.Parsed), _commonExpr_1.Remainder);
                }
            }
        }
    }
    
}
