namespace __GeneratedOdata.Parsers.Inners
{
    using __GeneratedOdata.CstNodes.Inners;
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_qualifiedEntityTypeNameParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _qualifiedEntityTypeName_1 in __GeneratedOdata.Parsers.Rules._qualifiedEntityTypeNameParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName(_ʺx2Fʺ_1, _qualifiedEntityTypeName_1);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName>
        {
            public IOutput<char, _ʺx2Fʺ_qualifiedEntityTypeName> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_1 = __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
                if (!_ʺx2Fʺ_1.Success)
                {
                    return Output.Create(
                        false,
                        default(_ʺx2Fʺ_qualifiedEntityTypeName)!,
                        input);
                }
                
                var _qualifiedEntityTypeName_1 = __GeneratedOdata.Parsers.Rules._qualifiedEntityTypeNameParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
                if (!_qualifiedEntityTypeName_1.Success)
                {
                    return Output.Create(
                        false,
                        default(_ʺx2Fʺ_qualifiedEntityTypeName)!,
                        input);
                }

                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_qualifiedEntityTypeName(_ʺx2Fʺ_1.Parsed, _qualifiedEntityTypeName_1.Parsed),
                    _qualifiedEntityTypeName_1.Remainder);
            }
        }
    }
    
}
