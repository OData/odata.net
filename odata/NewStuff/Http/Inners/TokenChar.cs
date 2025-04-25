namespace NewStuff.Http.Inners
{
    /// <summary>
    /// https://www.rfc-editor.org/rfc/rfc2616#section-2.2
    /// </summary>
    public abstract class TokenChar
    {
        private TokenChar()
        {
            //// TODO you have not completely fleshed out this discriminated union
        }

        public sealed class Period : TokenChar
        {
            private Period()
            {
            }

            public static Period Instance { get; } = new Period();
        }

        public sealed class ForwardSlash : TokenChar
        {
            private ForwardSlash()
            {
            }

            public static ForwardSlash Instance { get; } = new ForwardSlash();
        }

        public sealed class _4 : TokenChar
        {
            private _4()
            {
            }

            public static _4 Instance { get; } = new _4();
        }

        public sealed class _5 : TokenChar
        {
            private _5()
            {
            }

            public static _5 Instance { get; } = new _5();
        }

        public sealed class _7 : TokenChar
        {
            private _7()
            {
            }

            public static _7 Instance { get; } = new _7();
        }

        public sealed class Semicolon : TokenChar
        {
            private Semicolon()
            {
            }

            public static Semicolon Instance { get; } = new Semicolon();
        }

        public sealed class EqualsSign : TokenChar
        {
            private EqualsSign()
            {
            }

            public static EqualsSign Instance { get; } = new EqualsSign();
        }

        public sealed class C : TokenChar
        {
            private C()
            {
            }

            public static C Instance { get; } = new C();
        }

        public sealed class D : TokenChar
        {
            private D()
            {
            }

            public static D Instance { get; } = new D();
        }

        public sealed class E : TokenChar
        {
            private E()
            {
            }

            public static E Instance { get; } = new E();
        }

        public sealed class I : TokenChar
        {
            private I()
            {
            }

            public static I Instance { get; } = new I();
        }

        public sealed class _a : TokenChar
        {
            private _a()
            {
            }

            public static _a Instance { get; } = new _a();
        }

        public sealed class _b : TokenChar
        {
            private _b()
            {
            }

            public static _b Instance { get; } = new _b();
        }

        public sealed class _c : TokenChar
        {
            private _c()
            {
            }

            public static _c Instance { get; } = new _c();
        }

        public sealed class _d : TokenChar
        {
            private _d()
            {
            }

            public static _d Instance { get; } = new _d();
        }

        public sealed class _e : TokenChar
        {
            private _e()
            {
            }

            public static _e Instance { get; } = new _e();
        }

        public sealed class _f : TokenChar
        {
            private _f()
            {
            }

            public static _f Instance { get; } = new _f();
        }

        public sealed class _g : TokenChar
        {
            private _g()
            {
            }

            public static _g Instance { get; } = new _g();
        }

        public sealed class _i : TokenChar
        {
            private _i()
            {
            }

            public static _i Instance { get; } = new _i();
        }

        public sealed class _j : TokenChar
        {
            public _j()
            {
            }

            public static _j Instance { get; } = new _j();
        }

        public sealed class _l : TokenChar
        {
            private _l()
            {
            }

            public static _l Instance { get; } = new _l();
        }

        public sealed class _m : TokenChar
        {
            private _m()
            {
            }

            public static _m Instance { get; } = new _m();
        }

        public sealed class _n : TokenChar
        {
            private _n()
            {
            }

            public static _n Instance { get; } = new _n();
        }

        public sealed class _o : TokenChar
        {
            private _o()
            {
            }

            public static _o Instance { get; } = new _o();
        }

        public sealed class _p : TokenChar
        {
            private _p()
            {
            }

            public static _p Instance { get; } = new _p();
        }

        public sealed class _r : TokenChar
        {
            private _r()
            {
            }

            public static _r Instance { get; } = new _r();
        }

        public sealed class _s : TokenChar
        {
            private _s()
            {
            }

            public static _s Instance { get; } = new _s();
        }

        public sealed class _t : TokenChar
        {
            private _t()
            {
            }

            public static _t Instance { get; } = new _t();
        }

        public sealed class _u : TokenChar
        {
            private _u()
            {
            }

            public static _u Instance { get; } = new _u();
        }

        public sealed class _x : TokenChar
        {
            private _x()
            {
            }

            public static _x Instance { get; } = new _x();
        }
    }
}
