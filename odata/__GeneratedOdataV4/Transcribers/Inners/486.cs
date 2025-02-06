namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ>
    {
        private _ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺTranscriber()
        {
        }
        
        public static _ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺTranscriber Instance { get; } = new _ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx41ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx41ʺTranscriber.Instance.Transcribe(node._ʺx41ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx51ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx51ʺTranscriber.Instance.Transcribe(node._ʺx51ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx67ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx67ʺTranscriber.Instance.Transcribe(node._ʺx67ʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ._ʺx77ʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Inners._ʺx77ʺTranscriber.Instance.Transcribe(node._ʺx77ʺ_1, context);

return default;
            }
        }
    }
    
}
