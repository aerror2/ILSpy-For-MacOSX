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

		private static void showUsage()
		{
			Console.WriteLine (" Usage: ILSpyMac [options] directory/to/all/your/dll ");
			Console.WriteLine (" made it run at all platform support mono.");
			Console.WriteLine (" by aerror 2015/11/27");
			Console.WriteLine (" options:");
			Console.WriteLine ("       -n  Solution Name");
			Console.WriteLine ("       -l  References dll path which dll will be loaded but not decompile , they use as References.");

			Console.WriteLine (" Example:");
			Console.WriteLine (" ILSpyMac -n Example -l /directory/to/Rerences/dll /directory/to/all/your/dll");


		}

		public static void Main (string[] args)
		{
			string appPath = null;
			string slnName = null;
			string libPath = null;
			string expOpt = null;

			//parsing args
			foreach (string x in args) {

				if (expOpt == null) {
					switch (x) {
					case "-n":
						expOpt = x;
						continue;
					case "-l":
						expOpt = x;
						continue;
					
					default:
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

			if (slnName == null) {

				Console.WriteLine ("Solution Name missing");
				showUsage ();
				return;
			}




			Console.WriteLine ("Decompiling all dll in  " + appPath);
			Console.WriteLine ("Please wait...");

			DirectoryInfo di = new DirectoryInfo(appPath);
			FileInfo[] dllFileInfoList = di.GetFiles("*.dll");
			AssemblyList asmlist = new AssemblyList ("mylistname");

			foreach (var dllfile in dllFileInfoList)
			{
				asmlist.OpenAssembly (dllfile.FullName);
			}

			if (libPath != null) {
				di = new DirectoryInfo(libPath);
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
			decompilationOptions.DecompilerSettings = new DecompilerSettings ();

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



			foreach (LoadedAssembly asm in ls) {
				num++;
				Console.WriteLine(asm.FileName + " " + num+"/"+ls.Length);
				if (asm.IsAutoLoaded)
					continue;

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


			projSln.Append("Global\n");
				projSln.Append ("GlobalSection(SolutionConfigurationPlatforms) = preSolution\n");
				projSln.Append ("\t\t\t\tDebug|Any CPU = Debug|Any CPU\n");
				projSln.Append ("\t\t\t\tRelease|Any CPU = Release|Any CPU\n");
				projSln.Append ("EndGlobalSection\n");

				projSln.Append("GlobalSection(ProjectConfigurationPlatforms) = postSolution\n");
				projSln.Append(globSec.ToString());
				projSln.Append("EndGlobalSection\n");

				projSln.Append("GlobalSection(MonoDevelopProperties) = preSolution\n");
			    projSln.Append("\nEndGlobalSection\n");
			projSln.Append("EndGlobal\n\t\t");

			string slnFileName = appPath + "/" + slnName + ".sln";
			File.WriteAllText (slnFileName, projSln.ToString ());

		

		}
	}
}
