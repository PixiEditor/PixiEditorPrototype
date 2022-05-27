﻿using PixiEditor.ChangeableDocument.Changeables.Interfaces;

namespace PixiEditor.ChangeableDocument.Changeables;

internal class Layer : StructureMember, IReadOnlyLayer
{
    public bool LockTransparency { get; set; } = false;
    public ChunkyImage LayerImage { get; set; }
    IReadOnlyChunkyImage IReadOnlyLayer.LayerImage => LayerImage;

    public Layer(VecI size)
    {
        LayerImage = new(size);
    }

    public Layer(ChunkyImage image)
    {
        LayerImage = image;
    }

    public override void Dispose()
    {
        LayerImage.Dispose();
        Mask?.Dispose();
    }

    internal override Layer Clone()
    {
        return new Layer(LayerImage.CloneFromCommitted())
        {
            GuidValue = GuidValue,
            IsVisible = IsVisible,
            Name = Name,
            Opacity = Opacity,
            Mask = Mask?.CloneFromCommitted(),
        };
    }
}
