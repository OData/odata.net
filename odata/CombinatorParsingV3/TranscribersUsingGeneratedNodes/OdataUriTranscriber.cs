namespace CombinatorParsingV3.TranscribersUsingGeneratedNodes
{
    using System.Text;

    using GeneratorV3;
    using __GeneratedPartialV1.Deferred.CstNodes.Inners;
    using __GeneratedPartialV1.Deferred.CstNodes.Rules;

    public sealed class OdataUriTranscriber : ITranscriber<_odataUri<ParseMode.Realized>>
    {
        private OdataUriTranscriber()
        {
        }

        public static OdataUriTranscriber Instance { get; } = new OdataUriTranscriber();

        private static V3ParserPlayground.AtLeastOneTranscriber<_segment<ParseMode.Deferred>, _segment<ParseMode.Realized>> SegmentsTranscriber = new V3ParserPlayground.AtLeastOneTranscriber<_segment<ParseMode.Deferred>, _segment<ParseMode.Realized>>(SegmentTranscriber.Instance);

        public void Transcribe(_odataUri<ParseMode.Realized> value, StringBuilder builder)
        {

        }
    }

    public sealed class SegmentTranscriber : ITranscriber<_segment<ParseMode.Realized>>
    {
        private SegmentTranscriber()
        {
        }

        public static SegmentTranscriber Instance { get; } = new SegmentTranscriber();

        private static V3ParserPlayground.AtLeastOneTranscriber<_alphaNumeric<ParseMode.Deferred>, _alphaNumeric<ParseMode.Realized>> CharactersTranscriber { get; } = new V3ParserPlayground.AtLeastOneTranscriber<_alphaNumeric<ParseMode.Deferred>, _alphaNumeric<ParseMode.Realized>>()

        public void Transcribe(_segment<ParseMode.Realized> value, StringBuilder builder)
        {
            throw new System.NotImplementedException();
        }
    }

    public sealed class AlphaNumericTranscriber : ITranscriber<_alphaNumeric<ParseMode.Realized>>
    {
        private AlphaNumericTranscriber()
        {
        }

        public static AlphaNumericTranscriber Instance { get; } = new AlphaNumericTranscriber();

        public void Transcribe(_alphaNumeric<ParseMode.Realized> value, StringBuilder builder)
        {
            throw new System.NotImplementedException();
        }
    }
}
