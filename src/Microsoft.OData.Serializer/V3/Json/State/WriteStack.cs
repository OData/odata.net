using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.State;

// TODO: should this be a struct?
internal class WriteStack
{
    // Don't create stack array if we only have one frame
    private WriteStackFrame _current;
    private WriteStackFrame[]? _stack;

    private int _count;


    public ref readonly WriteStackFrame Parent
    {
        get
        {
            if (_count < 1)
            {
                throw new InvalidOperationException("Stack is empty, cannot access parent frame.");
            }

            Debug.Assert(_stack != null && _stack.Length > 0);

            return ref _stack![_count - 1];
        }
    }

    internal ref WriteStackFrame Current
    {
        get
        {
            return ref _current;
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
    public void Push()
    {
        if (_count == 0)
        {
            _count++;
            return;
        }

        if (_stack == null)
        {
            _stack = new WriteStackFrame[4];
            if (_count == 1)
            {
                _stack[0] = _current; // Preserve the current frame if it exists
            }
        }
        else if (_count >= _stack.Length)
        {
            Array.Resize(ref _stack, _stack.Length * 2);
        }

        _stack[_count++] = _current;
        _current = default;
    }

    public void Pop()
    {
        if (_count == 0)
        {
            throw new IndexOutOfRangeException("Stack is empty, cannot pop frame.");
        }

        if (_count == 1)
        {
            _current = default;
        }
        else if (_count > 1)
        {
            Debug.Assert(_stack != null && _stack.Length > 0);
            _current = _stack[_count];
        }

        _count--;
    }

    public bool IsTopLevel()
    {
        return _count < 2;
    }
}
