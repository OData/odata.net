namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _qcharⲻJSONⲻspecialParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial> Instance { get; } = (_SPParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial>(_ʺx3AʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial>(_ʺx7BʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial>(_ʺx7DʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial>(_ʺx5BʺParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial>(_ʺx5DʺParser.Instance);
        
        public static class _SPParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._SP> Instance { get; } = from _SP_1 in __GeneratedOdata.Parsers.Rules._SPParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._SP(_SP_1);
        }
        
        public static class _ʺx3AʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx3Aʺ> Instance { get; } = from _ʺx3Aʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3AʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx3Aʺ(_ʺx3Aʺ_1);
        }
        
        public static class _ʺx7BʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Bʺ> Instance { get; } = from _ʺx7Bʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx7BʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Bʺ(_ʺx7Bʺ_1);
        }
        
        public static class _ʺx7DʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Dʺ> Instance { get; } = from _ʺx7Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx7DʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Dʺ(_ʺx7Dʺ_1);
        }
        
        public static class _ʺx5BʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Bʺ> Instance { get; } = from _ʺx5Bʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx5BʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Bʺ(_ʺx5Bʺ_1);
        }
        
        public static class _ʺx5DʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Dʺ> Instance { get; } = from _ʺx5Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx5DʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Dʺ(_ʺx5Dʺ_1);
        }
    }
    
}