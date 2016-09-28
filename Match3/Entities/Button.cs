using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Match3.Entities
{
    public enum BState
    {
        Normal,
        Hover,
        Pressed
    }


    public struct ButtonTexturePack
    {
        public string BaseTextureName;
        public string OnHoverTextureName;
        public string OnPressTextureName;

        public ButtonTexturePack(string baseTextureName, string onHoverTextureName, string onPressTextureName)
        {
            BaseTextureName = baseTextureName;
            OnHoverTextureName = onHoverTextureName;
            OnPressTextureName = onPressTextureName;
        }
    }


    class Button : DrawableGameComponent
    {
        private Texture2D _baseTexture;
        private Texture2D _onHoverTexture;
        private Texture2D _onPressTexture;
        private ButtonTexturePack _textureNamePack;
        private Action _action;
        private SpriteBatch _spriteBatch;
        private Rectangle _boundingBox;
        private BState _state;

        public Button(ButtonTexturePack textureNamePack, Rectangle boundingBox,  Action action, Match3Game game) : base(game)
        {
            _textureNamePack = textureNamePack;
            _boundingBox = boundingBox;
            _action = action;
        }


        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _state = BState.Normal;

            base.Initialize();
        }


        protected override void LoadContent()
        {
            _baseTexture = Game.Content.Load<Texture2D>(_textureNamePack.BaseTextureName);
            _onHoverTexture = Game.Content.Load<Texture2D>(_textureNamePack.OnHoverTextureName);
            _onPressTexture = Game.Content.Load<Texture2D>(_textureNamePack.OnPressTextureName);

            base.LoadContent();
        }


        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            if (_boundingBox.Contains(mouseState.X, mouseState.Y))
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    _state = BState.Pressed;
                }
                else 
                {
                    if (_state == BState.Pressed && _action != null)
                    {
                        _state = BState.Normal;
                        _action.Invoke();
                    }
                    else
                    {
                        _state = BState.Hover;
                    }
                }
            }
            else
            {
                _state = BState.Normal;
            }

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            Texture2D currentTexture = _state == BState.Normal ? _baseTexture : _state == BState.Hover ? _onHoverTexture : _onPressTexture;

            _spriteBatch.Begin();
            _spriteBatch.Draw(currentTexture, new Vector2(0, 0), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
