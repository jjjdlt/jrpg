using Microsoft.Xna.Framework;

namespace JRPG.World.Maps.Tiles;

public class Tile
{
    public int TileID { get; private set; }
    public Rectangle SourceRectangle { get; private set; }
    public bool IsCollidable { get; private set; }
    
    public Tile(int tileID, Rectangle sourceRectangle, bool isCollidable)
    {
        TileID = tileID;
        SourceRectangle = sourceRectangle;
        IsCollidable = isCollidable;
    }

    public Tile()
    {
        TileID = 0;
        SourceRectangle = Rectangle.Empty;
        IsCollidable = false;
    }
}