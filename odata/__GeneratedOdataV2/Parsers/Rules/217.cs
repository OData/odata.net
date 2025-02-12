namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qcharⲻJSONⲻspecialParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial> Instance { get; } = (_SPParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial>(_ʺx3AʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial>(_ʺx7BʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial>(_ʺx7DʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial>(_ʺx5BʺParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial>(_ʺx5DʺParser.Instance);
        
        public static class _SPParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial._SP> Instance { get; } = from _SP_1 in __GeneratedOdataV2.Parsers.Rules._SPParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial._SP.Instance;
        }
        
        public static class _ʺx3AʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx3Aʺ> Instance { get; } = from _ʺx3Aʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx3AʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx3Aʺ.Instance;
        }
        
        public static class _ʺx7BʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Bʺ> Instance { get; } = from _ʺx7Bʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx7BʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Bʺ.Instance;
        }
        
        public static class _ʺx7DʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Dʺ> Instance { get; } = from _ʺx7Dʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx7DʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Dʺ.Instance;
        }
        
        public static class _ʺx5BʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Bʺ> Instance { get; } = from _ʺx5Bʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx5BʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Bʺ.Instance;
        }
        
        public static class _ʺx5DʺParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Dʺ> Instance { get; } = from _ʺx5Dʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx5DʺParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Dʺ.Instance;
        }
    }
    
}
