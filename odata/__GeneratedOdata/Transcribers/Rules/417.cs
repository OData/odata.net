namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _qcharⲻnoⲻAMPⲻDQUOTETranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE>
    {
        private _qcharⲻnoⲻAMPⲻDQUOTETranscriber()
        {
        }
        
        public static _qcharⲻnoⲻAMPⲻDQUOTETranscriber Instance { get; } = new _qcharⲻnoⲻAMPⲻDQUOTETranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._qcharⲻunescaped node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._qcharⲻunescapedTranscriber.Instance.Transcribe(node._qcharⲻunescaped_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._escape_ⲤescapeⳆquotationⲻmarkↃ node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._escapeTranscriber.Instance.Transcribe(node._escape_1, context);
__GeneratedOdata.Trancsribers.Inners._ⲤescapeⳆquotationⲻmarkↃTranscriber.Instance.Transcribe(node._ⲤescapeⳆquotationⲻmarkↃ_1, context);

return default;
            }
        }
    }
    
}