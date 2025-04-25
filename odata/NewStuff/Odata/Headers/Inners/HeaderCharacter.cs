namespace NewStuff.Odata.Headers.Inners
{
    public abstract class HeaderCharacter
    {
        private HeaderCharacter()
        {
            //// TODO implement all of the DU members
        }

        public sealed class A : HeaderCharacter
        {
            private A()
            {
                //// TODO you could make this a discriminated union so that you can have case insensitivity
            }
        }

        public sealed class D : HeaderCharacter
        {
            private D()
            {
                //// TODO you could make this a discriminated union so that you can have case insensitivity
            }
        }

        public sealed class O : HeaderCharacter
        {
            private O()
            {
                //// TODO you could make this a discriminated union so that you can have case insensitivity
            }
        }

        public sealed class T : HeaderCharacter
        {
            private T()
            {
                //// TODO you could make this a discriminated union so that you can have case insensitivity
            }
        }
    }
}
