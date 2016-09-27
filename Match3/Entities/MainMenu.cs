using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Match3.Entities
{
    class MainMenu : DrawableGameComponent
    {
        private const string _heading = "Simple Match3 Game";
        private const string _baseFontName = "main-font";

        private BitmapFont _font;
        private SpriteBatch _spriteBatch;

        private Button _playButton;

        public MainMenu(Match3Game game) : base(game) {}


        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _playButton = new Button("Play!", _baseFontName, Color.Orange, new Vector2(305, 340), playOnClick, (Match3Game)Game);
            _playButton.Initialize();

            base.Initialize();
        }


        private void playOnClick()
        {

        }


        protected override void LoadContent()
        {
            _font = Game.Content.Load<BitmapFont>(_baseFontName);
            base.LoadContent();
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, _heading, new Vector2(80, 30), Color.FloralWhite);
            _playButton.Draw(gameTime);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
