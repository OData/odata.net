namespace NewStuff.Http.Inners
{
    /// <summary>
    /// https://www.rfc-editor.org/rfc/rfc2616#section-14.1
    /// </summary>
    public sealed class MediaRangeAcceptParamsPair
    {
        public MediaRangeAcceptParamsPair(MediaRange mediaRange, AcceptParams? acceptParams)
        {
            MediaRange = mediaRange;
            AcceptParams = acceptParams;
        }

        public MediaRange MediaRange { get; }
        public AcceptParams? AcceptParams { get; }
    }
}
