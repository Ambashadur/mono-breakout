using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mono.Breakout.Components;

namespace Mono.Breakout;

public class GameExec : Game
{
    private const int WIDTH = 800;
    private const int HEIGHT = 800;
    private const int UI_BORDER = 162;
    private const int BLOCK_GAP = 2;

    private int _score;
    private GameState _gameState;
    private Bar _bar;
    private Ball _ball;
    private KeyboardState _keyboardState;
    private Label _gameOverLabel;
    private Label _scoreLabel;
    private Background _uiBackground;
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
            Bounds = new Rectangle((WIDTH - UI_BORDER - 100) / 2, HEIGHT - 40, 100, 20),
            Texture = new Texture2D(GraphicsDevice, 1, 1)
        };

        _bar.Texture.SetData([Color.Chocolate]);

        var ballTexture = new Texture2D(GraphicsDevice, 1, 1);
        ballTexture.SetData([Color.White]);
        _ball = new Ball {
            Speed = 150,
            Direction = new Vector2(1, 1),
            Bounds = new Rectangle((WIDTH - UI_BORDER - 15) / 2, (HEIGHT - 15) / 2, 15, 15),
            Texture = ballTexture
        };

        _gameOverLabel = new Label {
            Text = "GAME OVER",
            Font = Content.Load<SpriteFont>("monogram")
        };

        var labelBounds = _gameOverLabel.Font.MeasureString(_gameOverLabel.Text);
        _gameOverLabel.Position = new Vector2((WIDTH - labelBounds.X) / 2 + UI_BORDER / 2, (HEIGHT - labelBounds.Y) / 2);

        _blocks = new Block[8];
        var blockTexture = new Texture2D(GraphicsDevice, 1, 1);
        blockTexture.SetData([Color.Yellow]);
        var blockWidth = (WIDTH - UI_BORDER - 40 - BLOCK_GAP * (_blocks.Length - 1)) / _blocks.Length;
        var offset = new Vector2(UI_BORDER + 20, 20);

        for (int i = 0; i < _blocks.Length; i++) {
            _blocks[i].Texture = blockTexture;
            _blocks[i].Score = 3;
            _blocks[i].Bounds = new((int)offset.X, (int)offset.Y, blockWidth, 20);

            offset.X += blockWidth + BLOCK_GAP;
        }

        _uiBackground.Bounds = new Rectangle(0, 0, UI_BORDER, HEIGHT);
        _uiBackground.Texture = new Texture2D(GraphicsDevice, 1, 1);
        _uiBackground.Texture.SetData([Color.Purple]);

        _scoreLabel = new Label {
            Text = "Score: 0",
            Font = Content.Load<SpriteFont>("monogram")
        };

        var scoreLabelBounds = _scoreLabel.Font.MeasureString(_scoreLabel.Text);
        _scoreLabel.Position = new Vector2((UI_BORDER - scoreLabelBounds.X) / 2, 20);
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
        _spriteBatch.Draw(_uiBackground.Texture, _uiBackground.Bounds, Color.White);
        _spriteBatch.DrawString(_scoreLabel.Font, _scoreLabel.Text, _scoreLabel.Position, Color.White);

        for (int i = 0; i < _blocks.Length; i++) {
            if (_blocks[i].Hide) continue;
            _spriteBatch.Draw(_blocks[i].Texture, _blocks[i].Bounds, Color.White);
        }

        if (_gameState == GameState.GameOver)
            _spriteBatch.DrawString(_gameOverLabel.Font, _gameOverLabel.Text, _gameOverLabel.Position, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void HandleKeyboard(GameTime gameTime) {
        if (_keyboardState.IsKeyDown(Keys.Right) || _keyboardState.IsKeyDown(Keys.D)) {
            _bar.Direction = 1.0f;
        } else if (_keyboardState.IsKeyDown(Keys.Left) || _keyboardState.IsKeyDown(Keys.A)) {
            _bar.Direction = -1.0f;
        } else {
            _bar.Direction = 0.0f;
            return;
        }

        _bar.Bounds.X += (int)(_bar.Direction * _bar.Speed * gameTime.ElapsedGameTime.TotalSeconds);

        if (_bar.Bounds.X < UI_BORDER) _bar.Bounds.X = UI_BORDER;
        else if (_bar.Bounds.Right >= WIDTH) _bar.Bounds.X = WIDTH - _bar.Bounds.Width;
    }

    private void ProcessBall(GameTime gameTime) {
        _ball.Bounds.X += (int)(_ball.Direction.X * _ball.Speed * gameTime.ElapsedGameTime.TotalSeconds);
        _ball.Bounds.Y += (int)(_ball.Direction.Y * _ball.Speed * gameTime.ElapsedGameTime.TotalSeconds);

        if (_ball.Bounds.Intersects(_bar.Bounds)) {
            if (_ball.Bounds.Left < _bar.Bounds.Left) {
                _ball.Direction.X = -1.0f;
            } else if (_ball.Bounds.Right > _bar.Bounds.Right) {
                _ball.Direction.X = 1.0f;
            } else if (_ball.Bounds.Top < _bar.Bounds.Top) {
                _ball.Direction.Y = -1.0f;
                _ball.Direction.X += _bar.Direction;
            }

            return;
        }

        for (int i = 0; i < _blocks.Length; i++) {
            if (_blocks[i].Hide || !_blocks[i].Bounds.Intersects(_ball.Bounds)) continue;

            _blocks[i].Hide = true;
            _score += _blocks[i].Score;
            _scoreLabel.Text = $"Score: {_score}";
            var scoreLabelBounds = _scoreLabel.Font.MeasureString(_scoreLabel.Text);
            _scoreLabel.Position = new Vector2((UI_BORDER - scoreLabelBounds.X) / 2, 20);

            if (_ball.Bounds.Left < _blocks[i].Bounds.Left) _ball.Direction.X = -1.0f;
            else if (_ball.Bounds.Right > _blocks[i].Bounds.Right) _ball.Direction.X = 1.0f;
            else if (_ball.Bounds.Top < _blocks[i].Bounds.Top) _ball.Direction.Y = -1.0f;
            else if (_ball.Bounds.Bottom > _blocks[i].Bounds.Bottom) _ball.Direction.Y = 1.0f;

            return;
        }

        if (_ball.Bounds.X < UI_BORDER) _ball.Direction.X = 1.0f;
        else if (_ball.Bounds.Right >= WIDTH) _ball.Direction.X = -1.0f;
        else if (_ball.Bounds.Y < 0) _ball.Direction.Y = 1.0f;
        else if (_ball.Bounds.Bottom >= HEIGHT) _gameState = GameState.GameOver;
    }
}
