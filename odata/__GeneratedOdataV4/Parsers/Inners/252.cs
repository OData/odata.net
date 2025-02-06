namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ> Parse(IInput<char>? input)
            {
                var _qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty_1 = __GeneratedOdataV4.Parsers.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyParser.Instance.Parse(input);
if (!_qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ(_qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty_1.Parsed), _qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty_1.Remainder);
            }
        }
    }
    
}
