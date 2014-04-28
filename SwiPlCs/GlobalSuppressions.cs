// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File".
// You do not need to add suppressions to this file manually.


// CA5122 rule is turn off in CodeAnalyzeRuleSet.ruleset. See "Bogus CA5122 warning about P/Invoke declarations should not be safe-critical"
// http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-saf

// Rules which are swiched off
// CA1303:Do not pass literals as localized parameters

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Postcondition", Scope = "member", Target = "SbsSW.DesignByContract.Check.#Ensure(System.Boolean)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PLhalt", Scope = "member", Target = "SbsSW.SwiPlCs.libpl.#PL_halt(System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PlTerm", Scope = "member", Target = "SbsSW.SwiPlCs.PlTerm.#TermRef")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PlVar", Scope = "member", Target = "SbsSW.SwiPlCs.PlTerm.#TermRef")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PlTerm", Scope = "member", Target = "SbsSW.SwiPlCs.PlTerm.#Item[System.Int32]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PLgetarg", Scope = "member", Target = "SbsSW.SwiPlCs.PlTerm.#Item[System.Int32]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PlVar", Scope = "member", Target = "SbsSW.SwiPlCs.PlTermV.#Item[System.Int32]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PlTerm", Scope = "member", Target = "SbsSW.SwiPlCs.PlTermV.#Item[System.Int32]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PlEngine", Scope = "member", Target = "SbsSW.SwiPlCs.PlEngine.#Initialize(System.String[])")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "stdin", Scope = "member", Target = "SbsSW.SwiPlCs.PlEngine.#Sread_function(System.IntPtr,System.IntPtr,System.Int64)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PLcreateengine", Scope = "member", Target = "SbsSW.SwiPlCs.PlMtEngine.#.ctor()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PlSetEngine", Scope = "member", Target = "SbsSW.SwiPlCs.PlMtEngine.#PlSetEngine()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PlSetEngine", Scope = "member", Target = "SbsSW.SwiPlCs.PlMtEngine.#PlDetachEngine()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PlEngine", Scope = "member", Target = "SbsSW.SwiPlCs.PrologServer.#.ctor(System.Int32,System.String[])")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PlEngine", Scope = "member", Target = "SbsSW.SwiPlCs.PlMtEngine.#.ctor()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "readtermfromatom", Scope = "member", Target = "SbsSW.SwiPlCs.PlQuery.#.ctor(System.String,System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PlCall", Scope = "member", Target = "SbsSW.SwiPlCs.PlQuery.#.ctor(System.String,System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PlQuery", Scope = "member", Target = "SbsSW.SwiPlCs.PlQuery.#.ctor(System.String,System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PLgetarg", Scope = "member", Target = "SbsSW.SwiPlCs.PlQuery.#.ctor(System.String,System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PlQuery", Scope = "member", Target = "SbsSW.SwiPlCs.PlQuery.#NextSolution()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "NextSolution", Scope = "member", Target = "SbsSW.SwiPlCs.PlQuery.#NextSolution()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PlQuerySwitch", Scope = "member", Target = "SbsSW.SwiPlCs.PlQuery.#Query(SbsSW.SwiPlCs.PlQuerySwitch)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PLhalt", Scope = "member", Target = "SbsSW.SwiPlCs.LibPl.#PL_halt(System.Int32)")]
