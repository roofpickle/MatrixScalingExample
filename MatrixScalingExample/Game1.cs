using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MatrixScalingExample;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _awfulBackground;
    private SpriteFont _font;

    // Internal resolution you have chosen to render game at
    private const int _internalWidth = 1280;
    private const int _internalHeight = 720;

    // The resolution the game is rendered at (gets redetermined when window is resized)
    private int _virtualWidth = 1280;
    private int _virtualHeight = 720;

    private Matrix _scalingMatrix;
    private Viewport _viewport;
    private bool _resizing = false;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = _internalWidth;
        _graphics.PreferredBackBufferHeight = _internalHeight;
        _graphics.ApplyChanges();

        Window.ClientSizeChanged += OnClientSizeChanged;
        Window.AllowUserResizing = true;
        Window.IsBorderless = false;

        this.UpdateScalingMatrix();

        base.Initialize();
    }

    // Recalculate what our scaling logic will be whenever the window size is changed
    private void OnClientSizeChanged(object sender, EventArgs e)
    {
        // The resizing flag is just to stop the window needlessly recalculating as you drag the window (calculate once)
        if (!_resizing)
        {
            _resizing = true;
            UpdateScalingMatrix();
            _resizing = false;
        }
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _awfulBackground = Content.Load<Texture2D>("AwfulLandscape");
        _font = Content.Load<SpriteFont>("Tahoma32");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // This centers the scaled screen to the window, comment out to see what I mean if you want
        GraphicsDevice.Viewport = _viewport;

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _scalingMatrix);
        _spriteBatch.Draw(_awfulBackground, Vector2.Zero, Color.White);
        #if DEBUG
            _spriteBatch.DrawString(_font, "Internal Width: " + _internalWidth.ToString(), Vector2.Zero, Color.Red);
            _spriteBatch.DrawString(_font, "Internal Height: " + _internalHeight.ToString(), new Vector2(0, 50), Color.Red);
            _spriteBatch.DrawString(_font, "Virtual Width: " + _virtualWidth.ToString(), new Vector2(0, 100), Color.Red);
            _spriteBatch.DrawString(_font, "Virtual Height: " + _virtualHeight.ToString(), new Vector2(0, 150), Color.Red);
        #endif
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void UpdateScalingMatrix()
    {
        float screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
        float screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

        if (screenWidth / _internalWidth > screenHeight / _internalHeight)
        {
            float aspect = screenHeight / _internalHeight;
            _virtualWidth = (int)(aspect * _internalWidth);
            _virtualHeight = (int)screenHeight;
        }
        else
        {
            float aspect = screenWidth / _internalWidth;
            _virtualWidth = (int)screenWidth;
            _virtualHeight = (int)(aspect * _internalHeight);
        }

        _scalingMatrix = Matrix.CreateScale(_virtualWidth / (float)_internalWidth);

        _viewport = new Viewport
        {
            X = (int)(screenWidth / 2 - _virtualWidth / 2),
            Y = (int)(screenHeight / 2 - _virtualHeight / 2),
            Width = _virtualWidth,
            Height = _virtualHeight,
            MinDepth = 0,
            MaxDepth = 1
        };
    }
}
