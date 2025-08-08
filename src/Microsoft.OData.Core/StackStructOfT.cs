//---------------------------------------------------------------------
// <copyright file="StackStructOfT.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Core;

namespace Microsoft.OData;

/// <summary>
/// A lightweight, struct-based generic stack implementation for value and reference types.
/// Provides efficient push, pop, and peek operations with dynamic resizing.
/// </summary>
/// <typeparam name="T">The type of elements in the stack.</typeparam>
internal struct StackStruct<T>
{
    private T[] _items;
    private int _count;
    private const int DefaultCapacity = 4;

    /// <summary>
    /// Initializes a new instance of the <see cref="StackStruct{T}"/> struct with the default capacity.
    /// </summary>
    public StackStruct()
    {
        _items = new T[DefaultCapacity];
        _count = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StackStruct{T}"/> struct with the specified capacity.
    /// </summary>
    /// <param name="capacity">The initial number of elements the stack can contain.</param>
    public StackStruct(int capacity)
    {
        _items = new T[capacity];
        _count = 0;
    }

    /// <summary>
    /// Pushes an item onto the top of the stack.
    /// </summary>
    /// <param name="item">The item to push onto the stack.</param>
    public void Push(T item)
    {
        EnsureCapacity();
        _items[_count++] = item;
    }

    /// <summary>
    /// Removes and returns the item at the top of the stack.
    /// </summary>
    /// <returns>The item removed from the top of the stack.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the stack is empty.</exception>
    public T Pop()
    {
        ThrowIfNullOrEmpty();
        return _items[--_count];
    }

    /// <summary>
    /// Returns the item at the top of the stack without removing it.
    /// </summary>
    /// <returns>The item at the top of the stack.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the stack is empty.</exception>
    public T Peek()
    {
        ThrowIfNullOrEmpty();
        return _items[_count - 1];
    }

    /// <summary>
    /// Gets a value indicating whether the stack is empty.
    /// </summary>
    public bool IsEmpty => _count == 0;

    /// <summary>
    /// Gets the number of elements contained in the stack.
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Determines whether the current stack is equal to another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current stack.</param>
    /// <returns>true if the specified object is a <see cref="StackStruct{T}"/> and is equal to the current stack; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
        if (obj is not StackStruct<T> other)
        {
            return false;
        }

        return Equals(other);
    }

    /// <summary>
    /// Determines whether the current stack is equal to another <see cref="StackStruct{T}"/>.
    /// Two stacks are equal if they have the same count and all elements are equal in order.
    /// </summary>
    /// <param name="other">The stack to compare with the current stack.</param>
    /// <returns>true if the stacks are equal; otherwise, false.</returns>
    public readonly bool Equals(StackStruct<T> other)
    {
        if (_count != other._count)
            return false;

        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < _count; i++)
        {
            if (!comparer.Equals(_items[i], other._items[i]))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Returns a hash code for the current stack.
    /// </summary>
    /// <returns>A hash code for the current stack.</returns>
    public override int GetHashCode()
    {
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        int hash = 17;
        hash = hash * 31 + _count.GetHashCode();
        for (int i = 0; i < _count; i++)
        {
            hash = hash * 31 + comparer.GetHashCode(_items[i]);
        }
        return hash;
    }

    /// <summary>
    /// Determines whether two <see cref="StackStruct{T}"/> instances are equal.
    /// </summary>
    /// <param name="left">The first stack to compare.</param>
    /// <param name="right">The second stack to compare.</param>
    /// <returns>true if the stacks are equal; otherwise, false.</returns>
    public static bool operator ==(StackStruct<T> left, StackStruct<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="StackStruct{T}"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first stack to compare.</param>
    /// <param name="right">The second stack to compare.</param>
    /// <returns>true if the stacks are not equal; otherwise, false.</returns>
    public static bool operator !=(StackStruct<T> left, StackStruct<T> right)
    {
        return !(left == right);
    }

    private void EnsureCapacity()
    {
        if (_count == _items.Length)
        {
            Array.Resize(ref _items, _items.Length * 2);
        }
    }

    private readonly void ThrowIfNullOrEmpty()
    {
        if (_count == 0)
        {
            throw new InvalidOperationException(Error.Format(SRResources.ExceptionUtils_IsNullOrEmpty, "StackStruct"));
        }
    }
}
