﻿using ChunkyImageLib.DataHolders;
using SkiaSharp;

namespace ChunkyImageLib;

public interface IReadOnlyChunkyImage
{
    bool DrawLatestChunkOn(Vector2i chunkPos, ChunkResolution resolution, SKSurface surface, Vector2i pos, SKPaint? paint = null);
    bool LatestChunkExists(Vector2i chunkPos, ChunkResolution resolution);
    HashSet<Vector2i> FindAffectedChunks();
    HashSet<Vector2i> FindAllChunks();
}
