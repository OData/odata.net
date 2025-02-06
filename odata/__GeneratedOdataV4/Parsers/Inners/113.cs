namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSEParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE> Parse(IInput<char>? input)
            {
                var _OPEN_1 = __GeneratedOdataV4.Parsers.Rules._OPENParser.Instance.Parse(input);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE)!, input);
}

var _expandCountOption_1 = __GeneratedOdataV4.Parsers.Rules._expandCountOptionParser.Instance.Parse(_OPEN_1.Remainder);
if (!_expandCountOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE)!, input);
}

var _ⲤSEMI_expandCountOptionↃ_1 = Inners._ⲤSEMI_expandCountOptionↃParser.Instance.Many().Parse(_expandCountOption_1.Remainder);
if (!_ⲤSEMI_expandCountOptionↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE)!, input);
}

var _CLOSE_1 = __GeneratedOdataV4.Parsers.Rules._CLOSEParser.Instance.Parse(_ⲤSEMI_expandCountOptionↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE(_OPEN_1.Parsed, _expandCountOption_1.Parsed, _ⲤSEMI_expandCountOptionↃ_1.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
