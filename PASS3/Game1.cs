// Author: Julian Kim
// File Name: Game1.cs
// Project Name: Meow Meow Misses Home (Final Project Phase 2)
// Creation Date: Nov. 20, 2023
// Modified Date: Jan. 21, 2024
// Description: A platformer game where the player plays as a cat finding their way home

// Course Concepts: 
// Arrays:
// Every collision platform is stores in an array of rectangle. The array allows for each platform to be checked for collision easiler using iteration (for loops)
// Each enemy is stored in an array of the Enemy class. The array allows for each enemy to be updated easily through iteration (for loops)

// Selection: Most of the program is built with selection statements.
// Switch statements are used to select what to update or draw. It uses the gameState variable, and throughout the program, gameState is updated. As gameState changes, the selection path changes.
// Platform collisions are checked using if else if statements. It uses the Interects() method from rectangles, which returns a bool value to perform collision actions.
// Button presses are check using if statements. It uses the .IsDown() method from buttons, which returns a bool value to perform button press actions.
// Player movement is carried out using if else if statements. It uses the .IsKeyDown() method from KeyboardState, which return a bool value to perform respective movements
// If statements are used all over this program to allow or disallow certain actions. For example, player should not be able to place down a trampoline if they have not collected it yet.
// Selections is used to display certain images based on conditions. For example, level one images should not be displayed when player is playing a level two. Bools are updated from Update() for displaying conditions.

// Loops:
// Every platform array uses a for loop whenever each element of the array needs to be checked or drawn. For example, each platform should continuously be checked for collisions, so a for loop creates an index value which increments each loop and is used to access each element of the platform rectangle array. 
// The Enemy array is interated through using a for loop to update each Enemy instance. A for loop creates an index value which increments each loop and is used to access each element of the Enemy array. 
// Each Enemy instance is drawn by iterating through each element of the array using a for loop. A for loop creates an index value which increments each loop and is used to access each element of the Enemy array. 

// Methods:
// Many images or text needs to be centered in relation to a certain length. CenterX(float imgWidth, int length, int offsetX), CenterX(SpriteFont font, string s, int length, int offsetX), and CenterFontY(SpriteFont font, string s, int height, int offsetY) take the arguments and return X coordinate that would center the image/text
// Player movement is used throughout game states and levels, whether is is user controlled or automatic. Therefore, methods such as UpdateSpeed() and UpdatePlayerPos() need to be called in reused in different areas. 
// Some methods are used to group code by purpose, to declutter code and avoid huge blocks of code in single selection statements. For example, UpdateLevelOne() and UpdateLevelTwo() are used to separate each level updates to avoid clutter in the Update() method

