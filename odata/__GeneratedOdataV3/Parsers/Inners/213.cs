namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSEParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE> Parse(IInput<char>? input)
            {
                var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(input);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE)!, input);
}

var _selectOptionPC_1 = __GeneratedOdataV3.Parsers.Rules._selectOptionPCParser.Instance.Parse(_OPEN_1.Remainder);
if (!_selectOptionPC_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE)!, input);
}

var _ⲤSEMI_selectOptionPCↃ_1 = Inners._ⲤSEMI_selectOptionPCↃParser.Instance.Many().Parse(_selectOptionPC_1.Remainder);
if (!_ⲤSEMI_selectOptionPCↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_ⲤSEMI_selectOptionPCↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE(_OPEN_1.Parsed, _selectOptionPC_1.Parsed, _ⲤSEMI_selectOptionPCↃ_1.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
