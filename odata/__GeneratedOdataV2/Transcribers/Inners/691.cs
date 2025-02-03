namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _escapeⳆquotationⲻmarkTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._escapeⳆquotationⲻmark>
    {
        private _escapeⳆquotationⲻmarkTranscriber()
        {
        }
        
        public static _escapeⳆquotationⲻmarkTranscriber Instance { get; } = new _escapeⳆquotationⲻmarkTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._escapeⳆquotationⲻmark value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Inners._escapeⳆquotationⲻmark.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._escapeⳆquotationⲻmark._escape node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._escapeTranscriber.Instance.Transcribe(node._escape_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._escapeⳆquotationⲻmark._quotationⲻmark node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(node._quotationⲻmark_1, context);

return default;
            }
        }
    }
    
}
