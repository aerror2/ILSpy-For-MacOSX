
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


Features:

works on command line;
Decompile all dll in the input arguments and generate the project files and solution files just simply by one simple command line.

I like that style instead of a GUI. 

It's better that reading the codes in the IDE than reading it in the ILSpy GUI, more feature help you understand the decompiled codes.

Usage:
 ILSpyMac [options] directory/to/all/your/dll 
 made it run at all platform support mono.
 by aerror 2015/11/27
 options:
       -n  Solution Name
       -l  References dll path which dll will be loaded but not decompile , they use as References.
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


