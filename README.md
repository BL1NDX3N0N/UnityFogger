# Unity Fogger
A small and lightweight Unity Player patcher for hiding splash logos.

## Usage:
Run the utility then enter the path to UnityPlayer.dll.

## How it works:

This utility simply swaps out the shader used by Unity's player for rendering logos on the splash screen. Unity's player, by default, will use the built-in Sprites/Default shader for rendering logos on the ssplash screen and Hidden/InternalErrorShader as a fallback. For the best results, go into the Unity Player Settings in your project and ensure the logo list is empty and that your splash screen duration is 2 seconds.

If using this tool to patch the Unity Player shipped by another developer then you must ensure all of the above is set the same, if not, these settings can be tweaked from inside of the GlobalGameManagers file. I Have done some work reverse engineering such file and will provide my findings below.

| Offset          | Size (bytes) | Field               | Data Type | Endianness | Purpose                     |
|-----------------|--------------|---------------------|-----------|------------|-----------------------------|
| 0x10BC          | 4            | Unknown             | Int32     | BE         | The number of logo entries. |
| 0x10C0          | 16           | m_SplashScreenLogos | N/A       | N/A        | The first logo entry. Depending on how the splash screen is configured in the player settings, the first entry will be Unity's logo; otherwise the developer's logo. |

#### Entries in m_SplashScreenLogos:

Splash screen logo entries are contained in blocks spanning 16 bytes. Here is an example entry:

`02000000 A4280000 00000000 00000040`

| Offset | Size (bytes) | Field                  | Data Type | Endianness | Purpose                   |
|--------|--------------|------------------------|-----------|------------|---------------------------|
| 0x00   | 4            | m_SplashScreenDrawMode | Int32     | BE         | Sets the logos draw mode. |
| 0x04   | 4            | Unknown                | N/A       | N/A        | Appears to be a pointer to the logo. Unity will use `0xA4280000` for its logo. |
| 0x08   | 4            | Unknown                | N/A       | N/A        | Doubt it is padding and is instead for future-proofing, much like you would see in ELF. Other possibilities include `m_SplashScreenLogoStyle`, `m_SplashScreenAnimation`, `m_ShowUnitySplashScreen`, `m_ShowUnitySplashLogo`, etc. |
| 0x0C   | 4            | Unknown                | Float     | LE         | The logo duration in seconds, not including the 0.5 seconds the player adds for transitions.  Minimum value is 2 seconds and the maximum value is 10 seconds. |


#### m_SplashScreenDrawMode:

| Value (Int32) | Name             | Meaning |
|---------------|------------------|---------|
| 2             | All Sequential   | Logos will be drawn one after the other. |
| 1             | Unity Logo Below | The Unity logo will be drawn underneath other logos. This can also be used to force the Unity logo to the bottom of the splash screen. |

It is worth mentioning that setting `m_SplashScreenDrawMode` to any value other than `2` will cause the player to draw the Unity logo underneath other logos. I have yet to investigate further and assume this behaviour was intentionally implemented as a fallback.
