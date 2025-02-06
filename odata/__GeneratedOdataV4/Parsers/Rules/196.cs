namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexColInUriParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._complexColInUri> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._complexColInUri>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._complexColInUri> Parse(IInput<char>? input)
            {
                var _beginⲻarray_1 = __GeneratedOdataV4.Parsers.Rules._beginⲻarrayParser.Instance.Parse(input);
if (!_beginⲻarray_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._complexColInUri)!, input);
}

var _complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ_1 = __GeneratedOdataV4.Parsers.Inners._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃParser.Instance.Optional().Parse(_beginⲻarray_1.Remainder);
if (!_complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._complexColInUri)!, input);
}

var _endⲻarray_1 = __GeneratedOdataV4.Parsers.Rules._endⲻarrayParser.Instance.Parse(_complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ_1.Remainder);
if (!_endⲻarray_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._complexColInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._complexColInUri(_beginⲻarray_1.Parsed, _complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ_1.Parsed.GetOrElse(null), _endⲻarray_1.Parsed), _endⲻarray_1.Remainder);
            }
        }
    }
    
}
