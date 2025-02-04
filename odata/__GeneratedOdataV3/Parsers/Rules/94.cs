namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qualifiedActionNameParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._qualifiedActionName> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._qualifiedActionName>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._qualifiedActionName> Parse(IInput<char>? input)
            {
                var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._qualifiedActionName)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._qualifiedActionName)!, input);
}

var _action_1 = __GeneratedOdataV3.Parsers.Rules._actionParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_action_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._qualifiedActionName)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._qualifiedActionName(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed, _action_1.Parsed), _action_1.Remainder);
            }
        }
    }
    
}
