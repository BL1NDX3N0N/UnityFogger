# Unity Fogger
A small and lightweight Unity Player patcher for hiding splash logos.

## Usage:
Run the utility then enter the path to UnityPlayer.dll.

## How it works:

This utility simply swaps out the shader used by the splash screen for rendering logos. The splash screen in Unity's player, by default, uses the built-in *Sprites/Default* shader for rendering logos and the *Hidden/InternalErrorShader* shader as a fallback. For the best results, go into your projects **Player Settings** and ensure that both the **Logos** list is empty and your **Splash Screen Duration** is 2 seconds. To adjust the splash screen duration for already compiled games then you will need to modify the *GlobalGameManagers* file. I have done some work reverse engineering such file which you can find below.

### GlobalGameManagers

| Offset          | Size (bytes) | Field               | Data Type | Endianness | Purpose                     |
|-----------------|--------------|---------------------|-----------|------------|-----------------------------|
| 0x10BC          | 4            | Unknown             | Int32     | BE         | The number of logo entries. |
| 0x10C0          | 16           | m_SplashScreenLogos | N/A       | N/A        | The first logo entry. Depending on how the splash screen is configured in the player settings, the first entry will be Unity's logo; otherwise the developer's logo. |

#### Entries in m_SplashScreenLogos:

Splash screen logo entries are sequentially stored in 128-bit (16 bytes) data blocks with no padding between each block. Here is an example entry:

`02000000 A4280000 00000000 00000040`

| Offset (relative) | Size (bytes) | Field                  | Data Type | Endianness | Purpose                   |
|--------|--------------|------------------------|-----------|------------|---------------------------|
| 0x00   | 4            | m_SplashScreenDrawMode | Int32     | BE         | Sets the logos draw mode. |
| 0x04   | 4            | Unknown                | N/A       | N/A        | Appears to be a pointer to the logo. Unity will use `0xA4280000` for its logo. |
| 0x08   | 4            | Unknown                | N/A       | N/A        | Doubt it is padding and is instead for future-proofing, much like you would see in ELF. Other possibilities include `m_SplashScreenLogoStyle`, `m_SplashScreenAnimation`, `m_ShowUnitySplashScreen`, `m_ShowUnitySplashLogo`, etc. |
| 0x0C   | 4            | Unknown                | Float     | LE         | The logo duration in seconds, not including the 0.5 seconds the player adds for transitions.  Minimum value is 2 seconds and the maximum value is 10 seconds. |


#### m_SplashScreenDrawMode:

| Value (int32) | Name             | Meaning |
|---------------|------------------|---------|
| 1             | Unity Logo Below | The Unity logo will be drawn underneath other logos. This can also be used to force the Unity logo to the bottom of the splash screen. |
| 2             | All Sequential   | Logos will be drawn one after the other. |

It is worth mentioning that setting `m_SplashScreenDrawMode` to any value other than **'2'** will cause the player to draw the Unity logo underneath other logos. I have yet to investigate further and assume this behaviour was intentionally implemented as a fallback.

## Submitting Issues:
When submitting issues please follow GitHub's rules and guidelines. Issues not directly related to the code itself, such as the utility not working on certain games, are compatibility issues. Compatibility issues are always accepted and it is required that the original (that means unmodified) UnityPlayer.dll is provided in your submission so that support for it can be added. It would also help if you include the exact version of Unity, but this is not a requirement. Not following the above will result in your issue being closed without a response.
