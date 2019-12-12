﻿using Unity.Entities;
using Unity.Mathematics;

struct Map : IComponentData
{
    public int Size;
    public int TileSize;
    public float TrailDecay;
}

struct Tile : IComponentData
{
    public int2 Coordinates;
}
