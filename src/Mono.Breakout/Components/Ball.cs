using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mono.Breakout.Components;

internal struct Ball
{
    public float Speed;
    public Vector2 Direction;
    public Rectangle Bounds;
    public Texture2D Texture;
}
