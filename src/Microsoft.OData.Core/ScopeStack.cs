using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData
{
    /// <summary>
    /// Lightweight wrapper for the stack of scopes which exposes a few helper properties for getting parent scopes.
    /// </summary>
    internal sealed class ScopeStack<TScope>  where TScope : class
    {
        /// <summary>
        /// Use a list to store the scopes instead of a true stack so that parent/grandparent lookups will be fast.
        /// </summary>
        private readonly List<TScope> scopes = new List<TScope>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeStack<typeparamref name="TScope"/>"/> class.
        /// </summary>
        internal ScopeStack()
        {
        }

        /// <summary>
        /// Gets the count of items in the stack.
        /// </summary>
        internal int Count
        {
            get
            {
                return this.scopes.Count;
            }
        }

        /// <summary>
        /// Gets the scope below the current scope on top of the stack.
        /// </summary>
        internal TScope Parent
        {
            get
            {
                Debug.Assert(this.scopes.Count > 1, "this.scopes.Count > 1");
                return this.scopes[this.scopes.Count - 2];
            }
        }

        /// <summary>
        /// Gets the scope below the parent of the current scope on top of the stack.
        /// </summary>
        internal TScope ParentOfParent
        {
            get
            {
                Debug.Assert(this.scopes.Count > 2, "this.scopes.Count > 2");
                return this.scopes[this.scopes.Count - 3];
            }
        }

        /// <summary>
        /// Gets the scope below the current scope on top of the stack or null if there is only one item on the stack or the stack is empty.
        /// </summary>
        internal TScope ParentOrNull
        {
            get
            {
                return this.Count == 0 ? null : this.Parent;
            }
        }

        /// <summary>
        /// Pushes the specified scope onto the stack.
        /// </summary>
        /// <param name="scope">The scope.</param>
        internal void Push(TScope scope)
        {
            Debug.Assert(scope != null, "scope != null");
            this.scopes.Add(scope);
        }

        /// <summary>
        /// Pops the current scope off the stack.
        /// </summary>
        /// <returns>The popped scope.</returns>
        internal TScope Pop()
        {
            Debug.Assert(this.scopes.Count > 0, "this.scopes.Count > 0");
            int last = this.scopes.Count - 1;
            TScope current = this.scopes[last];
            this.scopes.RemoveAt(last);
            return current;
        }

        /// <summary>
        /// Peeks at the current scope on the top of the stack.
        /// </summary>
        /// <returns>The current scope at the top of the stack.</returns>
        internal TScope Peek()
        {
            Debug.Assert(this.scopes.Count > 0, "this.scopes.Count > 0");
            return this.scopes[this.scopes.Count - 1];
        }

        internal TScope FirstOrDefault()
        {
            if (this.scopes.Count > 0)
            {
                return this.Peek();
            }

            return null;
        }

        /// <summary>
        /// Seek scope in the stack which is type of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="TTarget">The type of scope to seek.</typeparam>
        /// <param name="maxDepth">The max depth to seek.</param>
        /// <returns>The scope with type of <typeparamref name="T"/></returns>
        internal TTarget SeekScope<TTarget>(int maxDepth) where TTarget : TScope
        {
            int count = 1;
            for (int i = this.scopes.Count - 1; i >= 0; i--)
            {
                if (count > maxDepth)
                {
                    return default;
                }

                if (this.scopes[i] is TTarget targetScope)
                {
                    return targetScope;
                }

                count++;
            }

            return default;
        }
    }
}
