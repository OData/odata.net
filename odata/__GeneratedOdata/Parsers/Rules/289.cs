namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _stringParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._string> Instance { get; } = from _SQUOTE_1 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
from _ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃParser.Instance.Many()
from _SQUOTE_2 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._string(_SQUOTE_1, _ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1, _SQUOTE_2);
    }
    
}
