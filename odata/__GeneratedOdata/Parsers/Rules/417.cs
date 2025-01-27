namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qcharⲻnoⲻAMPⲻDQUOTEParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE> Instance { get; } = (_qcharⲻunescapedParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE>(_escape_ⲤescapeⳆquotationⲻmarkↃParser.Instance);
        
        public static class _qcharⲻunescapedParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._qcharⲻunescaped> Instance { get; } = from _qcharⲻunescaped_1 in __GeneratedOdata.Parsers.Rules._qcharⲻunescapedParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._qcharⲻunescaped(_qcharⲻunescaped_1);
        }
        
        public static class _escape_ⲤescapeⳆquotationⲻmarkↃParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._escape_ⲤescapeⳆquotationⲻmarkↃ> Instance { get; } = from _escape_1 in __GeneratedOdata.Parsers.Rules._escapeParser.Instance
from _ⲤescapeⳆquotationⲻmarkↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤescapeⳆquotationⲻmarkↃParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._escape_ⲤescapeⳆquotationⲻmarkↃ(_escape_1, _ⲤescapeⳆquotationⲻmarkↃ_1);
        }
    }
    
}
