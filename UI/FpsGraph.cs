using System;
using System.Collections.Generic;
using StoneForge;

namespace ExampleMod.UI;

// A custom element: the last couple of seconds of frame times as bars, lit under the mouse.
public class FpsGraph : UIElement
{
    private readonly Queue<double> _frames = new();

    protected override void OnUpdate(double deltaTime)
    {
        _frames.Enqueue(deltaTime);
        while (_frames.Count > 100)
            _frames.Dequeue();
    }

    protected override void OnDraw(double x, double y)
    {
        Draw.Rectangle(x, y, x + Width - 1, y + Height - 1, Draw.Rgb(18, 16, 26));
        double barWidth = Width / 100, i = 0, total = 0;
        foreach (double frame in _frames)
        {
            double fps = frame > 0 ? 1 / frame : 0;
            double bar = Math.Min(Height - 2, fps / 60 * (Height - 2));
            int colour = fps >= 35 ? Draw.Rgb(90, 160, 90) : fps >= 20 ? Draw.Rgb(200, 170, 70) : Draw.Rgb(200, 70, 60);
            Draw.Rectangle(x + i * barWidth, y + Height - 1 - bar, x + (i + 1) * barWidth - 1, y + Height - 1, colour, IsHovered ? 1 : 0.7);
            i++;
            total += frame;
        }
        Draw.Rectangle(x, y, x + Width - 1, y + Height - 1, Draw.Rgb(74, 66, 86), outline: true);
        double average = _frames.Count > 0 && total > 0 ? _frames.Count / total : 0;
        Draw.Text(x + 4, y + 3, $"{average:0} fps", Draw.White);
    }
}
