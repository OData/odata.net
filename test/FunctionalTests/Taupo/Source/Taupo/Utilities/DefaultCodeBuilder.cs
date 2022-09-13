//---------------------------------------------------------------------
// <copyright file="DefaultCodeBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    
    /// <summary>
    /// Default implementation of the <see cref="CodeBuilder"/> contract.
    /// </summary>
    [ImplementationName(typeof(CodeBuilder), "Default", HelpText = "Default code builder.")]
    public class DefaultCodeBuilder : CodeBuilder
    {
        private IDependencyInjector injector;
        private IProgrammingLanguageStrategy language;
        private ICollection<string> namespaceImports;
        private ICollection<string> referenceAssemblies;
        private IDictionary<string, ExternalProperty> externalProperties;
        private Dictionary<string, int> nextVariableSuffix = new Dictionary<string, int>();
        private List<BlockInformation> variations = new List<BlockInformation>();
        private BlockInformation currentVariation;
        private Stack<BlockInformation> blockStack = new Stack<BlockInformation>();
        private BlockInformation currentBlock;
        private List<Action<IAsyncContinuation>> compiledActions;
        private List<CodeTypeMember> customMembers = new List<CodeTypeMember>();
        private Assembly compiledAssembly;
        private string generatedCode;

        /// <summary>
        /// Initializes a new instance of the DefaultCodeBuilder class.
        /// </summary>
        /// <param name="dependencyInjector">The dependency injector.</param>
        /// <param name="language">The programming language to use for generating code.</param>
        public DefaultCodeBuilder(
            IDependencyInjector dependencyInjector,
            IProgrammingLanguageStrategy language)
        {
            this.Log = Logger.Null;

            this.injector = dependencyInjector;
            this.language = language;
            this.namespaceImports = new List<string>();
            this.referenceAssemblies = new List<string>();
            this.externalProperties = new Dictionary<string, ExternalProperty>();
        }

        /// <summary>
        /// Types of blocks that statements can be added to the builder.
        /// Used to ensure Begin/End operations on blocks are correctly ordered
        /// </summary>
        private enum BlockType
        {
            Method,
            If,
            Else,
            Lambda,
        }

        /// <summary>
        /// Gets or sets logger to be used for printing diagnostics messages.
        /// </summary>
        [InjectDependency]
        public Logger Log { get; set; }

        /// <summary>
        /// Gets or sets Assertional handler to be used for assert statements.
        /// </summary>
        [InjectDependency]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Resets the state of the code builder and prepares for new code to be built.
        /// </summary>
        public override void Reset()
        {
            this.variations.Clear();
            this.currentVariation = null;

            this.blockStack.Clear();
            this.currentBlock = null;

            this.compiledAssembly = null;
            this.compiledActions = null;

            this.namespaceImports.Clear();
            this.referenceAssemblies.Clear();
            this.customMembers.Clear();
            this.externalProperties.Clear();
        }

        /// <summary>
        /// Adds new custom method to the current code.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <returns>
        /// CodeDOM <see cref="CodeMemberMethod"/> which can be used to customize generated method.
        /// </returns>
        public override CodeMemberMethod BeginCustomMethod(string methodName)
        {
            this.EnsureNotInBlock();

            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = methodName;
            this.customMembers.Add(method);

            this.nextVariableSuffix.Clear();
            this.currentVariation = this.currentBlock = new BlockInformation(methodName, BlockType.Method, method.Statements, false);
            return method;
        }

        /// <summary>
        /// Ends the current custom method.
        /// </summary>
        public override void EndCustomMethod()
        {
            this.EnsureInBlock(BlockType.Method);

            this.currentVariation = null;
            this.currentBlock = null;
        }

        /// <summary>
        /// Begins new synchronous variation.
        /// </summary>
        /// <param name="name">The name of the variation (will be the name of the method in the generated code).</param>
        public override void BeginVariation(string name)
        {
            this.EnsureNotInBlock();

            this.nextVariableSuffix.Clear();
            this.currentVariation = this.currentBlock = new BlockInformation(name, BlockType.Method, new CodeStatementCollection(), false);
        }

        /// <summary>
        /// Begins the asynchronous variation.
        /// </summary>
        /// <param name="name">The name of the variation (will be the name of the method in the generated code).</param>
        /// <returns>
        /// CodeDom <see cref="CodeExpression"/> which can be used to refer to asynchronous continuation parameter in the generated code.
        /// </returns>
        public override CodeExpression BeginAsyncVariation(string name)
        {
            this.EnsureNotInBlock();

            this.nextVariableSuffix.Clear();

            this.currentVariation = this.currentBlock = new BlockInformation(name, BlockType.Method, new CodeStatementCollection(), true);

            return Code.Argument("continuation");
        }

        /// <summary>
        /// Ends current variation.
        /// </summary>
        public override void EndVariation()
        {
            this.EnsureInBlock(BlockType.Method);
            this.variations.Add(this.currentVariation);
            this.currentVariation = null;
            this.currentBlock = null;
        }

        /// <summary>
        /// Declares a new variable of the specified type
        /// </summary>
        /// <param name="variableName">Human readable text to be included in variable name</param>
        /// <param name="type">Type of the variable</param>
        /// <param name="initExpression">Expression to assign to the variable</param>
        /// <returns>Reference to the created variable</returns>
        public override CodeExpression Declare(string variableName, CodeTypeReference type, CodeExpression initExpression)
        {
            string name = this.GetUniqueVariableName(variableName);
            this.currentBlock.Statements.Add(new CodeVariableDeclarationStatement(type, name, initExpression));
            return Code.Variable(name);
        }

        /// <summary>
        /// Adds the namespace import to the current code.
        /// </summary>
        /// <param name="namespaceName">Name of the namespace.</param>
        public override void AddNamespaceImport(string namespaceName)
        {
            if (!this.namespaceImports.Contains(namespaceName))
            {
                this.namespaceImports.Add(namespaceName);
            }
        }

        /// <summary>
        /// Adds the referenced assembly to the current code.
        /// </summary>
        /// <param name="referencedAssembly">The referenced assembly.</param>
        /// <remarks>Referenced assembly must be unqualified for system assemblies</remarks>
        public override void AddReferencedAssembly(string referencedAssembly)
        {
            if (!this.referenceAssemblies.Contains(referencedAssembly))
            {
                this.referenceAssemblies.Add(referencedAssembly);
            }
        }

        /// <summary>
        /// Adds the external property to the current code.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="propertyValue">The initial property value.</param>
        /// <returns>
        /// CodeDOM <see cref="CodeExpression"/> which can be used to refer to the property in the generated code.
        /// </returns>
        public override CodeExpression AddExternalProperty(string propertyName, CodeTypeReference propertyType, object propertyValue)
        {
            this.externalProperties[propertyName] = new ExternalProperty(propertyName, propertyType, propertyValue);
            return Code.This().Property(propertyName);
        }

        /// <summary>
        /// Adds the specified statement to current scope.
        /// </summary>
        /// <param name="statement">The statement to add.</param>
        public override void Add(CodeStatement statement)
        {
            this.EnsureInBlock();
            this.currentBlock.Statements.Add(statement);
        }

        /// <summary>
        /// Adds the conditional statement to the current scope and enters "then" scope.
        /// </summary>
        /// <param name="condition">The conditional expression.</param>
        public override void If(CodeExpression condition)
        {
            var statement = new CodeConditionStatement(condition);
            this.Add(statement);
            this.blockStack.Push(this.currentBlock);
            this.blockStack.Push(new BlockInformation(null, BlockType.Else, statement.FalseStatements, false));
            this.currentBlock = new BlockInformation(null, BlockType.If, statement.TrueStatements, false);
        }

        /// <summary>
        /// Exits current "then" scope and enters the "else" scope of current conditional statement.
        /// </summary>
        public override void Else()
        {
            this.EnsureInBlock(BlockType.If);
            this.currentBlock = this.blockStack.Pop();
        }

        /// <summary>
        /// Ends current conditional statement scope ("then" or "else") and moves back to the previous scope.
        /// </summary>
        public override void EndIf()
        {
            this.EnsureInBlock(BlockType.If, BlockType.Else);

            this.currentBlock = this.blockStack.Pop();
            if (this.currentBlock.BlockType == BlockType.Else)
            {
                this.currentBlock = this.blockStack.Pop();
            }
        }

        /// <summary>
        /// Begins the new scope which will add statements to given lambda body.
        /// </summary>
        /// <param name="lambda">The lambda.</param>
        public override void BeginLambda(CodeLambdaExpression lambda)
        {
            this.blockStack.Push(this.currentBlock);
            this.currentBlock = new BlockInformation(null, BlockType.Lambda, lambda.BodyStatements, false);
        }

        /// <summary>
        /// Ends the lambda scope.
        /// </summary>
        public override void EndLambda()
        {
            this.EnsureInBlock(BlockType.Lambda);

            this.currentBlock = this.blockStack.Pop();
        }

        /// <summary>
        /// Runs all variations synchronously.
        /// </summary>
        public override void RunVariations()
        {
            if (this.variations.Any(bi => bi.IsAsynchronous))
            {
                throw new TaupoInvalidOperationException("Cannot execute variations synchronously because at least one asynchronous variation was added.");
            }

            Exception exception = null;

            this.RunVariations(
                AsyncHelpers.CreateContinuation(
                    () => { },
                    ex => exception = ex));

            if (exception != null)
            {
                throw new TaupoInvalidOperationException("Variation execution failed", exception);
            }
        }

        /// <summary>
        /// Runs all this.variations and executes the specified continuation at the end.
        /// </summary>
        /// <param name="continuation">The continuation to execute at the end of execution.</param>
        public override void RunVariations(IAsyncContinuation continuation)
        {
            // Build CodeDOM tree
            var codeUnit = this.BuildCompiledCodeUnit();

            // Generate code
            ExtendedCodeGenerator generator = this.language.CreateCodeGenerator();
            var options = new SafeCodeGeneratorOptions()
            {
                BracingStyle = "C",
                IndentString = "    ",
            };

            this.generatedCode = generator.GenerateCodeFromCompileUnit(codeUnit, options);
            this.Log.WriteLine(LogLevel.Trace, this.generatedCode);

            this.CompileAndRunVariations(continuation, this.generatedCode);
        }

        /// <summary>
        /// Generates a variable name that is unique for the current variation
        /// </summary>
        /// <param name="variableName">Human readable text to be included in variable name</param>
        /// <returns>String containing the variable name</returns>
        public override string GetUniqueVariableName(string variableName)
        {
            string name;
            int suffix;
            if (this.nextVariableSuffix.TryGetValue(variableName, out suffix))
            {
                name = string.Format(CultureInfo.InvariantCulture, "{0}{1}", variableName, suffix);
                this.nextVariableSuffix[variableName] = ++suffix;
            }
            else
            {
                name = variableName;
                this.nextVariableSuffix.Add(variableName, 1);
            }

            return name;
        }

        /// <summary>
        /// Resolves the reference paths to have fully qualified locations
        /// </summary>
        /// <param name="combinedReferenceAssemblies">The list of assembly references to Taupo assemblies</param>
        /// <returns>The resolved reference paths</returns>
        [SecuritySafeCritical]
        public IEnumerable<string> ResolveTaupoReferencePaths(IEnumerable<string> combinedReferenceAssemblies)
        {
            string currentAssemblyLocation = Path.GetDirectoryName(this.GetAssemblyLocation());
            List<string> resolvedPaths = new List<string>();
            foreach (var assemblyReference in combinedReferenceAssemblies)
            {
                if (!assemblyReference.StartsWith("Microsoft.Test.Taupo", StringComparison.OrdinalIgnoreCase))
                {
                    resolvedPaths.Add(assemblyReference);
                }
                else
                {
                    resolvedPaths.Add(Path.Combine(currentAssemblyLocation, assemblyReference));
                }
            }

            return resolvedPaths.AsEnumerable();
        }

        /// <summary>
        /// Compiles and runs variations asynchronously
        /// </summary>
        /// <param name="continuation">The continuation.</param>
        /// <param name="generatedCodePreview">The preview of generated code.</param>
        /// <remarks>This method is virtual for testability.</remarks>
        protected virtual void CompileAndRunVariations(IAsyncContinuation continuation, string generatedCodePreview)
        {
            AsyncHelpers.RunActionSequence(continuation, this.CompileVariations, this.PrepareCompiledVariations, this.RunCompiledVariations);
        }

        /// <summary>
        /// Compiles all this.variations that have been finalized
        /// </summary>
        /// <param name="continuation">The continuation to invoke once the variations have been compiled.</param>
        private void CompileVariations(IAsyncContinuation continuation)
        {
            ExceptionUtilities.Assert(this.generatedCode != null, "generatedCode must be not null at this stage.");
            this.CompileCodeUnitAsync(continuation);
        }

        /// <summary>
        /// Gets the location of the Microsoft.Test.Taupo assembly.
        /// </summary>
        /// <returns>The path to where the assembly is located.</returns>
        [SecurityCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Calling Assembly.Location demands FileIOPermission (PathDiscovery flag) to that path.")]
        private string GetAssemblyLocation()
        {
            var assembly = this.GetType().GetAssembly();
            return assembly.Location;
        }

        /// <summary>
        /// Prepares compiled variations for execution.
        /// </summary>
        /// <param name="continuation">The continuation to invoke once the variations have been prepared.</param>
        private void PrepareCompiledVariations(IAsyncContinuation continuation)
        {
            ExceptionUtilities.Assert(this.compiledAssembly != null, "compiledAssembly must be not null at this stage.");

            // Load the compiled code
            var testClass = this.compiledAssembly.GetType("Tests.TestClass");
            var testClassInstance = Activator.CreateInstance(testClass);
            this.injector.InjectDependenciesInto(testClassInstance);

            foreach (var externalProperty in this.externalProperties.Values)
            {
                PropertyInfo propertyInfo = testClass.GetProperty(externalProperty.PropertyName);
                propertyInfo.SetValue(testClassInstance, externalProperty.InitialValue, null);
            }

            // Build an action for each variation
            var actions = new List<Action<IAsyncContinuation>>();
            foreach (var v in this.variations)
            {
                var testMethod = testClass.GetMethod(v.BlockName);
                var methodParams = testMethod.GetParameters();
                if (methodParams.Length > 0 && methodParams[0].ParameterType == typeof(IAsyncContinuation))
                {
                    var asyncAction = (Action<IAsyncContinuation>)testMethod.CreateDelegate(typeof(Action<IAsyncContinuation>), testClassInstance);
                    actions.Add(asyncAction);
                }
                else
                {
                    Action syncAction = (Action)testMethod.CreateDelegate(typeof(Action), testClassInstance);
                    Action<IAsyncContinuation> asyncAction = c => AsyncHelpers.TryCatch(c, syncAction);
                    actions.Add(asyncAction);
                }
            }

            this.compiledActions = actions;
            continuation.Continue();
        }

        /// <summary>
        /// Runs the compiled variations in a sequence.
        /// </summary>
        /// <param name="continuation">The continuation to invoke once the variations have completed.</param>
        private void RunCompiledVariations(IAsyncContinuation continuation)
        {
            ExceptionUtilities.Assert(this.compiledActions != null, "compiledActions must be not null at this stage.");

            AsyncHelpers.RunActionSequence(continuation, this.compiledActions);
        }

        /// <summary>
        /// Builds a <see cref="CodeCompileUnit"/> containing all the compiled variations
        /// </summary>
        /// <returns>Code unit containing a single class (Tests.TestClass) with a method for each variation</returns>
        private CodeCompileUnit BuildCompiledCodeUnit()
        {
            var ccu = new CodeCompileUnit();
            var ns = ccu.AddNamespace("Tests");

            foreach (var import in this.namespaceImports)
            {
                ns.ImportNamespace(import);
            }

            var codeClass = ns.DeclareType("TestClass").InheritsFrom(Code.TypeRef<GeneratedCode>());

            foreach (var externalProperty in this.externalProperties.Values)
            {
                codeClass.AddAutoImplementedProperty(externalProperty.PropertyType, externalProperty.PropertyName);
            }

            // Add variations to class
            foreach (var v in this.variations)
            {
                CodeMemberMethod codeVariation = codeClass.AddMethod(v.BlockName);

                if (v.IsAsynchronous)
                {
                    codeVariation = codeVariation.WithArgument(Code.TypeRef<IAsyncContinuation>(), "continuation");
                }

                codeVariation.Statements.AddRange(v.Statements);
            }

            codeClass.Members.AddRange(this.customMembers.ToArray());

            return ccu;
        }

        [SecuritySafeCritical]
        private void CompileCodeUnitAsync(IAsyncContinuation continuation)
        {
            string assemblyName = "CB" + Guid.NewGuid().ToString("N");
            var combinedReferenceAssemblies = new List<string>();

            // add assemblies specified by the user
            combinedReferenceAssemblies.AddRange(this.referenceAssemblies);

            // fix paths for assembly references 
            combinedReferenceAssemblies = this.ResolveTaupoReferencePaths(combinedReferenceAssemblies).ToList();

            // get Taupo assembly location
            string taupoAssemblyLocation = this.GetAssemblyLocation();

            // only add Taupo assembly if it is not added before
            if (!combinedReferenceAssemblies.Contains(Path.GetFileName(taupoAssemblyLocation)))
            {
                combinedReferenceAssemblies.Add(taupoAssemblyLocation);
            }

            this.language.CompileAssemblyAsync(
                assemblyName,
                new[] { this.generatedCode },
                combinedReferenceAssemblies.ToArray(),
                (asm, error) =>
                {
                    if (error != null)
                    {
                        continuation.Fail(error);
                    }
                    else
                    {
                        this.compiledAssembly = asm;
                        continuation.Continue();
                    }
                });
        }

        private void EnsureInBlock(params BlockType[] blockTypes)
        {
            if (this.currentBlock == null)
            {
                throw new TaupoInvalidOperationException("There is no code block currently opened.");
            }

            if (blockTypes.Length > 0 && blockTypes.All(bt => this.currentBlock.BlockType != bt))
            {
                throw new TaupoInvalidOperationException("This operation requires an open " + string.Join(" or ", blockTypes.Select(c => c.ToString()).ToArray()) + " but current block is " + this.currentBlock.BlockType + ".");
            }
        }

        private void EnsureNotInBlock()
        {
            if (this.currentBlock != null)
            {
                throw new TaupoInvalidOperationException("There is a code block currently opened (" + this.currentBlock.BlockType + "). You must end it before opening a new one.");
            }
        }

        /// <summary>
        /// Information about block of code (scope).
        /// </summary>
        private class BlockInformation
        {
            /// <summary>
            /// Initializes a new instance of the BlockInformation class.
            /// </summary>
            /// <param name="blockName">Name of the block (only variations have names).</param>
            /// <param name="blockType">Type of the block.</param>
            /// <param name="blockStatements">The statements in the block.</param>
            /// <param name="isAsynchronous">If set to <c>true</c> the block is asynchronous.</param>
            public BlockInformation(string blockName, BlockType blockType, CodeStatementCollection blockStatements, bool isAsynchronous)
            {
                this.BlockName = blockName;
                this.BlockType = blockType;
                this.Statements = blockStatements;
                this.IsAsynchronous = isAsynchronous;
            }

            /// <summary>
            /// Gets the name of the block.
            /// </summary>
            /// <value>The name of the block.</value>
            public string BlockName { get; private set; }

            /// <summary>
            /// Gets the statements.
            /// </summary>
            /// <value>The statements.</value>
            public CodeStatementCollection Statements { get; private set; }

            /// <summary>
            /// Gets the type of the block.
            /// </summary>
            public BlockType BlockType { get; private set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is asynchronous.
            /// </summary>
            public bool IsAsynchronous { get; set; }
        }

        /// <summary>
        /// Information about external propery passed to the generated code.
        /// </summary>
        private class ExternalProperty
        {
            /// <summary>
            /// Initializes a new instance of the ExternalProperty class.
            /// </summary>
            /// <param name="propertyName">Name of the property.</param>
            /// <param name="propertyType">Type of the property.</param>
            /// <param name="initialValue">The initial value of the property.</param>
            public ExternalProperty(string propertyName, CodeTypeReference propertyType, object initialValue)
            {
                this.PropertyName = propertyName;
                this.PropertyType = propertyType;
                this.InitialValue = initialValue;
            }

            /// <summary>
            /// Gets the name of the property.
            /// </summary>
            /// <value>The name of the property.</value>
            public string PropertyName { get; private set; }

            /// <summary>
            /// Gets the type of the property.
            /// </summary>
            /// <value>The type of the property.</value>
            public CodeTypeReference PropertyType { get; private set; }

            /// <summary>
            /// Gets the initial value of the property.
            /// </summary>
            /// <value>The initial value.</value>
            public object InitialValue { get; private set; }
        }
    }
}