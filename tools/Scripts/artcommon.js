//////////////////////////////////////////////////////////////////////////////
//                                                                          //
// artcommon.js                                                             //
//                                                                          //
// Provides utility functions used by multiple JScript files.               //
//                                                                          //
//////////////////////////////////////////////////////////////////////////////

// Deletes the specified folder, forcing read-only files to be deleted as well.
function DeleteFolder(path)
{
  var fso = new ActiveXObject("Scripting.FileSystemObject");
  fso.DeleteFolder(path, true);
}

// Checks whether the specified file exists.
function FileExists(path)
{
   var fso;
   fso = new ActiveXObject("Scripting.FileSystemObject");
   if (fso.FileExists(path))
   {
     return true;
   }
   else
   {
     return false;
   }
}

function GetEnvironmentVariable(name)
{
  var WshShell = new ActiveXObject("WScript.Shell");
  var result = WshShell.ExpandEnvironmentStrings("%" + name + "%");
  if (result == "%" + name + "%")
  {
    result = null;
  }
  return result;
}

// Returns the extension of the specified path string (including the ".");
// empty if there is no extension.
function PathGetExtension(path)
{
  var l = path.length;
  var startIndex = l;
  while (--startIndex >= 0)
  {
    var ch = path.substr(startIndex, 1);
    if (ch == ".")
    {
      if (startIndex != (l - 1))
      {
        return path.substr(startIndex, l - startIndex);
      }
      return "";
    }
    else if (ch == "\\" || ch == ":")
    {
      break;
    }
  }
  return "";
}

// Runs the specified function catching exceptions and quits the current script.
function RunAndQuit(f)
{
  try
  {
    f();
  }
  catch(e)
  {
    WScript.Echo("Error caught while running this function:");
    WScript.Echo(f.toString());
    WScript.Echo("Error details:");
    if (typeof(e) == "object" && e.toString() == "[object Error]")
    {
      for (var p in e) WScript.Echo(" " + p + ": " + p[e]);
    }
    else
    {
      WScript.Echo(e);
    }
    WScript.Quit(1);
  }
  WScript.Quit(0);
}

// Runs a command and waits for it to exit.
// Returns an array with stdout in 0, stderr in 1 and exit code in 2.
function RunConsoleCommand(strCommand)
{
  var WshShell = new ActiveXObject("WScript.Shell");
  var result = new Array(3);
  var oExec = WshShell.Exec(strCommand);
  
  result[0] = oExec.StdOut.ReadAll();
  result[1] = oExec.StdErr.ReadAll();
  result[2] = oExec.ExitCode;
  
  return result;
}

// Uses the Speech API to speak to the user.
function Say(text)
{
  var voice = new ActiveXObject("SAPI.SpVoice");
  try {
    voice.Speak(text);
  }
  catch (e) {
    // See http://msdn2.microsoft.com/en-us/library/ms717306.aspx for error codes.
    // SPERR_DEVICE_BUSY 0x80045006 -2147201018
    if (e.number == -2147201018) {
      WScript.Echo("The wave device is busy.");
    }
  }
}

// Splits a string into a string array.
function StringSplit(strLine, strSeparator)
{
  var result = new Array();
  var startIndex = 0;
  var resultIndex = 0;
  while (startIndex < strLine.length)
  {
    var endIndex = strLine.indexOf(strSeparator, startIndex);
    if (endIndex == -1)
    {
      endIndex = strLine.length;
    }
    result[resultIndex] = strLine.substring(startIndex, endIndex);
    startIndex = endIndex + strSeparator.length;
    resultIndex++;
  }
  return result;
}


//
// References:
//
// JScript Language Reference
// http://msdn2.microsoft.com/en-us/library/yek4tbz0
//
// Windows Script Host Object Model
// http://msdn2.microsoft.com/en-us/library/a74hyyw0
//
// Script Runtime
// http://msdn2.microsoft.com/en-us/library/hww8txat.aspx
//
