﻿using ChunkyImageLib.DataHolders;
using PixiEditor.ChangeableDocument.Changes;
using PixiEditor.ChangeableDocument.Changes.Drawing;

namespace PixiEditor.ChangeableDocument.Actions.Drawing.Rectangle
{
    public record class DrawRectangle_Action : IStartOrUpdateChangeAction
    {
        public DrawRectangle_Action(Guid layerGuid, ShapeData rectangle)
        {
            LayerGuid = layerGuid;
            Rectangle = rectangle;
        }

        public Guid LayerGuid { get; }
        public ShapeData Rectangle { get; }

        void IStartOrUpdateChangeAction.UpdateCorrespodingChange(UpdateableChange change)
        {
            ((DrawRectangle_UpdateableChange)change).Update(Rectangle);
        }

        UpdateableChange IStartOrUpdateChangeAction.CreateCorrespondingChange()
        {
            return new DrawRectangle_UpdateableChange(LayerGuid, Rectangle);
        }
    }
}