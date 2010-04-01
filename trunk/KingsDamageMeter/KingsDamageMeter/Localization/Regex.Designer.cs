﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KingsDamageMeter.Localization {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Regex {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Regex() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("KingsDamageMeter.Localization.Regex", typeof(Regex).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to \[charname:.
        /// </summary>
        internal static string Chat {
            get {
                return ResourceManager.GetString("Chat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  Effect.
        /// </summary>
        internal static string Effect {
            get {
                return ResourceManager.GetString("Effect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (?&lt;name&gt;.+) has joined your group\..
        /// </summary>
        internal static string JoinedGroupRegex {
            get {
                return ResourceManager.GetString("JoinedGroupRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (?&lt;name&gt;.+) has been kicked out of your group\..
        /// </summary>
        internal static string KickedFromGroupRegex {
            get {
                return ResourceManager.GetString("KickedFromGroupRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (?&lt;name&gt;.+) has left your group\..
        /// </summary>
        internal static string LeftGroupRegex {
            get {
                return ResourceManager.GetString("LeftGroupRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (?&lt;name&gt;.+) used (?&lt;skill&gt;.+) to inflict the continuous damage effect on (?&lt;target&gt;.+)\..
        /// </summary>
        internal static string OtherContinuousRegex {
            get {
                return ResourceManager.GetString("OtherContinuousRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (?&lt;target&gt;.+) is bleeding because (?&lt;name&gt;.+) used (?&lt;skill&gt;.+)\..
        /// </summary>
        internal static string OtherInflictedBleedRegex {
            get {
                return ResourceManager.GetString("OtherInflictedBleedRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (?&lt;name&gt;.+) inflicted (?&lt;damage&gt;[^a-zA-Z]+) damage on (?&lt;target&gt;.+)\..
        /// </summary>
        internal static string OtherInflictedRegex {
            get {
                return ResourceManager.GetString("OtherInflictedRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (?&lt;name&gt;.+) inflicted (?&lt;damage&gt;[^a-zA-Z]+) damage on (?&lt;target&gt;.+)( and caused the pattern engraving effect)? by using (?&lt;skill&gt;.+)\..
        /// </summary>
        internal static string OtherInflictedSkillRegex {
            get {
                return ResourceManager.GetString("OtherInflictedSkillRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (?&lt;name&gt;.+) received (?&lt;damage&gt;[^a-zA-Z]+) bleeding damage after you used (?&lt;skill&gt;.+)\..
        /// </summary>
        internal static string OtherReceivedBleedRegex {
            get {
                return ResourceManager.GetString("OtherReceivedBleedRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (?&lt;name&gt;.+) received (?&lt;damage&gt;[^a-zA-Z]+) damage from (?&lt;target&gt;.+)\..
        /// </summary>
        internal static string OtherReceivedRegex {
            get {
                return ResourceManager.GetString("OtherReceivedRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (?&lt;name&gt;.+) received (?&lt;damage&gt;[^a-zA-Z]+) damage due to the effect of (?&lt;skill&gt;.+)\..
        /// </summary>
        internal static string OtherReceivedSkillRegex {
            get {
                return ResourceManager.GetString("OtherReceivedSkillRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (?&lt;name&gt;.+) has summoned (?&lt;pet&gt;.+) to attack (?&lt;target&gt;.+) by using (?&lt;skill&gt;.+)\..
        /// </summary>
        internal static string OtherSummonedAttackRegex {
            get {
                return ResourceManager.GetString("OtherSummonedAttackRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (?&lt;name&gt;.+) summoned (?&lt;pet&gt;.+) by using (?&lt;skill&gt;.+)\..
        /// </summary>
        internal static string OtherSummonedRegex {
            get {
                return ResourceManager.GetString("OtherSummonedRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (?&lt;time&gt;[^a-zA-Z]+) : .
        /// </summary>
        internal static string TimestampRegex {
            get {
                return ResourceManager.GetString("TimestampRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You inflicted continuous damage on (?&lt;target&gt;.+) by using (?&lt;skill&gt;.+)\..
        /// </summary>
        internal static string YouContinuousRegex {
            get {
                return ResourceManager.GetString("YouContinuousRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Critical Hit! You inflicted (?&lt;damage&gt;[^a-zA-Z]+) critical damage on (?&lt;target&gt;.+)\..
        /// </summary>
        internal static string YouCriticalRegex {
            get {
                return ResourceManager.GetString("YouCriticalRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have earned (?&lt;kinah&gt;[^a-zA-Z]+) Kinah\..
        /// </summary>
        internal static string YouEarnedKinahRegex {
            get {
                return ResourceManager.GetString("YouEarnedKinahRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (?&lt;target&gt;.+) received (?&lt;damage&gt;[^a-zA-Z]+)( poisoning)? damage after you used (?&lt;effect&gt;.+)\..
        /// </summary>
        internal static string YouEffectDamageRegex {
            get {
                return ResourceManager.GetString("YouEffectDamageRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have gained (?&lt;ap&gt;[^a-zA-Z]+) Abyss Points\..
        /// </summary>
        internal static string YouGainedApRegex {
            get {
                return ResourceManager.GetString("YouGainedApRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You received the effect by using (?&lt;effect&gt;.+)\..
        /// </summary>
        internal static string YouGainedEffectRegex {
            get {
                return ResourceManager.GetString("YouGainedEffectRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have gained (?&lt;exp&gt;[^a-zA-Z]+) EXP from (?&lt;target&gt;.+)\..
        /// </summary>
        internal static string YouGainedExpRegex {
            get {
                return ResourceManager.GetString("YouGainedExpRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have joined the (?&lt;region&gt;.+) region channel\..
        /// </summary>
        internal static string YouHaveJoinedChannelRegex {
            get {
                return ResourceManager.GetString("YouHaveJoinedChannelRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (?&lt;target&gt;.+) is bleeding because You used (?&lt;skill&gt;.+)\..
        /// </summary>
        internal static string YouInflictedBleedRegex {
            get {
                return ResourceManager.GetString("YouInflictedBleedRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You inflicted (?&lt;damage&gt;[^a-zA-Z]+) damage on (?&lt;target&gt;.+)\..
        /// </summary>
        internal static string YouInflictedRegex {
            get {
                return ResourceManager.GetString("YouInflictedRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You inflicted (?&lt;damage&gt;[^a-zA-Z]+) damage( and the rune carve effect)? on (?&lt;target&gt;.+) by using (?&lt;skill&gt;.+)\..
        /// </summary>
        internal static string YouInflictedSkillRegex {
            get {
                return ResourceManager.GetString("YouInflictedSkillRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You received (?&lt;damage&gt;[^a-zA-Z]+) damage from (?&lt;target&gt;.+)\..
        /// </summary>
        internal static string YouReceivedRegex {
            get {
                return ResourceManager.GetString("YouReceivedRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (?&lt;target&gt;.+) has inflicted (?&lt;damage&gt;[^a-zA-Z]+) damage on you by using (?&lt;skill&gt;.+)\..
        /// </summary>
        internal static string YouReceivedSkillRegex {
            get {
                return ResourceManager.GetString("YouReceivedSkillRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You spent (?&lt;kinah&gt;[^a-zA-Z]+) Kinah\..
        /// </summary>
        internal static string YouSpentKinahRegex {
            get {
                return ResourceManager.GetString("YouSpentKinahRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You summoned (?&lt;pet&gt;.+) by using (?&lt;skill&gt;.+) to let it attack (?&lt;target&gt;.+)\..
        /// </summary>
        internal static string YouSummonedAttackRegex {
            get {
                return ResourceManager.GetString("YouSummonedAttackRegex", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You summoned (?&lt;pet&gt;.+) by using (?&lt;skill&gt;.+)\..
        /// </summary>
        internal static string YouSummonedRegex {
            get {
                return ResourceManager.GetString("YouSummonedRegex", resourceCulture);
            }
        }
    }
}
