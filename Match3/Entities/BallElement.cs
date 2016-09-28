using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3
{
    public enum BallColor
    {
        red,
        yellow,
        green,
        blue,
        light
    }


    class BallElement : DrawableGameComponent
    {
        public bool isActive = false;
        public BallColor CurrentColor;

        private Vector2 _position;
        private Texture2D _baseTexture;
        private Texture2D _activeTexture;
        private SpriteBatch _spriteBatch;

        public BallElement(BallColor ballColor, Vector2 position, Match3Game game) : base(game)
        {
            CurrentColor = ballColor;
            _position = position;
        }


        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }


        protected override void LoadContent()
        {
            string baseTextureName = getBaseTextureName();
            string activeTextureName = baseTextureName + "-active";

            _baseTexture = Game.Content.Load<Texture2D>(baseTextureName);
            _activeTexture = Game.Content.Load<Texture2D>(baseTextureName);

            base.LoadContent();
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(isActive ? _activeTexture : _baseTexture, _position, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }


        // Private methods.
        private string getBaseTextureName()
        {
            string textureName = "img/";

            switch (CurrentColor)
            {
                case BallColor.red:
                    textureName += "red";
                    break;
                case BallColor.yellow:
                    textureName += "yellow";
                    break;
                case BallColor.green:
                    textureName += "green";
                    break;
                case BallColor.blue:
                    textureName += "blue";
                    break;
                case BallColor.light:
                    textureName += "light";
                    break;
                default:
                    return "";
            }
            return textureName + "-ball";
        }

    }
}
