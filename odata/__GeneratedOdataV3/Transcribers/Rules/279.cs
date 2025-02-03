namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _decimalValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._decimalValue>
    {
        private _decimalValueTranscriber()
        {
        }
        
        public static _decimalValueTranscriber Instance { get; } = new _decimalValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._decimalValue value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Rules._decimalValue.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._decimalValue._꘡SIGN꘡_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_꘡ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT꘡ node, System.Text.StringBuilder context)
            {
                if (node._SIGN_1 != null)
{
__GeneratedOdataV3.Trancsribers.Rules._SIGNTranscriber.Instance.Transcribe(node._SIGN_1, context);
}
foreach (var _DIGIT_1 in node._DIGIT_1)
{
__GeneratedOdataV3.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, context);
}
if (node._ʺx2Eʺ_1ЖDIGIT_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ʺx2Eʺ_1ЖDIGITTranscriber.Instance.Transcribe(node._ʺx2Eʺ_1ЖDIGIT_1, context);
}
if (node._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ʺx65ʺ_꘡SIGN꘡_1ЖDIGITTranscriber.Instance.Transcribe(node._ʺx65ʺ_꘡SIGN꘡_1ЖDIGIT_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Rules._decimalValue._nanInfinity node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._nanInfinityTranscriber.Instance.Transcribe(node._nanInfinity_1, context);

return default;
            }
        }
    }
    
}
