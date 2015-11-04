using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
using Microsoft.Owin;
using NextLAP.IP1.ExecutionEngineWebAPI;

[assembly: AssemblyTitle("NextLAP.IP1.ExecutionEngineWebAPI")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("ce5f5ddb-8e9a-4834-af54-265cf9765e92")]

[assembly: OwinStartup(typeof(Startup))]
