Option Explicit

WScript.Echo "Starting IIS config for new Astoria DataService..."

''''''''''''''''''''''''
' Error codes 
Const ERR_OK = 0
Const ERR_GENERAL_FAILURE = 1

''''''''''''''''''''''''
' Paths to root of website the script operates on
Const WEBSITE_ROOT = "IIS://localhost/W3SVC/1/Root"
Const SUB_PATH = ""

''''''''''''''''''''''''
' Parse command line
' arg 0 : name of vdir to delete

Dim strVDirName
Dim oArgs
Dim strPath
Dim bResult

set oArgs = WScript.Arguments
strVDirName = oArgs(0)

bResult = False
bResult = DelVDir(strVDirName)

' Don't cause failure on error
If (bResult = False) then
    WScript.Quit()
End If

WScript.Sleep 5000

bResult = False
bResult = DelVDirFiles(strVDirName)

' Don't cause failure on error
If (bResult = False) then
    WScript.Quit()
End If

Function DelVDir(strVDir)
    Dim objWebServer
    Dim bResult
    Dim i
    
    Set objWebServer = GetObject(WEBSITE_ROOT)

    'Delete the Virtual subdirectory
    i = 0

    On Error Resume Next
    Do
        objWebServer.Delete "IISWebVirtualDir", strVDir

        if (Err <> ERR_OK) then
            if (i < 5) then
                Err.Clear
                WScript.Sleep 5000
                i = i + 1
            else
                wscript.echo "Failed deleting VDIR"
                DelVDir = False
                Exit Do
            end if
        else
            WScript.echo "Sucessfully deleted VDIR"
            DelVDir = True
            Exit Do
        end if
    Loop
    
    On Error Goto 0
End Function

Function DelVDirFiles(strVDir)
    Dim objWebServer
    Dim strPath
    Dim fso
    Dim bResult
    
    Dim WshShell
    Dim strSystemDrive
    
    Set objWebServer = GetObject(WEBSITE_ROOT)

    strPath = objWebServer.Path & "\" & strVDir

    Set fso = CreateObject("Scripting.FileSystemObject")

    On Error Resume Next
    If fso.FolderExists(strPath) Then
        fso.DeleteFolder strPath
        
        if (Err <> ERR_OK) then
            Err.Clear
            wscript.echo "Failed deleting folder: " & strPath
            DelVDirFiles = False
        else
            WScript.echo "Sucessfully deleted folder: " & strPath
        end if
        
    End If
    
    On Error Goto 0
    Set WshShell = WScript.CreateObject("WScript.Shell") 
    strSystemDrive = WshShell.ExpandEnvironmentStrings("%SystemDrive%") 

    strPath = strSystemDrive & "\Windows\Microsoft.NET\Framework\v2.0.50727\Temporary ASP.NET Files\" & strVDir
    
    On Error Resume Next
    If fso.FolderExists(strPath) Then
        fso.DeleteFolder strPath
        
        if (Err <> ERR_OK) then
            Err.Clear
            wscript.echo "Failed deleting folder: " & strPath
            DelVDirFiles = False
        else
            WScript.echo "Sucessfully deleted folder: " & strPath
        end if

    End If
    
    On Error Goto 0
    DelVDirFiles = True
    
End Function

