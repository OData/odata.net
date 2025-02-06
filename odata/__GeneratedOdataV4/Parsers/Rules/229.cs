namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _namespaceParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._namespace> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._namespace>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._namespace> Parse(IInput<char>? input)
            {
                var _namespacePart_1 = __GeneratedOdataV4.Parsers.Rules._namespacePartParser.Instance.Parse(input);
if (!_namespacePart_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._namespace)!, input);
}

var _Ⲥʺx2Eʺ_namespacePartↃ_1 = Inners._Ⲥʺx2Eʺ_namespacePartↃParser.Instance.Many().Parse(_namespacePart_1.Remainder);
if (!_Ⲥʺx2Eʺ_namespacePartↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._namespace)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._namespace(_namespacePart_1.Parsed, _Ⲥʺx2Eʺ_namespacePartↃ_1.Parsed), _Ⲥʺx2Eʺ_namespacePartↃ_1.Remainder);
            }
        }
    }
    
}
