namespace NewStuff.Http
{
    using System.Collections.Generic;

    /// <summary>
    /// https://www.rfc-editor.org/rfc/rfc2616#section-14.1
    /// </summary>
    public abstract class MediaRange
    {
        private MediaRange()
        {
        }

        public sealed class All : MediaRange
        {
            private All()
            {
            }
        }

        public sealed class TypeOnly : MediaRange
        {
            private TypeOnly()
            {
            }
        }

        public sealed class TypeAndSubType : MediaRange
        {
            public TypeAndSubType(Type type, Subtype subtype, IEnumerable<Parameter> parameters)
            {
                Type = type;
                Subtype = subtype;
                Parameters = parameters;
            }

            public Type Type { get; }
            public Subtype Subtype { get; }
            public IEnumerable<Parameter> Parameters { get; }
        }
    }
}
