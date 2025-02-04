namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ> Parse(IInput<char>? input)
            {
                var _selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName_1 = __GeneratedOdataV3.Parsers.Inners._selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameParser.Instance.Parse(input);
if (!_selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤselectPropertyⳆqualifiedActionNameⳆqualifiedFunctionNameↃ(_selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName_1.Parsed), _selectPropertyⳆqualifiedActionNameⳆqualifiedFunctionName_1.Remainder);
            }
        }
    }
    
}
