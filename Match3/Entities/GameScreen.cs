using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Match3.Entities
{
    class GameScreen : DrawableGameComponent
    {
        public bool isActive = false;
        public MainMenu mainMenu;

        //private int _width = 8;
        //private int _height = 8;
        private int _score;
        private int _duration;
        private bool _isGameOver = false;

        private BitmapFont _font;
        private SpriteBatch _spriteBatch;
        private Stopwatch _stopwatch;
        private Button _okButton;
        private Texture2D _gameOverMessage;
        private Texture2D _gameScreenTexture;

        public GameScreen(Match3Game game) : base(game) { }


        public override void Initialize()
        {
            Rectangle okButtonBox = new Rectangle(Constants.OkButtonX, Constants.OkButtonY, Constants.OkButtonWidth, Constants.OkButtonHeight);
            ButtonTexturePack okButtonTexturePack = new ButtonTexturePack(Constants.OkButtonTexture, Constants.OkButtonHoverTexture, Constants.OkButtonPressTexture);

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _stopwatch = new Stopwatch();
            _okButton = new Button(okButtonTexturePack, okButtonBox, gameOverClick, (Match3Game)Game);
            _okButton.Initialize();

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
            if (!isActive) { return; }

            if (!_isGameOver)
            {
                manageGameTime(); 
            }
            _okButton.Update(gameTime);

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            if (!isActive) { return; }

            _spriteBatch.Begin();

            _spriteBatch.Draw(_gameScreenTexture, new Vector2(0, 0), Color.White);
            _spriteBatch.DrawString(_font, "Score: " + _score, new Vector2(Constants.ScoreBoxX, Constants.ScoreBoardY), Color.White);
            _spriteBatch.DrawString(_font, "Time: " + _duration, new Vector2(Constants.TimeBoxX, Constants.ScoreBoardY), Color.White);

            if (_isGameOver)
            {
                _spriteBatch.Draw(_gameOverMessage, new Vector2 (0, 0), Color.White);
                _spriteBatch.End();
                _okButton.Draw(gameTime);
            }
            else
            {
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        
        // Private methods
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
            if (mainMenu == null) { return; }

            _isGameOver = false;
            isActive = false;
            mainMenu.isActive = true;
        }
    }
}
