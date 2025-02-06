namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _singleNavigationParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._singleNavigation> Instance { get; } = from _ʺx2Fʺ_qualifiedEntityTypeName_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional()
from _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalueParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._singleNavigation(_ʺx2Fʺ_qualifiedEntityTypeName_1.GetOrElse(null), _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1.GetOrElse(null));
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._singleNavigation> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._singleNavigation>
        {
            public IOutput<char, _singleNavigation> Parse(IInput<char>? input)
            {
                /*var _ʺx2Fʺ_qualifiedEntityTypeName_1 = __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional().Parse(input);
                if (!_ʺx2Fʺ_qualifiedEntityTypeName_1.Success)
                {
                    return Output.Create(
                        false,
                        default(_singleNavigation)!,
                        input);
                }
                */
                var _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1 = __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalueParser.Instance.Optional().Parse(input);
                if (!_ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1.Success)
                {
                    return Output.Create(
                        false,
                        default(_singleNavigation)!,
                        input);
                }

                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Rules._singleNavigation(null, _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1.Parsed.GetOrElse(null)),
                    _ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue_1.Remainder);
            }
        }
    }
    
}
