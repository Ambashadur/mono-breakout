using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mono.Breakout.Components;

namespace Mono.Breakout;

public class GameExec : Game
{
    private const float _barSpeed = 300;

    private GameState _gameState;
    private Rectangle _bar;
    private Ball _ball;
    private KeyboardState _keyboardState;
    private Label _gameOverLabel;
    private Texture2D _barTexture;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public GameExec() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize() {
        _graphics.PreferredBackBufferWidth = 600;
        _graphics.PreferredBackBufferHeight = 800;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _barTexture = new Texture2D(GraphicsDevice, 1, 1);
        _barTexture.SetData([Color.Chocolate]);
        _bar = new Rectangle(
            x: (_graphics.PreferredBackBufferWidth - 100) / 2,
            y: _graphics.PreferredBackBufferHeight - 50,
            width: 100,
            height: 20);

        var ballTexture = new Texture2D(GraphicsDevice, 1, 1);
        ballTexture.SetData([Color.White]);
        _ball = new Ball {
            Speed = 150,
            Direction = new Vector2(1, 1),
            Bounds = new Rectangle(
                x: (_graphics.PreferredBackBufferWidth - 15) / 2,
                y: (_graphics.PreferredBackBufferHeight - 15) / 2,
                width: 15,
                height: 15),
            Texture = ballTexture
        };

        _gameOverLabel = new Label {
            Text = "GAME OVER",
            Font = Content.Load<SpriteFont>("monogram")
        };

        var labelBounds = _gameOverLabel.Font.MeasureString(_gameOverLabel.Text);
        _gameOverLabel.Position = new Vector2(
            x: (_graphics.PreferredBackBufferWidth - labelBounds.X) / 2,
            y: (_graphics.PreferredBackBufferHeight - labelBounds.Y) / 2);
    }

    protected override void Update(GameTime gameTime) {
        _keyboardState = Keyboard.GetState();
        if (_keyboardState.IsKeyDown(Keys.Escape)) Exit();

        switch (_gameState) {
            case GameState.Running:
                HandleKeyboard(gameTime);
                ProcessBall(gameTime);
                break;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        switch (_gameState) {
            case GameState.Running:
                _spriteBatch.Draw(_barTexture, _bar, Color.White);
                _spriteBatch.Draw(_ball.Texture, _ball.Bounds, Color.White);
                break;

            case GameState.GameOver:
                _spriteBatch.DrawString(_gameOverLabel.Font, _gameOverLabel.Text, _gameOverLabel.Position, Color.White);
                break;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void HandleKeyboard(GameTime gameTime) {
        if (_keyboardState.IsKeyDown(Keys.Right) || _keyboardState.IsKeyDown(Keys.D)) {
            _bar.X += (int)(_barSpeed * gameTime.ElapsedGameTime.TotalSeconds);
        } else if (_keyboardState.IsKeyDown(Keys.Left) || _keyboardState.IsKeyDown(Keys.A)) {
            _bar.X -= (int)(_barSpeed * gameTime.ElapsedGameTime.TotalSeconds);
        }

        if (_bar.X < 0) _bar.X = 0;
        else if (_bar.Right >= _graphics.PreferredBackBufferWidth) _bar.X = _graphics.PreferredBackBufferWidth - _bar.Width;
    }

    private void ProcessBall(GameTime gameTime) {
        if (_ball.Bounds.Intersects(_bar)) _ball.Direction.Y = - _ball.Direction.Y;

        _ball.Bounds.X += (int)(_ball.Direction.X * _ball.Speed * gameTime.ElapsedGameTime.TotalSeconds);
        _ball.Bounds.Y += (int)(_ball.Direction.Y * _ball.Speed * gameTime.ElapsedGameTime.TotalSeconds);

        if (_ball.Bounds.X < 0) _ball.Direction.X = 1.0f;
        else if (_ball.Bounds.Right >= _graphics.PreferredBackBufferWidth) _ball.Direction.X = -1.0f;
        else if (_ball.Bounds.Y < 0) _ball.Direction.Y = 1.0f;
        else if (_ball.Bounds.Bottom >= _graphics.PreferredBackBufferHeight) _gameState = GameState.GameOver;
    }
}
