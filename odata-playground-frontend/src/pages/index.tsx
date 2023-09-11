import Head from "next/head";
import Link from "next/link";
import Editor from '@monaco-editor/react';
import * as monaco from 'monaco-editor/esm/vs/editor/editor.api';
import { useState } from "react";

export default function Home() {

  const defaultValue = `<edmx:Edmx xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx" Version="4.0">
  <edmx:DataService>
    <Schema xmlns="http://docs.oasis-open.org/odata/ns/edm" Namespace="Jetsons">
      <EntityType Name="Company">
        <Key>
          <PropertyRef Name="stockSymbol"/>
        </Key>
        <Property Name="stockSymbol" Type="Edm.String" Nullable="false"/>
        <Property Name="name" Type="Edm.String"/>
        <Property Name="incorporated" Type="Edm.DateTimeOffset" Nullable="false" Precision="0"/>
        <NavigationProperty Name="employees" Type="Collection(Jetsons.Employee)" Nullable="false" ContainsTarget="true"/>
      </EntityType>
      <EntityType Name="Employee">
        <Key>
          <PropertyRef Name="id"/>
        </Key>
        <Property Name="id" Type="Edm.Int32" Nullable="false"/>
        <Property Name="firstName" Type="Edm.String"/>
        <Property Name="lastName" Type="Edm.String"/>
        <Property Name="title" Type="Edm.String"/>
      </EntityType>
      <Function Name="topEmployees" IsBound="true" EntitySetPath="company/employees">
        <Parameter Name="company" Type="Jetsons.Company"/>
        <Parameter Name="num" Type="Edm.Int32" Nullable="false"/>
        <ReturnType Type="Collection(Jetsons.Employee)"/>
      </Function>
      <Action Name="youreFired" IsBound="true">
        <Parameter Name="employee" Type="Jetsons.Employee"/>
        <Parameter Name="reason" Type="Edm.String" Nullable="false"/>
      </Action>
      <EntityContainer Name="Container">
        <EntitySet Name="competitors" EntityType="Jetsons.Company"/>
        <Singleton Name="company" Type="Jetsons.Company"/>
      </EntityContainer>
    </Schema>
  </edmx:DataService>
</edmx:Edmx>
  `

  const [csdlInput, setCsdlInput] = useState(defaultValue);

  function handleEditorChange(value: string | undefined, event: monaco.editor.IModelContentChangedEvent): void {
    setCsdlInput(value ?? '');
  }

  return (
    <>
      <Head>
        <title>OData Playground</title>
        <meta name="description" content="OData Playground Created for Microsoft 2023 Hackathon" />
        <link rel="icon" href="/favicon.ico" />
      </Head>
      <main className="h-screen flex flex-col">
        <div className="bg-green-100 p-2 border-b text-2xl font-bold border-black">
          OData Playground
        </div> 
          <div className="h-full bg-white grid grid-cols-2 gap-4">
            <div className="border-red-500 border">
              <div className="h-full">
                <Editor defaultLanguage="xml" defaultValue={defaultValue} onChange={handleEditorChange} />
              </div>
            </div>
            <div className="bg-stone-100 p-4 border border-black">
              {csdlInput}
            </div>
          </div>
      </main>
    </>
  );
}


