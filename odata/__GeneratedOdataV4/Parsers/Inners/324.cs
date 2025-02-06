namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤCOMMA_BWS_commonExpr_BWSↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_BWS_commonExpr_BWSↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_BWS_commonExpr_BWSↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_BWS_commonExpr_BWSↃ> Parse(IInput<char>? input)
            {
                var _COMMA_BWS_commonExpr_BWS_1 = __GeneratedOdataV4.Parsers.Inners._COMMA_BWS_commonExpr_BWSParser.Instance.Parse(input);
if (!_COMMA_BWS_commonExpr_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_BWS_commonExpr_BWSↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤCOMMA_BWS_commonExpr_BWSↃ(_COMMA_BWS_commonExpr_BWS_1.Parsed), _COMMA_BWS_commonExpr_BWS_1.Remainder);
            }
        }
    }
    
}
