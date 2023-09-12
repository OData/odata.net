import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import * as serviceWorkerRegistration from './serviceWorkerRegistration';
import reportWebVitals from './reportWebVitals';
import MonacoEditor from '@monaco-editor/react';

const baseUrl = document.getElementsByTagName('base')[0]?.getAttribute('href') || undefined;
const rootElement = document.getElementById('root');

// Define a functional component to render the Monaco Editor
const CodeEditor: React.FC = () => {
  const handleEditorChange = (value: string | undefined) => {
    console.log(value); // You can handle the editor's value here
  };

  return (
    <MonacoEditor
      height="500px" // Set the desired height
      defaultLanguage="javascript" // Set the default language
      defaultValue="// Start coding here..."
      onChange={handleEditorChange}
    />
  );
};

ReactDOM.render(
  <BrowserRouter basename={baseUrl}>
    {/* <CodeEditor /> */}
    Hello World
  </BrowserRouter>,
  rootElement
);

serviceWorkerRegistration.unregister();

reportWebVitals();
