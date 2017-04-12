// Run with:
// cscript artdbclean.js //Nologo

//////////////////////////////////////////////////////////////////////////////
// Include artcommon.js
var artCommonPath = new ActiveXObject("WScript.Shell").ExpandEnvironmentStrings("artcommon.js");
eval(new ActiveXObject("Scripting.FileSystemObject").OpenTextFile(artCommonPath, 1).ReadAll());

// Opens a result set over the specified connection.
function OpenResultSet(connection, queryText)
{
  var command = new ActiveXObject("ADODB.Command");
  command.ActiveConnection = connection;
  command.CommandText = queryText;
  return command.Execute();
}

// Opens an ADO SQL Server driver connection to the specified.
function OpenSqlServerConnection(serverName, initialCatalog, useTrustedConnection,
  username, password, provider)
{
  var connectionString = "Provider='" + provider + "';Data Source=" + serverName + ";";
  if (initialCatalog != null && initialCatalog != "")
  {
    connectionString += "Initial Catalog='" + initialCatalog + "';";
  }
  if (useTrustedConnection == true)
  {
    connectionString += "Integrated Security='SSPI';";
  }
  else if (username != null && username != "")
  {
    connectionString += "Username='" + username + "';Password='" + password + "';";
  }
  
  var result = new ActiveXObject("ADODB.Connection");
  result.Open(connectionString);
  return result;
}

function DropDatabasesForServer(serverName, path, provider)
{
  var connection;
  try
  {
    connection = OpenSqlServerConnection(serverName, null, true, null, null, provider);
  } catch (e) {
    //WScript.Echo("Continuing after failing to connect to server '" + serverName + "' with error: " + e.message);
    return;
  }
  
  var rs = OpenResultSet(connection, "select d.database_id, d.name, mf.physical_name " +
    "from sys.master_files as mf inner join sys.databases as d on mf.database_id = d.database_id " +
    "where mf.physical_name like '" + path + "%'");
  var databases = new Object();
  while (!rs.EOF)
  {
    var databaseName = rs.fields.Item(1).Value;
    var databaseFile = rs.fields.Item(2).Value;
    databases[databaseName] = databaseFile;
    rs.MoveNext();
  }

  for (var m in databases)
  {
    WScript.Echo("Dropping database " + m + " from '" + serverName + "' to unlock file " + databases[m] + ".");
    try
    {
      connection.Execute("DROP DATABASE [" + m + "]");
    } catch(e) {
      if (e.message.indexOf("Unable to open the physical file") >= 0) {
        //WScript.Echo("Continuing after this error: " + e.message);
        //WScript.Echo(e.message);
      } else {
        throw e;
      }
    }
  }
}

var enlistmentRoot = GetEnvironmentVariable("ENLISTMENT_ROOT");
if (enlistmentRoot == null || enlistmentRoot == "")
{
  WScript.Echo("Enlistment_Root isn't defined.");
  WScript.Quit(1);
}

var oledbServerNames = ["(local)", ".\\SQLEXPRESS"];
for (var i in oledbServerNames)
{
  DropDatabasesForServer(oledbServerNames[i], enlistmentRoot, "sqloledb");
}

var sqlncliServerNames = ["(localdb)\\MSSQLLocalDB"];
for (var i in sqlncliServerNames)
{
  // Use Sql native client for localDB.
  DropDatabasesForServer(sqlncliServerNames[i], enlistmentRoot, "sqlncli11");
}

WScript.Quit(0);
