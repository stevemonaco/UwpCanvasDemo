using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.UI;
using Microsoft.Toolkit.Mvvm.ComponentModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpCanvasDemo;

public record LineState(float X, float Y, float Width, float Height, Color Color);

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[ObservableObject]
public sealed partial class MainPage : Page
{
    [ObservableProperty]
    private int _framesPerSecond;

    [ObservableProperty]
    private int _lineCount;

    private ICanvasBrush _brush;
    private TimeSpan _timeElapsed;
    private List<LineState> _lines = new();
    private Random _rng = new();
    
    private int _updatesWithinSecond;
    private TimeSpan _elapsed;

    public MainPage()
    {
        this.InitializeComponent();
        DataContext = this;
    }

    private void Canvas_CreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
    {
        _brush = new CanvasSolidColorBrush(sender, Color.FromArgb(200, 255, 255, 0));
    }

    private void Canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        _elapsed += args.Timing.ElapsedTime;
        _updatesWithinSecond++;

        if (_elapsed > TimeSpan.FromSeconds(1))
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                FramesPerSecond = _updatesWithinSecond;
                _updatesWithinSecond = 0;
                _elapsed = TimeSpan.Zero;
                LineCount = _lines.Count;
            });
        }

        var ds = args.DrawingSession;

        foreach (var line in _lines)
        {
            ds.DrawLine(line.X, line.Y, line.X + line.Width, line.Y + line.Height, line.Color, 3f);
        }
    }

    private void Canvas_GameLoopStarting(ICanvasAnimatedControl sender, object args)
    {

    }

    private void Canvas_GameLoopStopped(ICanvasAnimatedControl sender, object args)
    {

    }

    private void Canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        _timeElapsed = args.Timing.TotalTime;

        for (int i = 0; i < 500; i++)
        {
            var line = new LineState((float)_rng.NextDouble() * 1000, (float)_rng.NextDouble() * 1000, (float)_rng.NextDouble() * 500, (float)_rng.NextDouble() * 500, MakeRandomColor());
            _lines.Add(line);
        }
    }

    private Color MakeRandomColor()
    {
        return Color.FromArgb((byte)_rng.Next(256), (byte)_rng.Next(256), (byte)_rng.Next(256), (byte)_rng.Next(256));
    }
}
