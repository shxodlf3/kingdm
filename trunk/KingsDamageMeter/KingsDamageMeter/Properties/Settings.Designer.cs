﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KingsDamageMeter.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public double WindowMainX {
            get {
                return ((double)(this["WindowMainX"]));
            }
            set {
                this["WindowMainX"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public double WindowMainY {
            get {
                return ((double)(this["WindowMainY"]));
            }
            set {
                this["WindowMainY"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public double WindowMainOpacity {
            get {
                return ((double)(this["WindowMainOpacity"]));
            }
            set {
                this["WindowMainOpacity"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool WindowMainTopMost {
            get {
                return ((bool)(this["WindowMainTopMost"]));
            }
            set {
                this["WindowMainTopMost"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Program Files\\NCSoft\\Aion\\Chat.log")]
        public string AionLogPath {
            get {
                return ((string)(this["AionLogPath"]));
            }
            set {
                this["AionLogPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.ObjectModel.ObservableCollection<string> IgnoreList {
            get {
                return ((global::System.Collections.ObjectModel.ObservableCollection<string>)(this["IgnoreList"]));
            }
            set {
                this["IgnoreList"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.ObjectModel.ObservableCollection<string> GroupList {
            get {
                return ((global::System.Collections.ObjectModel.ObservableCollection<string>)(this["GroupList"]));
            }
            set {
                this["GroupList"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Documentation.pdf")]
        public string HelpFilePath {
            get {
                return ((string)(this["HelpFilePath"]));
            }
            set {
                this["HelpFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("196")]
        public int WindowMainWidth {
            get {
                return ((int)(this["WindowMainWidth"]));
            }
            set {
                this["WindowMainWidth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("105")]
        public int WindowMainHeight {
            get {
                return ((int)(this["WindowMainHeight"]));
            }
            set {
                this["WindowMainHeight"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("yyyy.MM.dd hh:mm:ss")]
        public string LogTimeFormat {
            get {
                return ((string)(this["LogTimeFormat"]));
            }
            set {
                this["LogTimeFormat"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Debug {
            get {
                return ((bool)(this["Debug"]));
            }
            set {
                this["Debug"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Debug.log")]
        public string DebugFile {
            get {
                return ((string)(this["DebugFile"]));
            }
            set {
                this["DebugFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsGroupOnly {
            get {
                return ((bool)(this["IsGroupOnly"]));
            }
            set {
                this["IsGroupOnly"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("None")]
        public global::KingsDamageMeter.Controls.PlayerSortType SortType {
            get {
                return ((global::KingsDamageMeter.Controls.PlayerSortType)(this["SortType"]));
            }
            set {
                this["SortType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Damage")]
        public global::KingsDamageMeter.Controls.DisplayType DisplayType {
            get {
                return ((global::KingsDamageMeter.Controls.DisplayType)(this["DisplayType"]));
            }
            set {
                this["DisplayType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsHideOthers {
            get {
                return ((bool)(this["IsHideOthers"]));
            }
            set {
                this["IsHideOthers"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public string GroupOnly {
            get {
                return ((string)(this["GroupOnly"]));
            }
            set {
                this["GroupOnly"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public string HideOthers {
            get {
                return ((string)(this["HideOthers"]));
            }
            set {
                this["HideOthers"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("(Default)")]
        public global::System.Globalization.CultureInfo SelectedLanguage {
            get {
                return ((global::System.Globalization.CultureInfo)(this["SelectedLanguage"]));
            }
            set {
                this["SelectedLanguage"] = value;
            }
        }
    }
}
