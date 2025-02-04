namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥvalueⲻseparator_complexInUriↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥvalueⲻseparator_complexInUriↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥvalueⲻseparator_complexInUriↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥvalueⲻseparator_complexInUriↃ> Parse(IInput<char>? input)
            {
                var _valueⲻseparator_complexInUri_1 = __GeneratedOdataV3.Parsers.Inners._valueⲻseparator_complexInUriParser.Instance.Parse(input);
if (!_valueⲻseparator_complexInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥvalueⲻseparator_complexInUriↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥvalueⲻseparator_complexInUriↃ(_valueⲻseparator_complexInUri_1.Parsed), _valueⲻseparator_complexInUri_1.Remainder);
            }
        }
    }
    
}
