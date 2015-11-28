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

		 


		public static void Main (string[] args)
		{
			if (args.Length != 2 ) {
				Console.WriteLine (" Usage: ILSpyMac  directory/to/all/your/dll slnName");
				Console.WriteLine (" made it run at all platform support mono.");
				Console.WriteLine (" by aerror 2015/11/27");
				return;
			}

			string appPath = args [0];
			string slnName = args [1];
			Console.WriteLine ("Decompiling all dll in  " + appPath);
			Console.WriteLine ("Please wait...");

			DirectoryInfo di = new DirectoryInfo(appPath);
			FileInfo[] allAssemblies = di.GetFiles("*.dll");
			AssemblyList asmlist = new AssemblyList ("mylistname");

			foreach (var assemblyFile in allAssemblies)
			{
				asmlist.OpenAssembly (assemblyFile.FullName);
			}

			StringBuilder projSln = new StringBuilder ();
			projSln.Append ("Microsoft Visual Studio Solution File, Format Version 11.00\n# Visual Studio 2010\n");

			StringBuilder globSec = new StringBuilder ();
			Guid slnProjGuid =  Guid.NewGuid();

			int num = 0;
			LoadedAssembly [] ls = asmlist.GetAssemblies ();
			foreach (LoadedAssembly asm in ls) {
				num++;
				Console.WriteLine(asm.FileName + " " + num+"/"+ls.Length);
				string projectPath = appPath + "/"+ asm.ShortName;
				Directory.CreateDirectory (projectPath);
				string projectFileName = projectPath + "/" + asm.ShortName + ".csproj";
				var csharpLanguage = new CSharpLanguage ();
				var textOutput = new PlainTextOutput ();
				var decompilationOptions = new DecompilationOptions ();
				decompilationOptions.FullDecompilation = true;
				decompilationOptions.SaveAsProjectDirectory = projectPath;
				decompilationOptions.DecompilerSettings = new DecompilerSettings ();
				csharpLanguage.DecompileAssembly (asm, textOutput, decompilationOptions);
				File.WriteAllText (projectFileName, textOutput.ToString ());

			
				Guid createdProjGuid = decompilationOptions.createdProjectGuid;
				
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
