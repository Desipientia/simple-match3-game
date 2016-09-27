using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Match3.Entities
{
    class Button : DrawableGameComponent
    {
        private string _text;
        private string _fontName;
        private Color _color;
        private Rectangle _rectangle;
        private Vector2 _position;
        private Action _action;
        private BitmapFont _font;
        private SpriteBatch _spriteBatch;

        public Button(string text, string fontName, Color color, Vector2 position, Action action, Match3Game game) : base(game)
        {
            _text = text;
            _fontName = fontName;
            _color = color;
            _position = position;
            _action = action;
        }


        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            base.Initialize();
        }


        protected override void LoadContent()
        {
            _font = Game.Content.Load<BitmapFont>(_fontName);
            base.LoadContent();
        }


        public override void Update(GameTime gameTime)
        {
            HandleInput(Mouse.GetState());
            base.Update(gameTime);
        }


        public void HandleInput(MouseState mouseState)
        {
            //if (mouseState.LeftButton.)
            //{

            //}
        }


        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, _text, _position, _color == null ? Color.White : _color);
            _spriteBatch.End();

            base.Draw(gameTime);
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
