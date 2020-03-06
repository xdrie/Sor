using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;

namespace Sor.Util {
    public class ColoredTextBuilder {
        public Color defaultColor { get; }
        private List<ColoredLine> lines = new List<ColoredLine>();

        struct ColoredLine {
            public string text;
            public Color col;

            public ColoredLine(string text, Color col) {
                this.text = text;
                this.col = col;
            }
        }

        public ColoredTextBuilder(Color defaultColor) {
            this.defaultColor = defaultColor;
        }

        public void appendLine(string line = "") => appendLine(line, defaultColor);

        public void appendLine(string line, Color color) {
            lines.Add(new ColoredLine(line, color));
        }

        public void drawTo(Batcher batcher, IFont font, Vector2 pos) {
            // draw each line as a string
            var drawPos = pos;
            foreach (var line in lines) {
                batcher.DrawString(font, line.text, drawPos, line.col);
                drawPos += new Vector2(0, font.LineSpacing);
            }
        }
    }
}