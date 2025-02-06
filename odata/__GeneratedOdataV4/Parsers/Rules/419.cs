namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _IRIⲻinⲻqueryParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._IRIⲻinⲻquery> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._IRIⲻinⲻquery>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._IRIⲻinⲻquery> Parse(IInput<char>? input)
            {
                var _qcharⲻnoⲻAMP_1 = __GeneratedOdataV4.Parsers.Rules._qcharⲻnoⲻAMPParser.Instance.Repeat(1, null).Parse(input);
if (!_qcharⲻnoⲻAMP_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._IRIⲻinⲻquery)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._IRIⲻinⲻquery(new __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP>(_qcharⲻnoⲻAMP_1.Parsed)), _qcharⲻnoⲻAMP_1.Remainder);
            }
        }
    }
    
}
