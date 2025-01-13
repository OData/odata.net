namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _durationParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._duration> Instance { get; } = from _ʺx64x75x72x61x74x69x6Fx6Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx64x75x72x61x74x69x6Fx6EʺParser.Instance.Optional()
from _SQUOTE_1 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
from _durationValue_1 in __GeneratedOdata.Parsers.Rules._durationValueParser.Instance
from _SQUOTE_2 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._duration(_ʺx64x75x72x61x74x69x6Fx6Eʺ_1.GetOrElse(null), _SQUOTE_1, _durationValue_1, _SQUOTE_2);
    }
    
}
