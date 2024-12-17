if not exist "D:\Working\Odyssey\Utilities\TDMEngine\bin"                      md "D:\Working\Odyssey\Utilities\TDMEngine\bin"
if not exist "D:\Working\Odyssey\Utilities\TDMEngine\bin\Release"              md "D:\Working\Odyssey\Utilities\TDMEngine\bin\Release"
if exist "D:\Working\EQECAT5\_dotNetCommon\Assemblies\Internal\Eqecat.AccessDbInterface.v1.0.dll"   copy /y "D:\Working\EQECAT5\_dotNetCommon\Assemblies\Internal\Eqecat.AccessDbInterface.v1.0.dll"   "D:\Working\Odyssey\Utilities\TDMEngine\bin\Release"
if exist "D:\Working\EQECAT5\_dotNetCommon\Assemblies\Internal\Eqecat.DatabaseInterface.v1.0.dll"   copy /y "D:\Working\EQECAT5\_dotNetCommon\Assemblies\Internal\Eqecat.DatabaseInterface.v1.0.dll"   "D:\Working\Odyssey\Utilities\TDMEngine\bin\Release"
if exist "D:\Working\EQECAT5\_dotNetCommon\Assemblies\Internal\Eqecat.ZipLibWrapper.dll"            copy /y "D:\Working\EQECAT5\_dotNetCommon\Assemblies\Internal\Eqecat.ZipLibWrapper.dll"            "D:\Working\Odyssey\Utilities\TDMEngine\bin\Release"
if exist "D:\Working\EQECAT5\_dotNetCommon\Assemblies\ThirdParty\SharpZipLib\net-20\ICSharpCode.SharpZipLib.dll"   copy /y "D:\Working\EQECAT5\_dotNetCommon\Assemblies\ThirdParty\SharpZipLib\net-20\ICSharpCode.SharpZipLib.dll"   "D:\Working\Odyssey\Utilities\TDMEngine\bin\Release"
if exist "D:\Working\EQECAT5\_dotNetCommon\Assemblies\Internal\WCeDBInterface.SQL.2005.dll"         copy /y "D:\Working\EQECAT5\_dotNetCommon\Assemblies\Internal\WCeDBInterface.SQL.2005.dll"         "D:\Working\Odyssey\Utilities\TDMEngine\bin\Release"

if exist "D:\Working\EQECAT5\_dotNetCommon\Assemblies\Internal\Eqecat.AccessDbInterface.v1.0.dll"   goto :end_copy

if exist "D:\Working\_SWCM\SOURCE\_dotNetCommon\Assemblies\Internal\Eqecat.AccessDbInterface.v1.0.dll"   copy /y "D:\Working\_SWCM\SOURCE\_dotNetCommon\Assemblies\Internal\Eqecat.AccessDbInterface.v1.0.dll"   "D:\Working\Odyssey\Utilities\TDMEngine\bin\Release"
if exist "D:\Working\_SWCM\SOURCE\_dotNetCommon\Assemblies\Internal\Eqecat.DatabaseInterface.v1.0.dll"   copy /y "D:\Working\_SWCM\SOURCE\_dotNetCommon\Assemblies\Internal\Eqecat.DatabaseInterface.v1.0.dll"   "D:\Working\Odyssey\Utilities\TDMEngine\bin\Release"
if exist "D:\Working\_SWCM\SOURCE\_dotNetCommon\Assemblies\Internal\Eqecat.ZipLibWrapper.dll"            copy /y "D:\Working\_SWCM\SOURCE\_dotNetCommon\Assemblies\Internal\Eqecat.ZipLibWrapper.dll"            "D:\Working\Odyssey\Utilities\TDMEngine\bin\Release"
if exist "D:\Working\_SWCM\SOURCE\_dotNetCommon\Assemblies\ThirdParty\SharpZipLib\net-20\ICSharpCode.SharpZipLib.dll"   copy /y "D:\Working\_SWCM\SOURCE\_dotNetCommon\Assemblies\ThirdParty\SharpZipLib\net-20\ICSharpCode.SharpZipLib.dll"   "D:\Working\Odyssey\Utilities\TDMEngine\bin\Release"
if exist "D:\Working\_SWCM\SOURCE\_dotNetCommon\Assemblies\Internal\WCeDBInterface.SQL.2005.dll"         copy /y "D:\Working\_SWCM\SOURCE\_dotNetCommon\Assemblies\Internal\WCeDBInterface.SQL.2005.dll"         "D:\Working\Odyssey\Utilities\TDMEngine\bin\Release"

:end_copy
