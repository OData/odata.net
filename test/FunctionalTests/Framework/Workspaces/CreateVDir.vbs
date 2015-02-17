
Option Explicit
'On Error Resume Next

WScript.Echo "Starting IIS config for new Astoria DataService..."

''''''''''''''''''''''''
' Error codes 
Const ERR_OK              = 0
Const ERR_GENERAL_FAILURE = 1


''''''''''''''''''''''''
' Paths to root of website the script operates on
Const WEBSITE_ROOT = "IIS://localhost/W3SVC/1/Root"
Const SUB_PATH = ""

''''''''''''''''''''''''
' Parse command line
' arg 0 : name of vdir to create
' arg 1 : path on file system to folder to associate with vdir 
Dim strVDirName
Dim oArgs
Dim strPath
Dim bResult
Dim bAnonymous
Dim bNtlm

set oArgs = WScript.Arguments
strVDirName = oArgs(0)
strPath = oArgs(1)
bAnonymous = oArgs(2)
bNtlm = oArgs(3)

AddMimeType ".xap", "application/x-silverlight-app"

bResult = False
bResult = CreateIISVDir(strVDirName, True, True, strPath)
if(bResult) then
   WScript.Quit(0)
else
   WScript.Quit(1)
end if

''''''''''''''''''''''''
'Check if a vdir already exists
''''''''''''''''''''''''
Function IISVDirExists(strVDir)
    WScript.Echo
    WScript.Echo "Checking if vdir=" & strVDir & " exists ..." 
    On Error Resume Next
    
    Dim bVDirExists
    Dim oIISAdmin
    'create IIS admin object
    Set oIISAdmin = GetObject("IISWebVirtualDir",SUB_PATH & "/" & strVDir)
    
    'if error, then no dir exists
    If Err.number = ERR_OK Then
        WScript.Echo "vdir=" & strVDir & " already exists" 
        bVDirExists = True
    Else
        WScript.Echo "VDir was not found (likely doesnt already exist) - cause: " & Err.Description
        bVDirExists = False
    End If
    
    'release IISAdmin object
    Set oIISAdmin = Nothing
    
    IISVDirExists = bVDirExists    
End Function


''''''''''''''''
' Creates a vdir 
''''''''''''''''
Function CreateIISVDir(strVDir, bAllowScript, bAllowExecute, strVDirPath)
    Dim oIISAdmin
    Dim oVirtualDir
    Dim bResult
    Dim MappingListArray
    Dim szTestAppMap
    
    Set oIISAdmin = GetObject(WEBSITE_ROOT)
    Set oVirtualDir = oIISAdmin.Create("IISWebVirtualDir", strVDir)

    'oVirtualDir.Path = SUB_PATH & "/" & strVDir
    oVirtualDir.AccessScript = bAllowScript
    oVirtualDir.AccessExecute = bAllowExecute
    oVirtualDir.Path = strVDirPath

    oVirtualDir.AuthAnonymous = bAnonymous
    oVirtualDir.AuthBasic = False
    oVirtualDir.AuthNTLM = bNtlm
    oVirtualDir.DefaultLogonDomain = ""

    MappingListArray=oIISAdmin.ScriptMaps
    WScript.echo Join(MappingListArray,vbCrLf) 
 
    dim llngIndex
    For llngIndex = 0 To UBound(MappingListArray)
        If left(MappingListArray(llngIndex),4)=".svc" then
            dim sScriptMap
            sScriptMap = split(MappingListArray(llngIndex),",")
            MappingListArray(llngIndex)= sScriptMap(0) + "," + sScriptMap(1) + "," + sScriptMap(2)
        End If
    Next
    oVirtualDir.ScriptMaps=MappingListArray

    ' all sites currently go in the default app pool
    'appcreate2 documentation: https://msdn2.microsoft.com/en-us/library/ms525461.aspx
    oVirtualDir.AppCreate2 2
    
    ' name & friendly name is just the same as the vdir name
    oVirtualDir.AppFriendlyName = strVDir
  
    ' save updates
    oVirtualDir.SetInfo 
    
    if (Err <> ERR_OK) then
	wscript.echo "Failed creating VDIR"
        bResult = False
    else
	WScript.echo "Sucessfully created VDIR"
        bResult = True
    end if
    
    CreateIISVDir = bResult
End Function

Sub AddMimeType(ExtensionToAdd, MimeTypeToAdd)
	Dim LocalMimeMap, MimeMap
	Dim CurrentExtension
	Dim i
	Const ADS_PROPERTY_UPDATE = 2
	
	Set LocalMimeMap = GetObject(WEBSITE_ROOT)
	MimeMap = LocalMimeMap.GetEx("MimeMap")

	Dim Found
	Found = false
	For i = LBound(MimeMap) To UBound(MimeMap)
		CurrentExtension = MimeMap(i).Extension
		If ExtensionToAdd = CurrentExtension Then
			Found = true
		End If
	Next

	If Not Found then
		i = UBound(MimeMap)+1
		Redim Preserve MimeMap(i)

		Set MimeMap(i) = CreateObject("MimeMap")
		MimeMap(i).Extension = ExtensionToAdd
		MimeMap(i).MimeType = MimeTypeToAdd
		LocalMimeMap.PutEx ADS_PROPERTY_UPDATE,"MimeMap",MimeMap
		LocalMimeMap.SetInfo
	End If

End Sub
