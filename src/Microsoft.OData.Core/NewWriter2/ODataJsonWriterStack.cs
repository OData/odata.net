using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;

namespace Microsoft.OData.Core.NewWriter2;

// TODO make this a struct
internal class ODataJsonWriterStack
{
    // Don't create stack array if we only have one frame
    private ODataJsonWriterStackFrame _current;
    private ODataJsonWriterStackFrame[] _stack;

    private int _count;

    public ref readonly ODataJsonWriterStackFrame Parent
    {
        get
        {
            if (_count < 1)
            {
                throw new IndexOutOfRangeException("Stack is empty, cannot access parent frame.");
            }

            return ref _stack[_count - 1];
        }
    }

    public ref readonly ODataJsonWriterStackFrame Current
    {
        get
        {
            return ref _current;
        }
    }

    public void Push(ODataJsonWriterStackFrame frame)
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
            _stack = new ODataJsonWriterStackFrame[4];
        }
        else if (_count >= _stack.Length)
        {
            Array.Resize(ref _stack, _stack.Length * 2);
        }

        _stack[_count++] = frame;
        _current = frame;
    }

    public bool IsTopLevel()
    {
        return _count < 2;
    }
}
