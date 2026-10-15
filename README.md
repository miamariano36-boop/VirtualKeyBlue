# VirtualKeyBlue

Mod base para teclado virtual en español para Stardew Valley.

## Estructura
- manifest.json
- ModEntry.cs
- VirtualKeyboardMenu.cs
- assets/ (svg/png opcionales)

## Compilar
1. Crea un proyecto de tipo Class Library en C# (por ejemplo en Visual Studio).
2. Añade estos archivos al proyecto (`ModEntry.cs`, `VirtualKeyboardMenu.cs`).
3. Añade referencias a:
   - StardewValley.dll
   - StardewModdingAPI.dll
   - xTile.dll
   - Microsoft.Xna.Framework.dll
   - Microsoft.Xna.Framework.Graphics.dll
4. Compila.
5. Copia el .dll generado a la carpeta del mod (`VirtualKeyBlue.dll`).
6. Crea la carpeta dentro de `Mods/`:
   - `Mods/VirtualKeyBlue/`
7. Coloca en esa carpeta:
   - `manifest.json`
   - `VirtualKeyBlue.dll`
   - `config.json` (se generará automáticamente la primera vez si no existe)

## Uso
- Presiona A para abrir/cerrar.
- Comandos en consola (SMAPI):
  - `vk` / `vkopen` / `vkclose`
  - `vkaddkey 2 ñ` — añade la tecla ñ a la fila 2 y la guarda
  - `vkaddrow á é í ó ú` — añade una fila nueva y la guarda
  - `vksave` — guarda manualmente

## Configuración
La primera vez que se ejecuta el mod, crea un archivo `config.json` con el diseño español por defecto.
