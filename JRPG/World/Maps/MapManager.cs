using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

// First, the Tile class that defines properties for each type of tile
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

// Static definitions for our tile types
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

// The main MapManager class that handles all map-related functionality
public class MapManager
{
    private readonly GraphicsDevice _graphicsDevice;
    private SpriteBatch _spriteBatch;
    private Texture2D _tilesetTexture;
    private Texture2D _debugTexture;
    private int[,] _currentMap;
    
    // Adjusted tile size for better visibility
    private const int TILE_DISPLAY_SIZE = 32;
    
    public int MapWidth { get; private set; }
    public int MapHeight { get; private set; }
    
    public MapManager(GraphicsDevice graphicsDevice, ContentManager content)
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch = new SpriteBatch(graphicsDevice);
        LoadContent(content);
        InitializeMap();
    }

    private void LoadContent(ContentManager content)
    {
        try
        {
            _tilesetTexture = content.Load<Texture2D>("pokemon_tileset");
            if (_tilesetTexture == null)
            {
                throw new ContentLoadException("Pokemon tileset failed to load");
            }
        }
        catch (ContentLoadException e)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load tileset: {e.Message}");
            _tilesetTexture = new Texture2D(_graphicsDevice, 1, 1);
            _tilesetTexture.SetData(new[] { Color.Magenta });
        }
    }

    private void InitializeMap()
    {
        int[,] initialMap = new int[,]
        {
            { 1, 1, 1, 1 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 1, 1, 1, 1 }
        };
        
        SetCurrentMap(initialMap);
    }

    public void SetCurrentMap(int[,] mapData)
    {
        _currentMap = mapData;
        MapHeight = _currentMap.GetLength(0);
        MapWidth = _currentMap.GetLength(1);
    }

    public Rectangle GetMapBounds()
    {
        return new Rectangle(0, 0, MapWidth * TILE_DISPLAY_SIZE, MapHeight * TILE_DISPLAY_SIZE);
    }

    public void Update(GameTime gameTime)
    {
        // Will be used for animated tiles in the future
    }

    public void DrawCurrentMap(SpriteBatch spriteBatch)
    {
        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                int tileID = _currentMap[y, x];
                
                if (TileDefinitions.Tiles.TryGetValue(tileID, out Tile tile))
                {
                    Vector2 position = new Vector2(x * TILE_DISPLAY_SIZE, y * TILE_DISPLAY_SIZE);
                    
                    spriteBatch.Draw(
                        _tilesetTexture,
                        new Rectangle(
                            (int)position.X,
                            (int)position.Y,
                            TILE_DISPLAY_SIZE,
                            TILE_DISPLAY_SIZE
                        ),
                        tile.SourceRectangle,
                        Color.White
                    );
                }
            }
        }
    }

    public void DrawDebugGrid(SpriteBatch spriteBatch)
    {
        if (_debugTexture == null)
        {
            _debugTexture = new Texture2D(_graphicsDevice, 1, 1);
            _debugTexture.SetData(new[] { Color.White });
        }

        for (int x = 0; x <= MapWidth; x++)
        {
            spriteBatch.Draw(_debugTexture, 
                new Rectangle(x * TILE_DISPLAY_SIZE, 0, 1, MapHeight * TILE_DISPLAY_SIZE), 
                Color.Red * 0.5f);
        }
        
        for (int y = 0; y <= MapHeight; y++)
        {
            spriteBatch.Draw(_debugTexture,
                new Rectangle(0, y * TILE_DISPLAY_SIZE, MapWidth * TILE_DISPLAY_SIZE, 1),
                Color.Red * 0.5f);
        }
    }

    public bool CheckCollision(Rectangle bounds)
    {
        int startX = Math.Max(0, bounds.Left / TILE_DISPLAY_SIZE);
        int startY = Math.Max(0, bounds.Top / TILE_DISPLAY_SIZE);
        int endX = Math.Min(MapWidth - 1, bounds.Right / TILE_DISPLAY_SIZE);
        int endY = Math.Min(MapHeight - 1, bounds.Bottom / TILE_DISPLAY_SIZE);

        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                int tileID = _currentMap[y, x];
                if (TileDefinitions.Tiles.TryGetValue(tileID, out Tile tile) && tile.IsCollidable)
                {
                    return true;
                }
            }
        }

        return false;
    }
}