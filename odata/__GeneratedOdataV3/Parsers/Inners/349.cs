namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _complexInUri_ЖⲤvalueⲻseparator_complexInUriↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ> Parse(IInput<char>? input)
            {
                var _complexInUri_1 = __GeneratedOdataV3.Parsers.Rules._complexInUriParser.Instance.Parse(input);
if (!_complexInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ)!, input);
}

var _Ⲥvalueⲻseparator_complexInUriↃ_1 = Inners._Ⲥvalueⲻseparator_complexInUriↃParser.Instance.Many().Parse(_complexInUri_1.Remainder);
if (!_Ⲥvalueⲻseparator_complexInUriↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ(_complexInUri_1.Parsed, _Ⲥvalueⲻseparator_complexInUriↃ_1.Parsed), _Ⲥvalueⲻseparator_complexInUriↃ_1.Remainder);
            }
        }
    }
    
}
