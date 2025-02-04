namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx2Eʺ_namespacePartↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx2Eʺ_namespacePartↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx2Eʺ_namespacePartↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx2Eʺ_namespacePartↃ> Parse(IInput<char>? input)
            {
                var _ʺx2Eʺ_namespacePart_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Eʺ_namespacePartParser.Instance.Parse(input);
if (!_ʺx2Eʺ_namespacePart_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx2Eʺ_namespacePartↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx2Eʺ_namespacePartↃ(_ʺx2Eʺ_namespacePart_1.Parsed), _ʺx2Eʺ_namespacePart_1.Remainder);
            }
        }
    }
    
}
