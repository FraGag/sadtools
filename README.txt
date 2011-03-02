Sonic Adventure Tools
=====================

This is a set of Visual C# projects providing librairies and utilities that
assist in working with Sonic Adventure and its derived games. Here is a short
description of the projects in the Visual Studio solution:

CExporter
---------

This library contains functions that generate C code from data in memory,
contained in the structures defined in the Data project.

Data
----

This library defines structures that represent the data structures used by
Sonic Adventure and derived games.

PCTools
-------

This library contains features that apply only to the PC version of Sonic
Adventure DX: Director's Cut. Currently, the main feature is PEReader, which is
able to extract data from the main executable (sonic.exe) and from the DLLs.

SADXPCDecompiler
----------------

This application generates C code files containing the data from a DLL as
specified in an XML description file.
