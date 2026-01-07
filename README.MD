# Usage
1. Download the mod from [release](https://github.com/SkyDynamic/CmMaiKiraMod/releases).
2. Put the mod into `BepInEx\plugins`.
3. Start CardMaker after first install, will generate a `CmMaiKiraMod.cfg` file in `BepInEx\config`.
4. Modify the `CmMaiKiraMod.cfg` file and restart CardMaker.


# Extra Notes 
## Install BepInEx with segatools
1. Make sure your segatools is updated to version `2024.04.29` or higher
2. Download [BepInEx](https://github.com/BepInEx/BepInEx/releases/latest) and unzip the `BepInEx` folder into game `package` folder
3. Open `segatools.ini`, modify or add the following line: 
```ini
[unity]
targetAssembly=BepInEx\core\BepInEx.Preloader.dll
```
4. Restart CardMaker

# Mod Future
- Custom Maimai User Data
- Ignore Maimai Login Version check
- Fix Maimai Rating Spacing Calculation
- Serial Id Simulator, make sure id start with `0` or `1`