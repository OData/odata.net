namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pointLiteralParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pointLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pointLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pointLiteral> Parse(IInput<char>? input)
            {
                var _ʺx50x6Fx69x6Ex74ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx50x6Fx69x6Ex74ʺParser.Instance.Parse(input);
if (!_ʺx50x6Fx69x6Ex74ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pointLiteral)!, input);
}

var _pointData_1 = __GeneratedOdataV3.Parsers.Rules._pointDataParser.Instance.Parse(_ʺx50x6Fx69x6Ex74ʺ_1.Remainder);
if (!_pointData_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pointLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pointLiteral(_ʺx50x6Fx69x6Ex74ʺ_1.Parsed, _pointData_1.Parsed), _pointData_1.Remainder);
            }
        }
    }
    
}
