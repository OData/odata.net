### Compatibility Flags

Any semantic breaking change (those changes which do not break source or binary compatibility, but change the behavior in such a way that callers will receive unexpected results) should be added behind a compatibility flag.
The flag should allow callers to opt in to the new behavior and should be marked `Obsolete`. 

At the next major release, the behavior should be changed so that the default behavior acts as though the flag were enabled.
The flag should be removed and replace with a new flag that allows callers to opt out of the new behavior.
The new flag should be marked `Obsolete`.

At the subsequent major release, the old behavior should be removed along with the flag. 
