namespace odata.tests
{
    [TestClass]
    public sealed class NullableTests
    {
        [TestMethod]
        public void Test1()
        {
            var iterations = 10000000;
            var classes = new List<ClassNullable<int>>();
            var structs = new List<StructNullable<int>>();

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                classes.Add(new ClassNullable<int>());
            }

            for (int i = 0; i < iterations; ++i)
            {
                classes[i].TryGetValue(out var value);
            }

            Console.WriteLine(stopwatch.ElapsedTicks);

            stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                structs.Add(new StructNullable<int>());
            }

            for (int i = 0; i < iterations; ++i)
            {
                structs[i].TryGetValue(out var value);
            }

            Console.WriteLine(stopwatch.ElapsedTicks);
        }

        [TestMethod]
        public void Test2()
        {
            var iterations = 10000000;
            var classes = new List<ClassNullable<int>>();
            var structs = new List<StructNullable<int>>();

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                classes.Add(new ClassNullable<int>());
            }

            for (int i = 0; i < iterations; ++i)
            {
                var nullable = classes[i];
                nullable.TryGetValue(out var value);
            }

            Console.WriteLine(stopwatch.ElapsedTicks);

            stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                structs.Add(new StructNullable<int>());
            }

            for (int i = 0; i < iterations; ++i)
            {
                var nullable = structs[i];
                nullable.TryGetValue(out var value);
            }

            Console.WriteLine(stopwatch.ElapsedTicks);
        }

        [TestMethod]
        public void Test3()
        {
            var iterations = 10000000;
            var classes = new List<ClassNullable<int>>();
            var structs = new List<StructNullable<int>>();

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                classes.Add(new ClassNullable<int>(42));
            }

            for (int i = 0; i < iterations; ++i)
            {
                var nullable = classes[i];
                nullable.TryGetValue(out var value);
            }

            Console.WriteLine(stopwatch.ElapsedTicks);
            
            stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                structs.Add(new StructNullable<int>(42));
            }

            for (int i = 0; i < iterations; ++i)
            {
                var nullable = structs[i];
                nullable.TryGetValue(out var value);
            }

            Console.WriteLine(stopwatch.ElapsedTicks);
        }

        [TestMethod]
        public void Test4()
        {
            var iterations = 100000000;
            var classes = new List<INullable<int>>();
            var structs = new List<INullable<int>>();

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                classes.Add(new ClassNullable<int>(42));
            }

            for (int i = 0; i < iterations; ++i)
            {
                var nullable = classes[i];
                nullable.TryGetValue(out var value);
            }

            Console.WriteLine(stopwatch.ElapsedTicks);

            stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; ++i)
            {
                structs.Add(new StructNullable<int>(42));
            }

            for (int i = 0; i < iterations; ++i)
            {
                var nullable = structs[i];
                nullable.TryGetValue(out var value);
            }

            Console.WriteLine(stopwatch.ElapsedTicks);
        }
    }

    public interface INullable<T>
    {
        bool TryGetValue(out T value);
    }

    /// <summary>
    /// TODO are you even ok with this being able to be null itself?
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ClassNullable<T> : INullable<T>
    {
        private readonly T value;
        private readonly bool hasValue;

        public ClassNullable()
        {
            this.hasValue = false;
        }

        public ClassNullable(T value)
        {
            this.value = value;

            this.hasValue = true;
        }

        public bool TryGetValue(out T value)
        {
            value = this.value;
            return this.hasValue;
        }
    }

    public readonly struct StructNullable<T> : INullable<T>
    {
        private readonly T value;
        private readonly bool hasValue;

        public StructNullable(T value)
        {
            this.value = value;

            this.hasValue = true;
        }

        public bool TryGetValue(out T value)
        {
            value = this.value;
            return this.hasValue;
        }
    }
}
