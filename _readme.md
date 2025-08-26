

TODO try the case where a customer is using the an old version of the framework and a package that uses a new version of the framework
TODO go ahead and add a v3 to this as well just to really be sure
TODO make sure to update the hard-coded binaries in each project




### someone is calling the odata reader v1 implementation and updates their odata nuget package

1. build `_readercaller1`
2. execute `_readercaller1.exe`; this is using `_readerpocv1`; it should output "success!"
3. build `_readerpocv2`
4. copy `_readerpocv2.dll` to the folder of `_readercaller1.exe`
5. execute `_readercaller1.exe`; this is now using `_readerpocv2`; it should output "success!"

### someone is calling the odata reader v2 implementation

1. build `_readercaller2`
2. execute `_readercaller2.exe`; this is using `_readerpocv2`; it should output "success!"

### someone is implementing a v1 reader and updates their nuget package

1. build `_readerimplementer1`
2. execute `_readerimplementer1.exe`; this is using `_readerpocv1`; it should output "success!"
3. build `_readerpocv2`
4. copy `_readerpocv2.dll` to the folder of `_readerimplementer1.exe`
5. execute `_readerimplementer1.exe`; this is now using `_readerpocv2`; it should output "success!"

### someone is implementing a v2 reader

1. build `_readerimplementer2`
2. execute `readerimplementer2.exe`; this is using `_readerpocv2`; it should output "success!"

### someone is calling an implementation that leverages the v1 package *and* an implementation that leverages the v2 package

1. build `_readercaller3`
2. execute `_readercaller3.exe`; this is using `_readerpocv2`; it should output "success!\r\nsuccess!\r\nsuccess!"
