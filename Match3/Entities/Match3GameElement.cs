using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Match3.Entities
{
    public enum ElementColor
    {
        Red,
        Yellow,
        Green,
        Blue,
        Light
    }

    public enum ElementState
    {
        Normal,
        Moving,
        Waiting,
        Vanishing,
        Removed
    }


    abstract class Match3GameElement : DrawableGameComponent
    {
        public ElementColor CurrentColor;
        public Vector2 Position;
        public ElementState State;

        protected Texture2D _baseTexture;
        protected SpriteBatch _spriteBatch;

        public Match3GameElement(Game game) : base(game) { }


        protected string getBaseTextureName()
        {
            string textureName = "img/";

            switch (CurrentColor)
            {
                case ElementColor.Red:
                    textureName += "red";
                    break;
                case ElementColor.Yellow:
                    textureName += "yellow";
                    break;
                case ElementColor.Green:
                    textureName += "green";
                    break;
                case ElementColor.Blue:
                    textureName += "blue";
                    break;
                case ElementColor.Light:
                    textureName += "light";
                    break;
                default:
                    return "";
            }
            return textureName;
        }

    }
}
