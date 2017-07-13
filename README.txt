
Overview:


Make ILSpy a command line tools for MacOsx ,Linux and any mono supported  platform, because ILSpy does not work in MacOsx.

It's import from https://github.com/icsharpcode/ILSpy
So thanks to ILSpy Contributors:
	Daniel Grunwald
	David Srbecky
	Ed Harvey
	Siegfried Pammer
	Artur Zgodzinski
	Eusebiu Marcu
	Pent Ploompuu

They did a great work.


New Features for ILSpy-ForMacOSX:
1. add Couroutine decompiling feature for mono's dll, the original ILSPY always failed for mono's dll which mostly uses in Unity3D's game. 
2. works on command line;
3. Decompile all dll in the input arguments and generate the project files and solution files just simply by one simple command line.
   I like that style instead of a GUI. It's better that reading the codes in the IDE than reading it in the ILSpy GUI, more features help you understand the decompiled codes.


Usage: ILSpyMac [options] directory/to/all/your/dll 
 made it run at all platform support mono.
 by aerror 2015/11/27
 options:
       -a  Decompile yield. OFF if exists this option, default ON.
       -b  Decompile anonymous methods/lambdas. OFF  if exists this option, default ON. 
       -c  Decompile asyncwait. OFF  if exists this option, default ON. 
       -d  Decompile automatic events. OFF  if exists this option, default ON. 
       -e  Decompile expression trees. OFF  if exists this option, default ON. 
       -f  Decompile automatic properties. OFF  if exists this option, default ON. 
       -g  Decompile using statements if. OFF  exists this option, default ON. 
       -h  Decompile foreach statements. OFF  if exists this option, default ON. 
       -i  Decompile lock statements if. OFF  exists this option, default ON. 
       -j  Decompile SwitchStatement On String. OFF  if exists this option, default ON. 
       -k  Decompile Using Declarations. OFF  if exists this option, default ON. 
       -l  References dll path which dll will be loaded but not decompile , they use as References.
       -n  Solution Name
       -r  Decompile query Expressions. OFF  if exists this option, default ON. 
       -s  Decompile fully Qualify Ambiguous Type Names. OFF  if exists this option, default ON. 
       -t  Output language type, accept il or csharp, default is csharp.
       -p  Use variable names from debug symbols, if available. OFF  if exists this option, default ON. 
       -x  Use C# 3.0 object/collection initializers. OFF if exists this option, default ON. 
       -y  Include XML documentation comments in the decompiled code. OFF  if exists this option, default ON.
       -z  Fold braces. ON if exists this option, default OFF 
       -C  class Name 
       -D  Ony specitfied files to do decompiling in the Directory , should be the last option, for examaple: ILSpyMac -n Example /directory/to/all/your/dll -D main.dll
 Example:
 	ILSpyMac -n Example -l /directory/to/Rerences/dll /directory/to/all/your/dll


How to build:

Open the ILSpyMac.sln by MonoDevelop or Xamarin, then click the build button.

.NET 4.0 required.

Check your Mono.Framework at /Library/Frameworks, it should looks like:


ll /Library/Frameworks/Mono.framework/Versions/
total 8
drwxr-xr-x  13 root  admin  442 11 28 22:36 3.10.0
drwxr-xr-x@ 12 root  admin  408  9 29 05:51 4.0.4
lrwxr-xr-x   1 root  admin   49 11 28 22:37 Current -> /Library/Frameworks/Mono.framework/Versions/4.0.4


