namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _hierⲻpartParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart> Instance { get; } = (_ʺx2Fx2Fʺ_authority_pathⲻabemptyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart>(_pathⲻabsoluteParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart>(_pathⲻrootlessParser.Instance);
        
        public static class _ʺx2Fx2Fʺ_authority_pathⲻabemptyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._ʺx2Fx2Fʺ_authority_pathⲻabempty> Instance { get; } = from _ʺx2Fx2Fʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx2Fx2FʺParser.Instance
from _authority_1 in __GeneratedOdataV3.Parsers.Rules._authorityParser.Instance
from _pathⲻabempty_1 in __GeneratedOdataV3.Parsers.Rules._pathⲻabemptyParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._ʺx2Fx2Fʺ_authority_pathⲻabempty(_ʺx2Fx2Fʺ_1, _authority_1, _pathⲻabempty_1);
        }
        
        public static class _pathⲻabsoluteParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._pathⲻabsolute> Instance { get; } = from _pathⲻabsolute_1 in __GeneratedOdataV3.Parsers.Rules._pathⲻabsoluteParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._pathⲻabsolute(_pathⲻabsolute_1);
        }
        
        public static class _pathⲻrootlessParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._pathⲻrootless> Instance { get; } = from _pathⲻrootless_1 in __GeneratedOdataV3.Parsers.Rules._pathⲻrootlessParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._pathⲻrootless(_pathⲻrootless_1);
        }
    }
    
}
