namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _charInJSONTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._charInJSON>
    {
        private _charInJSONTranscriber()
        {
        }
        
        public static _charInJSONTranscriber Instance { get; } = new _charInJSONTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._charInJSON value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Rules._charInJSON.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._charInJSON._qcharⲻunescaped node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._qcharⲻunescapedTranscriber.Instance.Transcribe(node._qcharⲻunescaped_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._charInJSON._qcharⲻJSONⲻspecial node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._qcharⲻJSONⲻspecialTranscriber.Instance.Transcribe(node._qcharⲻJSONⲻspecial_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Rules._charInJSON._escape_ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._escapeTranscriber.Instance.Transcribe(node._escape_1, context);
__GeneratedOdataV2.Trancsribers.Inners._ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃTranscriber.Instance.Transcribe(node._ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ_1, context);

return default;
            }
        }
    }
    
}
