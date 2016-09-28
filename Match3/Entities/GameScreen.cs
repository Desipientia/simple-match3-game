using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;

namespace Match3.Entities
{
    class GameScreen : DrawableGameComponent
    {
        public bool IsActive = false;
        public MainMenu CurrentMainMenu;

        private int _width = 8;
        private int _height = 8;
        private int _score;
        private int _duration;
        private bool _isGameOver = false;

        private BitmapFont _font;
        private SpriteBatch _spriteBatch;
        private Stopwatch _stopwatch;
        private Button _okButton;
        private Texture2D _gameOverMessage;
        private Texture2D _gameScreenTexture;
        private Rectangle _fieldBoundingBox;
        private GameComponentCollection _gameField;

        private ButtonState _previousLeftButtonState = ButtonState.Pressed;

        public GameScreen(Match3Game game) : base(game) { }


        public override void Initialize()
        {
            Rectangle okButtonBox = new Rectangle(Constants.OkButtonX, Constants.OkButtonY, Constants.OkButtonWidth, Constants.OkButtonHeight);
            ButtonTexturePack okButtonTexturePack = new ButtonTexturePack(Constants.OkButtonTexture, Constants.OkButtonHoverTexture, Constants.OkButtonPressTexture);

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _stopwatch = new Stopwatch();
            _fieldBoundingBox = new Rectangle(Constants.GameFieldX, Constants.GameFieldY, Constants.GameFieldCell * _width, Constants.GameFieldCell * _height);

            _okButton = new Button(okButtonTexturePack, okButtonBox, gameOverClick, (Match3Game)Game);
            _okButton.Initialize();

            generateGameField();

            foreach (BallElement ball in _gameField)
            {
                ball.Initialize();
            }

            base.Initialize();
        }


        protected override void LoadContent()
        {
            _font = Game.Content.Load<BitmapFont>(Constants.BaseFontName);
            _gameScreenTexture = Game.Content.Load<Texture2D>(Constants.GameScreenTexture);
            _gameOverMessage = Game.Content.Load<Texture2D>(Constants.GameOverMessageTexture);

            base.LoadContent();
        }


        public override void Update(GameTime gameTime)
        {
            if (!IsActive) { return; }

            if (!_isGameOver)
            {
                manageGameTime();
                manageBallClick();
            }
            else
            {
                _okButton.Update(gameTime);
            }

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            if (!IsActive) { return; }

            _spriteBatch.Begin();

            _spriteBatch.Draw(_gameScreenTexture, new Vector2(0, 0), Color.White);
            _spriteBatch.DrawString(_font, "Score: " + _score, new Vector2(Constants.ScoreBoxX, Constants.ScoreBoardY), Color.White);
            _spriteBatch.DrawString(_font, "Time: " + _duration, new Vector2(Constants.TimeBoxX, Constants.ScoreBoardY), Color.White);
            _spriteBatch.End();

            foreach (BallElement ball in _gameField)
            {
                ball.Draw(gameTime);
            }

            if (_isGameOver)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(_gameOverMessage, new Vector2 (0, 0), Color.White);
                _spriteBatch.End();
                _okButton.Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        
        // Private methods.
        private void manageGameTime()
        {
            if (_stopwatch.IsRunning)
            {
                if (_duration < 0)
                {
                    _stopwatch.Stop();
                    _stopwatch.Reset();
                    _isGameOver = true;
                }

                _duration = Constants.GameDuration - (int)(_stopwatch.ElapsedTicks / Stopwatch.Frequency);
            }
            else
            {
                _duration = Constants.GameDuration;
                _score = 0;
                _stopwatch.Start();
            }
        }


        private void gameOverClick()
        {
            if (CurrentMainMenu == null) { return; }

            _isGameOver = false;
            IsActive = false;
            CurrentMainMenu.IsActive = true;
        }


        private void generateGameField()
        {
            Random rand = new Random();

            _gameField = new GameComponentCollection();

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    bool isColorSutable = false;;
                    BallColor color = 0;

                    while (!isColorSutable)
                    {
                        isColorSutable = true;
                        color = (BallColor)rand.Next(0, 5);

                        if (x >= 2)
                        {
                            isColorSutable &= !ballsAtLeftFormChain(x, y, color);
                        }
                        if (y > 2)
                        {
                            isColorSutable &= !ballsAtTopFormChain(x, y, color);
                        }
                    }

                    _gameField.Add(new BallElement(color, new Point(x, y), (Match3Game)Game));
                }
            }
        }


        private bool ballsAtLeftFormChain(BallElement ball)
        {
            return ballsAtLeftFormChain((int)ball.Position.X, (int)ball.Position.Y, ball.CurrentColor);
        }

        private bool ballsAtLeftFormChain(int x, int y, BallColor color)
        {
            int firstBallIndex = y * _width + x - 1;
            int secondBallIndex = firstBallIndex - 1;

            return ((BallElement)_gameField[firstBallIndex]).CurrentColor == color &&
                ((BallElement)_gameField[secondBallIndex]).CurrentColor == color;
        }


        private bool ballsAtTopFormChain(BallElement ball)
        {
            return ballsAtTopFormChain((int)ball.Position.X, (int)ball.Position.Y, ball.CurrentColor);
        }

        private bool ballsAtTopFormChain(int x, int y, BallColor color)
        {
            int firstBallIndex = (y - 1) * _width + x;
            int secondBallIndex = firstBallIndex - _width;

            return ((BallElement)_gameField[firstBallIndex]).CurrentColor == color &&
                ((BallElement)_gameField[secondBallIndex]).CurrentColor == color;
        }

    }
}
