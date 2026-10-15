using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;

namespace VirtualKeyBlue
{
    public class ModEntry : Mod
    {
        private VirtualKeyboardMenu? _keyboardMenu;
        private ModConfig _config = new();

        public override void Entry(IModHelper helper)
        {
            try
            {
                _config = helper.ReadConfig<ModConfig>() ?? new ModConfig();
            }
            catch (Exception)
            {
                _config = new ModConfig();
            }

            if (_config.KeyboardRows == null || _config.KeyboardRows.Count == 0)
            {
                _config.KeyboardRows = DefaultSpanishLayout();
                helper.WriteConfig(_config);
            }

            helper.Events.Input.ButtonPressed += OnButtonPressed;

            helper.ConsoleCommands.Add("vk", "Abre o cierra el teclado virtual", (_, _) => ToggleKeyboard());
            helper.ConsoleCommands.Add("vkopen", "Abre el teclado virtual", (_, _) => OpenKeyboard());
            helper.ConsoleCommands.Add("vkclose", "Cierra el teclado virtual", (_, _) => CloseKeyboard());

            helper.ConsoleCommands.Add(
                "vkaddkey",
                "Uso: vkaddkey <fila(1-based)> <tecla> — añade una tecla y la guarda.",
                (cmd, args) =>
                {
                    if (args.Length < 2)
                    {
                        Monitor.Log("Uso: vkaddkey <fila(1-based)> <tecla>", LogLevel.Info);
                        return;
                    }

                    if (!int.TryParse(args[0], out int rowNumber))
                    {
                        Monitor.Log("La primera parte debe ser un número de fila.", LogLevel.Info);
                        return;
                    }

                    if (rowNumber < 1 || rowNumber > _config.KeyboardRows.Count)
                    {
                        Monitor.Log("Fila fuera de rango.", LogLevel.Warn);
                        return;
                    }

                    string key = args[1];
                    _config.KeyboardRows[rowNumber - 1].Add(key);
                    SaveConfig();

                    if (Game1.activeClickableMenu is VirtualKeyboardMenu menu)
                    {
                        menu.AddKeyToRow(rowNumber - 1, key);
                        Monitor.Log($"Tecla '{key}' añadida a la fila {rowNumber}.", LogLevel.Info);
                    }
                    else
                    {
                        Monitor.Log($"Tecla '{key}' añadida a la configuración.", LogLevel.Info);
                    }
                });

            helper.ConsoleCommands.Add(
                "vkaddrow",
                "Uso: vkaddrow <tecla1> <tecla2> ... — añade una nueva fila y la guarda.",
                (cmd, args) =>
                {
                    if (args.Length < 1)
                    {
                        Monitor.Log("Uso: vkaddrow <tecla1> <tecla2> ...", LogLevel.Info);
                        return;
                    }

                    _config.KeyboardRows.Add(new List<string>(args));
                    SaveConfig();

                    if (Game1.activeClickableMenu is VirtualKeyboardMenu menu)
                    {
                        menu.AddRow(new List<string>(args));
                        Monitor.Log("Nueva fila añadida al teclado.", LogLevel.Info);
                    }
                    else
                    {
                        Monitor.Log("Nueva fila añadida a la configuración.", LogLevel.Info);
                    }
                });

            helper.ConsoleCommands.Add(
                "vksave",
                "Guarda la configuración actual del teclado.",
                (_, _) => SaveConfig()
            );
        }

        private void OnButtonPressed(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.A)
            {
                ToggleKeyboard();
            }
        }

        public void OpenKeyboard()
        {
            if (Game1.activeClickableMenu is VirtualKeyboardMenu)
                return;

            _keyboardMenu = new VirtualKeyboardMenu(Monitor, _config.KeyboardRows);
            Game1.activeClickableMenu = _keyboardMenu;
        }

        public void CloseKeyboard()
        {
            if (Game1.activeClickableMenu is VirtualKeyboardMenu)
            {
                Game1.exitActiveMenu();
                _keyboardMenu = null;
            }
        }

        public void ToggleKeyboard()
        {
            if (Game1.activeClickableMenu is VirtualKeyboardMenu)
                CloseKeyboard();
            else
                OpenKeyboard();
        }

        private void SaveConfig()
        {
            try
            {
                Helper.WriteConfig(_config);
                Monitor.Log("Configuración guardada en config.json", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Monitor.Log("Error al guardar config.json: " + ex.Message, LogLevel.Error);
            }
        }

        private static List<List<string>> DefaultSpanishLayout()
        {
            return new List<List<string>>
            {
                new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "Backspace" },
                new List<string> { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "Ñ" },
                new List<string> { "A", "S", "D", "F", "G", "H", "J", "K", "L", "Enter" },
                new List<string> { "Z", "X", "C", "V", "B", "N", "M", ",", ".", "Shift" },
                new List<string> { "Space" }
            };
        }
    }

    public class ModConfig
    {
        public List<List<string>> KeyboardRows { get; set; } = new List<List<string>>();
    }
}
