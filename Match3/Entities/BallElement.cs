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
        public bool IsActive = false;
        public Point GridPosition;
        public Vector2 Position;
        public BallColor CurrentColor;

        private Texture2D _baseTexture;
        private Texture2D _activeTexture;
        private SpriteBatch _spriteBatch;

        public BallElement(BallColor ballColor, Point position, Match3Game game) : base(game)
        {
            CurrentColor = ballColor;
            GridPosition = position;
        }


        public override void Initialize()
        {
            Position = new Vector2(0, 0);
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }


        protected override void LoadContent()
        {
            string baseTextureName = getBaseTextureName();
            string activeTextureName = baseTextureName + "-active";

            _baseTexture = Game.Content.Load<Texture2D>(baseTextureName);
            _activeTexture = Game.Content.Load<Texture2D>(activeTextureName);

            base.LoadContent();
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            Position.X = Constants.GameFieldX + Constants.GameFieldCell * GridPosition.X;
            Position.Y = Constants.GameFieldY + Constants.GameFieldCell * GridPosition.Y;

            _spriteBatch.Begin();
            _spriteBatch.Draw(IsActive ? _activeTexture : _baseTexture, Position, Color.White);
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
