namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤprimitiveKeyPropertyⳆkeyPropertyAliasↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤprimitiveKeyPropertyⳆkeyPropertyAliasↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤprimitiveKeyPropertyⳆkeyPropertyAliasↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤprimitiveKeyPropertyⳆkeyPropertyAliasↃ> Parse(IInput<char>? input)
            {
                var _primitiveKeyPropertyⳆkeyPropertyAlias_1 = __GeneratedOdataV4.Parsers.Inners._primitiveKeyPropertyⳆkeyPropertyAliasParser.Instance.Parse(input);
if (!_primitiveKeyPropertyⳆkeyPropertyAlias_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤprimitiveKeyPropertyⳆkeyPropertyAliasↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤprimitiveKeyPropertyⳆkeyPropertyAliasↃ(_primitiveKeyPropertyⳆkeyPropertyAlias_1.Parsed), _primitiveKeyPropertyⳆkeyPropertyAlias_1.Remainder);
            }
        }
    }
    
}
