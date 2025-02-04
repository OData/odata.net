namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃ> Parse(IInput<char>? input)
            {
                var _OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm_1 = __GeneratedOdataV3.Parsers.Inners._OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermParser.Instance.Parse(input);
if (!_OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤOPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTermↃ(_OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm_1.Parsed), _OPEN_BWS_searchExpr_BWS_CLOSEⳆsearchTerm_1.Remainder);
            }
        }
    }
    
}
