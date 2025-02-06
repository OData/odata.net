namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexInUriParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._complexInUri> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._complexInUri>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._complexInUri> Parse(IInput<char>? input)
            {
                var _beginⲻobject_1 = __GeneratedOdataV4.Parsers.Rules._beginⲻobjectParser.Instance.Parse(input);
if (!_beginⲻobject_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._complexInUri)!, input);
}

var _ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃ_ЖⲤvalueⲻseparator_ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃↃ_1 = __GeneratedOdataV4.Parsers.Inners._ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃ_ЖⲤvalueⲻseparator_ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃↃParser.Instance.Optional().Parse(_beginⲻobject_1.Remainder);
if (!_ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃ_ЖⲤvalueⲻseparator_ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._complexInUri)!, input);
}

var _endⲻobject_1 = __GeneratedOdataV4.Parsers.Rules._endⲻobjectParser.Instance.Parse(_ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃ_ЖⲤvalueⲻseparator_ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃↃ_1.Remainder);
if (!_endⲻobject_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._complexInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._complexInUri(_beginⲻobject_1.Parsed, _ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃ_ЖⲤvalueⲻseparator_ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃↃ_1.Parsed.GetOrElse(null), _endⲻobject_1.Parsed), _endⲻobject_1.Remainder);
            }
        }
    }
    
}
