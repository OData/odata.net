namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPathParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_qualifiedComplexTypeName_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameParser.Instance.Optional().Parse(input);
if (!_ʺx2Fʺ_qualifiedComplexTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath)!, input);
}

var _ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance.Parse(_ʺx2Fʺ_qualifiedComplexTypeName_1.Remainder);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath)!, input);
}

var _contextPropertyPath_1 = __GeneratedOdataV3.Parsers.Rules._contextPropertyPathParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_contextPropertyPath_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath(_ʺx2Fʺ_qualifiedComplexTypeName_1.Parsed.GetOrElse(null), _ʺx2Fʺ_1.Parsed,  _contextPropertyPath_1.Parsed), _contextPropertyPath_1.Remainder);
            }
        }
    }
    
}
