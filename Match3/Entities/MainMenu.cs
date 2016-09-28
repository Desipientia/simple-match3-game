using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3.Entities
{
    class MainMenu : DrawableGameComponent
    {
        public bool IsActive = true;
        public GameScreen CurrentGameScreen;

        private SpriteBatch _spriteBatch;
        private Button _playButton;
        private Texture2D _mainMenuTexture;

        public MainMenu(Match3Game game) : base(game) { }


        public override void Initialize()
        {
            Rectangle playButtonBox = new Rectangle(Constants.PlayButtonX, Constants.PlayButtonY, Constants.PlayButtonWidth, Constants.PlayButtonHeight);
            ButtonTexturePack playButtonTexturePack = new ButtonTexturePack(Constants.PlayButtonTexture, Constants.PlayButtonHoverTexture, Constants.PlayButtonPressTexture);

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _playButton = new Button(playButtonTexturePack, playButtonBox, playOnClick, (Match3Game)Game);
            _playButton.Initialize();

            base.Initialize();
        }


        protected override void LoadContent()
        {
            _mainMenuTexture = Game.Content.Load<Texture2D>(Constants.MainMenuTexture);

            base.LoadContent();
        }


        public override void Update(GameTime gameTime)
        {
            if (!IsActive) { return; }

            _playButton.Update(gameTime);
            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            if (!IsActive) { return; }

            _spriteBatch.Begin();
            _spriteBatch.Draw(_mainMenuTexture, new Vector2(0, 0), Color.White);
            _spriteBatch.End();

            _playButton.Draw(gameTime);

            base.Draw(gameTime);
        }


        // Private methods
        private void playOnClick()
        {
            if (CurrentGameScreen == null) { return; }

            IsActive = false;
            CurrentGameScreen.IsActive = true;
        }

    }
}
