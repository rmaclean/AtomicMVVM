///-----------------------------------------------------------------------
/// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
/// License: MS-PL http://www.opensource.org/licenses/MS-PL
/// Notes:
///-----------------------------------------------------------------------

using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("AtomicMVVM")]
[assembly: AssemblyCompany("Robert MacLean")]
[assembly: AssemblyProduct("AtomicMVVM")]
[assembly: AssemblyCopyright("Copyright © Robert MacLean 2012")]
[assembly:System.CLSCompliant(false)]
[assembly:System.Resources.NeutralResourcesLanguage("en")]
[assembly: ComVisible(false)]
[assembly: Guid("dd623e44-e327-437d-8462-00676ccd42f0")]
#if !(SILVERLIGHT || NETFX_CORE)
[assembly: System.Security.SecurityRules(System.Security.SecurityRuleSet.Level2)]
#endif
[assembly: AssemblyVersion("5.0.0.0")]
[assembly: AssemblyFileVersion("5.0.0.0")]
