namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤcomplexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriↃParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤcomplexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ⲤcomplexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ⲤcomplexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriↃ> Parse(IInput<char>? input)
            {
                var _complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri_1 = __GeneratedOdataV4.Parsers.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriParser.Instance.Parse(input);
if (!_complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ⲤcomplexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ⲤcomplexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriↃ(_complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri_1.Parsed), _complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri_1.Remainder);
            }
        }
    }
    
}
