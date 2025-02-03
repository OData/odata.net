namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTETranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE>
    {
        private _SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTETranscriber()
        {
        }
        
        public static _SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTETranscriber Instance { get; } = new _SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTETranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV2.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._SQUOTEⲻinⲻstring node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._SQUOTEⲻinⲻstringTranscriber.Instance.Transcribe(node._SQUOTEⲻinⲻstring_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV2.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV2.Trancsribers.Rules._pcharⲻnoⲻSQUOTETranscriber.Instance.Transcribe(node._pcharⲻnoⲻSQUOTE_1, context);

return default;
            }
        }
    }
    
}
