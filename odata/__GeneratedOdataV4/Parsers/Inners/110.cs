namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSEParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE> Parse(IInput<char>? input)
            {
                var _OPEN_1 = __GeneratedOdataV4.Parsers.Rules._OPENParser.Instance.Parse(input);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE)!, input);
}

var _expandRefOption_1 = __GeneratedOdataV4.Parsers.Rules._expandRefOptionParser.Instance.Parse(_OPEN_1.Remainder);
if (!_expandRefOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE)!, input);
}

var _ⲤSEMI_expandRefOptionↃ_1 = Inners._ⲤSEMI_expandRefOptionↃParser.Instance.Many().Parse(_expandRefOption_1.Remainder);
if (!_ⲤSEMI_expandRefOptionↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE)!, input);
}

var _CLOSE_1 = __GeneratedOdataV4.Parsers.Rules._CLOSEParser.Instance.Parse(_ⲤSEMI_expandRefOptionↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE(_OPEN_1.Parsed, _expandRefOption_1.Parsed, _ⲤSEMI_expandRefOptionↃ_1.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
            }
        }
    }
    
}
