namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _boundActionCallTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._boundActionCall>
    {
        private _boundActionCallTranscriber()
        {
        }
        
        public static _boundActionCallTranscriber Instance { get; } = new _boundActionCallTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._boundActionCall value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._actionTranscriber.Instance.Transcribe(value._action_1, builder);

        }
    }
    
}
