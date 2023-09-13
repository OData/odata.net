import Head from "next/head";
import Link from "next/link";
import Editor from "@monaco-editor/react";
import * as monaco from "monaco-editor/esm/vs/editor/editor.api";
import React, { useState, useEffect, useRef } from "react";
import SwaggerUI from "./SwaggerUI";
import { csdl2openapi } from "../utils/csdl2openapi";
import { xml2json } from "../utils/xml2json";
import { set } from "zod";
import { useRouter } from 'next/router';

export default function Home() {
  const router = useRouter();
  const sampleCsdl = `<?xml version="1.0" encoding="utf-8"?>
  <edmx:Edmx Version="4.0" xmlns:edmx="http://docs.oasis-open.org/odata/ns/edmx">
    <edmx:Reference Uri="./Products.xml">
      <edmx:Include Namespace="ProductService" />
    </edmx:Reference>
    <edmx:Reference Uri="https://oasis-tcs.github.io/odata-vocabularies/vocabularies/Org.OData.Core.V1.xml">
      <edmx:Include Namespace="Org.OData.Core.V1" Alias="Core" />
    </edmx:Reference>
    <edmx:Reference Uri="https://oasis-tcs.github.io/odata-vocabularies/vocabularies/Org.OData.Capabilities.V1.xml">
      <edmx:Include Alias="Capabilities" Namespace="Org.OData.Capabilities.V1" />
    </edmx:Reference>
    <edmx:DataServices>
      <Schema Namespace="PeopleService" xmlns="http://docs.oasis-open.org/odata/ns/edm">
        <EntityType Name="Supplier">
          <Key>
            <PropertyRef Name="ID" />
          </Key>
          <Property Name="ID" Type="Edm.Int32" Nullable="false" />
          <Property Name="Name" Type="Edm.String" />
          <Property Name="Address" Type="PeopleService.Address" />
          <Property Name="Location" Type="Edm.GeographyPoint" SRID="variable" />
          <Property Name="Concurrency" Type="Edm.Int32" Nullable="false" />
          <NavigationProperty Name="Products" Type="Collection(ProductService.Product)" Partner="Supplier" />
        </EntityType>
        <ComplexType Name="Address">
          <Property Name="Street" Type="Edm.String" />
          <Property Name="City" Type="Edm.String" />
          <Property Name="State" Type="Edm.String" />
          <Property Name="ZipCode" Type="Edm.String" />
          <Property Name="Country" Type="Edm.String" />
        </ComplexType>
        <EntityType Name="Person">
          <Key>
            <PropertyRef Name="ID" />
          </Key>
          <Property Name="ID" Type="Edm.Int32" Nullable="false" />
          <Property Name="Name" Type="Edm.String" />
          <NavigationProperty Name="PersonDetail" Type="PeopleService.PersonDetail" Partner="Person" />
        </EntityType>
        <EntityType Name="Customer" BaseType="PeopleService.Person">
          <Property Name="TotalExpense" Type="Edm.Decimal" Nullable="false" />
        </EntityType>
        <EntityType Name="Employee" BaseType="PeopleService.Person">
          <Property Name="EmployeeID" Type="Edm.Int64" Nullable="false" />
          <Property Name="HireDate" Type="Edm.DateTimeOffset" Nullable="false" />
          <Property Name="Salary" Type="Edm.Single" Nullable="false" />
        </EntityType>
        <EntityType Name="PersonDetail">
          <Key>
            <PropertyRef Name="PersonID" />
          </Key>
          <Property Name="PersonID" Type="Edm.Int32" Nullable="false" />
          <Property Name="Age" Type="Edm.Byte" Nullable="false" />
          <Property Name="Gender" Type="Edm.Boolean" Nullable="false" />
          <Property Name="Phone" Type="Edm.String" />
          <Property Name="Address" Type="PeopleService.Address" />
          <Property Name="Photo" Type="Edm.Stream" Nullable="false" />
          <NavigationProperty Name="Person" Type="PeopleService.Person" Partner="PersonDetail" />
        </EntityType>
        <EntityContainer Name="Container">
          <Annotation Term="Capabilities.KeyAsSegmentSupported" />
          <EntitySet Name="Suppliers" EntityType="PeopleService.Supplier">
            <NavigationPropertyBinding Path="Products" Target="ProductService.Container/Products" />
          </EntitySet>
          <EntitySet Name="People" EntityType="PeopleService.Person">
            <NavigationPropertyBinding Path="PersonDetail" Target="PersonDetails" />
          </EntitySet>
          <EntitySet Name="PersonDetails" EntityType="PeopleService.PersonDetail">
            <NavigationPropertyBinding Path="Person" Target="Persons" />
          </EntitySet>
        </EntityContainer>
        <Annotations Target="PeopleService.Container">
          <Annotation Term="Org.OData.Display.V1.Description" String="This is a sample OData service with vocabularies" />
        </Annotations>
        <Annotations Target="PeopleService.Product">
          <Annotation Term="Org.OData.Display.V1.Description" String="All Products available in the online store" />
        </Annotations>
        <Annotations Target="PeopleService.Product/Name">
          <Annotation Term="Org.OData.Display.V1.DisplayName" String="Product Name" />
        </Annotations>
        <Annotations Target="PeopleService.Container/Suppliers">
          <Annotation Term="Org.OData.Publication.V1.PublisherName" String="Microsoft Corp." />
          <Annotation Term="Org.OData.Publication.V1.PublisherId" String="MSFT" />
          <Annotation Term="Org.OData.Publication.V1.Keywords" String="Inventory, Supplier, Advertisers, Sales, Finance" />
          <Annotation Term="Org.OData.Publication.V1.AttributionUrl" String="http://www.odata.org/" />
          <Annotation Term="Org.OData.Publication.V1.AttributionDescription" String="All rights reserved" />
          <Annotation Term="Org.OData.Publication.V1.DocumentationUrl " String="http://www.odata.org/" />
          <Annotation Term="Org.OData.Publication.V1.TermsOfUseUrl" String="All rights reserved" />
          <Annotation Term="Org.OData.Publication.V1.PrivacyPolicyUrl" String="http://www.odata.org/" />
          <Annotation Term="Org.OData.Publication.V1.LastModified" String="4/2/2013" />
          <Annotation Term="Org.OData.Publication.V1.ImageUrl " String="http://www.odata.org/" />
        </Annotations>
      </Schema>
    </edmx:DataServices>
  </edmx:Edmx>
`;
  const csdlParam = router.query.csdl as string | undefined;

  const defaultValue = (() => {
    if (typeof window !== 'undefined') {
      if (csdlParam) {
        // Decode the 'csdlParam' if needed
        const csdlId = decodeURIComponent(csdlParam);
      
        // Clear the query parameter from the URL
        window.history.replaceState({}, document.title, "/");
        console.log("Param Found, Saving to Local Storage and Redirecting")
        localStorage.setItem('csdlId', csdlId);
        window.location.href = '/';
      } else if (localStorage.getItem('csdlId')) {
        console.log("Local Storage Found, Fetching CSDL")
        const csdlId = localStorage.getItem('csdlId') ?? "";
        // Fetch the CSDL data from the backend
        fetch(`/csdls/${csdlId}`)
          .then((response) => {
            if (!response.ok) {
              throw new Error("Network response was not ok");
            }
            return response.text();
          })
          .then((csdlEncoded) => {
            // Decode the base64-encoded CSDL data
            const csdlIdBytes = atob(csdlEncoded);
      
            // Call updateEditorValue with the decoded CSDL
            console.log("Updating Editor Value");
            console.log(csdlIdBytes);
            setEditorContent(csdlIdBytes);
            // clear local storage
            localStorage.removeItem('csdlId');
            return csdlIdBytes
          })
          .catch((error) => {
            console.log('error', error);
            return 
          });
      } else {
        return sampleCsdl;
      }
    } else {
      return sampleCsdl;
    }
  })();

  var userInput = defaultValue;


  function parseXMLtoOpenAPIAsString(xml: string): string {
    console.log("[parseXMLtoOpenAPIAsString]Parsing XML to Open API as String");
    try {
      const parsed = xml2json(xml.replace("\ufeff", "") ?? "");
      const actual = csdl2openapi(parsed, {});
      const actualString = JSON.stringify(actual, null, 2);
      return actualString;
    } catch (e) {
      const errorMessage = (e as Error).message;
      console.log("[parseXMLtoOpenAPIAsString]Error: " + errorMessage);
      return errorMessage ?? "Failed to Parse CSDL";
    }
  }

  const [parsedOutput, setParsedOutput] = useState(
    () => {
      console.log("Setting Initial Value of parsedOutput");
      return parseXMLtoOpenAPIAsString(defaultValue ?? "") ;
    }, // Sets initial value of parsedOutput
  );

  const [swaggerJson, setSwaggerJson] = useState<Record<string, any> | null>(
    null,
  );

  // Reference to the Monaco Editor instance
  const editorRef = useRef<monaco.editor.IStandaloneCodeEditor | null>(null);

  // Function to set the editor value
  const setEditorContent = (content: string) => {
    if (editorRef.current) {
      editorRef.current.setValue(content);
    }
  };

  function handleEditorChange(
    value: string | undefined,
    event: monaco.editor.IModelContentChangedEvent,
  ): void {
    console.log("[handleEditorChange]Begin Editor Change");
    userInput = value ?? "";
    console.log("[handleEditorChange]UserInput: " + userInput);
    setParsedOutput(parseXMLtoOpenAPIAsString(value ?? "") ?? "");
    console.log("[handleEditorChange]End Editor Change");
  }

  function sendDataToBackend() {
    var encodedUserInput = btoa(userInput ?? "")
    console.log("[sendDataToBackend]Sending Data to Backend");
    var requestOptions: RequestInit = {
      method: 'PUT',
      headers: { 'Content-Type': 'text/plain' },
      body: encodedUserInput,
      redirect: 'follow' as RequestRedirect // Specify the type explicitly
    };
  
    fetch("/csdls", requestOptions)
    .then(response => {
      // Check if the response has a 'Location' header
      if (response.headers.has('Location')) {
        // Get the value of the 'Location' header
        const locationHeader = response.headers.get('Location') ?? "";
        const csdlId = locationHeader.substring(locationHeader.indexOf('/csdls/') + 7);
        console.log("CSDL ID: " + csdlId);
        const newURL = `${location.origin}?csdl=${csdlId}`;
        
        // Present the 'Location' header value in an alert
        alert(`Shareable URL: ${newURL}`);
      } else {
        // If 'Location' header is not present, show a message
        alert('Unable to Generate Shareable URL');
      }
      return response.text();
    })
    .then(result => console.log(result))
    .catch(error => console.log('error', error));
  }

  useEffect(() => {
    // Update Swagger JSON whenever parsedOutput changes
    console.log("[parsedOutputUpdate]Updating Swagger JSON");
    console.log("[parsedOutputUpdate]UserInput: " + userInput)
    console.log("[parsedOutputUpdate]ParsedOutput: "+ parsedOutput)
    if (parsedOutput) {
      try {
        const json = JSON.parse(parsedOutput);
        setSwaggerJson(json);
      } catch (error) {
        console.log("Error parsing JSON:", error);
        setSwaggerJson({}); // Handle parsing errors
      }
    } else {
      setSwaggerJson({}); // Handle the case where parsedOutput is empty
    }
  }, [parsedOutput]); // Include parsedOutput as a dependency
  

  return (
    <>
      <Head>
        <title>OData Playground</title>
        <meta
          name="description"
          content="OData Playground Created for Microsoft 2023 Hackathon"
        />
        <link rel="icon" href="/favicon.ico" />
      </Head>
      <main className="flex h-screen flex-col">
        <div className="flex items-center justify-between bg-[#0F6EBF] p-2 text-2xl font-bold text-white">
          <div>OData Playground</div>
          <button className="p-2 hover:bg-[#364853] rounded" onClick={sendDataToBackend}>Share</button>
        </div>

        <div className="grid h-full grid-cols-2 gap-4 bg-white">
          <div className="border border-red-500">
            <div className="h-full">
              <Editor
                defaultLanguage="xml"
                defaultValue={defaultValue}
                onChange={handleEditorChange}
                onMount={(editor) => {
                  editorRef.current = editor;
                }}
              />
            </div>
          </div>
          <div className="border border-black bg-stone-100 p-4">
            {swaggerJson ? (
              <SwaggerUI swaggerJson={swaggerJson} />
            ) : (
              <p>Loading Swagger UI...</p>
            )}
          </div>
        </div>
      </main>
    </>
  );
}
