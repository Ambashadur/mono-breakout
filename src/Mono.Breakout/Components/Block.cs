using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono.Breakout.Components;

internal struct Block
{
    public bool Hide;
    public int Score;
    public Rectangle Bounds;
    public Texture2D Texture;
}
