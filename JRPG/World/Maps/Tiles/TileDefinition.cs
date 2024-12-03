using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace JRPG.World.Maps.Tiles;

public static class TileDefinitions
{
    public static readonly Dictionary<int, Tile> Tiles = new Dictionary<int, Tile>
    {
        // Grass tile [0] - Using exact coordinates from the Pokemon tileset
        { 0, new Tile(0, new Rectangle(96, 2240, 32, 32), false) },
        
        // Water tile [1] - Using exact coordinates from the Pokemon tileset
        { 1, new Tile(1, new Rectangle(16, 3663, 64, 64), true) }
    };
}