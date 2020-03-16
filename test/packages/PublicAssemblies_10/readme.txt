The PublicAssemblies folder is designed to contain managed assemblies that run within the development environment and are typically called from macros, add-ins, and other user code. The assemblies in this directory are displayed in the Project Add References dialog box and the Object Browser's Component Selector dialog box. For example, COM interoperability wrappers for automation object models (e.g. vslangproj.dll) should be installed in the PublicAssemblies folder.

 

Assemblies that are not intended to be called from user code should be installed into the PrivateAssemblies folder.

