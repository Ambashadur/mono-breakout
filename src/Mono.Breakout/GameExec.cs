using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mono.Breakout;

public class GameExec : Game
{
    private const float _barSpeed = 250;

    private Rectangle _bar;
    private Texture2D _barTexture;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public GameExec() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize() {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _barTexture = new Texture2D(GraphicsDevice, 1, 1);
        _barTexture.SetData([Color.Chocolate]);
        _bar = new Rectangle(100, 300, 100, 20);
    }

    protected override void Update(GameTime gameTime) {
        if (IsPressed(PlayerIndex.One, Buttons.Back) || IsPressed(Keys.Escape)) Exit();

        var state = Keyboard.GetState();
        if (state.IsKeyDown(Keys.Right)) {
            _bar.X += (int)(_barSpeed * gameTime.ElapsedGameTime.TotalSeconds);
        } else if (state.IsKeyDown(Keys.Left)) {
            _bar.X -= (int)(_barSpeed * gameTime.ElapsedGameTime.TotalSeconds);
        }

        if (_bar.X < 0) _bar.X = 0;
        else if (_bar.Right >= _graphics.PreferredBackBufferWidth) _bar.X = _graphics.PreferredBackBufferWidth - _bar.Width;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        _spriteBatch.Draw(_barTexture, _bar, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPressed(Keys key) => Keyboard.GetState().IsKeyDown(key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsPressed(PlayerIndex player, Buttons button) =>
        GamePad.GetState(player).IsButtonDown(button);
}
