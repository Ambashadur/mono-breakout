using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mono.Breakout.Components;

namespace Mono.Breakout;

public class GameExec : Game
{
    private const int WIDTH = 600;
    private const int HEIGHT = 800;

    private GameState _gameState;
    private Bar _bar;
    private Ball _ball;
    private KeyboardState _keyboardState;
    private Label _gameOverLabel;
    private Block[] _blocks;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public GameExec() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize() {
        _graphics.PreferredBackBufferWidth = WIDTH;
        _graphics.PreferredBackBufferHeight = HEIGHT;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _bar = new Bar {
            Speed = 300,
            Direction = 0.0f,
            Bounds = new Rectangle((WIDTH - 100) / 2, HEIGHT - 50, 100, 20),
            Texture = new Texture2D(GraphicsDevice, 1, 1)
        };

        _bar.Texture.SetData([Color.Chocolate]);

        var ballTexture = new Texture2D(GraphicsDevice, 1, 1);
        ballTexture.SetData([Color.White]);
        _ball = new Ball {
            Speed = 150,
            Direction = new Vector2(1, 1),
            Bounds = new Rectangle((WIDTH - 15) / 2, (HEIGHT - 15) / 2, 15, 15),
            Texture = ballTexture
        };

        _gameOverLabel = new Label {
            Text = "GAME OVER",
            Font = Content.Load<SpriteFont>("monogram")
        };

        var labelBounds = _gameOverLabel.Font.MeasureString(_gameOverLabel.Text);
        _gameOverLabel.Position = new Vector2((WIDTH - labelBounds.X) / 2, (HEIGHT - labelBounds.Y) / 2);

        _blocks = new Block[6];
        var blockTexture = new Texture2D(GraphicsDevice, 1, 1);
        blockTexture.SetData([Color.Red]);
        var blockWidth = (WIDTH - 40 - 20 * (_blocks.Length - 1)) / _blocks.Length;
        var offset = new Vector2(20, 20);

        for (int i = 0; i < _blocks.Length; i++) {
            _blocks[i].Texture = blockTexture;
            _blocks[i].Score = 1;
            _blocks[i].Bounds.X = (int)offset.X;
            _blocks[i].Bounds.Y = (int)offset.Y;
            _blocks[i].Bounds.Width = blockWidth;
            _blocks[i].Bounds.Height = 20;

            offset.X += blockWidth + 20;
        }
    }

    protected override void Update(GameTime gameTime) {
        _keyboardState = Keyboard.GetState();
        if (_keyboardState.IsKeyDown(Keys.Escape)) Exit();

        _gameState = _keyboardState.IsKeyDown(Keys.Space) switch {
            true when _gameState is GameState.Running => GameState.Pause,
            true when _gameState is GameState.Pause => GameState.Running,
            _ => _gameState
        };

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

        _spriteBatch.Draw(_bar.Texture, _bar.Bounds, Color.White);
        _spriteBatch.Draw(_ball.Texture, _ball.Bounds, Color.White);

        for (int i = 0; i < _blocks.Length; i++)
            _spriteBatch.Draw(_blocks[i].Texture, _blocks[i].Bounds, Color.White);

        if (_gameState == GameState.GameOver)
            _spriteBatch.DrawString(_gameOverLabel.Font, _gameOverLabel.Text, _gameOverLabel.Position, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void HandleKeyboard(GameTime gameTime) {
        if (_keyboardState.IsKeyDown(Keys.Right) || _keyboardState.IsKeyDown(Keys.D)) _bar.Direction = 1.0f;
        else if (_keyboardState.IsKeyDown(Keys.Left) || _keyboardState.IsKeyDown(Keys.A)) _bar.Direction = -1.0f;
        else _bar.Direction = 0.0f;

        _bar.Bounds.X += (int)(_bar.Direction * _bar.Speed * gameTime.ElapsedGameTime.TotalSeconds);

        if (_bar.Bounds.X < 0) _bar.Bounds.X = 0;
        else if (_bar.Bounds.Right >= WIDTH) _bar.Bounds.X = WIDTH - _bar.Bounds.Width;
    }

    private void ProcessBall(GameTime gameTime) {
        if (_ball.Bounds.Intersects(_bar.Bounds)) {
            _ball.Direction.Y = -1.0f;
            _ball.Direction.X = _bar.Direction;
            _ball.Bounds.Y = _bar.Bounds.Y - _ball.Bounds.Height;
        }

        _ball.Bounds.X += (int)(_ball.Direction.X * _ball.Speed * gameTime.ElapsedGameTime.TotalSeconds);
        _ball.Bounds.Y += (int)(_ball.Direction.Y * _ball.Speed * gameTime.ElapsedGameTime.TotalSeconds);

        if (_ball.Bounds.X < 0) _ball.Direction.X = 1.0f;
        else if (_ball.Bounds.Right >= WIDTH) _ball.Direction.X = -1.0f;
        else if (_ball.Bounds.Y < 0) _ball.Direction.Y = 1.0f;
        else if (_ball.Bounds.Bottom >= HEIGHT) _gameState = GameState.GameOver;
    }
}
