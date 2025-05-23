# Unity Extensions

Collection of small extensions, utils and property attributes.

- [SingleLineAttribute](Runtime/PropertyAttributes/SingleLineAttribute.cs) - Draw a serialized field on a single line even if it's a struct with multiple fields
- [EnumDropdownAttribute](Runtime/PropertyAttributes/EnumDropdownAttribute.cs) - Draw a string or int field as an enum dropdown
- [CSVParser](Runtime/CSVParser.cs) - A simple utility to parse a csv file
- [SDateTime](Runtime/SDateTime.cs) - A DateTime value that can be serialized in Unity and displayed in inspector
- [EventSystemCallbacks](Runtime/EventSystemCallbacks.cs) - Global callbacks just before any EventSystem event is executed

See [samples](Samples~)


## Installation

Install via Package Manager by adding the following line to *Packages/manifest.json*:
- `"com.villevli.extensions": "https://github.com/villevli/UnityExtensions.git"`

Or copy the contents of this repository to a folder in your Assets folder.


## Development

Clone this package into `Packages/com.villevli.extensions` in a Unity project.
For now development should be done in Unity 2021.3 to preserve backwards compability. Test in latest Unity versions separately.

See the `TestAssets/` folder for scenes and assets to test the different scripts.

Avoid dependencies to other packages. Use the Version Defines in Assembly Definition to conditionally support some if needed. E.g. define `USE_UGUI` when using `com.unity.ugui` and `USE_TMPRO` when using `com.unity.textmeshpro`.

To test in different Unity versions easily, create a separate project for each version and create a symbolic link in the Packages folder. Any changes you make will then appear in all projects automatically. Or you can use the "Add package from disk" option in Package Manager in the other projects.

Example for Windows to create a link in UnityPackageTestProject6000 that points to UnityPackageTestProject. Run cmd as admin
```bat
mklink /D "W:\villevli\UnityPackageTestProject6000\Packages\com.villevli.extensions" "W:\villevli\UnityPackageTestProject\Packages\com.villevli.extensions"
```
