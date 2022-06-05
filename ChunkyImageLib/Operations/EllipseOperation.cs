﻿using ChunkyImageLib.DataHolders;
using SkiaSharp;

namespace ChunkyImageLib.Operations;
internal class EllipseOperation : IDrawOperation
{
    public bool IgnoreEmptyChunks => false;

    public readonly RectI location;
    private readonly SKColor strokeColor;
    private readonly SKColor fillColor;
    private readonly int strokeWidth;
    private bool init = false;
    private SKPaint paint = new();
    private SKPath? outerPath;
    private SKPath? innerPath;
    private SKPoint[]? ellipse;
    private SKPoint[]? ellipseFill;
    private RectI? ellipseFillRect;

    public EllipseOperation(RectI location, SKColor strokeColor, SKColor fillColor, int strokeWidth)
    {
        this.location = location;
        this.strokeColor = strokeColor;
        this.fillColor = fillColor;
        this.strokeWidth = strokeWidth;
    }

    private void Init()
    {
        init = true;
        if (strokeWidth == 1)
        {
            var ellipseList = EllipseHelper.GenerateEllipseFromRect(location);
            ellipse = ellipseList.Select(a => (SKPoint)a).ToArray();
            if (fillColor.Alpha > 0)
            {
                (var fill, ellipseFillRect) = EllipseHelper.SplitEllipseIntoRegions(ellipseList, location);
                ellipseFill = fill.Select(a => (SKPoint)a).ToArray();
            }
        }
        else
        {
            outerPath = new SKPath();
            outerPath.ArcTo(location, 0, 359, true);
            innerPath = new SKPath();
            innerPath.ArcTo(location.Inflate(-strokeWidth), 0, 359, true);
        }
    }

    public void DrawOnChunk(Chunk chunk, VecI chunkPos)
    {
        if (!init)
            Init();
        var surf = chunk.Surface.SkiaSurface;
        surf.Canvas.Save();
        surf.Canvas.Scale((float)chunk.Resolution.Multiplier());
        surf.Canvas.Translate(-chunkPos * ChunkyImage.FullChunkSize);

        if (strokeWidth == 1)
        {
            if (fillColor.Alpha > 0)
            {
                paint.Color = fillColor;
                surf.Canvas.DrawPoints(SKPointMode.Lines, ellipseFill, paint);
                surf.Canvas.DrawRect((SKRect)ellipseFillRect!, paint);
            }
            paint.Color = strokeColor;
            surf.Canvas.DrawPoints(SKPointMode.Points, ellipse, paint);
        }
        else
        {
            if (fillColor.Alpha > 0)
            {
                surf.Canvas.Save();
                surf.Canvas.ClipPath(innerPath);
                surf.Canvas.DrawColor(fillColor, SKBlendMode.SrcOver);
                surf.Canvas.Restore();
            }
            surf.Canvas.Save();
            surf.Canvas.ClipPath(outerPath);
            surf.Canvas.ClipPath(innerPath, SKClipOperation.Difference);
            surf.Canvas.DrawColor(strokeColor, SKBlendMode.SrcOver);
            surf.Canvas.Restore();
        }
        surf.Canvas.Restore();
    }

    public HashSet<VecI> FindAffectedChunks()
    {
        var chunks = OperationHelper.FindChunksTouchingEllipse
            (location.Center, location.Width / 2.0, location.Height / 2.0, ChunkyImage.FullChunkSize);
        if (fillColor.Alpha == 0)
        {
            chunks.ExceptWith(OperationHelper.FindChunksFullyInsideEllipse
                (location.Center, location.Width / 2.0 - strokeWidth * 2, location.Height / 2.0 - strokeWidth * 2, ChunkyImage.FullChunkSize));
        }
        return chunks;
    }

    public IDrawOperation AsMirrored(int? verAxisX, int? horAxisY)
    {
        RectI newLocation = location;
        if (verAxisX is not null)
            newLocation = newLocation.ReflectX((int)verAxisX);
        if (horAxisY is not null)
            newLocation = newLocation.ReflectY((int)horAxisY);
        return new EllipseOperation(newLocation, strokeColor, fillColor, strokeWidth);
    }

    public void Dispose()
    {
        paint?.Dispose();
        outerPath?.Dispose();
        innerPath?.Dispose();
    }
}