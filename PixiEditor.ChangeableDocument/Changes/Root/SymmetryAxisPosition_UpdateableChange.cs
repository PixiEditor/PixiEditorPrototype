﻿using PixiEditor.ChangeableDocument.Changeables;
using PixiEditor.ChangeableDocument.ChangeInfos;
using PixiEditor.ChangeableDocument.ChangeInfos.Root;
using PixiEditor.ChangeableDocument.Enums;

namespace PixiEditor.ChangeableDocument.Changes.Root;
internal class SymmetryAxisPosition_UpdateableChange : UpdateableChange
{
    private readonly SymmetryAxisDirection direction;
    private int newPos;
    private int originalPos;

    public SymmetryAxisPosition_UpdateableChange(SymmetryAxisDirection direction, int pos)
    {
        this.direction = direction;
        newPos = pos;
    }

    public void Update(int pos)
    {
        newPos = pos;
    }

    public override void Initialize(Document target)
    {
        originalPos = direction switch
        {
            SymmetryAxisDirection.Horizontal => target.HorizontalSymmetryAxisY,
            SymmetryAxisDirection.Vertical => target.VerticalSymmetryAxisX,
            _ => throw new NotImplementedException(),
        };
    }

    private void SetPosition(Document target, int position)
    {
        if (direction == SymmetryAxisDirection.Horizontal)
            target.HorizontalSymmetryAxisY = position;
        else if (direction == SymmetryAxisDirection.Vertical)
            target.VerticalSymmetryAxisX = position;
        else
            throw new NotImplementedException();
    }

    public override IChangeInfo? Apply(Document target, out bool ignoreInUndo)
    {
        ignoreInUndo = originalPos == newPos;
        SetPosition(target, newPos);
        return new SymmetryAxisPosition_ChangeInfo() { Direction = direction };
    }

    public override IChangeInfo? ApplyTemporarily(Document target)
    {
        SetPosition(target, newPos);
        return new SymmetryAxisPosition_ChangeInfo() { Direction = direction };
    }

    public override IChangeInfo? Revert(Document target)
    {
        if (originalPos == newPos)
            return null;
        SetPosition(target, originalPos);
        return new SymmetryAxisPosition_ChangeInfo() { Direction = direction };
    }

    public override bool IsMergeableWith(Change other)
    {
        return other is SymmetryAxisPosition_UpdateableChange;
    }
}