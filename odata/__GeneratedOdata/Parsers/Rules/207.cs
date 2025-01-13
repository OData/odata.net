namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _beginⲻobjectParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._beginⲻobject> Instance { get; } = from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _Ⲥʺx7BʺⳆʺx25x37x42ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx7BʺⳆʺx25x37x42ʺↃParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
select new __GeneratedOdata.CstNodes.Rules._beginⲻobject(_BWS_1, _Ⲥʺx7BʺⳆʺx25x37x42ʺↃ_1, _BWS_2);
    }
    
}
