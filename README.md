# Confuser.Protections.HoLLy
Extra protection modules for ConfuserEx.

Get the latest build at AppVeyor:  
[![Build status](https://ci.appveyor.com/api/projects/status/4s8myfmgvk0u4wcq?svg=true)](https://ci.appveyor.com/project/HoLLy-HaCKeR/confuser-protections-holly/build/artifacts)

## Modules
### Anti Watermark
This protection removes the "ConfusedBy" attribute to prevent reverse engineers for identifying 
your assembly as obfuscated by ConfuserEx.

Id: `anti watermark`  
FullId: `HoLLy.AntiWatermark`  
Preset: Minimum

### Fake Obfuscator
Adds a bunch of types to the module so de4dot will not be able to determine the correct obfuscator. 
This also has the side-effect of making de4dot freeze when deobfuscating (as of 2017-11-26).

Id: `fake obfuscator`  
FullId: `HoLLy.FakeObfuscator`  
Preset: Normal

### Memory Protection
Hides the marked types from normal memory searching/editing.

Id: `memory protection`  
FullId: `HoLLy.MemoryProtection`  
Preset: None, on-demand only
