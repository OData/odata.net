namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _pathⲻrootlessTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._pathⲻrootless>
    {
        private _pathⲻrootlessTranscriber()
        {
        }
        
        public static _pathⲻrootlessTranscriber Instance { get; } = new _pathⲻrootlessTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._pathⲻrootless value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._segmentⲻnzTranscriber.Instance.Transcribe(value._segmentⲻnz_1, builder);
foreach (var _Ⲥʺx2Fʺ_segmentↃ_1 in value._Ⲥʺx2Fʺ_segmentↃ_1)
{
Inners._Ⲥʺx2Fʺ_segmentↃTranscriber.Instance.Transcribe(_Ⲥʺx2Fʺ_segmentↃ_1, builder);
}

        }
    }
    
}
