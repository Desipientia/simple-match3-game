﻿using System;
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
        private int _activeBallIndex = -1;
        private int _currentBallIndex = -1;

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
                if (_currentBallIndex == -1)
                {
                    manageBallClick();
                }
                else
                {
                    manageBallVanish();
                }
            }
            else
            {
                _okButton.Update(gameTime);
            }

            foreach (BallElement ball in _gameField)
            {
                ball.Update(gameTime);
            }

            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            if (!IsActive) { return; }

            _spriteBatch.Begin();

            Tuple<int, int[]> ballChains = findBallChains();
            string a = ballChains.Item1.ToString();
            for (int i = 0; i < ballChains.Item1; i++)
            {
                ((BallElement)_gameField[ballChains.Item2[i]]).IsActive = true;
            }

            _spriteBatch.Draw(_gameScreenTexture, new Vector2(0, 0), Color.White);
            _spriteBatch.DrawString(_font, "Score: " + a, new Vector2(Constants.ScoreBoxX, Constants.ScoreBoardY), Color.White);
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
                generateGameField();

                foreach (BallElement ball in _gameField)
                {
                    ball.Initialize();
                }

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
                        if (y >= 2)
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
            return ballsAtLeftFormChain((int)ball.GridPosition.X, (int)ball.GridPosition.Y, ball.CurrentColor);
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
            return ballsAtTopFormChain((int)ball.GridPosition.X, (int)ball.GridPosition.Y, ball.CurrentColor);
        }

        private bool ballsAtTopFormChain(int x, int y, BallColor color)
        {
            int firstBallIndex = (y - 1) * _width + x;
            int secondBallIndex = firstBallIndex - _width;

            return ((BallElement)_gameField[firstBallIndex]).CurrentColor == color &&
                ((BallElement)_gameField[secondBallIndex]).CurrentColor == color;
        }


        private void manageBallClick()
        {
            var mouseState = Mouse.GetState();

            if (_fieldBoundingBox.Contains(mouseState.X, mouseState.Y))
            {
                if (mouseState.LeftButton == ButtonState.Pressed &&
                    _previousLeftButtonState == ButtonState.Released)
                {
                    if (_activeBallIndex == -1)
                    {
                        BallElement ball = getSelectedBall(mouseState);

                        ball.IsActive = true;
                        _activeBallIndex = getBallCollectionIndex(ball);
                        _previousLeftButtonState = ButtonState.Pressed;
                    }
                    else
                    {
                        BallElement currentBall = getSelectedBall(mouseState);
                        BallElement activeBall = ((BallElement)_gameField[_activeBallIndex]);

                        activeBall.IsActive = false;

                        if (areNeighbours(currentBall.GridPosition, activeBall.GridPosition))
                        {
                            _currentBallIndex = getBallCollectionIndex(currentBall);
                            swapBalls(currentBall, activeBall);
                        }

                        _previousLeftButtonState = ButtonState.Pressed;
                    }
                }
                else if (mouseState.LeftButton == ButtonState.Released)
                {
                    _previousLeftButtonState = ButtonState.Released;
                }
            }
        }


        private BallElement getSelectedBall(MouseState mouseState)
        {
            int x = (mouseState.X - Constants.GameFieldX) / Constants.GameFieldCell;
            int y = (mouseState.Y - Constants.GameFieldY) / Constants.GameFieldCell;

            return (BallElement)_gameField[y * _width + x];
        }


        private bool areNeighbours(Point firstBall, Point secondBall)
        {
            bool areHorisontalNeighbours = firstBall.Y == secondBall.Y && differByOne(firstBall.X, secondBall.X);
            bool areVerticalNeighbours = firstBall.X == secondBall.X && differByOne(firstBall.Y, secondBall.Y);

            return (areHorisontalNeighbours && !areVerticalNeighbours) 
                || (areVerticalNeighbours && !areHorisontalNeighbours);
        }


        private bool differByOne(int a, int b)
        {
            return a == b - 1 || a == b + 1;
        }


        private void swapBalls(BallElement firstBall, BallElement secondBall)
        {
            int firstBallIndex = getBallCollectionIndex(firstBall);
            int secondBallIndex = getBallCollectionIndex(secondBall);
            BallElement newFirstBall = new BallElement(secondBall);
            BallElement newSecondBall = new BallElement(firstBall);

            newFirstBall.GridPosition = firstBall.GridPosition;
            newSecondBall.GridPosition = secondBall.GridPosition;
            newFirstBall.Move();
            newSecondBall.Move();

            _gameField.RemoveAt(firstBallIndex);
            _gameField.Insert(firstBallIndex, newFirstBall);
            _gameField.RemoveAt(secondBallIndex);
            _gameField.Insert(secondBallIndex, newSecondBall);
        }


        private int getBallCollectionIndex(BallElement ball)
        {
            return ball.GridPosition.Y * _width + ball.GridPosition.X;
        }


        private Tuple<int, int[]> findBallChains()
        {
            const int n = 32;
            int currentIndex = 0;
            int[] chainsArray = new int[n];

            foreach (BallElement ball in _gameField)
            {
                int ballIndex = getBallCollectionIndex(ball);
                
                if (Array.IndexOf(chainsArray, ballIndex) == -1 || ballIndex == 0)
                {
                    int tempIndex = currentIndex;

                    chainsArray[currentIndex++] = ballIndex;
                    currentIndex = addBallChain(ball, currentIndex, ballIndex, chainsArray, "hor");
                    currentIndex = addBallChain(ball, currentIndex, ballIndex, chainsArray, "vert");

                    if (currentIndex - tempIndex < 3)
                    {
                        currentIndex = tempIndex;
                    }
                }
            }

            return new Tuple<int, int[]>(currentIndex, chainsArray);
        }


        private int addBallChain(BallElement ball, int currentIndex, int ballIndex, int[] chainsArray, string direction)
        {
            int i;
            int x = ball.GridPosition.X;
            int y = ball.GridPosition.Y;
            int n = direction == "vert" ? _height - y : _width - x;

            for (i = 1; i < n; i++)
            {
                int index = direction == "vert" ? (y + i) * _height + x : y * _height + x + i;

                if (ball.CurrentColor == ((BallElement)_gameField[index]).CurrentColor)
                {
                    chainsArray[currentIndex++] = index;
                }
                else
                {
                    break;
                }
            }

            if (i < 3)
            {
                currentIndex -= i - 1;
            }

            return currentIndex;
        }


        private void manageBallVanish()
        {
            Tuple<int, int[]> ballChains = findBallChains();

            if (ballChains.Item1 == 0)
            {
                swapBalls(((BallElement)_gameField[_activeBallIndex]), ((BallElement)_gameField[_currentBallIndex]));
            }
            _currentBallIndex = -1;
            _activeBallIndex = -1;
        }

    }
}
