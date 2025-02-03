namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _namespace_ʺx2EʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._namespace_ʺx2Eʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._namespace_ʺx2Eʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._namespace_ʺx2Eʺ> Parse(IInput<char>? input)
            {
                var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._namespace_ʺx2Eʺ)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._namespace_ʺx2Eʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._namespace_ʺx2Eʺ(_namespace_1.Parsed,  _ʺx2Eʺ_1.Parsed), _ʺx2Eʺ_1.Remainder);
            }
        }
    }
    
}