using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PASS3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // Store window dimensions
        int screenWidth;
        int screenHeight;

        // Store fonts
        SpriteFont defaultFont;
        SpriteFont helpFont;
        SpriteFont actionFont;
        SpriteFont titleFont;

        // Store random class instance
        Random random = new Random();

        // Store current level
        const int LEVEL_ONE = 1;
        const int LEVEL_TWO = 2;
        int levelPlaying = LEVEL_ONE;

        // Store current game state
        const int MENU = 0;
        const int SETTINGS = 1;
        const int GAME = 2;
        const int ENDGAME = 3;
        int gameState = MENU;

        // Store input states
        KeyboardState kb;
        MouseState mouse;

        // Store background music
        Song gameMusic;

        // Store menu backgrounds
        Texture2D menuBkg;
        Rectangle menuRec1;
        Rectangle menuRec2;
        Rectangle menuRec3;

        // Store play button and action
        Texture2D playUnpressedImg;
        Texture2D playHoverImg;
        Button playButton;
        Action goToGame;

        // Store exit button
        Texture2D exitUnpressedImg;
        Texture2D exitHoverImg;
        Button exitButton;

        // Store settings button and action
        Texture2D settingsUnpressedImg;
        Texture2D settingsHoverImg;
        Button settingsButton;
        Action goToSettings;

        // Store select sound effect
        SoundEffect selectSfx;

        // Store settings background
        Texture2D settingsBkg;
        Rectangle settingsBkgRec;

        // Store music slider image
        Texture2D musicSliderImg;
        Rectangle musicSliderRec;

        // Store slider bar rectangle
        GameRectangle musicSliderBox;

        // Store previous player position and speed
        Vector2 prevPlayerPos;
        Vector2 prevPlayerSpeed;

        // Store controls image
        Texture2D controlsImg;
        Rectangle controlsRec;
        Rectangle controlsShadowRec;

        // Store camera following player
        Cam2D camLvl1;
        Cam2D camLvl2;

        // Store world boundaries
        Rectangle worldBounds;

        // Store player position and collision rectangles
        Rectangle playerRec;
        Rectangle feetRec;
        Rectangle headRec;
        Rectangle rightSideRec;
        Rectangle leftSideRec;

        // Store player position and speeds
        Vector2 playerPos;
        Vector2 playerSpeed;
        const float MAX_SPEED = 5f;
        const float PLAYER_ACCEL = 0.2f;

        // Store max jump speed
        float jumpSpeed = -6.0f;

        // Store check for touching ground
        bool onGround = false;

        // Store forces
        const float PLAYER_FRICTION = PLAYER_ACCEL * 0.7f;
        const float PLAYER_TOL = PLAYER_ACCEL * 0.9f;
        const float GRAVITY = 9.81f / 60;
        Vector2 forces = new Vector2(PLAYER_FRICTION, GRAVITY);

        // Store direction player faces
        int playerDir = 1;

        // Store player movement animations
        Dictionary<string, Animation> playerMoveAnims;

        // Store check for movement
        bool isPlayerMoving = false;

        // Store background animation
        Animation grassyBkgAnim;

        // Store sword animations
        Animation getSwordAnim;
        Animation swingAnim;

        // Store lift floor image and rectangles
        Texture2D liftFloorImg;
        Rectangle liftFloorRec;
        Rectangle collLiftFloorRec;

        // Store lift checks
        bool onLift = false;
        bool liftStarted = false;

        // Store lift arc iamge and rectangle
        Texture2D liftArchImg;
        Rectangle liftArchRec;

        // Store lift sound effect
        SoundEffect liftSfx;

        // Store lift locked sound effect
        SoundEffect lockedSfx;

        // Store platform collision rectangles and check for collision
        Rectangle[] lvl1Plats = new Rectangle[10];

        // Store stair collision rectangles
        Rectangle[] stairRecs = new Rectangle[7];

        // Store bridge rectangle
        Rectangle bridgeRec;

        // Store checks for fishing
        bool isFishing = false;
        bool onBridge = false;

        // Store splash sound effect
        SoundEffect splashSfx;

        // Store fishing timer
        Timer caughtFishTimer;
        int caughtTimerTarget;

        // Store fishing timer visual
        string timerVisual = "";

        // Store collect sound effect
        SoundEffect collectSfx;

        // Store trampoline checks
        bool hasTrampoline = false;
        bool placedTrampoline = false;

        // Store trampoline placeable area
        Rectangle trampolinePlaceArea;
        GameRectangle trampolinePlaceRec;

        // Store trampoline image and collision rectangle
        Texture2D trampolineImg;
        Rectangle trampolineRec;

        // Store boing sound effect
        SoundEffect boingSfx;

        // Store trampoline placing sound effect
        SoundEffect placeSfx;

        // Store key image and collision rectangle
        Texture2D keyImg;
        Rectangle keyRec;
        float keyAlpha = 1;

        // Store key checks
        bool collectedKey = false;
        bool drawNoKeyInteract = false;

        // Store sign collision rectangle and check
        Rectangle signRec;
        bool onSign = false;
        bool signInteract = false;

        // Store interact button image and rectangle
        Texture2D eImg;
        Rectangle eRec;

        // Store message box image, rectangle, and text
        Texture2D messageBoxImg;
        Rectangle messageBoxRec;
        const string HELP_TEXT = "The next platform is too\nfar away to reach.\nMaybe you can find something\nto help in the water...";

        // Store HUD values
        Rectangle levelBkgRec;
        Rectangle invBkgRec;
        Rectangle keyHud;
        float keyHudAlpha = 0;
        Rectangle trampolineHud;
        float trampolineAlpha;

        // Store underground background images
        Texture2D undergroundBkg;
        Rectangle undergroundBkgRec;
        Texture2D basementBkg;
        Rectangle basementBkgRec;

        // Store sword image and rectangle
        Texture2D swordImg;
        Rectangle swordRec;

        // Store checks for sword
        bool gettingSword = false;
        bool hasSword = false;
        bool swingingSword = false;

        // Store level two collision rectangles
        Rectangle[] lvl2Plats = new Rectangle[15];

        // Store spike collision rectangle
        Rectangle spikesRec;

        // Store vine rectangle
        Rectangle vineRec;

        // Store check for current floor
        bool inBasement = false;

        // Store basement collision rectangles
        Rectangle[] basementPlats = new Rectangle[3];

        // Store enemy animations and enemy classes
        Animation[] skeletonWalkAnim = new Animation[5];
        Enemy[] skeletons = new Enemy[5];

        // Store bone rattle sound effect
        SoundEffect boneRattleSfx;

        // Store player health
        int playerHealth = 3;

        // Store check for player death display
        bool deathScreen = false;

        // Store death sound effect
        SoundEffect deathSfx;

        // Store check for pickaxe collection
        bool collectedPickaxe = false;

        // Store launcher image and rectangles
        Texture2D launcherImg;
        Rectangle basementLaunchRec;
        float basementLauncherAlpha;
        Rectangle launchRec;

        // Store pickaxe icon image and rectangle
        Texture2D pickaxeImg;
        Rectangle pickaxeIconRec;
        int pickDir = 1;

        // Store level 2 hud values
        Rectangle pickaxeHud;
        float pickaxeHudAlpha = 0;

        // Store transition timer and checks from game to endgame
        Timer endTransitionTimer;
        bool startedMining = false;
        bool inTown = false;

        // Store mining sound effect
        SoundEffect miningSfx;

        // Store endgame background image and rectangle
        Texture2D endBkg;
        Rectangle endBkgRec;

        // Store wind sound effect
        SoundEffect windSfx;

        // Store check for player control
        bool playerControl = false;

        // Store door collision rectangle
        Rectangle doorRec;
        bool onDoor = false;
        bool enteredHouse = false;

        // Store end game sign collision rectangle
        Rectangle endGameSignRec;
        bool onEndGameSign = false;
        bool displayHomeSign = false;

        // Store end game sign message box
        Rectangle homeSignBoxRec;

        // Store alpha for all drawn images in endgame
        float endGameAlpha = 1;

        // Store door sound effect
        SoundEffect doorSfx;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Set window dimensions
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            // Enable multisampling
            graphics.PreferMultiSampling = true;

            // Disable VSync
            graphics.SynchronizeWithVerticalRetrace = false;

            // Show mouse
            IsMouseVisible = true;

            // Apply graphical changes
            graphics.ApplyChanges();

            // Store window dimensions
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load fonts
            defaultFont = Content.Load<SpriteFont>("Fonts/MenuFont");
            helpFont = Content.Load<SpriteFont>("Fonts/HelpFont");
            actionFont = Content.Load<SpriteFont>("Fonts/ActionFont");
            titleFont = Content.Load<SpriteFont>("Fonts/TitleFont");

            // Load background music
            gameMusic = Content.Load<Song>("Audio/Music/GameSong");
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(gameMusic);

            // Load sound effects
            selectSfx = Content.Load<SoundEffect>("Audio/Sounds/SelectSfx");
            miningSfx = Content.Load<SoundEffect>("Audio/Sounds/MiningSfx");
            splashSfx = Content.Load<SoundEffect>("Audio/Sounds/SplashSfx");
            collectSfx = Content.Load<SoundEffect>("Audio/Sounds/CollectSfx");
            boneRattleSfx = Content.Load<SoundEffect>("Audio/Sounds/BoneRattleSfx");
            placeSfx = Content.Load<SoundEffect>("Audio/Sounds/PlaceSfx");
            boingSfx = Content.Load<SoundEffect>("Audio/Sounds/BoingSfx");
            liftSfx = Content.Load<SoundEffect>("Audio/Sounds/LiftSfx");
            lockedSfx = Content.Load<SoundEffect>("Audio/Sounds/LockedSfx");
            deathSfx = Content.Load<SoundEffect>("Audio/Sounds/DeathSfx");
            windSfx = Content.Load<SoundEffect>("Audio/Sounds/WindSfx");
            doorSfx = Content.Load<SoundEffect>("Audio/Sounds/DoorSfx");
            // Set sound effect volumes
            SoundEffect.MasterVolume = 1f;

            // Level 1 objects
            // Load menu backgrounds
            menuBkg = Content.Load<Texture2D>("Images/Backgrounds/MenuBkg");
            menuRec1 = new Rectangle(-menuBkg.Width * 4, screenHeight - menuBkg.Height * 4, menuBkg.Width * 4, menuBkg.Height * 4);
            menuRec2 = new Rectangle(0, screenHeight - menuBkg.Height * 4, menuBkg.Width * 4, menuBkg.Height * 4);
            menuRec3 = new Rectangle(menuRec2.Right, screenHeight - menuBkg.Height * 4, menuBkg.Width * 4, menuBkg.Height * 4);

            // Set button actions
            goToSettings = () => gameState = SETTINGS;
            goToGame = () => gameState = GAME;

            // Load play button
            playUnpressedImg = Content.Load<Texture2D>("Images/Sprites/PlayUnpressed");
            playHoverImg = Content.Load<Texture2D>("Images/Sprites/PlayPressed");
            playButton = new Button(playUnpressedImg, playHoverImg, new Rectangle((int)CenterX(playUnpressedImg.Width * 6, screenWidth, 0), 250, playUnpressedImg.Width * 6, playUnpressedImg.Height * 6));

            // Load exit button
            exitUnpressedImg = Content.Load<Texture2D>("Images/Sprites/exitUnpressed");
            exitHoverImg = Content.Load<Texture2D>("Images/Sprites/exitPressed");
            exitButton = new Button(exitUnpressedImg, exitHoverImg, new Rectangle((int)CenterX(exitUnpressedImg.Width * 6, screenWidth, 0), 400, exitUnpressedImg.Width * 6, exitUnpressedImg.Height * 6));
            exitButton.Clicked += Exit;

            // Load settings button
            settingsUnpressedImg = Content.Load<Texture2D>("Images/Sprites/settingsUnpressed");
            settingsHoverImg = Content.Load<Texture2D>("Images/Sprites/settingsPressed");
            settingsButton = new Button(settingsUnpressedImg, settingsHoverImg, new Rectangle(screenWidth - settingsUnpressedImg.Width * 3 - 15, 0, settingsUnpressedImg.Width * 3, settingsUnpressedImg.Height * 3));

            // Load settings background
            settingsBkg = Content.Load<Texture2D>("Images/Backgrounds/SettingsMenu");
            settingsBkgRec = new Rectangle((int)CenterX(settingsBkg.Width * 6, screenWidth, 0), 0, settingsBkg.Width * 6, settingsBkg.Height * 6);

            // Load volume slider
            musicSliderImg = Content.Load<Texture2D>("Images/Sprites/SoundSlider");
            musicSliderRec = new Rectangle((int)CenterX(musicSliderImg.Width * 6, settingsBkgRec.Width, settingsBkgRec.X), 150, musicSliderImg.Width * 6, musicSliderImg.Height * 6);
            musicSliderBox = new GameRectangle(GraphicsDevice, (int)CenterX(18, musicSliderRec.Width, musicSliderRec.X), musicSliderRec.Y, 18, 60, 6);

            // Load controls image
            controlsImg = Content.Load<Texture2D>("Images/Sprites/Controls");
            controlsRec = new Rectangle(50, screenHeight - controlsImg.Height * 2 - 50, controlsImg.Width * 2, controlsImg.Height * 2);
            controlsShadowRec = new Rectangle(controlsRec.X - 2, controlsRec.Y - 2, controlsRec.Width, controlsRec.Height);

            // Load level 1 background
            grassyBkgAnim = new Animation(Content.Load<Texture2D>("Images/Backgrounds/GrassyAnimBkg4"), 1, 2, 2, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 500, new Vector2(0, 0), 4f, true);

            // Set world boundaries
            worldBounds = grassyBkgAnim.GetDestRec();

            // Set camera
            camLvl1 = new Cam2D(GraphicsDevice.Viewport, worldBounds, 1.0f, 4.0f, 0f, playerRec);

            // Load player movement animations
            playerMoveAnims = new Dictionary<string, Animation>()
            {
                { "Run", new Animation(Content.Load<Texture2D>("Images/Sprites/CatRun"), 1, 9, 9, 0, Animation.NO_IDLE, 1, 1500, new Vector2(0, 0), 4f, 4f, true) }, 
                { "Idle", new Animation(Content.Load<Texture2D>("Images/Sprites/CatIdle"), 1, 6, 6, 0, Animation.NO_IDLE, 1, 1500, new Vector2(0, 0), 4f, 4f, false) }
            };

            // Set player attribute positions
            playerRec = new Rectangle(0, screenHeight - 64, playerMoveAnims["Run"].GetDestRec().Width, playerMoveAnims["Run"].GetDestRec().Height);
            playerPos = new Vector2(playerRec.X, playerRec.Y);
            feetRec = new Rectangle(0, 0, playerRec.Width / 2, playerRec.Height / 8);
            headRec = new Rectangle(0, 0, playerRec.Width * 5 / 8, playerRec.Height / 4);
            rightSideRec = new Rectangle(0, 0, playerRec.Width / 2, playerRec.Height * 5 / 8);
            leftSideRec = new Rectangle(0, 0, playerRec.Width / 2, playerRec.Height * 5 / 8);

            // Set overworld ground collision rectangle
            lvl1Plats[0] = new Rectangle(0, worldBounds.Bottom - 64, 1840 + 800, 16 * 4);

            // Set platform rectangles
            lvl1Plats[1] = new Rectangle(88 * 4, 167 * 4, 48 * 4, 16 * 4);
            lvl1Plats[2] = new Rectangle(151 * 4, 147 * 4, 48 * 4, 16 * 4);
            lvl1Plats[3] = new Rectangle(217 * 4, 122 * 4, 48 * 4, 16 * 4);
            lvl1Plats[4] = new Rectangle(149 * 4, 94 * 4, 48 * 4, 16 * 4);
            lvl1Plats[5] = new Rectangle(87 * 4, 71 * 4, 48 * 4, 16 * 4);
            lvl1Plats[6] = new Rectangle(1 * 4, 17 * 4, 16 * 4, 16 * 4);
            lvl1Plats[7] = new Rectangle(332 * 4, 160 * 4, 128 * 4, 32 * 4);
            lvl1Plats[8] = new Rectangle(282 * 4, 174 * 4, 50 * 4, 18 * 4);

            // Set stair rectangles
            for (int i = 0; i < stairRecs.Length; i++)
                stairRecs[i] = new Rectangle(460 * 4 + i * 44, 161 * 4 + i * 16, 11 * 4, 7 * 4);

            // Set sign rectangle
            signRec = new Rectangle(120 * 4, 56 * 4, 14 * 4, 15 * 4);

            // Load interact button display
            eImg = Content.Load<Texture2D>("Images/Sprites/E");
            eRec = new Rectangle((int)CenterX(eImg.Width * 4, screenWidth, 0), screenHeight - eImg.Height * 4 - 150, eImg.Width * 4, eImg.Height * 4);

            // Load message box display
            messageBoxImg = Content.Load<Texture2D>("Images/Sprites/MessageBox");
            messageBoxRec = new Rectangle(165 * 4, 0, messageBoxImg.Width * 3, messageBoxImg.Height * 3);

            // Set bridge collision rectangle
            bridgeRec = new Rectangle(538 * 4, 178 * 4, 74 * 4, 13 * 4);

            // Set fishing timer
            caughtTimerTarget = random.Next(5000, 10000);
            caughtFishTimer = new Timer(caughtTimerTarget, false);

            // Set trampoline placeable area
            trampolinePlaceArea = new Rectangle(lvl1Plats[5].X, lvl1Plats[5].Top - 16 * 4, 48 * 4, 16 * 4);
            trampolinePlaceRec = new GameRectangle(GraphicsDevice, trampolinePlaceArea, 5.0f);

            // Load trampoline
            trampolineImg = Content.Load<Texture2D>("Images/Sprites/Trampoline");
            trampolineRec = new Rectangle(lvl1Plats[5].Left, lvl1Plats[5].Top - trampolineImg.Height * 2, trampolineImg.Width * 2, trampolineImg.Height * 2);

            // Load key
            keyImg = Content.Load<Texture2D>("Images/Sprites/Key");
            keyRec = new Rectangle((int)CenterX(keyImg.Width * 4, lvl1Plats[6].Width, lvl1Plats[6].X), lvl1Plats[6].Top - keyImg.Height * 4, keyImg.Width * 4, keyImg.Height * 4);

            // Load lift arch
            liftArchImg = Content.Load<Texture2D>("Images/Sprites/LiftArch");
            liftArchRec = new Rectangle(lvl1Plats[7].Right + 800, lvl1Plats[0].Y - liftArchImg.Height * 4, liftArchImg.Width * 4, liftArchImg.Height * 4);

            // Load lift floor
            liftFloorImg = Content.Load<Texture2D>("Images/Sprites/LiftFloor");
            liftFloorRec = new Rectangle((int)CenterX(liftFloorImg.Width * 4, liftArchRec.Width, liftArchRec.X), lvl1Plats[0].Y - liftFloorImg.Height * 4, liftFloorImg.Width * 4, liftFloorImg.Height * 4);
            collLiftFloorRec = new Rectangle(liftFloorRec.X, liftFloorRec.Bottom - liftFloorRec.Height / 2, liftFloorRec.Width, liftFloorRec.Height / 2);

            // Store collision rectangle past lift
            lvl1Plats[9] = new Rectangle(liftArchRec.Right, liftArchRec.Bottom, worldBounds.Right - liftArchRec.Right, worldBounds.Bottom - liftArchRec.Bottom);

            // Set level 1 hud icons
            trampolineHud = new Rectangle(screenWidth - trampolineImg.Width - 110, 15, trampolineImg.Width, trampolineImg.Height);
            keyHud = new Rectangle(screenWidth - keyImg.Width * 4 - 95, 0, keyImg.Width * 3, keyImg.Height * 3);

            // Set hud backgrounds
            invBkgRec = new Rectangle(screenWidth - 350, 2, 260, 50);
            levelBkgRec = new Rectangle(invBkgRec.Left - 260 - 10, 2, 260, 50);

            // Level 2 objects
            // Load underground background
            undergroundBkg = Content.Load<Texture2D>("Images/Backgrounds/UndergroundBkg3");
            undergroundBkgRec = new Rectangle(0, 0, undergroundBkg.Width * 2, undergroundBkg.Height * 2);

            // Set underground camera
            camLvl2 = new Cam2D(GraphicsDevice.Viewport, undergroundBkgRec, 1.0f, 4.0f, 0, playerRec);

            // Load sword animations
            getSwordAnim = new Animation(Content.Load<Texture2D>("Images/Sprites/CatSword"), 1, 3, 3, 0, Animation.NO_IDLE, 1, 3000, new Vector2(0, 0), 4f, 4f, false);
            swingAnim = new Animation(Content.Load<Texture2D>("Images/Sprites/CatSwing"), 1, 7, 7, 0, Animation.NO_IDLE, 1, 500, new Vector2(0, 0), 4f, 4f, false);

            // Load launchers
            launcherImg = Content.Load<Texture2D>("Images/Sprites/Launcher");
            launchRec = new Rectangle(420 * 2, 313 * 2, launcherImg.Width * 2, launcherImg.Height * 2);
            basementLaunchRec = new Rectangle(-100, -100, launcherImg.Width * 2, launcherImg.Height * 2);

            // Set collision platforms
            lvl2Plats[0] = new Rectangle(0, 303 * 2, 134 * 2, undergroundBkgRec.Bottom - 303 * 2);
            lvl2Plats[1] = new Rectangle(134 * 2, 197 * 2, 72 * 2, undergroundBkgRec.Bottom - 197 * 2);
            lvl2Plats[2] = new Rectangle(93 * 2, 242 * 2, 40 * 2, 11 * 2);
            lvl2Plats[3] = new Rectangle(241 * 2, 233 * 2, 143 * 2, undergroundBkgRec.Bottom - 233 * 2);
            lvl2Plats[4] = new Rectangle(170 * 2, 0, 143 * 2, 126 * 2);
            lvl2Plats[5] = new Rectangle(348 * 2, 0, 143 * 2, 91 * 2);
            lvl2Plats[6] = new Rectangle(384 * 2, 339 * 2, 142 * 2, undergroundBkgRec.Bottom - 339 * 2);
            lvl2Plats[7] = new Rectangle(526 * 2, 375 * 2, 107 * 2, undergroundBkgRec.Bottom - 375 * 2);
            lvl2Plats[8] = new Rectangle(631 * 2, 339 * 2, 110 * 2, 16 * 2);
            lvl2Plats[9] = new Rectangle(706 * 2, 306 * 2, 32 * 2, 32 * 2);
            lvl2Plats[10] = new Rectangle(739 * 2, 410 * 2, 70 * 2, undergroundBkgRec.Bottom - 410 * 2);
            lvl2Plats[11] = new Rectangle(810 * 2, 375 * 2, 108 * 2, undergroundBkgRec.Bottom - 375 * 2);
            lvl2Plats[12] = launchRec;
            lvl2Plats[13] = new Rectangle(561 * 2, 0, 286 * 2, 162 * 2);
            lvl2Plats[14] = new Rectangle(28 * 2, 126 * 2, 107 * 2, 14 * 2);

            // Load sword image
            swordImg = Content.Load<Texture2D>("Images/Sprites/Sword");
            swordRec = new Rectangle((int)CenterX(swordImg.Width * 4, lvl2Plats[14].Width, lvl2Plats[14].X), lvl2Plats[14].Y - swordImg.Height * 4 - 25, swordImg.Width * 4, swordImg.Height * 4);

            // Set spike collision rectangle
            spikesRec = new Rectangle(634 * 2, 497 * 2, 104 * 2, 16 * 2);

            // Set vine collision rectangle
            vineRec = new Rectangle(206 * 2, 128 * 2, 35 * 2, undergroundBkgRec.Bottom - 128 * 2);

            // Load basement background image
            basementBkg = Content.Load<Texture2D>("Images/Backgrounds/BasementBkg");
            basementBkgRec = new Rectangle(0, 0, basementBkg.Width * 2, basementBkg.Height * 2);

            // Set basement collision rectangles
            basementPlats[0] = new Rectangle(169 * 2, 81 * 2, 107 * 2, 12 * 2);
            basementPlats[1] = new Rectangle(0, 304 * 2, basementBkgRec.Width, basementBkgRec.Height - 304 * 2);
            basementPlats[2] = basementLaunchRec;

            // Load skeleton walking animations and enemy instances
            for (int i = 0; i < skeletonWalkAnim.Length; i++)
            {
                skeletonWalkAnim[i] = new Animation(Content.Load<Texture2D>("Images/Sprites/SkeletonWalk.png"), 6, 1, 6, 0, Animation.NO_IDLE, Animation.ANIMATE_FOREVER, 500, new Vector2(0, 0), 4f, true);

                // Set positions based on index number
                if (i % 2 == 0)
                    skeletonWalkAnim[i].TranslateTo(-skeletonWalkAnim[i].GetDestRec().Width - i * 50, basementPlats[1].Y - skeletonWalkAnim[i].GetDestRec().Height);
                else
                    skeletonWalkAnim[i].TranslateTo(basementBkgRec.Right + i * 50, basementPlats[1].Y - skeletonWalkAnim[i].GetDestRec().Height);

                // Set enemy instances
                skeletons[i] = new Enemy(skeletonWalkAnim[i], new Animation(Content.Load<Texture2D>("Images/Sprites/SkeletonDead"), 2, 3, 6, 0, Animation.NO_IDLE, 1, 500, new Vector2(0, 0), 4f, false));
            }

            // Load pickaxe icon image
            pickaxeImg = Content.Load<Texture2D>("Images/Sprites/Pickaxe");
            pickaxeIconRec = new Rectangle(undergroundBkgRec.Right - pickaxeImg.Width * 2, lvl2Plats[11].Y - pickaxeImg.Height * 2, pickaxeImg.Width * 2, pickaxeImg.Height * 2);

            // Load pickaxe hud image
            pickaxeHud = new Rectangle(invBkgRec.Right - pickaxeImg.Width - 20, invBkgRec.Top - 5, pickaxeImg.Width, pickaxeImg.Height);

            // Set transition timer
            endTransitionTimer = new Timer(4000, false);

            // Load endgame background
            endBkg = Content.Load<Texture2D>("Images/Backgrounds/EndBkg");
            endBkgRec = new Rectangle(0, screenHeight - endBkg.Height * 4, endBkg.Width * 4, endBkg.Height * 4);

            // Set end game door collision rectangle
            doorRec = new Rectangle(145 * 4, 159 * 4, 24 * 4, 33 * 4);

            // Set end game sign collision rectangle
            endGameSignRec = new Rectangle(111 * 4, screenHeight - 31 * 4, 14 * 4, 15 * 4);

            // Set home sign rectangle
            homeSignBoxRec = new Rectangle(0, screenHeight - 250, 100 * 4, 20 * 4);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Retrieve current mouse and keyboard states
            mouse = Mouse.GetState();
            kb = Keyboard.GetState();

            // Update based on current game state
            switch (gameState)
            {
                case MENU:
                    PlayerBackground(gameTime);

                    // Update buttons
                    playButton.Update(mouse);
                    exitButton.Update(mouse);

                    // Perform action when play button is down
                    if (playButton.IsDown())
                    {
                        // Play select sound effect
                        selectSfx.CreateInstance().Play();

                        // Reset player position and go to game
                        playerPos.X = 0;
                        playerPos.Y = worldBounds.Bottom - 64;
                        goToGame();
                    }

                    break;
                case SETTINGS:
                    // Update exit button
                    exitButton.Update(mouse);

                    // Perform actions if player holds escape key
                    if (kb.IsKeyDown(Keys.Escape))
                    {
                        // Play select sound effect
                        selectSfx.CreateInstance().Play();

                        // Reset player position and speeds
                        playerPos = prevPlayerPos;
                        playerSpeed = prevPlayerSpeed;

                        // Go to game
                        goToGame();
                    }

                    PlayerBackground(gameTime);

                    // Perform action when left mouse button is pressed
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        // Move slider to mouse position when cursor is over the slider rectangle
                        if (musicSliderRec.Contains(mouse.Position))
                            musicSliderBox.TranslateTo(MathHelper.Clamp(mouse.X - musicSliderBox.Width / 2, musicSliderRec.X, musicSliderRec.X + musicSliderRec.Width - musicSliderBox.Width), musicSliderBox.Top);

                        // Increase volume based on slider location
                        MediaPlayer.Volume = ((float)musicSliderBox.Left - musicSliderRec.X) / ((float)musicSliderRec.Width - musicSliderBox.Width);
                    }

                    break;

                case GAME:
                    // Update settings button
                    settingsButton.Update(mouse);

                    // Perform actions if settings button is pressed
                    if (settingsButton.IsDown())
                    {
                        // Play select sound effect
                        selectSfx.CreateInstance().Play();

                        // Save current player position and speeds
                        prevPlayerPos = playerPos;
                        prevPlayerSpeed = playerSpeed;

                        // Go to settings
                        goToSettings();

                        // Set different exit button size when in settings
                        exitButton.SetSize(exitButton.GetRec().Width / 2, exitButton.GetRec().Height / 2);

                        // Set different exit button position when in settings
                        exitButton.SetPosition(screenWidth - exitButton.GetRec().Width, screenHeight - exitButton.GetRec().Height);
                    }

                    // Update based on current level
                    if (levelPlaying == 1)
                        UpdateLevelOne(gameTime);
                    else
                        UpdateLevelTwo(gameTime);

                    break;
                case ENDGAME:
                    // Update transition timer
                    endTransitionTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

                    // Update player running animation
                    playerMoveAnims["Run"].Update(gameTime);

                    // Perform action if they have not already been performed
                    if (!startedMining)
                    {
                        // Pause background music
                        MediaPlayer.Pause();

                        // Flag that mining sound has started
                        startedMining = true;

                        // Start mining timer
                        endTransitionTimer.Activate();

                        // Play mining sound effect
                        miningSfx.CreateInstance().Play();
                    }

                    // Start end game sequence when timer is finished
                    if (endTransitionTimer.IsFinished() && !inTown)
                    {
                        // Flag that player is in town
                        inTown = true;

                        // Set initial positions and speeds
                        playerPos.X = -playerRec.Width;
                        playerRec.X = (int)playerPos.X;
                        playerPos.Y = (screenHeight - 16 * 4) - playerRec.Height;
                        playerRec.Y = (int)playerPos.Y;
                        playerSpeed.X = 0;
                        playerSpeed.Y = 0;

                        // Start player running animation
                        playerMoveAnims["Run"].Activate(true);
                    }

                    // Perform actions if player has entered town
                    if (inTown && !playerControl)
                    {
                        // Auto move player until reaches certain point
                        if (playerPos.X < 80 * 4)
                        {
                            // Increment player position
                            playerPos.X += 0.5f;
                            playerRec.X = (int)playerPos.X;
                            playerMoveAnims["Run"].TranslateTo(playerRec.X, playerRec.Y);

                            // Repeat player running animation
                            if (!playerMoveAnims["Run"].IsAnimating())
                                playerMoveAnims["Run"].Activate(true);
                        }
                        else
                        {
                            // Flag that player now has control of movement
                            playerControl = true;

                            // Play wind sound effect
                            windSfx.CreateInstance().Play();
                        }
                    }

                    // Perform actions if player has control of movement
                    if (playerControl)
                    {
                        WallCollision(new Rectangle(0, 0, screenWidth, screenHeight));

                        // Update speed and position if player has not entered house
                        if (!enteredHouse)
                        {
                            UpdateSpeed();
                            UpdatePlayerPos();
                        }

                        UpdateOnDoor();
                        UpdateOnSignEndGame();

                        ManagePlayerMovementAnims(gameTime);
                    }

                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Set window background to black
            GraphicsDevice.Clear(Color.Black);

            // Display based on current game state
            switch (gameState)
            {
                case MENU:
                    // Start sprite batch without a camera
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

                    DrawMenuBkg();

                    // Display buttons
                    playButton.Draw(spriteBatch);
                    exitButton.Draw(spriteBatch);

                    // Display title
                    spriteBatch.DrawString(titleFont, "Meow Meow", new Vector2(CenterFontX(titleFont, "Meow Meow", screenWidth, 0) - 5, 50 - 5), Color.Black);
                    spriteBatch.DrawString(titleFont, "Meow Meow", new Vector2(CenterFontX(titleFont, "Meow Meow", screenWidth, 0), 50), Color.LightPink);
                    spriteBatch.DrawString(titleFont, "Misses Home", new Vector2(CenterFontX(titleFont, "Misses Home", screenWidth, 0) - 5, 140 - 5), Color.Black);
                    spriteBatch.DrawString(titleFont, "Misses Home", new Vector2(CenterFontX(titleFont, "Misses Home", screenWidth, 0), 140), Color.LightPink);

                    spriteBatch.End();

                    break;
                case SETTINGS:
                    // Start sprite batch without a camera
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

                    DrawMenuBkg();

                    // Display settings background
                    spriteBatch.Draw(settingsBkg, settingsBkgRec, Color.White);

                    // Display sound slider
                    spriteBatch.Draw(musicSliderImg, musicSliderRec, Color.White);
                    musicSliderBox.Draw(spriteBatch, Color.Red, true);
                    spriteBatch.DrawString(actionFont, "Music Volume", new Vector2(CenterFontX(actionFont, "Music Volume", settingsBkgRec.Width, settingsBkgRec.X), musicSliderRec.Top - 50), Color.White);

                    // Display key to exit settings
                    spriteBatch.DrawString(actionFont, "Return to Game [ESC]", new Vector2(5, screenHeight - 30), Color.White);

                    // Display exit button
                    exitButton.Draw(spriteBatch);

                    spriteBatch.End();

                    break;
                case GAME:
                    // Display based on which level is being played
                    if (levelPlaying == LEVEL_ONE)
                    {
                        // Start sprite batch with a camera
                        spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, camLvl1.GetTransformation());

                        // Display animated background
                        grassyBkgAnim.Draw(spriteBatch, Color.White);

                        // Display player basic controls
                        spriteBatch.Draw(controlsImg, controlsShadowRec, Color.Black);
                        spriteBatch.Draw(controlsImg, controlsRec, Color.White);

                        DrawPlayerAnims();

                        // Display interact button when in certain areas
                        if ((onSign && !signInteract && !hasTrampoline) || (onBridge && !isFishing) || (onLift && !drawNoKeyInteract))
                            spriteBatch.Draw(eImg, eRec, Color.White);

                        // Display hint when sign is interacted with
                        if (signInteract)
                        {
                            spriteBatch.Draw(messageBoxImg, messageBoxRec, Color.White);
                            spriteBatch.DrawString(helpFont, HELP_TEXT, new Vector2(CenterFontX(helpFont, HELP_TEXT, messageBoxRec.Width, messageBoxRec.X), CenterFontY(helpFont, HELP_TEXT, messageBoxRec.Height, messageBoxRec.Y)), Color.LightSkyBlue);
                        }

                        // Display lift
                        spriteBatch.Draw(liftArchImg, liftArchRec, Color.White);
                        spriteBatch.Draw(liftFloorImg, liftFloorRec, Color.White);

                        // Display fishing prompt
                        if (onBridge && !isFishing)
                        {
                            spriteBatch.DrawString(actionFont, "Start Fishing", new Vector2(CenterFontX(actionFont, "Start Fishing", eRec.Width, eRec.X) - 2, eRec.Y - 35 - 2), Color.Black);
                            spriteBatch.DrawString(actionFont, "Start Fishing", new Vector2(CenterFontX(actionFont, "Start Fishing", eRec.Width, eRec.X), eRec.Y - 35), Color.White);
                        }

                        // Display fishing progress visual
                        if (caughtFishTimer.IsActive())
                            spriteBatch.DrawString(defaultFont, timerVisual, new Vector2(CenterX(defaultFont.MeasureString(timerVisual).X, bridgeRec.Width, bridgeRec.X), bridgeRec.Top - 150), Color.White);

                        // Perform action when player has trampoline
                        if (hasTrampoline && !placedTrampoline)
                        {
                            // Display placeable area for trampoline
                            trampolinePlaceRec.Draw(spriteBatch, Color.Orange, false);
                            // Display interact button when in trampoline area
                            if (playerRec.Intersects(trampolinePlaceArea))
                                spriteBatch.Draw(eImg, eRec, Color.White);
                        }

                        // Display trampoline when player places it
                        if (placedTrampoline)
                            spriteBatch.Draw(trampolineImg, trampolineRec, Color.White);

                        // Display key
                        spriteBatch.Draw(keyImg, keyRec, Color.Yellow * keyAlpha);

                        // Display condition when player tries to interact with lift without key
                        if (drawNoKeyInteract)
                            spriteBatch.DrawString(actionFont, "KEY NEEDED", new Vector2(CenterFontX(actionFont, "KEY NEEDED", playerRec.Width, playerRec.X), playerRec.Top - 35), Color.Red);

                        spriteBatch.End();

                        // Start sprite batch without a camera
                        spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

                        // Display inventory hud background
                        spriteBatch.Draw(messageBoxImg, invBkgRec, Color.White);

                        // Display key if it has not been used on lift
                        if (!liftStarted)
                            spriteBatch.Draw(keyImg, keyHud, Color.White * keyHudAlpha);

                        // Display trampoling if it has not been placed
                        if (!placedTrampoline)
                            spriteBatch.Draw(trampolineImg, trampolineHud, Color.White * trampolineAlpha);

                        // Display inventory text
                        spriteBatch.DrawString(actionFont, "Inventory: ", new Vector2(invBkgRec.X + 20, 12), Color.White);

                        // Display level hud 
                        spriteBatch.Draw(messageBoxImg, levelBkgRec, Color.White);
                        spriteBatch.DrawString(actionFont, "Level 1", new Vector2(CenterFontX(actionFont, "Level 1", levelBkgRec.Width, levelBkgRec.X), CenterFontY(actionFont, "Level 1", levelBkgRec.Height, levelBkgRec.Y)), Color.White);

                        spriteBatch.End();
                    }
                    else
                    {
                        // Display images based on current floor
                        if (!deathScreen)
                        {
                            // Start sprite batch with a camera
                            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, camLvl2.GetTransformation());

                            if (!inBasement)
                            {
                                // Display underground background
                                spriteBatch.Draw(undergroundBkg, undergroundBkgRec, Color.White);

                                // Display launcher
                                spriteBatch.Draw(launcherImg, launchRec, Color.Gold);

                                // Display pickaxe icon
                                spriteBatch.Draw(pickaxeImg, pickaxeIconRec, Color.White);

                                // Display sword based if player has not collected it already
                                if (!hasSword)
                                    spriteBatch.Draw(swordImg, swordRec, Color.White);

                                // Display control info when player is collecting sword
                                if (getSwordAnim.IsAnimating())
                                {
                                    spriteBatch.DrawString(actionFont, "LMB to Attack", new Vector2(CenterFontX(actionFont, "LMB to Attack", playerRec.Width, playerRec.X) - 2, playerPos.Y - actionFont.MeasureString("LMB to Attack").Y - 2), Color.Black);
                                    spriteBatch.DrawString(actionFont, "LMB to Attack", new Vector2(CenterFontX(actionFont, "LMB to Attack", playerRec.Width, playerRec.X), playerPos.Y - actionFont.MeasureString("LMB to Attack").Y), Color.White);
                                }
                            }
                            else
                            {
                                // Display basement background
                                spriteBatch.Draw(basementBkg, basementBkgRec, Color.White);

                                DrawEnemy();

                                // Display launcher when player has defeated all enemies
                                if (collectedPickaxe)
                                    spriteBatch.Draw(launcherImg, basementLaunchRec, Color.White * basementLauncherAlpha);

                                // Display interact button icon when on top basement platform
                                if (playerRec.Intersects(basementPlats[0]))
                                    spriteBatch.Draw(eImg, eRec, Color.White);
                            }

                            DrawPlayerAnims();

                            spriteBatch.End();

                            // Start sprite batch without a camera
                            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

                            // Display level hud
                            spriteBatch.Draw(messageBoxImg, levelBkgRec, Color.White);
                            spriteBatch.DrawString(actionFont, "Level 2", new Vector2(CenterFontX(actionFont, "Level 2", levelBkgRec.Width, levelBkgRec.X), CenterFontY(actionFont, "Level 2", levelBkgRec.Height, levelBkgRec.Y)), Color.White);

                            // Display inventory hud
                            spriteBatch.Draw(messageBoxImg, invBkgRec, Color.White);
                            spriteBatch.DrawString(actionFont, "Inventory: ", new Vector2(invBkgRec.X + 20, 12), Color.White);
                            spriteBatch.Draw(pickaxeImg, pickaxeHud, Color.White * pickaxeHudAlpha);

                            spriteBatch.End();
                        }
                        else
                        {
                            // Start sprite batch without a camera
                            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

                            // Display death message
                            spriteBatch.DrawString(titleFont, "You Died", new Vector2(CenterFontX(titleFont, "You Died", screenWidth, 0), CenterFontY(titleFont, "You Died", screenHeight, 0)), Color.Red);

                            // Display respawn instruction
                            spriteBatch.DrawString(actionFont, "Respawn [ENTER]", new Vector2(CenterFontX(actionFont, "Respawn [ENTER]", screenWidth, 0), screenHeight - 100), Color.White);

                            spriteBatch.End();
                        }
                    }

                    // Start sprite batch without camera
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

                    // Display settings button
                    settingsButton.Draw(spriteBatch);

                    spriteBatch.End();

                    break;
                case ENDGAME:
                    // Display if player has entered town
                    if (inTown)
                    {
                        // Start sprite batch without a camera
                        spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

                        // Display end game background
                        spriteBatch.Draw(endBkg, endBkgRec, Color.White * endGameAlpha);

                        // Display player animations based on player control state
                        if (!playerControl)
                            playerMoveAnims["Run"].Draw(spriteBatch, Color.White);
                        else
                            DrawPlayerAnims();

                        // Display interact button when in certain areas
                        if (onDoor || onEndGameSign && !displayHomeSign)
                            spriteBatch.Draw(eImg, eRec, Color.LightSkyBlue * endGameAlpha);

                        // Display home sign message if player has interacted with sign
                        if (displayHomeSign)
                        {
                            spriteBatch.Draw(messageBoxImg, homeSignBoxRec, Color.White);
                            spriteBatch.DrawString(helpFont, "Meow Meow's Home", new Vector2(CenterFontX(helpFont, "Meow Meow's Home", homeSignBoxRec.Width, homeSignBoxRec.X), CenterFontY(helpFont, "Meow Meow's Homes", homeSignBoxRec.Height, homeSignBoxRec.Y)), Color.White);
                        }

                        spriteBatch.End();
                    }

                    break;
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Will draw player animation based on player's movement or action
        /// </summary>
        private void DrawPlayerAnims()
        {
            // Display images based on current player state
            if (gettingSword)
                // Display sword obtain animation
                getSwordAnim.Draw(spriteBatch, Color.White);
            else if (swingingSword)
            {
                // Display opposite orientation swinging animation based on direction player faces
                if (playerDir < 0)
                    swingAnim.Draw(spriteBatch, Color.White, SpriteEffects.FlipHorizontally);
                else
                    swingAnim.Draw(spriteBatch, Color.White);
            }
            else if (isPlayerMoving)
            {
                // Display opposite orientation running animation based on direction player faces
                if (playerDir < 0)
                    playerMoveAnims["Run"].Draw(spriteBatch, Color.White, SpriteEffects.FlipHorizontally);
                else
                    playerMoveAnims["Run"].Draw(spriteBatch, Color.White);
            }
            else
            {
                // Display opposite orientation idle animation based on direction player faces
                if (playerDir < 0)
                    playerMoveAnims["Idle"].Draw(spriteBatch, Color.White, SpriteEffects.FlipHorizontally);
                else
                    playerMoveAnims["Idle"].Draw(spriteBatch, Color.White);
            }
        }

        /// <summary>
        /// Will draw the menu background and auto running player
        /// </summary>
        private void DrawMenuBkg()
        {
            // Display scrolling background
            spriteBatch.Draw(menuBkg, menuRec1, Color.White);
            spriteBatch.Draw(menuBkg, menuRec2, Color.White);
            spriteBatch.Draw(menuBkg, menuRec3, Color.White);

            // Display auto running player in background
            playerMoveAnims["Run"].Draw(spriteBatch, Color.White);
        }

        /// <summary>
        /// Updates autorunning player
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void PlayerBackground(GameTime gameTime)
        {
            // Update speed based based on relation to ground
            if (onGround)
                playerSpeed.Y = jumpSpeed;
            else
                playerSpeed.Y += forces.Y;

            // Update player position
            playerPos.X += MAX_SPEED;
            playerPos.Y += playerSpeed.Y;
            playerRec.X = (int)playerPos.X;
            playerRec.Y = (int)playerPos.Y;
            playerMoveAnims["Run"].TranslateTo(playerRec.X, playerRec.Y);
            playerMoveAnims["Run"].Update(gameTime);
            playerMoveAnims["Run"].Activate(false);

            // Set player position backwards when player passes screen far enough
            if (playerPos.X >= screenWidth + 100)
                playerPos.X = -100;

            // Update player relation to ground
            if (playerRec.Bottom >= screenHeight)
                onGround = true;
            else
                onGround = false;
        }

        /// <summary>
        /// Will inflate key image while fading in the key hud icon
        /// </summary>
        private void KeyZoom()
        {
            // Inflate key when collected
            if (collectedKey && keyRec.Width < 300)
                keyRec.Inflate(1.05f, 1.05f);

            // Fade in key in hud when collected
            if (collectedKey && keyHudAlpha < 1)
                keyHudAlpha += 0.01f;
        }

        /// <summary>
        /// Calculates the x-coordinate for centering an image within a specified length, considering an additional offset
        /// </summary>
        /// <param name="imgWidth">Width of image to be centered</param>
        /// <param name="length">Length within which the image is to be centered</param>
        /// <param name="offsetX">Additional offset to be applied</param>
        /// <returns></returns>
        private float CenterX(float imgWidth, int length, int offsetX)
        {
            return offsetX + (length / 2 - imgWidth / 2);
        }

        /// <summary>
        /// Calculates the x-coordinate for centering text within a specified length, considering an additional offset
        /// </summary>
        /// <param name="font">Font of text to be centered</param>
        /// <param name="s">Text to be centered</param>
        /// <param name="len">Length within which the image is to be centered</param>
        /// <param name="offsetX">Additional offset to be applied</param>
        /// <returns></returns>
        private float CenterFontX(SpriteFont font, string s, int len, int offsetX)
        {
            return offsetX + (len / 2 - font.MeasureString(s).X / 2);
        }

        /// <summary>
        /// Calculates the y-coordinate for centering a text within a specified length, considering an additional offset
        /// </summary>
        /// <param name="font">Font of text to be centered</param>
        /// <param name="s">Text to be centered</param>
        /// <param name="height">Height within which the image is to be centered</param>
        /// <param name="offsetY">Additional offset to be applied</param>
        /// <returns></returns>
        private float CenterFontY(SpriteFont font, string s, int height, int offsetY)
        {
            return offsetY + (height / 2 - font.MeasureString(s).Y / 2);
        }

        /// <summary>
        /// Manages player animation translations and updates
        /// </summary>
        /// <param name="gameTime"></param>
        private void ManagePlayerMovementAnims(GameTime gameTime)
        {
            // Iterate through each animation index
            for (int i = 0; i < playerMoveAnims.Count; i++)
            {
                playerMoveAnims.Values.ToList().ElementAt(i).TranslateTo(playerRec.X, playerRec.Y);
                playerMoveAnims.Values.ToList().ElementAt(i).Update(gameTime);
            }

            // Activate appropriate animations based on player movement
            if (Math.Abs(playerSpeed.X) > 0)
            {
                playerMoveAnims["Run"].Activate(false);
                isPlayerMoving = true;
            }
            else
            {
                playerMoveAnims["Idle"].Activate(false);
                isPlayerMoving = false;
            }
        }

        /// <summary>
        /// Updates the player speed based on keyboard input for horizontal movement
        /// </summary>
        private void UpdateSpeed()
        {
            // Perform action based on key pressed
            if (kb.IsKeyDown(Keys.D))
            {
                // Set player direction to right
                playerDir = 1;

                PlayerGroundMovement();
            }
            else if (kb.IsKeyDown(Keys.A))
            {
                // Set player direction to left
                playerDir = -1;

                PlayerGroundMovement();
            }
            else
            {
                // Slowly decrease player speed
                playerSpeed.X += -Math.Sign(playerSpeed.X) * forces.X;

                // Stop player movement when speed is within tolerance
                if (Math.Abs(playerSpeed.X) <= PLAYER_TOL)
                    playerSpeed.X = 0f;
            }
        }

        /// <summary>
        /// Handles player jumping logic
        /// </summary>
        private void PlayerJumping()
        {
            // Set player vertical speed for jumping when player has pressed jump key and is on a platform
            if ((kb.IsKeyDown(Keys.Space) || kb.IsKeyDown(Keys.W)) && onGround)
                playerSpeed.Y = jumpSpeed;

            // Apply gravity to player vertical speed when player is not on a platform
            if (!onGround)
                playerSpeed.Y += forces.Y;
        }

        /// <summary>
        /// Handles player ground movement
        /// </summary>
        private void PlayerGroundMovement()
        {
            // Update player horizontal speed
            playerSpeed.X += playerDir * PLAYER_ACCEL;

            // Limit player horizontal speed to max speed
            playerSpeed.X = MathHelper.Clamp(playerSpeed.X, -MAX_SPEED, MAX_SPEED);
        }

        /// <summary>
        /// Updates player position and calculate player collision rectangles
        /// </summary>
        private void UpdatePlayerPos()
        {
            // Update player horizontal position
            playerPos.X += playerSpeed.X;
            playerRec.X = (int)playerPos.X;

            // Update player vertical position
            playerPos.Y += playerSpeed.Y;
            playerRec.Y = (int)playerPos.Y;

            // Update player feet rectangle position
            feetRec.X = (int)CenterX(playerRec.Width / 2, playerRec.Width, playerRec.X);
            feetRec.Y = playerRec.Bottom - feetRec.Height;

            // Update player head rectange position
            headRec.X = (int)CenterX(headRec.Width, playerRec.Width, playerRec.X);
            headRec.Y = playerRec.Top + playerRec.Height / 16;

            // Update player right side rectangle positon
            rightSideRec.X = playerRec.X + playerRec.Width / 2;
            rightSideRec.Y = playerRec.Bottom - feetRec.Height - rightSideRec.Height;

            // Update player left side rectangle position
            leftSideRec.X = playerRec.Left;
            leftSideRec.Y = playerRec.Bottom - feetRec.Height - leftSideRec.Height;
        }

        /// <summary>
        /// Handles collisions with vertical walls
        /// </summary>
        /// <param name="bounds">The bounding rectangle for wall</param>
        private void WallCollision(Rectangle bounds)
        {
            // Perform action based on which wall player is colliding with
            if (playerRec.Right > bounds.Right)
            {
                // Adjust player position to prevent going beyond wall
                playerRec.X = bounds.Right - playerRec.Width;
                playerPos.X = playerRec.X;

                // Stop horizontal movement
                playerSpeed.X = 0;
            }
            else if (playerRec.Left < 0)
            {
                // Adjust player position to prevent going beyond wall
                playerRec.X = 0;
                playerPos.X = playerRec.X;

                // Stop horizontal movement
                playerSpeed.X = 0;
            }
        }

        /// <summary>
        /// Handles player collisions with platforms
        /// </summary>
        /// <param name="recs">An array of rectangles of platforms</param>
        /// <returns>True if player is colliding with the floor</returns>
        private bool PlatformColl(Rectangle[] recs)
        {
            // Flag for whether a collision has occured
            bool onFloor = false;

            // Iterate through each platform index
            for (int i = 0; i < recs.Length; i++)
            {
                // Perform action if player rectangle has collided with the platform
                if (playerRec.Intersects(recs[i]))
                {
                    // Perform actions based on which collision rectangle has collided with platform
                    if (feetRec.Intersects(recs[i]))
                    {
                        // Update position and speed to prevent going into platform
                        playerPos.Y = recs[i].Y - playerRec.Height + 1;
                        playerRec.Y = (int)playerPos.Y;
                        playerSpeed.Y = 0;

                        // Flag floor collision
                        onFloor = true;
                    }
                    else if (rightSideRec.Intersects(recs[i]))
                    {
                        // Update position and speed to prevent going into platform
                        playerPos.X = recs[i].Left - playerRec.Width;
                        playerRec.X = (int)playerPos.X;
                        playerSpeed.X = 0;
                    }
                    else if (leftSideRec.Intersects(recs[i]))
                    {
                        // Update position and speed to prevent going into platform
                        playerPos.X = recs[i].Right;
                        playerRec.X = (int)playerPos.X;
                        playerSpeed.X = 0;
                    }
                    else if (headRec.Intersects(recs[i]))
                    {
                        // Update position and speed to prevent going into platform
                        playerPos.Y = recs[i].Bottom + 1;
                        playerSpeed.Y = 0;
                        playerRec.Y = (int)playerPos.Y;
                    }
                }
            }

            // Perform actions if playing level one
            if (levelPlaying == LEVEL_ONE)
            {
                // Iterate through each stair rectangle index
                for (int i = 0; i < stairRecs.Length; i++)
                {
                    // Perform actions if player is colliding with stairs
                    if (playerRec.Intersects(stairRecs[i]))
                    {
                        // Perform actions if player feet is colliding with stairs
                        if (feetRec.Intersects(stairRecs[i]))
                        {
                            // Update position and speed to prevent going into platform
                            playerPos.Y = stairRecs[i].Y - playerRec.Height + 1;
                            playerRec.Y = (int)playerPos.Y;
                            playerSpeed.Y = 0;

                            // Flag floor collision
                            onFloor = true;
                        }
                    }
                }
            }

            return onFloor;
        }

        /// <summary>
        /// Display "E" prompt
        /// </summary>
        private void DisplayEToInteract()
        {
            // Update position of "E" key prompt
            eRec.X = (int)playerPos.X;
            eRec.Y = (int)playerPos.Y - 64;
        }

        /// <summary>
        /// Update flag for sign collision
        /// </summary>
        private void UpdateSignState()
        {
            // Flag if player is colliding with sign
            if (playerRec.Intersects(signRec))
                onSign = true;
            else
                onSign = false;
        }

        /// <summary>
        /// Flags help message interaction
        /// </summary>
        private void DisplayHelp()
        {
            // Flag if player presses "E", is on the sign, and has not already collected the trampoline
            if (kb.IsKeyDown(Keys.E) && onSign && !hasTrampoline)
                signInteract = true;
            else
                signInteract = false;
        }

        /// <summary>
        /// Handles fishing mechanics for obtaining trampoline
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void FishForTrampoline(GameTime gameTime)
        {
            // Update fishing timer
            caughtFishTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            // Perform actions if player is on bridge and does not have trampoline
            if (playerRec.Intersects(bridgeRec) && !hasTrampoline)
            {
                // Flag bridge collision
                onBridge = true;

                // Perform actions player has started fishing
                if (caughtFishTimer.IsActive())
                {
                    // Update timer indicator based on time passed
                    if (caughtFishTimer.GetTimePassedInt() >= caughtTimerTarget * (2.0 / 3.0))
                        timerVisual = "...";
                    else if (caughtFishTimer.GetTimePassedInt() >= caughtTimerTarget * (1.0 / 3.0))
                        timerVisual = "..";
                    else
                        timerVisual = ".";
                }

                // Start fishing if player has pressed "E" and is not already fishing
                if (kb.IsKeyDown(Keys.E) && !isFishing)
                {
                    // Flag that player has started fishing
                    isFishing = true;

                    // Start fishing timer
                    caughtFishTimer.ResetTimer(true);

                    // Play splash sound effect
                    splashSfx.CreateInstance().Play();
                }

                // Perform actions if player has caught trampoline
                else if (caughtFishTimer.IsFinished())
                {
                    // Update flags
                    isFishing = false;
                    onBridge = false;
                    hasTrampoline = true;

                    // Play collect sound effect
                    collectSfx.CreateInstance().Play();
                }
            }
            else
            {
                // Reset fishing values if player is not on the bridge
                onBridge = false;
                isFishing = false;

                // Reset timer if it is running
                if (caughtFishTimer.IsActive())
                    caughtFishTimer.ResetTimer(false);
            }
        }

        /// <summary>
        /// Handles key collection flag
        /// </summary>
        private void KeyCollect()
        {
            // Perform actions when player collides with key
            if (playerRec.Intersects(keyRec) && !collectedKey)
            {
                // Play collect sound effect
                collectSfx.CreateInstance().Play();

                // Flag key collection
                collectedKey = true;
            }
        }

        /// <summary>
        /// Handles trampoline mechanics
        /// </summary>
        private void ManageTrampoline()
        {
            // Perform actions if player has collected trampoline
            if (hasTrampoline)
            {
                // Fade in trampoline hud icon
                if (trampolineAlpha < 1)
                    trampolineAlpha += 0.01f;

                // Perform actions if player is inside placeable trampoline area
                if (playerRec.Intersects(trampolinePlaceArea))
                {
                    // Perform actions if player presses "E"
                    if (kb.IsKeyDown(Keys.E))
                    {
                        // Play place sound effect
                        placeSfx.CreateInstance().Play();

                        // Set flag indicating trampoline has been placed
                        placedTrampoline = true;
                    }
                }
            }

            // Perform actions if player lands on placed trampoline
            if (feetRec.Intersects(trampolineRec) && placedTrampoline)
            {
                // Adjust player position and speed for trampoline bounce
                playerPos.Y -= 1;
                playerRec.Y = (int)playerPos.Y;
                playerSpeed.Y = -8.5f;

                // Play boing sound effect
                boingSfx.CreateInstance().Play();
            }
        }

        /// <summary>
        /// Update key transparency
        /// </summary>
        private void DrawKeyAlpha()
        {
            // Gradually increase key image transparency when collected
            if (collectedKey)
            {
                if (keyAlpha >= 0)
                    keyAlpha -= 0.01f;
            }
        }

        /// <summary>
        /// Handles collision with lift floor
        /// </summary>
        private void LiftCollision()
        {
            // Perform actions if player collides with lift floor
            if (playerRec.Intersects(collLiftFloorRec))
            {
                // Perform actions if player's feet collide with lift floor
                if (feetRec.Intersects(collLiftFloorRec))
                {
                    // Adjust player vertical position
                    playerPos.Y = collLiftFloorRec.Y - playerRec.Height + 1;
                    playerRec.Y = (int)playerPos.Y;

                    // Disallow jumping
                    playerSpeed.Y = 0;

                    // Flag that player is touching ground
                    onGround = true;
                }
            }
        }

        /// <summary>
        /// Manage interactions with lift
        /// </summary>
        private void ManageLift()
        {
            // Perform actions if player is on lift
            if (playerRec.Intersects(collLiftFloorRec))
            {
                // Flag on lift
                onLift = true;

                // Perform actions if player presses "E"
                if (kb.IsKeyDown(Keys.E))
                {
                    // Perform actions if player has collected key
                    if (collectedKey)
                    {
                        // Adjust player position
                        playerPos.X = CenterX(playerRec.Width, liftFloorRec.Width, liftFloorRec.X);
                        playerPos.Y = collLiftFloorRec.Top - playerRec.Height;

                        // Flag that lift has started
                        liftStarted = true;
                        liftSfx.CreateInstance().Play();
                    }
                    else if (!collectedKey)
                    {
                        // Flag that player cannot start lift without key
                        drawNoKeyInteract = true;

                        // Play locked sound effect
                        lockedSfx.CreateInstance().Play();
                    }
                }
            }
            else
            {
                // Flag that player is not on lift
                onLift = false;
                drawNoKeyInteract = false;
            }

            // Perform actions if lift has started
            if (liftStarted)
            {
                // Flag that player has left
                onLift = false;

                // Lower lift floor
                liftFloorRec.Y += 1;
                collLiftFloorRec.Y = liftFloorRec.Bottom - collLiftFloorRec.Height;

                // Disallow player horizontal movement
                playerSpeed.X = 0;
            }
        }

        /// <summary>
        /// Manage swinging sword animation
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void SwingSword(GameTime gameTime)
        {
            // Perform actions when player has sword
            if (hasSword)
            {
                // Set animation position based on direction player is looking
                if (playerDir < 0)
                    swingAnim.TranslateTo(playerRec.Right - swingAnim.GetDestRec().Width, playerRec.Y - 64);
                else
                    swingAnim.TranslateTo(playerRec.X, playerRec.Y - 64);

                // Update sword swing animation
                swingAnim.Update(gameTime);

                // Flag not swinging sword if animation is not running
                if (!swingAnim.IsAnimating())
                    swingingSword = false;

                // Start swing animation when player pressed left mouse button
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    swingAnim.Activate(false);
                    swingingSword = true;
                }
            }
        }

        /// <summary>
        /// Handles player obtaining sword
        /// </summary>
        private void GetSword()
        {
            // Perform actions if player has collided with sword and has not already collected it
            if (playerRec.Intersects(swordRec) && !hasSword)
            {
                // Adjust sword obtaining animation position
                getSwordAnim.TranslateTo(playerRec.X - 64, playerRec.Y - 64);

                // Start sword obtaining animation
                getSwordAnim.Activate(false);

                // Flag getting sword and has sword
                gettingSword = true;
                hasSword = true;

                // Play collect SFX
                collectSfx.CreateInstance().Play();
            }
        }

        /// <summary>
        /// Manage player interactions with launchers
        /// </summary>
        private void ManageLaunchers()
        {
            // Set launch speeds based on location and if player has collided with a launcher
            if (feetRec.Intersects(launchRec) && !inBasement)
            {
                // Play boing sound effect
                boingSfx.CreateInstance().Play();
                playerSpeed.Y = -10;
            }
            else if (feetRec.Intersects(basementLaunchRec) && inBasement && collectedPickaxe)
            {
                // Play boing sound effect
                boingSfx.CreateInstance().Play();
                playerSpeed.Y = -13;
            }
        }

        /// <summary>
        /// Display enemies
        /// </summary>
        private void DrawEnemy()
        {
            // Iterate through each skeleton index
            for (int i = 0; i < skeletons.Length; i++)
            {
                // Change animation orientation based on directions skeleton is facing
                if (skeletons[i].dir > 0)
                {
                    // Display animation based on walking animation state
                    if (skeletons[i].walkAnim.IsAnimating())
                        skeletons[i].walkAnim.Draw(spriteBatch, Color.White);
                    else
                        skeletons[i].deadAnim.Draw(spriteBatch, Color.White);
                }
                else
                {
                    // Display animation based on walking animation state
                    if (skeletons[i].walkAnim.IsAnimating())
                        skeletons[i].walkAnim.Draw(spriteBatch, Color.White, SpriteEffects.FlipHorizontally);
                    else
                        skeletons[i].deadAnim.Draw(spriteBatch, Color.White, SpriteEffects.FlipHorizontally);
                }
            }
        }

        /// <summary>
        /// Manages player death conditions, updates death screen, and player respawns
        /// </summary>
        private void PlayerDeath()
        {
            // Perform actions if player meets death conditons
            if ((playerHealth <= 2 || playerRec.Intersects(spikesRec)) && !deathScreen)
            {
                // Play death sound effect
                deathSfx.CreateInstance().Play();

                // Flag death screen
                deathScreen = true;
            }

            // Perform actions if player presses "Enter"
            if (kb.IsKeyDown(Keys.Enter) && deathScreen)
            {
                // Play select sound effect
                selectSfx.CreateInstance().Play();

                // Reset game values
                deathScreen = false;
                playerHealth = 3;
                inBasement = false;

                // Interate through each skeleton
                foreach (Enemy e in skeletons)
                {
                    // Reset skeletons
                    e.position = e.startPos;
                    e.walkAnim.Activate(true);
                }

                // Reset player speeds and position
                playerSpeed.X = 0;
                playerSpeed.Y = 0;
                playerPos.X = 0;
                playerPos.Y = lvl2Plats[0].Y - playerRec.Height;
            }
        }

        /// <summary>
        /// Manages skeleton behaviour
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void ManageEnemy(GameTime gameTime)
        {
            // Store dead skeleton count
            int count = 0;

            // Iterate through each skeleton index
            for (int i = 0; i < skeletons.Length; i++)
            {
                // Update skeleton
                skeletons[i].Update(gameTime, playerPos);

                // Perform actions based on skeleton state
                if (skeletons[i].walkAnim.IsAnimating())
                {
                    // Set skeleton to dead if it is attacked by player
                    if (swingingSword && swingAnim.GetDestRec().Intersects(skeletons[i].walkAnim.GetDestRec()) && playerDir == Math.Sign(skeletons[i].walkAnim.GetDestRec().X - playerPos.X))
                    {
                        skeletons[i].walkAnim.Deactivate();
                        skeletons[i].SkeletonDead(boneRattleSfx);
                    }

                    // Set player to dead if it is attacked by skeleton
                    if (skeletons[i].walkAnim.GetDestRec().Intersects(playerRec))
                    {
                        skeletons[i].walkAnim.Deactivate();
                        skeletons[i].SkeletonDead(boneRattleSfx);
                        playerHealth--;
                    }
                }
                else
                {
                    // Increment dead skeleton count
                    count++;
                }

                // Perform actions if all skeletons are dead
                if (count == skeletons.Length && !collectedPickaxe)
                {
                    // Flag that player has pickaxe
                    collectedPickaxe = true;

                    // Set basement launcher position
                    basementLaunchRec.X = 302 * 2;
                    basementLaunchRec.Y = 278 * 2;

                    // Play collect SFX
                    collectSfx.CreateInstance().Play();
                }
            }
        }

        /// <summary>
        /// Updates basement launcher transparency
        /// </summary>
        private void UpdateLauncherAlpha()
        {
            // Increase alpha of basement launcher if pickaxe has been collected
            if (collectedPickaxe && basementLauncherAlpha < 1)
                basementLauncherAlpha += 0.05f;
        }

        /// <summary>
        /// Modify player speeds when in contact with vine
        /// </summary>
        private void OnVine()
        {
            // Decrease player speeds when player collides vine
            if (playerRec.Intersects(vineRec))
            {
                playerSpeed.X *= 0.92f;
                playerSpeed.Y *= 0.92f;
            }
        }

        /// <summary>
        /// Handles vine climbing mechanics
        /// </summary>
        private void ClimbVine()
        {
            // Update interact button position
            eRec.X = (int)playerPos.X;
            eRec.Y = (int)playerPos.Y - 64;

            // Perform actions when player is in basement, on lift, and "E" is pressed
            if (kb.IsKeyDown(Keys.E) && inBasement && playerRec.Intersects(basementPlats[0]))
            {
                // Update current floor
                inBasement = false;

                // Set top floor position
                playerPos.X = 244 * 2;
                playerPos.Y = 233 * 2 - playerRec.Height;
            }
        }

        /// <summary>
        /// Handles player descent on vine
        /// </summary>
        private void DownVine()
        {
            // Perform actions when player is not in basement and is at bottom of world
            if (playerRec.Y >= undergroundBkgRec.Bottom && !inBasement)
            {
                // Update current floor
                inBasement = true;

                // Set basement position
                playerPos.X = CenterX(playerRec.Width, basementPlats[0].Width, basementPlats[0].X);
                playerPos.Y = basementPlats[0].Y - playerRec.Height;
            }
        }

        /// <summary>
        /// Manages pickaxe icon movement
        /// </summary>
        private void FloatPickaxeIcon()
        {
            // Invert pickaxe movement direction if it has reached its vertical boundaries
            if (pickaxeIconRec.Bottom <= 350 * 2 || pickaxeIconRec.Bottom >= lvl2Plats[11].Top)
                pickDir = -pickDir;

            // Update pickaxe icon position
            pickaxeIconRec.Y += pickDir;
        }

        /// <summary>
        /// Update pickaxe hud transparency
        /// </summary>
        private void PickaxeHudAlpha()
        {
            // Increase alpha of pickaxe hud icon if player has collected pickaxe
            if (collectedPickaxe && pickaxeHudAlpha < 1)
                pickaxeHudAlpha += 0.01f;
        }

        /// <summary>
        /// Handle transition from game to endgame
        /// </summary>
        private void MineExit()
        {
            // Go to endgame state if player has used pickaxe to exit
            if (playerRec.Intersects(pickaxeIconRec) && collectedPickaxe)
                gameState = ENDGAME;
        }

        /// <summary>
        /// Manages gameplay logic for level one
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void UpdateLevelOne(GameTime gameTime)
        {
            // Update background animation
            grassyBkgAnim.Update(gameTime);

            // Update camera position
            camLvl1.LookAt(playerRec);

            // Update on ground flag
            onGround = PlatformColl(lvl1Plats);

            PlayerJumping();
            LiftCollision();
            WallCollision(worldBounds);

            // Update speed if lift has not started
            if (!liftStarted)
                UpdateSpeed();

            // Perform actions if player has passed bottom of world
            if (playerRec.Y >= worldBounds.Bottom)
            {
                // Set level two variables
                playerPos.X = 0;
                playerPos.Y = lvl2Plats[0].Y - playerRec.Height;
                playerSpeed.X = 0;
                playerSpeed.Y = 0;
                jumpSpeed = -7.0f;

                // Set level to level two
                levelPlaying = LEVEL_TWO;
            }

            UpdatePlayerPos();
            ManageTrampoline();
            KeyCollect();
            UpdateSignState();
            DisplayEToInteract();
            DisplayHelp();
            DrawKeyAlpha();
            ManageLift();
            FishForTrampoline(gameTime);
            KeyZoom();
            ManagePlayerMovementAnims(gameTime);
        }

        /// <summary>
        /// Manages gameplay logic for level two
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void UpdateLevelTwo(GameTime gameTime)
        {
            // Update camera position
            camLvl2.LookAt(playerRec);

            // Perform actions if death screen is not active
            if (!deathScreen)
            {
                // Perform action based on sword collection status
                if (!gettingSword)
                {
                    SwingSword(gameTime);
                    GetSword();
                }
                else
                {
                    // Update get sword animation
                    getSwordAnim.Update(gameTime);

                    // Flag that player is getting sword if get sword animation is playing
                    if (!getSwordAnim.IsAnimating())
                        gettingSword = false;
                }

                // Perform actions based on current floor
                if (!inBasement)
                {
                    // Update on ground flag
                    onGround = PlatformColl(lvl2Plats);

                    OnVine();
                    FloatPickaxeIcon();
                }
                else if (inBasement)
                {
                    // Update on ground flag
                    onGround = PlatformColl(basementPlats);

                    ManageEnemy(gameTime);
                    UpdateLauncherAlpha();
                }

                WallCollision(undergroundBkgRec);
                PlayerJumping();

                // Disallow speed updates when swinging or getting sword
                if (!swingingSword || !gettingSword)
                    UpdateSpeed();

                ClimbVine();
                ManageLaunchers();

                // Disallow player movement when getting sword
                if (!gettingSword)
                    UpdatePlayerPos();

                DownVine();
                PickaxeHudAlpha();
                MineExit();

                // Update player movement animations
                ManagePlayerMovementAnims(gameTime);
            }

            PlayerDeath();
        }

        /// <summary>
        /// Checks for player collisions with sign
        /// Handles sign interactions
        /// </summary>
        private void UpdateOnSignEndGame()
        {
            // Perform actions based on intersection status with sign
            if (playerRec.Intersects(endGameSignRec))
            {
                // Flag that player is on sign
                onEndGameSign = true;

                DisplayEToInteract();
            }
            else
                // Flag that player is not on sign
                onEndGameSign = false;

            // Flag to display sign message if player interacts with sign
            if (kb.IsKeyDown(Keys.E) && onEndGameSign)
                displayHomeSign = true;
            else
                displayHomeSign = false;

            // Set new position for message box if it is being displayed
            if (displayHomeSign)
                homeSignBoxRec.X = (int)CenterX(homeSignBoxRec.Width, playerRec.Width, playerRec.X);
        }

        /// <summary>
        /// Checks for player collision with door
        /// Handles entering house
        /// </summary>
        private void UpdateOnDoor()
        {
            // Check if player is colliding with door
            if (playerRec.Intersects(doorRec))
            {
                // Flag that player is on door
                onDoor = true;

                DisplayEToInteract();
            }
            else
                // Flag that player is not on door
                onDoor = false;

            // Perform actions if interacts with door
            if (kb.IsKeyDown(Keys.E) && onDoor && !enteredHouse)
            {
                // Play door sound effect
                doorSfx.CreateInstance().Play();

                // Flag that player has entered house
                enteredHouse = true;
            }

            // Perform actions if player has entered house
            if (enteredHouse)
            {
                // Disallow movement
                playerSpeed.X = 0;
                playerSpeed.Y = 0;

                // Gradually decrease end game scene alpha value and exit game when reaches 0
                if (endGameAlpha > 0)
                    endGameAlpha -= 0.005f;
                else if (endGameAlpha <= 0)
                    Exit();
            }
        }
    }

    /// <summary>
    /// Represents an enemy in the game
    /// </summary>
    class Enemy
    {
        /// <summary>
        /// Get or set starting position of the enemy
        /// </summary>
        public Vector2 startPos { get; set; }

        /// <summary>
        /// Get or set current position of the enemy
        /// </summary>
        public Vector2 position { get; set; }

        /// <summary>
        /// Get or set current horizontal speed of the enemy
        /// </summary>
        public float speedX { get; set; }

        /// <summary>
        /// Get or set direction of the enemy
        /// </summary>
        public int dir { get; set; }

        /// <summary>
        /// Get or set walking animation of the enemy
        /// </summary>
        public Animation walkAnim { get; set; }

        /// <summary>
        /// Get or set death animation of the enemy
        /// </summary>
        public Animation deadAnim { get; set; }

        /// <summary>
        /// Initializes a new instance of an enemy
        /// </summary>
        /// <param name="walkAnim">The walking animation for enemy</param>
        /// <param name="deadAnim">The death animation for enemy</param>
        public Enemy(Animation walkAnim, Animation deadAnim)
        {
            // Set initial values
            position = walkAnim.GetDestRec().Location.ToVector2();
            startPos = position;
            speedX = 2.5f;
            this.walkAnim = walkAnim;
            this.deadAnim = deadAnim;
        }

        /// <summary>
        /// Updates the enemy state
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        /// <param name="playerPos">The current position of player</param>
        public void Update(GameTime gameTime, Vector2 playerPos)
        {
            // Update animations
            walkAnim.Update(gameTime);
            deadAnim.Update(gameTime);

            // Set direction towards player position
            dir = Math.Sign(playerPos.X - position.X);

            // Update position if enemy has a direction to move in
            if (dir != 0)
            {
                position = new Vector2(position.X + dir * speedX, position.Y);
                walkAnim.TranslateTo(position.X, position.Y);
            }
        }

        /// <summary>
        /// Activates enemy death animation
        /// </summary>
        public void SkeletonDead(SoundEffect deathSfx)
        {
            // Set death animation position based on direction enemy is facing
            if (dir < 0)
                deadAnim.TranslateTo(position.X - 32 * 4, position.Y);
            else
                deadAnim.TranslateTo(position.X, position.Y);

            // Play death sound effect if skeleton death animation is playing
            if (!deadAnim.IsAnimating())
                deathSfx.CreateInstance().Play();

            // Activate death animation
            deadAnim.Activate(true);
        }
    }
}
