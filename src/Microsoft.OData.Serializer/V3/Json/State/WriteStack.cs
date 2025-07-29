using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.State;

// TODO: should this be a struct?
internal class WriteStack
{
    // Don't create stack array if we only have one frame
    private WriteStackFrame _current;
    private WriteStackFrame[] _stack;

    private int _count;


    public ref readonly WriteStackFrame Parent
    {
        get
        {
            if (_count < 1)
            {
                throw new InvalidOperationException("Stack is empty, cannot access parent frame.");
            }

            return ref _stack[_count - 1];
        }
    }

    public ref readonly WriteStackFrame Current
    {
        get
        {
            return ref _current;
        }
    }

    public void Push(WriteStackFrame frame)
    {
        if (_count == 0)
        {
            // If the stack is empty, we can just set the current frame directly
            _current = frame;
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

        _stack[_count++] = frame;
        _current = frame;
    }

    public WriteStackFrame Pop()
    {
        if (_count == 0)
        {
            throw new IndexOutOfRangeException("Stack is empty, cannot pop frame.");
        }

        _count--;
        var frame = _current;
        if (_count > 0)
        {
            _current = _stack[_count - 1];
        }
        else
        {
            _current = default; // Reset current when stack is empty
        }
        return frame;
    }

    public bool IsTopLevel()
    {
        return _count < 2;
    }
}
