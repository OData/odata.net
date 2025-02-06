namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _contextTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._context>
    {
        private _contextTranscriber()
        {
        }
        
        public static _contextTranscriber Instance { get; } = new _contextTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._context value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._ʺx23ʺTranscriber.Instance.Transcribe(value._ʺx23ʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._contextFragmentTranscriber.Instance.Transcribe(value._contextFragment_1, builder);

        }
    }
    
}
