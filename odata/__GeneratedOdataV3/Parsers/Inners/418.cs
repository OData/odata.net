namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Eʺ_namespacePartParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ_namespacePart> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ_namespacePart>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ_namespacePart> Parse(IInput<char>? input)
            {
                var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(input);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ_namespacePart)!, input);
}

var _namespacePart_1 = __GeneratedOdataV3.Parsers.Rules._namespacePartParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_namespacePart_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ_namespacePart)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ʺx2Eʺ_namespacePart(_ʺx2Eʺ_1.Parsed,  _namespacePart_1.Parsed), _namespacePart_1.Remainder);
            }
        }
    }
    
}
