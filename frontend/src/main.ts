import { xml } from '@codemirror/lang-xml'
import { json } from '@codemirror/lang-json'
import { EditorState } from '@codemirror/state';
import { EditorView} from '@codemirror/view'
import { basicSetup } from 'codemirror'

const csdlInput = document.querySelector('#csdl-input')!;
const routesOutput = document.querySelector('#routes-output')!;

const csdlInputEditor = new EditorView({
  parent: csdlInput,
  state: EditorState.create({
    doc: '',
    extensions: [
      basicSetup,
      xml(),
    ],
  }),
})

const routesOutputEditor = new EditorView({
  parent: routesOutput,
  state: EditorState.create({
    doc: '',
    extensions: [
      basicSetup,
      json(),
    ],
  }),
})

routesOutputEditor.contentDOM.contentEditable = 'false';

// Function to load file content and set it in the editor
async function loadSampleFile(filePath: string, editorView: EditorView): Promise<void> {
  try {
    const response = await fetch(filePath);
    if (!response.ok) {
      throw new Error(`Failed to fetch file: ${filePath}`);
    }
    const fileContent = await response.text();
    editorView.dispatch({
      changes: { from: 0, to: editorView.state.doc.length, insert: fileContent },
    });
  } catch (error) {
    console.error(error);
  }
}

// Call the function to initially populate the editors with file contents
loadSampleFile('../samples/sample_1.csdl', csdlInputEditor);

function updateSecondEditorContent() {
  const content = csdlInputEditor.state.doc.toString().toUpperCase();
  routesOutputEditor.dispatch({
    changes: { from: 0, to: routesOutputEditor.state.doc.length, insert: content },
  });
  
}

// Call the function to initially populate the second editor
updateSecondEditorContent();

csdlInputEditor.dom.addEventListener('input', updateSecondEditorContent);
