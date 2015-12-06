using System;
using System.IO;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using System.Text;
using ICSharpCode.ILSpy;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;

namespace Decomplier
{
	
	class MainClass
	{

		private const string LAN_TYPE_CSHARP = "csharp";
		private const string LAN_TYPE_IL	 = "il";
		private static void showUsage()
		{
			Console.WriteLine (" Usage: ILSpyMac [options] directory/to/all/your/dll ");
			Console.WriteLine (" made it run at all platform support mono.");
			Console.WriteLine (" by aerror 2015/11/27");
			Console.WriteLine (" options:");
			Console.WriteLine ("       -n  Solution Name");
			Console.WriteLine ("       -l  References dll path which dll will be loaded but not decompile , they use as References.");
			Console.WriteLine ("       -t  Output language type, accept il or csharp, default is csharp.");

			Console.WriteLine ("       -a  Decompile yield. OFF if exists this option, default ON.");
			Console.WriteLine ("       -b  Decompile anonymous methods/lambdas. OFF  if exists this option, default ON. ");
			Console.WriteLine ("       -c  Decompile asyncwait. OFF  if exists this option, default ON. ");
			Console.WriteLine ("       -d  Decompile automatic events. OFF  if exists this option, default ON. ");

			Console.WriteLine ("       -e  Decompile expression trees. OFF  if exists this option, default ON. ");
			Console.WriteLine ("       -f  Decompile automatic properties. OFF  if exists this option, default ON. ");
			Console.WriteLine ("       -g  Decompile using statements if. OFF  exists this option, default ON. ");
			Console.WriteLine ("       -h  Decompile foreach statements. OFF  if exists this option, default ON. ");

			Console.WriteLine ("       -i  Decompile lock statements if. OFF  exists this option, default ON. ");
			Console.WriteLine ("       -j  Decompile SwitchStatement On String. OFF  if exists this option, default ON. ");
			Console.WriteLine ("       -k  Decompile Using Declarations. OFF  if exists this option, default ON. ");
			Console.WriteLine ("       -r  Decompile query Expressions. OFF  if exists this option, default ON. ");

			Console.WriteLine ("       -s  Decompile fully Qualify Ambiguous Type Names. OFF  if exists this option, default ON. ");
			Console.WriteLine ("       -p  Use variable names from debug symbols, if available. OFF  if exists this option, default ON. ");
			Console.WriteLine ("       -x  Use C# 3.0 object/collection initializers. OFF if exists this option, default ON. ");
			Console.WriteLine ("       -y  Include XML documentation comments in the decompiled code. OFF  if exists this option, default ON.");
			Console.WriteLine ("       -z  Fold braces. ON if exists this option, default OFF ");

			Console.WriteLine ("       -C  class Name ");


			Console.WriteLine (" Example:");
			Console.WriteLine (" ILSpyMac -n Example -l /directory/to/Rerences/dll /directory/to/all/your/dll");


		}


		public static bool praseDecompileSetting(char c, DecompilerSettings ds)
		{
			switch (c) {
			case 'a':
				ds.YieldReturn = false;
				break;
			case 'b':
				ds.AnonymousMethods = false;
				break;
			case 'c':
				ds.AsyncAwait = false;
				break;
			case 'd':
				ds.AutomaticEvents = false;
				break;


			case 'e':
				ds.ExpressionTrees = false;
				break;

			case 'f':
				ds.AutomaticProperties = false;
				break;

			case 'g':
				ds.UsingStatement = false;
				break;
			case 'h':
				ds.ForEachStatement = false;
				break;
			
			case 'i':
				ds.LockStatement = false;
				break;
			case 'j':
				ds.SwitchStatementOnString = false;
				break;
			case 'k':
				ds.UsingDeclarations = false;
				break;
			case 'r':
				ds.QueryExpressions = false;
				break;

			case 's':
				ds.FullyQualifyAmbiguousTypeNames = false;
				break;
			case 'p':
				ds.UseDebugSymbols = false;
				break;
			case 'x':
				ds.ObjectOrCollectionInitializers = false;
				break;
			case 'y':
				ds.ShowXmlDocumentation = false;
				break;
			case 'z':
				ds.FoldBraces = true;
				break;
			default:
				return false;
			}

			return true;
		}

