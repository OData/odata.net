namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ> Instance { get; } = (_STARParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ>(_namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃParser.Instance);
        
        public static class _STARParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._STAR> Instance { get; } = from _STAR_1 in __GeneratedOdataV3.Parsers.Rules._STARParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._STAR(_STAR_1);
        }
        
        public static class _namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ> Parse(IInput<char>? input)
                {
                    var _namespace_1 = __GeneratedOdataV3.Parsers.Rules._namespaceParser.Instance.Parse(input);
if (!_namespace_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ)!, input);
}

var _ʺx2Eʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2EʺParser.Instance.Parse(_namespace_1.Remainder);
if (!_ʺx2Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ)!, input);
}

var _ⲤtermNameⳆSTARↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤtermNameⳆSTARↃParser.Instance.Parse(_ʺx2Eʺ_1.Remainder);
if (!_ⲤtermNameⳆSTARↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._STARⳆnamespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ._namespace_ʺx2Eʺ_ⲤtermNameⳆSTARↃ(_namespace_1.Parsed, _ʺx2Eʺ_1.Parsed,  _ⲤtermNameⳆSTARↃ_1.Parsed), _ⲤtermNameⳆSTARↃ_1.Remainder);
                }
            }
        }
    }
    
}
