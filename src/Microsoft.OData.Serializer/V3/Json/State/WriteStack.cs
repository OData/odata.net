using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.State;

// TODO: should this be a struct?
internal class WriteStack<TCustomState>
{
    // TODO: Don't create stack array if we only have one frame
    //private WriteStackFrame _current;
    private WriteStackFrame<TCustomState>[]? _stack;

    /// <summary>
    /// The number of frames currently in the stack.
    /// But since the stack can be pushed and popped without losing state,
    /// this count represents the current depth of the stack. Its always
    /// updated when the stack is pushed or popped.
    /// </summary>
    private int _count;

    /// <summary>
    /// The total number of active or pending frames in the stack.
    /// This includes the frames that have been "soft" popped
    /// when writing a value is suspended before the value is completed
    /// so that we can flush the buffer or fetch data and resume.
    /// This _pending length therefore allows us to know at what
    /// frame the suspension/pause started so that the root
    /// of the serializer can be able to access the state in the frame
    /// without calling Push().
    /// </summary>
    private int _suspendedLength;


    public ref readonly WriteStackFrame<TCustomState> Parent
    {
        get
        {
            if (_count < 2)
            {
                throw new InvalidOperationException("Stack is at the root or empty, cannot access parent frame.");
            }

            Debug.Assert(_stack != null && _stack.Length > 0);

            return ref _stack![_count - 2];
        }
    }

    internal ref WriteStackFrame<TCustomState> Current
    {
        get
        {
            if (_count == 0)
            {
                throw new InvalidOperationException("Stack is empty, cannot access current frame.");
            }

            return ref _stack![_count - 1];
        }
    }

    internal ref WriteStackFrame<TCustomState> LastSuspendedFrame
    {
        get
        {
            if (_suspendedLength == 0)
            {
                throw new InvalidOperationException("Stack is empty, cannot access suspended frame.");
            }

            return ref _stack![_suspendedLength - 1];
        }
    }

    internal ref TCustomState CurrentCustomState
    {
        get
        {
            if (_count == 0)
            {
                throw new InvalidOperationException("Stack is empty, cannot access current custom state.");
            }
            return ref _stack![_count - 1].CustomState;
        }
    }

    //public void Push(ref WriteStackFrame frame)
    //{
    //    if (_count == 0)
    //    {
    //        // If the stack is empty, we can just set the current frame directly
    //        _current = frame;
    //        _count++;
    //        return;
    //    }

    //    if (_stack == null)
    //    {
    //        _stack = new WriteStackFrame[4];
    //        if (_count == 1)
    //        {
    //            _stack[0] = _current; // Preserve the current frame if it exists
    //        }
    //    }
    //    else if (_count >= _stack.Length)
    //    {
    //        Array.Resize(ref _stack, _stack.Length * 2);
    //    }

    //    _stack[_count++] = frame;
    //    _current = frame;
    //}

    // since the WriteStack is relatively large and not all its properties
    // are set in every scope, we don't pass the WriteFrame as a parameter
    // to avoid copying the entire struct into the stack on each Push.
    // Instead, Push() prepares inits space for the next frame
    // and the caller is expected to modify directly using Stack.Current.
    //public void Push()
    //{
    //    if (_count == 0)
    //    {
    //        _count++;
    //        return;
    //    }

    //    if (_stack == null)
    //    {
    //        _stack = new WriteStackFrame[4];
    //        if (_count == 1)
    //        {
    //            _stack[0] = _current; // Preserve the current frame if it exists
    //        }
    //    }
    //    else if (_count >= _stack.Length)
    //    {
    //        Array.Resize(ref _stack, _stack.Length * 2);
    //    }

    //    _stack[_count++] = _current;
    //    _current = default;
    //}

    //public void Pop()
    //{
    //    if (_count == 0)
    //    {
    //        throw new IndexOutOfRangeException("Stack is empty, cannot pop frame.");
    //    }

    //    if (_count == 1)
    //    {
    //        _current = default;
    //    }
    //    else if (_count > 1)
    //    {
    //        Debug.Assert(_stack != null && _stack.Length > 0);
    //        _current = _stack[_count];
    //    }

    //    _count--;
    //}

    public void Push()
    {
        if (_stack == null)
        {
            _stack = new WriteStackFrame<TCustomState>[4];
            _count = 1;
            _suspendedLength = Math.Max(_suspendedLength, _count);
            return;
        }

        if (_count == _stack.Length)
        {
            Array.Resize(ref _stack, _stack.Length * 2);
        }

        _count++;
        _suspendedLength = Math.Max(_suspendedLength, _count);
    }

    public void Pop(bool success)
    {
        if (_count == 0)
        {
            throw new InvalidOperationException("Stack is empty, cannot pop frame.");
        }

        // If success is false, then the write at this position did not complete,
        // it was suspended, e.g. so that we can flush the buffer.
        // So we should not clear the current frame so then when we resume writing,
        // the write at this position can use the stack frame to restore its state
        // and pick up from where it left off.
        // If success is true, then we completed writing the value at the current position,
        // so we should clear the frame so that if we push the stack back to this frame,
        // we'll treat is as a fresh slate and not a continuation.
        if (success)
        {
            // If we've successfully completed, let's clear the current frame
            Current = default;

            _suspendedLength--;
        }

        _count--;
    }

    public void EndProperty()
    {
        Current.PropertyInfo = default;
        Current.PropertyProgress = default;
    }

    public void EndCollectionElement()
    {
        Current.EnumeratorIndex = default;
        Current.CurrentEnumerator = default;
    }

    public bool IsTopLevel()
    {
        return _count < 2;
    }

    public bool HasSuspendedFrames()
    {
        return _suspendedLength > 0;
    }
}
