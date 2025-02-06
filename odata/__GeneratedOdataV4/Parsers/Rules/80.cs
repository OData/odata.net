namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _searchParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._search> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._search>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._search> Parse(IInput<char>? input)
            {
                var _Ⲥʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺↃ_1 = __GeneratedOdataV4.Parsers.Inners._Ⲥʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺↃParser.Instance.Parse(input);
if (!_Ⲥʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._search)!, input);
}

var _EQ_1 = __GeneratedOdataV4.Parsers.Rules._EQParser.Instance.Parse(_Ⲥʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺↃ_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._search)!, input);
}

var _BWS_1 = __GeneratedOdataV4.Parsers.Rules._BWSParser.Instance.Parse(_EQ_1.Remainder);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._search)!, input);
}

var _searchExpr_1 = __GeneratedOdataV4.Parsers.Rules._searchExprParser.Instance.Parse(_BWS_1.Remainder);
if (!_searchExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._search)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._search(_Ⲥʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺↃ_1.Parsed, _EQ_1.Parsed, _BWS_1.Parsed, _searchExpr_1.Parsed), _searchExpr_1.Remainder);
            }
        }
    }
    
}
