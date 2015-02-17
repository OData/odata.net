//---------------------------------------------------------------------
// <copyright file="DefaultPayloadTransformFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Default payload transform factory for generating payload transform instances.
    /// </summary>
    [ImplementationName(typeof(IPayloadTransformFactory), "Default", HelpText = "Gets payload transformation class instances if the injected test parameter value is true.")]
    public class DefaultPayloadTransformFactory : IPayloadTransformFactory
    {
        /// <summary>
        /// Stack of payload transform scopes.
        /// </summary>
        private readonly Stack<TransformationScope> scopeStack = new Stack<TransformationScope>();
        
        /// <summary>
        /// Default set of payload transformation wrappers.
        /// </summary>
        private readonly TransformationPerTypeMap defaultTransformations = new TransformationPerTypeMap();
        
        /// <summary>
        /// Checks if the default payload transformation wrapper set is initialized or not.
        /// </summary>
        private bool initialized = false;

        /// <summary>
        /// Initializes a new instance of the DefaultPayloadTransformFactory class 
        /// and sets the injected test parameter which specifies whether transforms are applied or not. 
        /// </summary>
        public DefaultPayloadTransformFactory()
        {
            this.ApplyTransform = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Transforms will be applied or not.
        /// </summary>
        [InjectTestParameter("ApplyTransform", DefaultValueDescription = "true", HelpText = "Specifies if the Transforms need to be applied or not.")]
        public bool ApplyTransform { get; set; }

        /// <summary>
        /// Gets or sets injected test parameter AddXMLCommentsPayloadTransform.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AddXmlCommentsPayloadTransform AddXmlCommentsPayloadTransform { get; set; }

        /// <summary>
        /// Gets or sets injected test parameter ChangeXmlNamespaceTransform.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ChangeXmlNamespaceTransform ChangeXmlNamespaceTransform { get; set; }

        /// <summary>
        /// Gets Transforms of payload element type (TPayload). The retrieval of Transforms depends on an injected test parameter value.
        /// </summary>
        /// <typeparam name="TPayload">Type of payload element.</typeparam>
        /// <returns>Composite transform of all the transform instances.</returns>
        public IPayloadTransform<TPayload> GetTransform<TPayload>()
        {
            var transforms = new List<IPayloadTransform<TPayload>>();

            if (this.ApplyTransform)
            {
                transforms.AddRange(this.GetCurrentTransforms().Get<TPayload>());
            }

            return new CompositePayloadTransform<TPayload>(transforms.ToArray());
        }

        /// <summary>
        /// Gets a scope that can be used to modify what transforms are returned by the factory in a way that does not corrupt the factory itself.
        /// </summary>
        /// <param name="empty">A value indicating whether the scope should start out empty. If false, the current set of default transformations will be used.</param>
        /// <returns>A scope which can be used to fluently add/remove transforms without modifying the factory's long-term state.</returns>
        public IPayloadTransformationScope GetScope(bool empty)
        {
            var initialSet = new TransformationPerTypeMap();
            if (!empty)
            {
                initialSet.AddRange(this.GetCurrentTransforms());
            }

            return new TransformationScope(initialSet, this.ApplyScope);
        }

        /// <summary>
        /// Gets current transforms either from the scope stack or from the default set of transforms.
        /// </summary>
        /// <returns>Set of wrapped transforms.</returns>
        private TransformationPerTypeMap GetCurrentTransforms()
        {
            this.Initialize();

            TransformationPerTypeMap wrappers;
            if (this.scopeStack.Count > 0)
            {
                wrappers = this.scopeStack.Peek().Transforms;
            }
            else
            {
                wrappers = this.defaultTransformations;
            }

            return wrappers;
        }

        /// <summary>
        /// Applies the passed scope onto the scope stack.
        /// </summary>
        /// <param name="scope">Scope to be applied.</param>
        /// <returns>An IDisposable object which helps in applying a scope and then disposing it once used. Used to maintain a clean factory state.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Method returns IDisposable")]
        private IDisposable ApplyScope(TransformationScope scope)
        {
            lock (this.scopeStack)
            {
                this.scopeStack.Push(scope);
                return new DelegateBasedDisposable(() => this.ExitScope(scope));
            }
        }

        /// <summary>
        /// Removes the scope from the scope stack.
        /// </summary>
        /// <param name="scope">Scope to be removed.</param>
        private void ExitScope(TransformationScope scope)
        {
            lock (this.scopeStack)
            {
                ExceptionUtilities.Assert(object.ReferenceEquals(scope, this.scopeStack.Peek()), "Wrong scope given when exiting.");
                this.scopeStack.Pop();
            }
        }

        /// <summary>
        /// Initializes default set of transforms the first time. 
        /// </summary>
        private void Initialize()
        {
            if (!this.initialized)
            {
                this.GetDefaultTransforms().ForEach(w => this.defaultTransformations.Add(w));
                this.initialized = true;
            }
        }

        /// <summary>
        /// Gets the default set of transforms using the inject transform properties.
        /// </summary>
        /// <returns>List of default transforms.</returns>
        private IEnumerable<TransformationWrapper> GetDefaultTransforms()
        {
            // TODO: Consider adding support to the dependency injector to say 'give me ALL the instances of some type'. 
            List<TransformationWrapper> wrappers = new List<TransformationWrapper>();
            var createMethod = typeof(TransformationWrapper).GetMethod("Create", false, true);

            foreach (var property in this.GetType().GetProperties().OrderBy(p => p.Name))
            {
                var genericArgs = property.PropertyType.GetInterfaces().First().GetGenericArguments();

                if (genericArgs.Length > 0)
                {
                    var payloadType = genericArgs[0];
                    var transform = property.GetValue(this, null);
                    var wrapper = (TransformationWrapper)createMethod.MakeGenericMethod(payloadType).Invoke(null, new object[] { transform });
                    wrappers.Add(wrapper);
                }
            }

            return wrappers;
        }

        /// <summary>
        /// Private scope implementation that implements run by modifying the state of the factory.
        /// </summary>
        private class TransformationScope : IPayloadTransformationScope
        {
            /// <summary>
            /// Used for running a given action on a scope.
            /// </summary>
            private readonly Func<TransformationScope, IDisposable> onApply;

            /// <summary>
            /// Initializes a new instance of the TransformationScope class.
            /// </summary>
            /// <param name="initialTransforms">The initial set of transforms.</param>
            /// <param name="onApply">The function to run on applying the scope.</param>
            internal TransformationScope(TransformationPerTypeMap initialTransforms, Func<TransformationScope, IDisposable> onApply)
            {
                this.Transforms = new TransformationPerTypeMap();
                this.Transforms.AddRange(initialTransforms);
                this.onApply = onApply;
            }

            /// <summary>
            /// Gets transforms associated with this scope.
            /// </summary>
            internal TransformationPerTypeMap Transforms { get; private set; }

            /// <summary>
            /// Adds the transform to the scope.
            /// </summary>
            /// <typeparam name="TPayload">The payload type for the transform.</typeparam>
            /// <param name="transform">The transform to add.</param>
            public void Add<TPayload>(IPayloadTransform<TPayload> transform)
            {
                this.Transforms.Add(TransformationWrapper.Create(transform));
            }

            /// <summary>
            /// Removes matching transforms from the scope.
            /// </summary>
            /// <typeparam name="TPayload">The payload type for the transforms.</typeparam>
            /// <param name="predicate">The predicate to use to find matching transforms.</param>
            public void RemoveAll<TPayload>(Predicate<IPayloadTransform<TPayload>> predicate)
            {
                this.Transforms.RemoveAll(predicate);
            }

            /// <summary>
            /// Runs the given action in the current scope.
            /// </summary>
            /// <returns>An IDisposable object which helps in applying a scope and then disposing it once used.</returns>
            public IDisposable Apply()
            {
                return this.onApply(this);
            }
        }

        /// <summary>
        /// Helper class for handling a group of transforms across multiple payload types.
        /// </summary>
        private class TransformationPerTypeMap
        {
            /// <summary>
            /// Current list of transforms.
            /// </summary>
            private readonly IDictionary<Type, IList<TransformationWrapper>> transforms = new Dictionary<Type, IList<TransformationWrapper>>();

            /// <summary>
            /// Adds the transformation wrapper to the current set.
            /// </summary>
            /// <param name="transform">The transform wrapper to add.</param>
            internal void Add(TransformationWrapper transform)
            {
                this.GetForType(transform.PayloadType).Add(transform);
            }

            /// <summary>
            /// Adds the transformations in the given set to the current set.
            /// </summary>
            /// <param name="transformSet">The transforms to add.</param>
            internal void AddRange(TransformationPerTypeMap transformSet)
            {
                foreach (var pair in transformSet.transforms)
                {
                    var withSameType = this.GetForType(pair.Key);
                    withSameType.AddRange(pair.Value);
                }
            }

            /// <summary>
            /// Gets the transforms that operate on the given type from the current set.
            /// </summary>
            /// <typeparam name="TPayload">The payload type.</typeparam>
            /// <returns>The transforms that operate on the given payload type.</returns>
            internal IEnumerable<IPayloadTransform<TPayload>> Get<TPayload>()
            {
                return this.GetForType(typeof(TPayload)).Select(w => w.Unwrap<TPayload>());
            }

            /// <summary>
            /// Removes matching transforms.
            /// </summary>
            /// <typeparam name="TPayload">The payload type for the transforms.</typeparam>
            /// <param name="predicate">The predicate to use to find matching transforms.</param>
            internal void RemoveAll<TPayload>(Predicate<IPayloadTransform<TPayload>> predicate)
            {
                this.GetForType(typeof(TPayload)).RemoveAll(w => predicate(w.Unwrap<TPayload>()));
            }

            /// <summary>
            /// Gets a list transformation wrappers for the specified type.
            /// </summary>
            /// <param name="type">Type for which wrappers need to be returned.</param>
            /// <returns>List of transformation wrappers.</returns>
            private IList<TransformationWrapper> GetForType(Type type)
            {
                IList<TransformationWrapper> withSameType;
                if (!this.transforms.TryGetValue(type, out withSameType))
                {
                    this.transforms[type] = withSameType = new List<TransformationWrapper>();
                }

                return withSameType;
            }
        }

        /// <summary>
        /// Helper class for dealing with the generic transform instances uniformly.
        /// </summary>
        private class TransformationWrapper
        {
            /// <summary>
            /// Holds a payload transform for a wrapper.
            /// </summary>
            private readonly object transform;

            /// <summary>
            /// Initializes a new instance of the TransformationWrapper class and prevents construction outside the Create method.
            /// </summary>
            /// <param name="transform">The transform instance.</param>
            /// <param name="payloadType">The payload type the transform goes with.</param>
            private TransformationWrapper(object transform, Type payloadType)
            {
                this.transform = transform;
                this.PayloadType = payloadType;
            }

            /// <summary>
            /// Gets the payload type the transformation operates on.
            /// </summary>
            internal Type PayloadType { get; private set; }

            /// <summary>
            /// Creates a wrapper for the given transform.
            /// </summary>
            /// <typeparam name="TPayload">The payload type for the transform.</typeparam>
            /// <param name="transform">The transform to wrap.</param>
            /// <returns>The wrapped transform.</returns>
            internal static TransformationWrapper Create<TPayload>(IPayloadTransform<TPayload> transform)
            {
                ExceptionUtilities.CheckArgumentNotNull(transform, "transform");
                return new TransformationWrapper(transform, typeof(TPayload));
            }

            /// <summary>
            /// Unwraps the given transform assuming that the given type matches the transforms type.
            /// </summary>
            /// <typeparam name="TPayload">The expected payload type.</typeparam>
            /// <returns>The unwrapped transform.</returns>
            internal IPayloadTransform<TPayload> Unwrap<TPayload>()
            {
                ExceptionUtilities.Assert(typeof(TPayload) == this.PayloadType, "Wrong payload type given. Expected '{0}', actual '{1}'", this.PayloadType, typeof(TPayload));
                return (IPayloadTransform<TPayload>)this.transform;
            }
        }
    }
}