﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
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

        private GameComponentCollection _vanishingBalls;
        private GameComponentCollection _movingBalls;
        private GameComponentCollection _destroyers;

        public GameScreen(Match3Game game) : base(game) { }


        public override void Initialize()
        {
            Rectangle okButtonBox = new Rectangle(Constants.OkButtonX, Constants.OkButtonY, Constants.OkButtonWidth, Constants.OkButtonHeight);
            ButtonTexturePack okButtonTexturePack = new ButtonTexturePack(Constants.OkButtonTexture, Constants.OkButtonHoverTexture, Constants.OkButtonPressTexture);

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _stopwatch = new Stopwatch();
            _fieldBoundingBox = new Rectangle(Constants.GameFieldX, Constants.GameFieldY, Constants.GameFieldCell * _width, Constants.GameFieldCell * _height);
            _destroyers = new GameComponentCollection();

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

                manageBallVanish();
            }
            else
            {
                _okButton.Update(gameTime);
            }

            foreach (BallElement ball in _gameField)
            {
                ball.Update(gameTime);
            }

            updateDestroyers(gameTime);
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

            foreach (Destroyer destroyer in _destroyers)
            {
                destroyer.Draw(gameTime);
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

            _activeBallIndex = -1;
            _currentBallIndex = -1;
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
                    ElementColor color = 0;

                    while (!isColorSutable)
                    {
                        isColorSutable = true;
                        color = (ElementColor)rand.Next(0, 5);

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

        private bool ballsAtLeftFormChain(int x, int y, ElementColor color)
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

        private bool ballsAtTopFormChain(int x, int y, ElementColor color)
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
                            activeBall.IsNew = true;
                            currentBall.IsNew = true;
                            swapBalls(currentBall, activeBall);
                        }
                        else
                        {
                            _activeBallIndex = -1;
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
            BallElement newFirstBall = createNewBallElement(secondBall);
            BallElement newSecondBall = createNewBallElement(firstBall);

            newFirstBall.GridPosition = firstBall.GridPosition;
            newSecondBall.GridPosition = secondBall.GridPosition;
            newFirstBall.State = ElementState.Moving;
            newSecondBall.State = ElementState.Moving;

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
                   
                currentIndex = addBallChain(ball, currentIndex, ballIndex, chainsArray, "horisontal");
                currentIndex = addBallChain(ball, currentIndex, ballIndex, chainsArray, "vertical"); 
            }

            return new Tuple<int, int[]>(currentIndex, chainsArray);
        }


        private int addBallChain(BallElement ball, int currentIndex, int ballIndex, int[] chainsArray, string direction)
        {
            int i;
            int x = ball.GridPosition.X;
            int y = ball.GridPosition.Y;
            int n = direction == "vertical" ? _height - y : _width - x;

            int[] tempArray = new int[n - 1];

            for (i = 1; i < n; i++)
            {
                int index = direction == "vertical" ? (y + i) * _height + x : y * _height + x + i;

                if (ball.CurrentColor == ((BallElement)_gameField[index]).CurrentColor)
                {
                    tempArray[i - 1] = index;
                }
                else
                {
                    break;
                }
            }

            if (i >= 3)
            {
                Array.Copy(tempArray, 0, chainsArray, currentIndex, i - 1);
                currentIndex += i - 1;

                if (Array.IndexOf(chainsArray, ballIndex) == -1)
                {
                    chainsArray[currentIndex++] = ballIndex;
                }

                if (ball.State != ElementState.Vanishing && ball.State != ElementState.Removed)
                { 
                    if (i == 4)
                    {
                        int lineIndex = getLastMovedBallIndex(tempArray, i - 1, ballIndex);
                        BonusType lineType = direction == "vertical" ? BonusType.VerticalLine : BonusType.HorisontalLine;

                        ((BallElement)_gameField[lineIndex]).whatBonusNext = lineType;
                    }
                    else if (i >= 5)
                    {
                        int bombIndex = getLastMovedBallIndex(tempArray, i - 1, ballIndex);

                        ((BallElement)_gameField[bombIndex]).whatBonusNext = BonusType.Bomb;
                    }
                }
            }

            return currentIndex;
        }


        private void manageBallVanish()
        {
            Tuple<int, int[]> ballChains = findBallChains();

            int length = ballChains.Item1;
            int[] chainsArray = ballChains.Item2;

            if (_activeBallIndex >= 0 && _currentBallIndex >= 0)
            {
                if (length == 0)
                {
                    swapBalls(((BallElement)_gameField[_activeBallIndex]), ((BallElement)_gameField[_currentBallIndex]));
                }
                _currentBallIndex = -1;
                _activeBallIndex = -1;
            }

            _vanishingBalls = new GameComponentCollection();

            for (int i = 0; i < length; i++)
            {
                destroyBall((BallElement)_gameField[chainsArray[i]]);
            }

            manageDestroyersInterception();

            fillEmptyCells();
            _vanishingBalls = null;
        }  


        private void fillEmptyCells()
        {
            int aboveCellIndex;
            Random rand = new Random();

            _movingBalls = new GameComponentCollection();

            for (int i = _gameField.Count - 1; i >= 0; i--)
            {
                BallElement ball = (BallElement)_gameField[i];
       
                if (ball.State != ElementState.Removed) { continue; }

                if (ball.whatBonusNext == BonusType.None && (aboveCellIndex = getFromAboveCell(i)) != -1)
                {
                    BallElement aboveBall = (BallElement)_gameField[aboveCellIndex];
                    BallElement newBall = createNewBallElement(aboveBall);
                    newBall.GridPosition = ball.GridPosition;
                    newBall.Wait(_vanishingBalls, ElementState.Moving);
                    aboveBall.State = ElementState.Removed;

                    _movingBalls.Add(newBall);
                    _gameField.RemoveAt(i);
                    _gameField.Insert(i, newBall);
                }
                else
                {
                    addBall(i, rand);
                }
            }
        }


        private int getFromAboveCell(int index) 
        {
            BallElement ball = (BallElement)_gameField[index];

            for (int i = ball.GridPosition.Y - 1; i >= 0; i--)
            {
                int aboveBallIndex = i * _width + ball.GridPosition.X;

                if (((BallElement)_gameField[aboveBallIndex]).State == ElementState.Normal)
                {
                    return aboveBallIndex;
                }
            }

            return -1;
        }


        private void addBall(int index, Random rand)
        {
            BallElement ball = (BallElement)_gameField[index];
            BallElement newBall = null;
            Vector2 position = ball.Position;
            GameComponentCollection waitFor = _movingBalls.Count == 0 ? _vanishingBalls : _movingBalls;

            switch (ball.whatBonusNext)
            {
                case BonusType.None:
                    newBall = new BallElement((ElementColor)rand.Next(0, 5), ball.GridPosition, (Match3Game)Game);
                    position = new Vector2(Constants.GameFieldX + ball.GridPosition.X * Constants.GameFieldCell, Constants.GameFieldY);
                    break;
                case BonusType.VerticalLine:
                case BonusType.HorisontalLine:
                    newBall = new LineElement(ball.whatBonusNext, ball);
                    break;
                case BonusType.Bomb:
                    newBall = new BombElement(ball);
                    break;
                default:
                    break;
            }

            newBall.Initialize();
            newBall.Position = position;
            newBall.Wait(waitFor, ElementState.Moving);

            _gameField.RemoveAt(index);
            _gameField.Insert(index, newBall);
        }


        private int getLastMovedBallIndex(int[] balls, int n, int currentballIndex)
        {
            for (int i = 0; i < n; i++)
            {
                if (((BallElement)_gameField[balls[i]]).IsNew)
                {
                    return balls[i];
                }
            }

            return currentballIndex;
        }


        private BallElement createNewBallElement(BallElement oldBall)
        {
            Type type = oldBall.GetType();
            
            if (type.Equals(typeof(LineElement)))
            {
                return new LineElement((LineElement)oldBall);
            }
            else if (type.Equals(typeof(BombElement)))
            {
                return new BombElement(oldBall);
            }
            else
            {
                return new BallElement(oldBall);
            }
        }


        private void createDestroyers(LineElement line)
        {
            int width = _width * Constants.GameFieldCell;
            int height = _height * Constants.GameFieldCell;
            Vector2 position = new Vector2(0, 0);

            position.X = Constants.GameFieldX + Constants.GameFieldCell * line.GridPosition.X + Constants.GameFieldCell / 2;
            position.Y = Constants.GameFieldY + Constants.GameFieldCell * line.GridPosition.Y + Constants.GameFieldCell / 2;

            Tuple<Direction, Direction> direction = line.GetDestroyersDirection();
            Destroyer firstDestroyer = new Destroyer(direction.Item1, position, line.CurrentColor, width, height, (Match3Game)Game);
            Destroyer secondDestroyer = new Destroyer(direction.Item2, position, line.CurrentColor, width, height, (Match3Game)Game);

            firstDestroyer.Initialize();
            secondDestroyer.Initialize();

            _destroyers.Add(firstDestroyer);
            _destroyers.Add(secondDestroyer);
        }


        private void manageDestroyersInterception()
        {
            for (int i = 0; i < _destroyers.Count; i++)
            {
                Destroyer destroyer = (Destroyer)_destroyers[i];

                int x = ((int)destroyer.Position.X - Constants.GameFieldX) / Constants.GameFieldCell;
                int y = ((int)destroyer.Position.Y - Constants.GameFieldY) / Constants.GameFieldCell;
                int index = y * _width + x;

                if (index > _width * _height - 1) { continue; }

                destroyBall((BallElement)_gameField[index]);
            }
        }


        private void updateDestroyers(GameTime gameTime)
        {
            List<Destroyer> toRemove = new List<Destroyer>();

            foreach (Destroyer destroyer in _destroyers)
            {
                destroyer.Update(gameTime);

                if (destroyer.State == ElementState.Removed)
                {
                    toRemove.Add(destroyer);
                    continue;
                }
            }

            foreach (Destroyer destroyer in toRemove)
            {
                _destroyers.Remove(destroyer);
            }
        }


        private void destroyBall(BallElement ball)
        {
            if (!(ball.State == ElementState.Vanishing || ball.State == ElementState.Removed))
            {
                if (ball.GetType().Equals(typeof(LineElement)))
                {
                    createDestroyers((LineElement)ball);
                }

                ball.Vanish();
                _vanishingBalls.Add(ball);
                _score += 10;
            }
        }

    }
}
