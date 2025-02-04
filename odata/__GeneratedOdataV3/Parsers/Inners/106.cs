namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _refⳆOPEN_levels_CLOSEParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE> Instance { get; } = (_refParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE>(_OPEN_levels_CLOSEParser.Instance);
        
        public static class _refParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE._ref> Instance { get; } = from _ref_1 in __GeneratedOdataV3.Parsers.Rules._refParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE._ref(_ref_1);
        }
        
        public static class _OPEN_levels_CLOSEParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE._OPEN_levels_CLOSE> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE._OPEN_levels_CLOSE>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE._OPEN_levels_CLOSE> Parse(IInput<char>? input)
                {
                    var _OPEN_1 = __GeneratedOdataV3.Parsers.Rules._OPENParser.Instance.Parse(input);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE._OPEN_levels_CLOSE)!, input);
}

var _levels_1 = __GeneratedOdataV3.Parsers.Rules._levelsParser.Instance.Parse(_OPEN_1.Remainder);
if (!_levels_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE._OPEN_levels_CLOSE)!, input);
}

var _CLOSE_1 = __GeneratedOdataV3.Parsers.Rules._CLOSEParser.Instance.Parse(_levels_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE._OPEN_levels_CLOSE)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._refⳆOPEN_levels_CLOSE._OPEN_levels_CLOSE(_OPEN_1.Parsed, _levels_1.Parsed,  _CLOSE_1.Parsed), _CLOSE_1.Remainder);
                }
            }
        }
    }
    
}
