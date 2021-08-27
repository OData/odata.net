//---------------------------------------------------------------------
// <copyright file="TestItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Base class for test items (modules, cases and variations)
    /// </summary>
    public abstract class TestItem
    {
        private static Func<DateTime> now = () => DateTime.Now;
        private Collection<BugAttribute> bugs = new Collection<BugAttribute>();
        private ReadOnlyCollection<TestItem> children;
        private Func<TestItem, bool> testItemFilter = testItem => true;

        /// <summary>
        /// Initializes a new instance of the TestItem class and assigns default priority of 2.
        /// </summary>
        protected TestItem()
        {
            this.Metadata = new TestItemMetadata()
            {
                Priority = 2,
            };

            this.ExplorationKind = null;
        }

        /// <summary>
        /// Gets or sets the seed to use when generating test items with a <see cref="TestMatrixAttribute"/>.
        /// </summary>
        public int ExplorationSeed { get; set; }

        /// <summary>
        /// Gets or sets the exploration kind to use when generating test items with <see cref="TestMatrixAttribute"/>.
        /// If the value is null exploration kind specified on the <see cref="TestMatrixAttribute"/> is used.
        /// If the value is not null it wins over the exploration kind specified on the <see cref="TestMatrixAttribute"/>.
        /// </summary>
        public TestMatrixExplorationKind? ExplorationKind { get; set; }

        /// <summary>
        /// Gets all of the bugs associated with this <see cref="TestItem"/>.
        /// </summary>
        public Collection<BugAttribute> Bugs
        {
            get { return this.bugs; }
        }

        /// <summary>
        /// Gets collection of child items (test cases and variations) created dynamically based 
        /// on [TestCase] and [TestVariation] attributes.
        /// </summary>
        public ReadOnlyCollection<TestItem> Children
        {
            get
            {
                if (this.children == null)
                {
                    var unfilteredChildren = this.DetermineChildren().ToList();

                    // include all items that have passed the filter or have children which have passed
                    // the filter recursively
                    this.children = unfilteredChildren
                        .Where(ti => ti.Children.Any() || this.GetTestItemFilter()(ti))
                        .ToList()
                        .AsReadOnly();
                }

                return this.children;
            }
        }

        /// <summary>
        /// Gets the metadata for the test item.
        /// </summary>
        public TestItemMetadata Metadata { get; private set; }

        /// <summary>
        /// Gets or sets a function that provides the current time.
        /// </summary>
        internal static Func<DateTime> Now
        {
            get { return now; }
            set { now = value; }
        }

        /// <summary>
        /// Gets or sets this <see cref="TestItem"/>'s parent, if one exists.
        /// </summary>
        protected internal TestItem Parent { get; protected set; }

        /// <summary>
        /// Returns a value indicating whether the given type is a test case
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True if the class derives from TestCase and has appropriate attributes</returns>
        public static bool IsTestCase(Type type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            return type.IsSubclassOf(typeof(TestCase)) && (type.IsDefined(typeof(TestCaseAttribute), false) || type.IsDefined(typeof(TestMatrixAttribute), false));
        }

        /// <summary>
        /// Intitializes test item. Will be invoked before test item is executed.
        /// </summary>
        public virtual void Init()
        {
            if (this.Metadata.SkipUntil.HasValue)
            {
                if (now() > this.Metadata.SkipUntil.Value)
                {
                    throw new TaupoInvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "This item was set to be skipped until {0} but it is now after that date. Please investigate. Skip Reason: {1}",
                            this.Metadata.SkipUntil.Value,
                            this.Metadata.SkipReason));
                }
                else
                {
                    throw new TestSkippedException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "This item was set to be skipped until {0}. Skip Reason: {1}",
                            this.Metadata.SkipUntil.Value,
                            this.Metadata.SkipReason));
                }
            }
        }

        /// <summary>
        /// Intitializes test item asynchronously. Will be invoked before test item is executed and
        /// after <see cref="Init"/> has been invoked.
        /// </summary>
        /// <param name="continuation">The continuation.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Needs to catch all possible exceptions.")]
        public void InitAsync(IAsyncContinuation continuation)
        {
            ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");

            try
            {
                IEnumerable<Action<IAsyncContinuation>> actions = null;
                using (var context = AsyncExecutionContext.Begin())
                {
                    this.Init();
                    actions = context.GetQueuedActions();
                }

                AsyncHelpers.RunActionSequence(continuation, actions);
            }
            catch (Exception ex)
            {
                if (ExceptionUtilities.IsCatchable(ex))
                {
                    continuation.Fail(ex);
                }
            }
        }

        /// <summary>
        /// Executes the test item.
        /// </summary>
        public virtual void Execute()
        {
        }

        /// <summary>
        /// Cleans up after test item execution.
        /// </summary>
        public virtual void Terminate()
        {
            this.bugs = null;
            this.children = null;
            this.Metadata = null;
            this.Parent = null;
        }

        /// <summary>
        /// Cleans up after test item execution (asynchronous operations). This method will be invoked
        /// after regular cleanup has happened.
        /// </summary>
        /// <param name="continuation">The continuation.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Needs to catch all possible exceptions.")]
        public void TerminateAsync(IAsyncContinuation continuation)
        {
            ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");

            try
            {
                IEnumerable<Action<IAsyncContinuation>> actions = null;
                using (var context = AsyncExecutionContext.Begin())
                {
                    this.Terminate();
                    actions = context.GetQueuedActions();
                }

                AsyncHelpers.RunActionSequence(continuation, actions);
            }
            catch (Exception ex)
            {
                continuation.Fail(ex);
            }
        }

        /// <summary>
        /// Sets the test item filter which determines whether test items returned by <see cref="DetermineChildren"/>
        /// should be included.
        /// </summary>
        /// <param name="newFilter">Filter function or null which means no filter.</param>
        /// <remarks>This function can only be called before children have been determined.</remarks>
        public virtual void SetTestItemFilter(Func<TestItem, bool> newFilter)
        {
            if (newFilter == null)
            {
                newFilter = testItem => true;
            }

            if (this.children != null)
            {
                throw new TaupoInvalidOperationException("Test item filter can only be specified before children have been determined.");
            }

            this.testItemFilter = newFilter;
        }

        /// <summary>
        /// Returns a sequence containing this test item and all children (recursive, using pre-order walk).
        /// </summary>
        /// <returns>Sequence of <see cref="TestItem"/>.</returns>
        public IEnumerable<TestItem> GetAllChildrenRecursive()
        {
            return new TestItem[1] { this }
                .Concat(this.Children.SelectMany(c => c.GetAllChildrenRecursive()));
        }

        /// <summary>
        /// Returns a string representation of the test item.
        /// </summary>
        /// <returns>Item Description</returns>
        public override string ToString()
        {
            return this.Metadata.Description;
        }

        /// <summary>
        /// Fills in metadata from a given attribute.
        /// </summary>
        /// <param name="attribute">Attribute to read metadata from</param>
        protected void ReadMetadataFromAttribute(TestItemBaseAttribute attribute)
        {
            var metadata = this.Metadata;
            metadata.Id = attribute.Id;
            metadata.Owner = attribute.Owner;
            metadata.Priority = attribute.Priority;
            metadata.Name = attribute.Name ?? this.GetDefaultMetadataName();
            metadata.Description = attribute.Description ?? metadata.Name;
            metadata.Version = attribute.Version;
            metadata.SkipUntil = attribute.SkipUntilDate;
            metadata.SkipReason = attribute.SkipReason;
        }

        /// <summary>
        /// Gets default value for <see cref="TestItemMetadata.Name"/> property.
        /// </summary>
        /// <returns>Unqualified name of the type.</returns>
        protected virtual string GetDefaultMetadataName()
        {
            return this.GetType().Name;
        }

        /// <summary>
        /// Gets the test item filter which determines whether test items returned by <see cref="DetermineChildren"/>
        /// should be included.
        /// </summary>
        /// <returns>Test Item Filter field</returns>
        protected virtual Func<TestItem, bool> GetTestItemFilter()
        {
            return this.testItemFilter;
        }

        /// <summary>
        /// Determines how the TestCases are found
        /// </summary>
        /// <returns>Returns a list of TestCases</returns>
        protected virtual IEnumerable<Type> DetermineTestCasesTypes()
        {
            // add nested types with [TestCase]
            IEnumerable<Type> nestedTestCases = new List<Type>();
            Type type = this.GetType();
            do
            {
                nestedTestCases = nestedTestCases.Union(type.GetNestedTypes(true)
                    .Where(t => IsTestCase(t)));
                type = type.GetBaseType();
            }
            while (type != null);

            return nestedTestCases;
        }

        /// <summary>
        /// Calculates the child tests items for this item
        /// </summary>
        /// <returns>Enumerable of children belonging this item</returns>
        protected virtual IEnumerable<TestItem> DetermineChildren()
        {
            List<TestItem> result = new List<TestItem>();

            this.GenerateTestCases(result);

            this.GenerateVariations(result);

            return result.OrderBy(c => c.Metadata.Id).ThenBy(c => c.Metadata.Description);
        }

        /// <summary>
        /// Creates a child test case based on the specified <see cref="Type"/>, <see cref="TestCaseAttribute"/>,
        /// and bug attributes.
        /// </summary>
        /// <param name="testCaseType">The <see cref="Type"/> of the test case to create.</param>
        /// <param name="testCaseAttribute">The <see cref="TestCaseAttribute"/> which contains metadata.</param>
        /// <param name="testCaseBugs">The bugs associated with this test case.</param>
        /// <returns>The created test case.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "We specifically want a TestCaseAttribute, not a TestItemWithParametersAttribute, because this method is about creating test cases.")]
        protected TestCase CreateChildTestCase(Type testCaseType, TestCaseAttribute testCaseAttribute, IEnumerable<BugAttribute> testCaseBugs)
        {
            ExceptionUtilities.CheckArgumentNotNull(testCaseType, "testCaseType");
            ExceptionUtilities.CheckArgumentNotNull(testCaseAttribute, "testCaseAttribute");
            ExceptionUtilities.CheckArgumentNotNull(testCaseBugs, "testCaseBugs");

            TestCase tc = (TestCase)Activator.CreateInstance(testCaseType, testCaseAttribute.Parameters);
            tc.ExplorationSeed = this.ExplorationSeed;
            tc.ExplorationKind = this.ExplorationKind;
            tc.ReadMetadataFromAttribute(testCaseAttribute);
            tc.Parent = this;
            tc.SetTestItemFilter(this.GetTestItemFilter());

            foreach (BugAttribute attr in testCaseBugs)
            {
                tc.Bugs.Add(attr);
            }

            return tc;
        }

        /// <summary>
        /// Finds all bugs associated with the specified child <see cref="MemberInfo"/>:
        /// either a <see cref="Type"/> in the case of a nested <see cref="TestCase"/> or
        /// a <see cref="MethodInfo"/> in the case of a nested <see cref="VariationTestItem"/>.
        /// </summary>
        /// <param name="testCaseMember">The <see cref="MemberInfo"/> from which to find bugs.</param>
        /// <returns>
        /// An enumerable of bug attributes describing all bugs that affect this test case.
        /// </returns>
        protected IEnumerable<BugAttribute> FindBugs(MemberInfo testCaseMember)
        {
            ExceptionUtilities.CheckArgumentNotNull(testCaseMember, "testCaseMember");

            Collection<BugAttribute> allBugs = new Collection<BugAttribute>();
            foreach (BugAttribute bugAttr in ((BugAttribute[])testCaseMember.GetCustomAttributes(typeof(BugAttribute), false)).Union(this.bugs))
            {
                allBugs.Add(bugAttr);
            }

            return allBugs;
        }

        private void GenerateTestCases(List<TestItem> result)
        {
            // add nested types with [TestCase]
            IEnumerable<Type> nestedTestCases = this.DetermineTestCasesTypes();

            // Verify results coming back are valid
            ExceptionUtilities.CheckObjectNotNull(nestedTestCases, "Error, DetermineTestCases returns a null list of TestCase Types");

            foreach (var testCase in nestedTestCases)
            {
                var allBugs = this.FindBugs(testCase);

                var testCaseAttributes = testCase.GetCustomAttributes(typeof(TestCaseAttribute), false).Cast<TestCaseAttribute>().ToList();
                var matrixAttr = (TestMatrixAttribute)testCase.GetCustomAttributes(typeof(TestMatrixAttribute), false).SingleOrDefault();
                if (matrixAttr != null)
                {
                    var dimensionAttrs = testCase.GetCustomAttributes(typeof(TestMatrixDimensionAttribute), false).Cast<TestMatrixDimensionAttribute>();
                    var constraintsInfo = this.GetConstraintsInfo(testCase);
                    foreach (var attr in this.GenerateTestAttributesFromMatrix<TestCaseAttribute>(matrixAttr, dimensionAttrs, constraintsInfo))
                    {
                        testCaseAttributes.Add(attr);
                    }
                }

                foreach (TestCaseAttribute testCaseAttr in testCaseAttributes)
                {
                    TestCase tc = this.CreateChildTestCase(testCase, testCaseAttr, allBugs);

                    if (matrixAttr != null)
                    {
                        tc = this.InvokeMatrixCombinationCallback(testCase, tc, testCaseAttr);
                        this.AssertTestItemIsNotSkippedWhenExplorationIsNotExhaustive(tc, matrixAttr);
                    }

                    if (tc != null)
                    {
                        result.Add(tc);
                    }
                }
            }
        }

        private void GenerateVariations(List<TestItem> result)
        {
            // add methods with [Variation] or [TestMatrix]
            foreach (MethodInfo mi in this.GetType().GetMethods())
            {
                // get any [___Bug] attributes
                var allBugs = ((BugAttribute[])mi.GetCustomAttributes(typeof(BugAttribute), false)).Union(this.bugs).ToList();

                // get any [Variation] attributes
                var variationAttributes = mi.GetCustomAttributes(typeof(VariationAttribute), false).Cast<VariationAttribute>().ToList();

                // get the [TestMatrix] if it exists
                var matrixAttribute = (TestMatrixAttribute)mi.GetCustomAttributes(typeof(TestMatrixAttribute), false).SingleOrDefault();
                if (matrixAttribute != null)
                {
                    var dimensionAttributes = mi.GetCustomAttributes(typeof(TestMatrixDimensionAttribute), false).Cast<TestMatrixDimensionAttribute>();
                    var constraintsInfo = this.GetConstraintsInfo(mi);
                    variationAttributes.AddRange(this.GenerateTestAttributesFromMatrix<VariationAttribute>(matrixAttribute, dimensionAttributes, constraintsInfo));
                }

                foreach (VariationAttribute varAttr in variationAttributes)
                {
                    VariationTestItem variation = new VariationTestItem(this, mi, varAttr);
                    variation.SetTestItemFilter(this.GetTestItemFilter());

                    foreach (BugAttribute attr in allBugs)
                    {
                        variation.Bugs.Add(attr);
                    }

                    if (matrixAttribute != null)
                    {
                        variation = this.InvokeMatrixCombinationCallback(mi, variation, varAttr);
                        this.AssertTestItemIsNotSkippedWhenExplorationIsNotExhaustive(variation, matrixAttribute);
                    }

                    if (variation != null)
                    {
                        result.Add(variation);
                    }
                }
            }
        }

        private IEnumerable<TAttribute> GenerateTestAttributesFromMatrix<TAttribute>(TestMatrixAttribute matrix, IEnumerable<TestMatrixDimensionAttribute> dimensions, IEnumerable<KeyValuePair<MethodInfo, object>> constraintsInfo)
            where TAttribute : TestItemWithParametersAttribute, new()
        {
            var attributes = new List<TAttribute>();
            int currentIdOffset = 0;
            TestMatrixExplorationKind explorationKind = this.ExplorationKind.HasValue ? this.ExplorationKind.Value : matrix.ExplorationKind;
            foreach (var row in this.GenerateMatrixFromDimensions(dimensions, constraintsInfo, explorationKind))
            {
                string suffix = " - " + string.Join(" ", row.Select(pair => pair.Key + "=" + pair.Value).ToArray());
                int id = matrix.Id;
                if (id > 0)
                {
                    id += currentIdOffset++;
                }

                string name = null;
                if (matrix.Name != null)
                {
                    name = matrix.Name + suffix;
                }

                string description = null;
                if (matrix.Description != null)
                {
                    description = matrix.Description + suffix;
                }

                attributes.Add(new TAttribute()
                {
                    Id = id,
                    Name = name,
                    Description = description,
                    Owner = matrix.Owner,
                    Parameters = row.Select(pair => pair.Value).ToArray(),
                    Priority = matrix.Priority,
                    SkipReason = matrix.SkipReason,
                    SkipUntil = matrix.SkipUntil,
                    Timeout = matrix.Timeout,
                    Version = matrix.Version,
                });
            }

            return attributes;
        }

        private IEnumerable<IEnumerable<KeyValuePair<string, object>>> GenerateMatrixFromDimensions(IEnumerable<TestMatrixDimensionAttribute> dimensions, IEnumerable<KeyValuePair<MethodInfo, object>> constraintsInfo, TestMatrixExplorationKind explorationKind)
        {
            int order = -1;
            switch (explorationKind)
            {
                case TestMatrixExplorationKind.Pairwise:
                    order = 2;
                    break;

                case TestMatrixExplorationKind.Exhaustive:
                    order = dimensions.Count();
                    break;

                default:
                    throw new TaupoInvalidOperationException("Unhandled test matrix exploration kind: " + explorationKind);
            }

            return this.PairwiseGenerateMatrixFromDimensions(dimensions, constraintsInfo, order);
        }

        private IEnumerable<IEnumerable<KeyValuePair<string, object>>> PairwiseGenerateMatrixFromDimensions(IEnumerable<TestMatrixDimensionAttribute> dimensionAttributes, IEnumerable<KeyValuePair<MethodInfo, object>> constraintsInfo, int order)
        {
            var random = new Random(this.ExplorationSeed);
            var dimensionsWithStrategies = dimensionAttributes.OrderBy(a => a.Position).Select(a => this.GetDimensionWithStrategy(a)).ToArray();
            var dimensions = dimensionsWithStrategies.Select(ds => ds.Key).ToArray();
            var matrix = new Matrix(dimensions);

            var constraints = constraintsInfo.Select(ci => (IConstraint)new TestMatrixMethodBasedConstraint(ci.Key, ci.Value, dimensions));
            var pairwise = new PairwiseStrategy(matrix, random.Next, constraints);
            pairwise.Order = order;
            foreach (var ds in dimensionsWithStrategies)
            {
                // Note: strategy can be null for closed domains like bool or enum types
                if (ds.Value != null)
                {
                    pairwise.SetDimensionStrategy(ds.Key, ds.Value);
                }
            }

            var vectors = pairwise.Explore();
            var result = vectors.Select(v => (IEnumerable<KeyValuePair<string, object>>)dimensions.Select(d => new KeyValuePair<string, object>(d.Name, v.GetValue(d))).ToList()).ToList();
            return result;
        }

        private IEnumerable<KeyValuePair<MethodInfo, object>> GetConstraintsInfo(MethodInfo method)
        {
            var constraintAttributes = method.GetCustomAttributes(typeof(TestMatrixConstraintAttribute), false).Cast<TestMatrixConstraintAttribute>();
            return constraintAttributes.Select(a =>
                {
                    var methodInfo = this.GetType().GetMethod(a.ConstraintMethodName, (bool?)null, null);
                    ExceptionUtilities.CheckObjectNotNull(methodInfo, "Failed to create constraint. Could not find method '{0}' on type '{1}'", a.ConstraintMethodName, this.GetType());
                    return new KeyValuePair<MethodInfo, object>(methodInfo, this);
                });
        }

        private IEnumerable<KeyValuePair<MethodInfo, object>> GetConstraintsInfo(Type type)
        {
            var constraintAttributes = type.GetCustomAttributes(typeof(TestMatrixConstraintAttribute), false).Cast<TestMatrixConstraintAttribute>();
            return constraintAttributes.Select(constraintAttr =>
                {
                    object instance = null;
                    Type currentType = type;
                    MethodInfo methodInfo = null;
                    bool completed = false;
                    
                    // First try to find static method on the type or the its derived types
                    do
                    {
                        methodInfo = currentType.GetMethod(constraintAttr.ConstraintMethodName, null, true);

                        currentType = currentType.BaseType;
                        if (methodInfo != null || currentType == null)
                        {
                            completed = true;
                        }
                    }
                    while (!completed);

                    if (methodInfo == null)
                    {
                        // Then try to find method on the current test item
                        methodInfo = this.GetType().GetMethod(constraintAttr.ConstraintMethodName, (bool?)null, null);
                        instance = this;
                    }

                    ExceptionUtilities.CheckObjectNotNull(methodInfo, "Failed to create constraint. Could not find '{0}' method. It should be either static method on type '{1}' or instance or static method on type '{2}'", constraintAttr.ConstraintMethodName, type, this.GetType());

                    return new KeyValuePair<MethodInfo, object>(methodInfo, instance);
                });
        }

        private KeyValuePair<Dimension, IExplorationStrategy> GetDimensionWithStrategy(TestMatrixDimensionAttribute dimensionAttribute)
        {
            Dimension dimension;
            Type domain = dimensionAttribute.Domain ?? typeof(object);
            dimension = (Dimension)typeof(Dimension<>).MakeGenericType(domain).GetInstanceConstructor(true, new Type[] { typeof(string) }).Invoke(new object[] { dimensionAttribute.Name });

            // Note: for closed domains like bool or enum types values don't need to be specified
            // which means such dimensions should be explored exhaustively and we don't need to create strategy in this case
            IExplorationStrategy strategy = null;

            // Setup exhaustive strategy for exploring dimension when values are provided.
            ExceptionUtilities.Assert(dimensionAttribute.Values != null, "Values cannnot be null. Dimension: '{0}'.", dimensionAttribute.Name);
            if (dimensionAttribute.Values.Length > 0)
            {
                var typedValuesArray = Array.CreateInstance(domain, dimensionAttribute.Values.Length);
                dimensionAttribute.Values.CopyTo(typedValuesArray, 0);
                strategy = (IExplorationStrategy)typeof(ExhaustiveIEnumerableStrategy<>).MakeGenericType(domain)
                    .GetInstanceConstructor(true, new Type[] { typedValuesArray.GetType() }).Invoke(new object[] { typedValuesArray });
            }

            return new KeyValuePair<Dimension, IExplorationStrategy>(dimension, strategy);
        }

        private VariationTestItem InvokeMatrixCombinationCallback(MethodInfo method, VariationTestItem variation, VariationAttribute attribute)
        {
            var callbackAttr = (TestMatrixCombinationCallbackAttribute)method.GetCustomAttributes(typeof(TestMatrixCombinationCallbackAttribute), false).SingleOrDefault();
            if (callbackAttr != null)
            {
                var callbackMethod = this.GetType().GetMethod(callbackAttr.CallbackMethodName, true, false);
                ExceptionUtilities.CheckObjectNotNull(callbackMethod, "Could not find instance method '{0}' on type '{1}'", callbackAttr.CallbackMethodName, this.GetType());
                ExceptionUtilities.Assert(typeof(VariationTestItem).IsAssignableFrom(callbackMethod.ReturnType), "Matrix combination callback '{0}' returns unexpected type '{1}'", callbackMethod.Name, callbackMethod.ReturnType);

                var arguments = new object[attribute.Parameters.Length + 1];
                arguments[0] = variation;
                Array.Copy(attribute.Parameters, 0, arguments, 1, attribute.Parameters.Length);

                variation = (VariationTestItem)callbackMethod.Invoke(this, arguments);
            }

            return variation;
        }

        private TestCase InvokeMatrixCombinationCallback(Type type, TestCase testCase, TestCaseAttribute attribute)
        {
            var callbackAttr = (TestMatrixCombinationCallbackAttribute)type.GetCustomAttributes(typeof(TestMatrixCombinationCallbackAttribute), false).SingleOrDefault();
            if (callbackAttr != null)
            {
                var callbackMethod = type.GetMethod(callbackAttr.CallbackMethodName, true, true);
                ExceptionUtilities.CheckObjectNotNull(callbackMethod, "Could not find static method '{0}' on type '{1}'", callbackAttr.CallbackMethodName, type);
                ExceptionUtilities.Assert(typeof(TestCase).IsAssignableFrom(callbackMethod.ReturnType), "Matrix combination callback '{0}' returns unexpected type '{1}'", callbackMethod.Name, callbackMethod.ReturnType);

                var arguments = new object[attribute.Parameters.Length + 1];
                arguments[0] = testCase;
                Array.Copy(attribute.Parameters, 0, arguments, 1, attribute.Parameters.Length);

                testCase = (TestCase)callbackMethod.Invoke(null, arguments);
            }

            return testCase;
        }

        private void AssertTestItemIsNotSkippedWhenExplorationIsNotExhaustive(TestItem testItem, TestMatrixAttribute matrixAttribute)
        {
            // Note: because MatrixCombinationCallback is called on the test item after test matrix generation is complete
            // we cannot incorporate the callback as a constraint into the pairwise generation.
            // Hence we allow callback to return null (i.e. test item is skipped) only for the exhaustive exploration.
            ExceptionUtilities.Assert(
                testItem != null || matrixAttribute.ExplorationKind == TestMatrixExplorationKind.Exhaustive,
                "MatrixCombinationCallback retured null (i.e. skipping test item) - this is only allowed when doing exhaustive test matrix exploration.");
        }
    }
}