		public static void Main (string[] args)
		{

			string appPath = null;
			string slnName = null;
			string libPath = null;
			string expOpt = null;
			string outLanguageType = LAN_TYPE_CSHARP;
			DecompilerSettings ds = new DecompilerSettings ();
			ds.AnonymousMethods = true;
			ds.AsyncAwait = true;
			ds.YieldReturn = true;
			string onlyDecomileClassName = null;
			//parsing args
			foreach (string x in args) {

				if (expOpt == null) {
					switch (x) {
					case "-n":
					case "-l":
					case "-t":
					case "-C":
						expOpt = x;
						continue;
					
					default:

						if (x.StartsWith ("-")) {

							if (x.Length < 2) {
								Console.WriteLine (" Unexpected options " + x);
								showUsage ();
								return;
							}

							for (int i = 0; i < x.Length; i++) {
								if (!praseDecompileSetting (x [i], ds)) {
									Console.WriteLine (" Unexpected options " + x);
									showUsage ();
									return;
								}

							}
							continue;
						} 

						if (appPath == null) {
							appPath = x;
							continue;
						} else {
							Console.WriteLine (" Unexpected options " + x);
							showUsage ();
							return;
						}


					}

				} else {

					switch (expOpt) {
					case "-n":
						slnName = x;

						break;
					case "-l":
						libPath = x;
						break;
					case "-t":
						if(x!=LAN_TYPE_CSHARP && x!=LAN_TYPE_IL)
						{
							Console.WriteLine (" Unexpected Output language type: " + x);
							showUsage();
							return ;
						}
						outLanguageType = x;

						break;
					case "-C":
						onlyDecomileClassName = x;
						break;
					default:
						showUsage ();
						return;
					
					}
					expOpt = null;

				}
					
			}


			if (appPath == null) {
			
				Console.WriteLine ("directory/to/all/your/dll missing");
				showUsage ();
				return;
			}

			if (slnName == null && outLanguageType==LAN_TYPE_CSHARP) {

				Console.WriteLine ("Solution Name missing");
				showUsage ();
				return;
			}


			Console.WriteLine ("Decompiling all dll in  " + appPath);
			Console.WriteLine ("Please wait...");

			DirectoryInfo di = new DirectoryInfo(appPath);
			appPath = di.FullName;
			FileInfo[] dllFileInfoList = di.GetFiles("*.dll");
			FileInfo[] exeFileInfoList = di.GetFiles ("*.exe");


			AssemblyList asmlist = new AssemblyList ("mylistname");

			foreach (var dllfile in dllFileInfoList)
			{
				asmlist.OpenAssembly (dllfile.FullName);
			}

			foreach (var dllfile in exeFileInfoList)
			{
				asmlist.OpenAssembly (dllfile.FullName);
			}

			if (libPath != null) {
				di = new DirectoryInfo(libPath);
				libPath = di.FullName;
				dllFileInfoList = di.GetFiles("*.dll");
				foreach (var dllfile in dllFileInfoList) {
					asmlist.OpenAssembly (dllfile.FullName,true);
				}
			}
			


			StringBuilder projSln = new StringBuilder ();
			projSln.Append ("Microsoft Visual Studio Solution File, Format Version 11.00\n# Visual Studio 2010\n");

			StringBuilder globSec = new StringBuilder ();
			Guid slnProjGuid =  Guid.NewGuid();

			int num = 0;
			LoadedAssembly [] ls = asmlist.GetAssemblies ();
			var decompilationOptions = new DecompilationOptions ();
			decompilationOptions.FullDecompilation = true;
			decompilationOptions.assenmlyList = asmlist;
			decompilationOptions.DecompilerSettings = ds;
			decompilationOptions.IncludedClassName = onlyDecomileClassName;

			if(outLanguageType==LAN_TYPE_CSHARP)
			{
				foreach (LoadedAssembly asm in ls) {
					if (asm.IsAutoLoaded)
						continue;




					string projectPath = appPath + "/"+ asm.ShortName;
					if(!Directory.Exists(projectPath))
					{
						Directory.CreateDirectory (projectPath);
					}
					string projectFileName = projectPath + "/" + asm.ShortName + ".csproj";
					asm.ProjectGuid = Guid.NewGuid();
					asm.ProjectFileName = projectFileName;
				}
			}



			foreach (LoadedAssembly asm in ls) {
				num++;
				Console.WriteLine(asm.FileName + " " + num+"/"+ls.Length);
				if (asm.IsAutoLoaded)
					continue;

				if(outLanguageType==LAN_TYPE_CSHARP)
				{
					var csharpLanguage = new CSharpLanguage ();
					var textOutput = new PlainTextOutput ();
					decompilationOptions.SaveAsProjectDirectory =  appPath + "/"+ asm.ShortName;

					csharpLanguage.DecompileAssembly (asm, textOutput, decompilationOptions);
					File.WriteAllText (asm.ProjectFileName, textOutput.ToString ());

					
					Guid createdProjGuid = asm.ProjectGuid;
					
					projSln.Append("   Project(\"{");
					projSln.Append (slnProjGuid.ToString());
					projSln.Append ("}\") = \"");
					projSln.Append (asm.ShortName);
					projSln.Append ("\", \"");
					projSln.Append (asm.ShortName+"/"+ asm.ShortName + ".csproj");
					projSln.Append ("\", \"{");
					projSln.Append (createdProjGuid.ToString());
					projSln.Append ("}\"\n");
					projSln.Append("EndProject\n");

					globSec.Append ("   {"+createdProjGuid.ToString()+"}.Debug|Any CPU.ActiveCfg = Debug|Any CPU\n");
					globSec.Append ("   {"+createdProjGuid.ToString()+"}.Debug|Any CPU.Build.0 = Debug|Any CPU\n");
					globSec.Append ("   {"+createdProjGuid.ToString()+"}.Release|Any CPU.ActiveCfg = Release|Any CPU\n");
					globSec.Append ("   {"+createdProjGuid.ToString()+"}.Release|Any CPU.Build.0 = Release|Any CPU\n");
				}
				else
				{
					var ilLanguage = new ILLanguage(true);
					var textOutput = new PlainTextOutput ();
					ilLanguage.DecompileAssembly (asm, textOutput, decompilationOptions);
					string ilFileName = appPath + "/"+ asm.ShortName+".il";
					File.WriteAllText(ilFileName,textOutput.ToString());
				}

			}

			if (outLanguageType == LAN_TYPE_CSHARP) {
				projSln.Append ("Global\n");
				projSln.Append ("GlobalSection(SolutionConfigurationPlatforms) = preSolution\n");
				projSln.Append ("\t\t\t\tDebug|Any CPU = Debug|Any CPU\n");
				projSln.Append ("\t\t\t\tRelease|Any CPU = Release|Any CPU\n");
				projSln.Append ("EndGlobalSection\n");

				projSln.Append ("GlobalSection(ProjectConfigurationPlatforms) = postSolution\n");
				projSln.Append (globSec.ToString ());
				projSln.Append ("EndGlobalSection\n");

				projSln.Append ("GlobalSection(MonoDevelopProperties) = preSolution\n");
				projSln.Append ("\nEndGlobalSection\n");
				projSln.Append ("EndGlobal\n\t\t");

				string slnFileName = appPath + "/" + slnName + ".sln";
				File.WriteAllText (slnFileName, projSln.ToString ());
			}
		

		}
	}
}
