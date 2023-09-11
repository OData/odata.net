import { xml } from '@codemirror/lang-xml'
import { json } from '@codemirror/lang-json'
import { EditorState } from '@codemirror/state'
import { EditorView } from '@codemirror/view'
import { basicSetup } from 'codemirror'

const initialText = 'console.log("hello, world")'

const csdlInput = document.querySelector('#csdl-input')!;
const routesOutput = document.querySelector('#routes-output')!;

const csdlInputEditor = new EditorView({
  parent: csdlInput,
  state: EditorState.create({
    doc: initialText,
    extensions: [
      basicSetup,
      xml(),
    ],
  }),
})

const routesOutputEditor = new EditorView({
  parent: routesOutput,
  state: EditorState.create({
    doc: initialText,
    extensions: [
      basicSetup,
      json(),
    ],
  }),

})

function updateSecondEditorContent() {
  const content = csdlInputEditor.state.doc.toString().toUpperCase();
  routesOutputEditor.dispatch({
    changes: { from: 0, to: routesOutputEditor.state.doc.length, insert: content },
  });
}

// Call the function to initially populate the second editor
updateSecondEditorContent();

setInterval(updateSecondEditorContent, 3000);

csdlInputEditor.dom.addEventListener('input', updateSecondEditorContent);
