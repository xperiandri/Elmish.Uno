using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SolutionTemplate;

//-:cnd:noEmit
#if(__ANDROID__ || __IOS__ || __MACOS__ || WINDOWS_UWP)

[assembly: AssemblyCompany(AssemblyInfo.Company)]
[assembly: AssemblyProduct(AssemblyInfo.Product)]
[assembly: AssemblyCopyright("Copyright © " + AssemblyInfo.Company + " 2021")]
[assembly: AssemblyTrademark("")]

namespace SolutionTemplate
{
    internal static class AssemblyInfo
    {
        internal const string AssemblyBaseName = "Simple template";
        internal const string Company = "Template company";
        internal const string Product = "Template";
        internal const string Description = " It's Uno template";
    }
}
#endif
//+:cnd:noEmit
