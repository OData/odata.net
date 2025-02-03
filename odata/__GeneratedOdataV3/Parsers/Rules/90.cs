namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectPathParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectPath> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectPath>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._selectPath> Parse(IInput<char>? input)
            {
                var _ⲤcomplexPropertyⳆcomplexColPropertyↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃParser.Instance.Parse(input);
if (!_ⲤcomplexPropertyⳆcomplexColPropertyↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectPath)!, input);
}

var _ʺx2Fʺ_qualifiedComplexTypeName_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameParser.Instance.Optional().Parse(_ⲤcomplexPropertyⳆcomplexColPropertyↃ_1.Remainder);
if (!_ʺx2Fʺ_qualifiedComplexTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._selectPath)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._selectPath(_ⲤcomplexPropertyⳆcomplexColPropertyↃ_1.Parsed,  _ʺx2Fʺ_qualifiedComplexTypeName_1.Parsed.GetOrElse(null)), _ʺx2Fʺ_qualifiedComplexTypeName_1.Remainder);
            }
        }
    }
    
}
