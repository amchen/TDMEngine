﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TDMEngine {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.10.0.0")]
    internal sealed partial class TDMEngineSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static TDMEngineSettings defaultInstance = ((TDMEngineSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new TDMEngineSettings())));
        
        public static TDMEngineSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"Migration failed, view the log for detailed information.\n\nThe log file for the Restore may be viewed by going into Batch Manager and clicking on the failed job in the top grid. Then go to the bottom grid and click on the job step with the Status = """"F"""". Right click and select View Log on the pop-up menu. Search the log for the word """"Error"""" to locate the reason why the restore failed.")]
        public string TDMFailureMsg {
            get {
                return ((string)(this["TDMFailureMsg"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CheckDate {
            get {
                return ((bool)(this["CheckDate"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("select * from TDMCTRL where DESTBUILD > \'{0}\' and DESTBUILD<=\'{1}\' order by KEY")]
        public string A_SQL_GetDestBuilds {
            get {
                return ((string)(this["A_SQL_GetDestBuilds"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("@(#)")]
        public string StringToReplace {
            get {
                return ((string)(this["StringToReplace"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("select count(*) as CNT from {0}")]
        public string A_SQL_GetMigrStepCount {
            get {
                return ((string)(this["A_SQL_GetMigrStepCount"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("select A.KEY,{1} SEQ,DEFECT,REM_SQL,TNAME,CMD_SQL,DEPEND,NOTES,A.TYPE,DESCR \r\n fr" +
            "om {0} a, TDMTYPE b where a.type=b.type order by  A.KEY\r\n")]
        public string A_SQL_GetTDMTableData {
            get {
                return ((string)(this["A_SQL_GetTDMTableData"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("if exists (select 1 from SYS.TABLES where NAME = \'{0}\') drop table {0}")]
        public string SQL_DropTable {
            get {
                return ((string)(this["SQL_DropTable"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("if exists (select 1 from SYS.TABLES where NAME = \'{0}\')  alter table {0} {1};")]
        public string SQL_AlterTable {
            get {
                return ((string)(this["SQL_AlterTable"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("absp_Util_CreateTableScript")]
        public string CreateTableScript_ProcName {
            get {
                return ((string)(this["CreateTableScript_ProcName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("if exists (select 1 from SYS.INDEXES where OBJECT_ID = OBJECT_ID(\'{0}\') and NAME " +
            "= \'{1}\') drop index {0}.{1};")]
        public string SQL_DropIndex {
            get {
                return ((string)(this["SQL_DropIndex"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("if exists (select 1 from sysobjects where name = \'{0}\' and type = \'P\')  drop proc" +
            "edure {0}")]
        public string SQL_DropProc {
            get {
                return ((string)(this["SQL_DropProc"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("if exists ( select 1 from SYSOBJECTS where NAME = \'absp_MessageEx\' and type = \'P\'" +
            " )    exec absp_MessageEx \'{0}\' ")]
        public string SQL_DBMsg {
            get {
                return ((string)(this["SQL_DBMsg"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("if exists ( select 1 from SYSOBJECTS where NAME = \'absp_Util_SetDBVersion\' and ty" +
            "pe = \'P\' ) \r\nbegin\r\n  exec absp_Util_SetDBVersion \'{0}\',\'{1}\',\'{2}\',\'{3}\',\'{4}\'," +
            "\'{5}\',\'{6}\'\r\nend ")]
        public string SQL_SetDBVersion {
            get {
                return ((string)(this["SQL_SetDBVersion"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(" if exists ( select 1 from SYSOBJECTS where NAME = \'absp_Util_GetDBVersion\' and t" +
            "ype = \'P\' ) \r\n begin   \r\n exec absp_Util_GetDBVersion \r\n end")]
        public string SQL_GetDBVersion {
            get {
                return ((string)(this["SQL_GetDBVersion"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("tdm.accdb")]
        public string TDMFileName {
            get {
                return ((string)(this["TDMFileName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("select distinct TABLENAME, {0} from EqecatSystem..DICTTBL where LOCATION in (\'B\'," +
            "\'S\')  and \r\n{0} in (\'Y\',\'L\')")]
        public string SQL_GetAllTableNames {
            get {
                return ((string)(this["SQL_GetAllTableNames"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ZZPROCS_MSSQL")]
        public string StoredProcPath {
            get {
                return ((string)(this["StoredProcPath"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("if exists ( select 1 from SYSOBJECTS where NAME = \'absp_Migr_ReloadTableEx\' and t" +
            "ype = \'P\' )\r\nbegin\r\n    exec absp_Migr_ReloadTableEx \'{0}\',\'{1}\',\'\',\'{2}\',\'{3}\'\r" +
            "\nend")]
        public string SQL_Migr_ReloadTable_PROC {
            get {
                return ((string)(this["SQL_Migr_ReloadTable_PROC"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("select 1 from SYS.TABLES where NAME = \'{0}\'")]
        public string SQL_CheckIfTableExists {
            get {
                return ((string)(this["SQL_CheckIfTableExists"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("if exists(select 1 from sys.databases where name = \'{0}\' )\r\nbegin\r\nUSE master;\r\nA" +
            "LTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;\r\nALTER DATABASE {0} S" +
            "ET {1} ;ALTER DATABASE {0} SET MULTI_USER;\r\nend;\r\n")]
        public string SQL_SetDBModeReadOnly {
            get {
                return ((string)(this["SQL_SetDBModeReadOnly"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DataDict.xlsx")]
        public string DataDictFileName {
            get {
                return ((string)(this["DataDictFileName"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("bcp.exe")]
        public string BCP_APP_NAME {
            get {
                return ((string)(this["BCP_APP_NAME"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <s" +
            "tring>|</string>\r\n  <string>,</string>\r\n</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection DelimitersUsed {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["DelimitersUsed"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public int MinCountForDelimiters {
            get {
                return ((int)(this["MinCountForDelimiters"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("select TableType from eqecatSystem..DICTTBL where TableName=\'{0}\'")]
        public string SQL_GetTableGroup {
            get {
                return ((string)(this["SQL_GetTableGroup"]));
            }
            set {
                this["SQL_GetTableGroup"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("SET ANSI_PADDING OFF;SET ANSI_NULL_DFLT_ON ON;SET ANSI_NULLS ON;")]
        public string CreateTablePrefix {
            get {
                return ((string)(this["CreateTablePrefix"]));
            }
            set {
                this["CreateTablePrefix"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("tdm_msg.log")]
        public string TDM_MsgLogFile {
            get {
                return ((string)(this["TDM_MsgLogFile"]));
            }
            set {
                this["TDM_MsgLogFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("absp_Util_GetEnvironmentVariable")]
        public string SQL_GetEnvironmentVar_PROC {
            get {
                return ((string)(this["SQL_GetEnvironmentVar_PROC"]));
            }
            set {
                this["SQL_GetEnvironmentVar_PROC"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\TDM_UnloadPath")]
        public string TDM_UnloadPath {
            get {
                return ((string)(this["TDM_UnloadPath"]));
            }
            set {
                this["TDM_UnloadPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("select TableName, CloneName from dictClon where Sys_DB=\'L\'")]
        public string SQL_GetSystemLookup {
            get {
                return ((string)(this["SQL_GetSystemLookup"]));
            }
            set {
                this["SQL_GetSystemLookup"] = value;
            }
        }
    }
}
