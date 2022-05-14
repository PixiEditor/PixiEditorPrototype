﻿using ChunkyImageLib.DataHolders;

namespace PixiEditor.ChangeableDocument.ChangeInfos.Drawing;

public record class MaskChunks_ChangeInfo : IChangeInfo
{
    public Guid MemberGuid { get; init; }
    public HashSet<Vector2i>? Chunks { get; init; }
}