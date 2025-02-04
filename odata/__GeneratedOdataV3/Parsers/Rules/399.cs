namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pathⲻabsoluteParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pathⲻabsolute> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pathⲻabsolute>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pathⲻabsolute> Parse(IInput<char>? input)
            {
                var _ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pathⲻabsolute)!, input);
}

var _segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ_1 = __GeneratedOdataV3.Parsers.Inners._segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃParser.Instance.Optional().Parse(_ʺx2Fʺ_1.Remainder);
if (!_segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pathⲻabsolute)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pathⲻabsolute(_ʺx2Fʺ_1.Parsed, _segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ_1.Parsed.GetOrElse(null)), _segmentⲻnz_ЖⲤʺx2Fʺ_segmentↃ_1.Remainder);
            }
        }
    }
    
}
