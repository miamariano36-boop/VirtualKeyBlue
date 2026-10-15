using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace VirtualKeyBlue
{
    public class VirtualKeyboardMenu : IClickableMenu
    {
        private readonly IMonitor _monitor;
        private readonly List<List<string>> _keyboardRows;
        private readonly Dictionary<string, Rectangle> _keyRects = new();

        private readonly int _padding = 10;
        private readonly int _keyHeight = 52;
        private readonly int _keyWidth = 52;
        private readonly int _keyboardTopY = 130;

        private readonly Rectangle _closeButtonRect;
        private readonly Rectangle _inputBoxRect;

        private string _inputBuffer = "";
        private bool _shiftPressed;

        public VirtualKeyboardMenu(IMonitor monitor, List<List<string>> rows)
        {
            _monitor = monitor;

            _keyboardRows = new List<List<string>>();
            foreach (var row in rows)
            {
                _keyboardRows.Add(new List<string>(row));
            }

            width = 980;
            height = 410;

            xPositionOnScreen = (Game1.viewport.Width - width) / 2;
            yPositionOnScreen = Game1.viewport.Height - height - 30;

            _closeButtonRect = new Rectangle(xPositionOnScreen + width - 40, yPositionOnScreen + 12, 28, 28);
            _inputBoxRect = new Rectangle(xPositionOnScreen + 30, yPositionOnScreen + 35, width - 60, 70);

            BuildKeyLayout();
        }

        public void AddKeyToRow(int rowIndex, string key)
        {
            if (rowIndex < 0 || rowIndex >= _keyboardRows.Count)
            {
                _monitor.Log($"Fila {rowIndex + 1} fuera de rango.", LogLevel.Warn);
                return;
            }

            _keyboardRows[rowIndex].Add(key);
            BuildKeyLayout();
        }

        public void AddRow(List<string> row)
        {
            _keyboardRows.Add(new List<string>(row));
            BuildKeyLayout();
        }

        private void BuildKeyLayout()
        {
            _keyRects.Clear();

            int y = yPositionOnScreen + _keyboardTopY;
            int startX = xPositionOnScreen + 25;

            foreach (var row in _keyboardRows)
            {
                if (row.Count == 0)
                    continue;

                int totalWidth = 0;
                for (int i = 0; i < row.Count; i++)
                {
                    totalWidth += GetKeyWidth(row[i]);
                }

                totalWidth += _padding * (Math.Max(0, row.Count - 1));

                int rowStartX = startX + ((width - 50) - totalWidth) / 2;

                for (int i = 0; i < row.Count; i++)
                {
                    string key = row[i];
                    int w = GetKeyWidth(key);
                    Rectangle rect = new Rectangle(rowStartX, y, w, _keyHeight);

                    string dictKey = key;
                    int suffix = 1;
                    while (_keyRects.ContainsKey(dictKey))
                    {
                        dictKey = key + "_" + suffix;
                        suffix++;
                    }

                    _keyRects[dictKey] = rect;
                    rowStartX += w + _padding;
                }

                y += _keyHeight + _padding;
            }
        }

        private int GetKeyWidth(string key)
        {
            if (key == "Backspace" || key == "Enter" || key == "Shift")
                return 95;

            if (key == "Space")
                return 380;

            return _keyWidth;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (_closeButtonRect.Contains(x, y))
            {
                Game1.exitActiveMenu();
                return;
            }

            foreach (var kvp in _keyRects)
            {
                if (kvp.Value.Contains(x, y))
                {
                    string keyName = kvp.Key.Contains("_") ? kvp.Key.Split('_')[0] : kvp.Key;
                    HandleKeyPress(keyName);
                    return;
                }
            }
        }

        private void HandleKeyPress(string key)
        {
            switch (key)
            {
                case "Backspace":
                    if (_inputBuffer.Length > 0)
                        _inputBuffer = _inputBuffer.Substring(0, _inputBuffer.Length - 1);
                    break;

                case "Enter":
                    _monitor.Log("Teclado virtual: " + _inputBuffer, LogLevel.Info);
                    Game1.showGlobalMessage("Texto capturado: " + _inputBuffer);
                    _inputBuffer = "";
                    break;

                case "Shift":
                    _shiftPressed = !_shiftPressed;
                    break;

                case "Space":
                    _inputBuffer += " ";
                    break;

                default:
                    if (_shiftPressed)
                    {
                        _inputBuffer += key.ToUpperInvariant();
                        _shiftPressed = false;
                    }
                    else
                    {
                        if (key.Length == 1)
                            _inputBuffer += key.ToLowerInvariant();
                        else
                            _inputBuffer += key;
                    }
                    break;
            }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), new Color(0, 0, 0, 180));

            IClickableMenu.drawTextureBox(
                b,
                Game1.mouseCursors,
                new Rectangle(384, 373, 18, 18),
                xPositionOnScreen,
                yPositionOnScreen,
                width,
                height,
                Color.White,
                1f,
                true);

            Utility.drawTextWithShadow(
                b,
                "Teclado Virtual",
                Game1.dialogueFont,
                new Vector2(xPositionOnScreen + 30, yPositionOnScreen + 10),
                Color.White);

            IClickableMenu.drawTextureBox(
                b,
                Game1.mouseCursors,
                new Rectangle(432, 439, 9, 9),
                _closeButtonRect.X,
                _closeButtonRect.Y,
                _closeButtonRect.Width,
                _closeButtonRect.Height,
                Color.White,
                1f,
                true);

            Utility.drawTextWithShadow(
                b,
                "X",
                Game1.smallFont,
                new Vector2(_closeButtonRect.X + 10, _closeButtonRect.Y + 5),
                Color.White);

            IClickableMenu.drawTextureBox(
                b,
                Game1.mouseCursors,
                new Rectangle(384, 373, 18, 18),
                _inputBoxRect.X,
                _inputBoxRect.Y,
                _inputBoxRect.Width,
                _inputBoxRect.Height,
                Color.White,
                1f,
                true);

            string displayText = string.IsNullOrEmpty(_inputBuffer) ? "Escribe aquí..." : _inputBuffer;
            Utility.drawTextWithShadow(
                b,
                displayText,
                Game1.dialogueFont,
                new Vector2(_inputBoxRect.X + 15, _inputBoxRect.Y + 18),
                Color.White);

            foreach (var kvp in _keyRects)
            {
                Rectangle rect = kvp.Value;
                string keyText = kvp.Key.Contains("_") ? kvp.Key.Split('_')[0] : kvp.Key;

                IClickableMenu.drawTextureBox(
                    b,
                    Game1.mouseCursors,
                    new Rectangle(432, 439, 9, 9),
                    rect.X,
                    rect.Y,
                    rect.Width,
                    rect.Height,
                    Color.White,
                    1f,
                    true);

                float fontScale = 1f;
                if (keyText == "Space") fontScale = 0.8f;
                if (keyText == "Backspace" || keyText == "Enter" || keyText == "Shift") fontScale = 0.75f;

                Vector2 size = Game1.smallFont.MeasureString(keyText);
                float textX = rect.X + (rect.Width - size.X) / 2f;
                float textY = rect.Y + (rect.Height - size.Y) / 2f;

                Utility.drawTextWithShadow(
                    b,
                    keyText,
                    Game1.smallFont,
                    new Vector2(textX, textY),
                    Color.White,
                    fontScale,
                    0.75f);
            }

            base.draw(b);
        }

        public override void update(GameTime time)
        {
            base.update(time);
        }
    }
}
