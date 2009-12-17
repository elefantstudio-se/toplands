using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using WinForms = System.Windows.Forms;
using xWinFormsLib;

namespace fpsrpg
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        SpriteBatch spriteBatch;
        SpriteFont UVfont;

        Bullet2 bullet;

        Player mPlayerSprite;
        Sprite Enemy;

        KeyboardState mPreviousKeyboardState;
        MouseState mPreviousMouseState;

        KeyboardState aCurrentKeyboardState = Keyboard.GetState();
        MouseState aCurrentMouseState = Mouse.GetState();

        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;

        Texture2D crosshair;
        Texture2D ak47;
        Texture2D muzzleflash;

        FormCollection formCollection;

        Color[] EnemyTextureData;
        Matrix EnemyTransform;
        Rectangle EnemyRectangle;

        public int ScreenWidth, ScreenHeight;

        float fireingDelay=0.0f;

        bool shot;
        bool InMenu;
        bool Enemy_hit;

        #region Network Variables

        const int maxGamers = 16;
        const int maxLocalGamers = 4;

        NetworkSession networkSession;

        PacketWriter packetWriter = new PacketWriter();
        PacketReader packetReader = new PacketReader();

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Components.Add(new GamerServicesComponent(this));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //this.IsMouseVisible = true;
            mPlayerSprite = new Player();
            Enemy = new Sprite();

            ScreenHeight = graphics.GraphicsDevice.Viewport.Height;
            ScreenWidth = graphics.GraphicsDevice.Viewport.Width;

            Enemy.Position = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
            Enemy.Rotation = 135.0f;

            shot = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //mPlayerSprite.mContentManager = this.Content;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            UVfont = Content.Load<SpriteFont>("TehText");

            // TODO: use this.Content to load your game content here
            mPlayerSprite.LoadContent(this.Content);
            Enemy.LoadContent(this.Content,"test_enemy");

            bullet = new Bullet2(Content.Load<Texture2D>("crappybulleteffect"));

            bullet.BulletTextureData = new Color[bullet.Texture.Width * bullet.Texture.Height];
            bullet.Texture.GetData(bullet.BulletTextureData);

            EnemyTextureData = new Color[Enemy.Size.Width * Enemy.Size.Height];
            Enemy.mSpriteTexture.GetData(EnemyTextureData);

            audioEngine = new AudioEngine("Content/sound/guns.xgs");
            waveBank = new WaveBank(audioEngine, "Content/sound/MyWaveBank.xwb");
            soundBank = new SoundBank(audioEngine, "Content/sound/MySoundBank.xsb");

            crosshair = Content.Load<Texture2D>("crosshair_mediumaccuracy");
            ak47 = Content.Load<Texture2D>("ak47");
            muzzleflash = Content.Load<Texture2D>("crappymuzzle");

            // Create the formCollection, where all our forms will reside
            formCollection = new FormCollection(this.Window, Services, ref graphics);

            #region Form #1

            //Create a new form
            formCollection.Add(new Form("form1", "Menu", new Vector2(600, 400),new Vector2(ScreenWidth/2 - 300,ScreenHeight/2 - 200),Form.BorderStyle.Fixed));
            //formCollection["form1"].Style = Form.BorderStyle.Sizable;
            //formCollection["form1"].FontName = "pericles9";
            //formCollection["form1"].FontName = "kootenay9";

            //Add a Button
            formCollection["form1"].Controls.Add(new Button("button1", new Vector2(15, 50), 130, "Button1", Color.White, Color.Black));
            formCollection["form1"]["button1"].OnPress += Button1_OnPress;
            formCollection["form1"]["button1"].OnRelease = Button1_OnRelease;

            formCollection["form1"].Controls.Add(new Button("btRemove", new Vector2(15, 320), 60, "Remove Listbox Item", Color.White, Color.Black));
            formCollection["form1"]["btRemove"].OnPress += Remove_OnPress;

            //Add a Button Row
            formCollection["form1"].Controls.Add(new ButtonRow("buttonRow1", new Vector2(15, 80), 250, new string[] { "RowButton1", "RowButton2", "RowButton3" }, Color.White, Color.Black));
            formCollection["form1"]["buttonRow1"].OnRelease = ButtonRow1_OnRelease;

            //Add a Checkbox
            formCollection["form1"].Controls.Add(new Checkbox("checkbox1", new Vector2(15, 110), "Checkbox", true));
            formCollection["form1"]["checkbox1"].OnRelease = Checkbox1_OnRelease;

            //Add a RadioButton
            formCollection["form1"].Controls.Add(new RadioButton("radiobutton1", new Vector2(170, 55), "RadioButton", true));
            formCollection["form1"]["radiobutton1"].OnRelease = Radiobutton1_OnRelease;

            //Add a Label
            formCollection["form1"].Controls.Add(new Label("label1", new Vector2(15, 135), "Label #1", Color.White, Color.Black, 70, Label.Align.Left));
            formCollection["form1"]["label1"].OnMouseOver = Label1_MouseOver;
            formCollection["form1"]["label1"].OnMouseOut = Label1_MouseOut;
            formCollection["form1"]["label1"].OnPress = Label1_OnPress;
            formCollection["form1"]["label1"].OnRelease = Label1_OnRelease;

            //Add a PictureBox
            formCollection["form1"].Controls.Add(new PictureBox("picturebox1", new Vector2(15, 160), @"content\textures\xna_logo.png", 1));
            formCollection["form1"]["picturebox1"].OnMouseOver = PictureBox1_MouseOver;
            formCollection["form1"]["picturebox1"].OnMouseOut = PictureBox1_MouseOut;
            formCollection["form1"]["picturebox1"].OnPress = PictureBox1_OnPress;
            formCollection["form1"]["picturebox1"].OnRelease = PictureBox1_OnRelease;

            //Add a CheckboxGroup
            formCollection["form1"].Controls.Add(new CheckboxGroup("checkboxgroup1", new Checkbox[] { 
                new Checkbox("group1check1", new Vector2(165, 130), "Group Check #1", true),
                new Checkbox("group1check2", new Vector2(165, 150), "Group Check #2", false),
                new Checkbox("group1check3", new Vector2(165, 170), "Group Check #3", false),
                new Checkbox("group1check4", new Vector2(165, 190), "Group Check #4", false),
                new Checkbox("group1check5", new Vector2(165, 210), "Group Check #5", false)}));
            ((CheckboxGroup)formCollection["form1"]["checkboxgroup1"]).OnChangeSelection = CheckboxGroup1_ChangeSelection;

            //Add a RadioButtonGroup
            formCollection["form1"].Controls.Add(new RadioButtonGroup("radiobuttongroup1", new RadioButton[] { 
                new RadioButton("group1check1", new Vector2(310, 280), "RadioButton #1", true),
                new RadioButton("group1check2", new Vector2(310, 300), "RadioButton #2", false),
                new RadioButton("group1check3", new Vector2(310, 320), "RadioButton #3", false),
                new RadioButton("group1check4", new Vector2(310, 340), "RadioButton #4", false),
                new RadioButton("group1check5", new Vector2(310, 360), "RadioButton #5", false)}));
            ((RadioButtonGroup)formCollection["form1"]["radiobuttongroup1"]).OnChangeSelection = RadioButtonGroup1_ChangeSelection;

            //Add a ButtonGroup
            formCollection["form1"].Controls.Add(new ButtonGroup("buttongroup1", new Button[] { 
                new Button("group2button1", new Vector2(165, 250), "Group Button #1", Color.White, Color.Black),
                new Button("group2button2", new Vector2(165, 275), "Group Button #2", Color.White, Color.Black),
                new Button("group2button3", new Vector2(165, 300), "Group Button #3", Color.White, Color.Black),
                new Button("group2button4", new Vector2(165, 325), "Group Button #4", Color.White, Color.Black),
                new Button("group2button5", new Vector2(165, 350), "Group Button #5", Color.White, Color.Black)}));
            ((ButtonGroup)formCollection["form1"]["buttongroup1"]).OnChangeSelection = ButtonGroup1_ChangeSelection;

            //Add a multiline Textbox
            formCollection["form1"].Controls.Add(new Textbox("textbox1", new Vector2(310, 50), 130, 80, "This is a test"));
            ((Textbox)formCollection["form1"]["textbox1"]).Scrollbar = Textbox.Scrollbars.Both;

            //Add a Listbox
            formCollection["form1"].Controls.Add(new Listbox("listbox1", new Vector2(310, 150), 130, 120,
                new string[] { "This is item #1 from the listbox", "Item2", "Item3", "Item4", "Item5", "Item6", "Item7", "Item8", "Item9", "Item10" }));
            ((Listbox)formCollection["form1"]["listbox1"]).HorizontalScrollbar = true;

            //Add a menu to the form
            //Note: inverted process, first create a submenu then add it when creating the menuItem
            #region Form1 Menu

            SubMenu mnuFile = new SubMenu(formCollection["form1"]);
            mnuFile.Add(new MenuItem("mnuFileClose", "&Close", Form1_mnuFileClose), null);
            mnuFile.Add(new MenuItem("", "-", Form1_mnuFileClose), null);
            mnuFile.Add(new MenuItem("mnuFileExit", "E&xit", Form1_mnuFileExit), null);

            SubMenu mnuView = new SubMenu(formCollection["form1"]);
            mnuView.Add(new MenuItem("mnuViewToggleFS", "&Toggle Fullscreen", Form1_mnuViewToggleFS), null);

            SubMenu mnuTestSubMenu0 = new SubMenu(formCollection["form1"]);
            mnuTestSubMenu0.Add(new MenuItem("mnuTestSubItem0", "SubMenuItem0", null), null);
            mnuTestSubMenu0.Add(new MenuItem("mnuTestSubItem1", "SubMenuItem1", null), null);

            SubMenu mnuTestSubMenu1 = new SubMenu(formCollection["form1"]);
            mnuTestSubMenu1.Add(new MenuItem("mnuTestSubItem0", "SubMenuItem0", null), null);
            mnuTestSubMenu1.Add(new MenuItem("mnuTestSubItem1", "SubMenuItem1", null), null);

            SubMenu mnuTest = new SubMenu(formCollection["form1"]);
            mnuTest.Add(new MenuItem("mnuTestItem0", "MenuItem0", null), mnuTestSubMenu0);
            mnuTest.Add(new MenuItem("mnuTestItem0", "MenuItem1", null), mnuTestSubMenu1);
            mnuTest.Add(new MenuItem("mnuTestItem0", "MenuItem2", null), null);

            formCollection["form1"].Menu = new Menu("form1Menu");
            formCollection["form1"].Menu.Add(new MenuItem("mnuFile", "&File", null), mnuFile);
            formCollection["form1"].Menu.Add(new MenuItem("mnuView", "&View", null), mnuView);
            formCollection["form1"].Menu.Add(new MenuItem("mnuView", "&Test", null), mnuTest);

            #endregion

            //Add a ProgressBar
            formCollection["form1"].Controls.Add(new Progressbar("progressbar1", new Vector2(15, 295), 125, 15, true));

            //Add a Potentiometer
            formCollection["form1"].Controls.Add(new Potentiometer("potentiometer1", new Vector2(120, 135)));
            ((Potentiometer)formCollection["form1"]["potentiometer1"]).OnChangeValue = Potentiometer_OnChangeValue;

            formCollection["form1"].Controls.Add(new ComboBox("combo1", new Vector2(450, 50), 120, new string[] { "Item1", "Item2", "Item3", "Item4", "Item5", "Item6", "Item7", "Item8" }));
            formCollection["form1"]["combo1"].FontName = "pescadero9b";

            formCollection["form1"].Controls.Add(new Button("btAdd", new Vector2(440, 100), "Add to Listbox", Color.White, Color.Black));
            formCollection["form1"]["btAdd"].OnPress = Button1_OnPress;

            //Show the form
            formCollection["form1"].Show();

            #endregion

            //white corner bug for some reason if not focused.. NEED TO FIX
            formCollection["form1"].Focus();
            formCollection["form1"].Minimize();
            formCollection["form1"].CloseButton.IsGrayedOut = true;
        }

        #region Network Functions
        /// <summary>
        /// Menu screen provides options to create or join network sessions.
        /// </summary>
        void UpdateMenuScreen()
        {
            if (IsActive)
            {
                if (Gamer.SignedInGamers.Count == 0)
                {
                    // If there are no profiles signed in, we cannot proceed.
                    // Show the Guide so the user can sign in.
                    Guide.ShowSignIn(maxLocalGamers, true);
                }
                else if (aCurrentKeyboardState.IsKeyDown(Keys.F1))
                {
                    // Create a new session?
                    CreateSession();
                }
                else if (aCurrentKeyboardState.IsKeyDown(Keys.F2))
                {
                    // Join an existing session?
                    JoinSession();
                }
            }
        }

        /// <summary>
        /// Starts hosting a new network session.
        /// </summary>
        void CreateSession()
        {
            try
            {
                networkSession = NetworkSession.Create(NetworkSessionType.PlayerMatch, maxLocalGamers, maxGamers);

                new MessageBox(new Vector2(250, 100), "Session created!", "Session created!", MessageBox.Type.MB_OK).Show();
                HookSessionEvents();
            }
            catch (Exception e)
            {
                new MessageBox(new Vector2(250, 100), "Error", e.Message, MessageBox.Type.MB_OK).Show();
            }
        }


        /// <summary>
        /// Joins an existing network session.
        /// </summary>
        void JoinSession()
        {
            try
            {
                // Search for sessions.
                using (AvailableNetworkSessionCollection availableSessions = NetworkSession.Find(NetworkSessionType.PlayerMatch, maxLocalGamers, null))
                {
                    if (availableSessions.Count == 0)
                    {
                        MessageBox NoSession = new MessageBox(new Vector2(250, 100), "No network session found.", "No network session found", MessageBox.Type.MB_OK);
                        NoSession.Show();
                        return;
                    }

                    // Join the first session we found.
                    networkSession = NetworkSession.Join(availableSessions[0]);

                    HookSessionEvents();
                }
            }
            catch (Exception e)
            {
                new MessageBox(new Vector2(250, 100), "Error", e.Message, MessageBox.Type.MB_OK).Show();
            }
        }

        /// <summary>
        /// After creating or joining a network session, we must subscribe to
        /// some events so we will be notified when the session changes state.
        /// </summary>
        void HookSessionEvents()
        {
            networkSession.GamerJoined += GamerJoinedEventHandler;
            networkSession.SessionEnded += SessionEndedEventHandler;
        }


        /// <summary>
        /// This event handler will be called whenever a new gamer joins the session.
        /// We use it to allocate a Tank object, and associate it with the new gamer.
        /// </summary>
        void GamerJoinedEventHandler(object sender, GamerJoinedEventArgs e)
        {
            int gamerIndex = networkSession.AllGamers.IndexOf(e.Gamer);

            e.Gamer.Tag = new Player();
        }


        /// <summary>
        /// Event handler notifies us when the network session has ended.
        /// </summary>
        void SessionEndedEventHandler(object sender, NetworkSessionEndedEventArgs e)
        {
            new MessageBox(new Vector2(100, 100), "Session ended", e.EndReason.ToString(), MessageBox.Type.MB_OK);

            networkSession.Dispose();
            networkSession = null;
        }


        /// <summary>
        /// Updates the state of the network session, moving the tanks
        /// around and synchronizing their state over the network.
        /// </summary>
        void UpdateNetworkSession(GameTime TheGameTime)
        {
            // Update our locally controlled tanks, and send their
            // latest position data to everyone in the session.
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                UpdateLocalGamer(gamer, TheGameTime);
            }

            // Pump the underlying session object.
            networkSession.Update();

            // Make sure the session has not ended.
            if (networkSession == null)
                return;

            // Read any packets telling us the positions of remotely controlled tanks.
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                ReadIncomingPackets(gamer);
            }
        }


        /// <summary>
        /// Helper for updating a locally controlled gamer.
        /// </summary>
        void UpdateLocalGamer(LocalNetworkGamer gamer, GameTime TheGameTime)
        {
            // Look up what tank is associated with this local player.
            Player localPlayer = gamer.Tag as Player;

            // Update the tank.
            mPlayerSprite.UpdateMovement(aCurrentKeyboardState);

            localPlayer.Update(TheGameTime, this.Content);

            // Write the tank state into a network packet.
            packetWriter.Write(localPlayer.Position);
            packetWriter.Write(localPlayer.Rotation);
            //packetWriter.Write(localTank.TurretRotation);

            // Send the data to everyone in the session.
            gamer.SendData(packetWriter, SendDataOptions.InOrder);
        }


        /// <summary>
        /// Helper for reading incoming network packets.
        /// </summary>
        void ReadIncomingPackets(LocalNetworkGamer gamer)
        {
            // Keep reading as long as incoming packets are available.
            while (gamer.IsDataAvailable)
            {
                NetworkGamer sender;

                // Read a single packet from the network.
                gamer.ReceiveData(packetReader, out sender);

                // Discard packets sent by local gamers: we already know their state!
                if (sender.IsLocal)
                    continue;

                // Look up the tank associated with whoever sent this packet.
                Player remotePlayer = sender.Tag as Player;

                // Read the state of this tank from the network packet.
                remotePlayer.Position = packetReader.ReadVector2();
                remotePlayer.Rotation = packetReader.ReadSingle();
                //remotePlayer.TurretRotation = packetReader.ReadSingle();
            }
        }
        #endregion

        #region Form Controls Events

        private void Button1_OnPress(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Button Pressed!");
            ((Listbox)formCollection["form1"]["listbox1"]).Add(formCollection["form1"].Controls["textbox1"].Text);
        }
        private void Button1_OnRelease(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Button Released!");
        }

        private void Remove_OnPress(object obj, EventArgs e)
        {
            Listbox listbox1 = (Listbox)formCollection["form1"]["listbox1"];
            listbox1.RemoveAt(listbox1.SelectedIndex);
        }

        private void ButtonRow1_OnRelease(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Selected Index : " + (formCollection["form1"]["buttonRow1"] as ButtonRow).SelectedIndex);
            System.Diagnostics.Debug.WriteLine("Selected Item  : " + formCollection["form1"]["buttonRow1"].Text);
        }

        private void Checkbox1_OnRelease(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Checkbox Value: " + ((Checkbox)obj).Value);
        }

        private void Radiobutton1_OnRelease(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("RadioButton Value: " + ((RadioButton)obj).Value);
        }

        private void Label1_MouseOver(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Label.OnMouseOver");
        }
        private void Label1_MouseOut(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Label.OnMouseOut");
        }
        private void Label1_OnPress(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Label.OnPress");
        }
        private void Label1_OnRelease(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Label.OnRelease");
        }

        private void PictureBox1_MouseOver(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("PictureBox.OnMouseOver");
        }
        private void PictureBox1_MouseOut(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("PictureBox.OnMouseOut");
        }
        private void PictureBox1_OnPress(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("PictureBox.OnPress");
        }
        private void PictureBox1_OnRelease(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("PictureBox.OnRelease");
        }

        private void Potentiometer_OnChangeValue(object obj, EventArgs e)
        {
            ((Progressbar)formCollection["form1"].Controls["progressbar1"]).Value = (int)obj;
        }

        private void Form1_mnuFileClose(object obj, EventArgs e)
        {
            formCollection["form1"].Close();
        }
        private void Form1_mnuFileExit(object obj, EventArgs e)
        {
            if (graphics.IsFullScreen)
                graphics.ToggleFullScreen();

            this.Exit();
        }
        private void Form1_mnuViewToggleFS(object obj, EventArgs e)
        {
            graphics.PreferredBackBufferWidth = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            graphics.PreferredBackBufferHeight = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            graphics.ToggleFullScreen();
        }

        private void CheckboxGroup1_ChangeSelection(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("CheckboxGroup1.OnChangeSelection : " + (string)obj);
        }

        private void RadioButtonGroup1_ChangeSelection(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("RadioButtonGroup1.OnChangeSelection : " + (string)obj);
        }

        private void ButtonGroup1_ChangeSelection(object obj, EventArgs e)
        {
            string selection = obj as string;
            System.Diagnostics.Debug.WriteLine("ButtonGroup1.OnChangeSelection : " + selection);
        }

        private void Listview_ColumnHeader0_OnPress(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ListView ColumnHeader0 Pressed");
        }
        private void Listview_ColumnHeader0_OnRelease(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ListView ColumnHeader0 Released");
        }

        private void msgbox0_OnOk(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox0 OK");
            System.Diagnostics.Debug.WriteLine("MessageBox0 OK");
        }
        private void msgbox1_OnOk(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox1 OK");
            System.Diagnostics.Debug.WriteLine("MessageBox1 OK");
        }
        private void msgbox1_OnCancel(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox1 Cancel");
            System.Diagnostics.Debug.WriteLine("MessageBox1 Cancel");
        }
        private void msgbox2_OnYes(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox2 Yes");
            System.Diagnostics.Debug.WriteLine("MessageBox2 Yes");
        }
        private void msgbox2_OnNo(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox2 No");
            System.Diagnostics.Debug.WriteLine("MessageBox2 No");
        }
        private void msgbox3_OnYes(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox3 Yes");
            System.Diagnostics.Debug.WriteLine("MessageBox3 Yes");
        }
        private void msgbox3_OnNo(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox3 No");
            System.Diagnostics.Debug.WriteLine("MessageBox3 No");
        }
        private void msgbox3_OnCancel(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox3 Cancel");
            System.Diagnostics.Debug.WriteLine("MessageBox3 Cancel");
        }
        private void msgbox4_OnOk(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox4 OK");
            //frmDebug.WriteLine("MessageBox4 Output: " + (string)obj);
            System.Diagnostics.Debug.WriteLine("MessageBox4 OK, Output : " + (string)obj);
        }
        private void msgbox5_OnOk(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox5 OK");
            //frmDebug.WriteLine("MessageBox5 Output: " + (string)obj);
            System.Diagnostics.Debug.WriteLine("MessageBox5 OK, Output : " + (string)obj);
        }
        private void msgbox5_OnCancel(object obj, EventArgs e)
        {
            //frmDebug.WriteLine("MessageBox5 Cancel");
            //frmDebug.WriteLine("MessageBox5 Output: " + (string)obj);
            System.Diagnostics.Debug.WriteLine("MessageBox5 Cancel, Output: " + (string)obj);
        }

        #endregion


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            aCurrentKeyboardState = Keyboard.GetState();
            aCurrentMouseState = Mouse.GetState();

            // Allows the game to exit
            if (aCurrentKeyboardState.IsKeyDown(Keys.Escape) == true)
                //this.Exit();
                formCollection["form1"].Show();

            //Update the form collection
            formCollection.Update(gameTime);

            if (formCollection["form1"].State != Form.WindowState.Minimized || Guide.IsVisible)
                InMenu = true;
            else
                InMenu = false;

            //gameTime = new GameTime(gameTime.TotalRealTime, gameTime.ElapsedRealTime, new TimeSpan((long)(gameTime.TotalGameTime.Ticks * 0.33f)), new TimeSpan((long)(gameTime.ElapsedGameTime.Ticks * 0.33f)));
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            audioEngine.Update();

            // Update the person's transform
            EnemyTransform = 
                    Matrix.CreateTranslation(new Vector3(-new Vector2(Enemy.Size.Width /2,Enemy.Size.Height/2), 0.0f)) *
                    Matrix.CreateRotationZ(Enemy.Rotation) *
                    Matrix.CreateTranslation(new Vector3(Enemy.Position, 0.0f));

            //Enemy Bounding rectangle
            EnemyRectangle = new Rectangle((int)Enemy.Position.X - ((int)Enemy.Size.Width/2), (int)Enemy.Position.Y - ((int)Enemy.Size.Height/2), Enemy.mSpriteTexture.Width, Enemy.mSpriteTexture.Height);

            // TODO: Add your update logic here
            if (InMenu == false)
                mPlayerSprite.Update(gameTime, this.Content);

            Enemy_hit = bullet.UpdateBullets(Enemy,EnemyRectangle,EnemyTextureData,EnemyTransform,ScreenWidth,ScreenHeight);

            fireingDelay += time;
            if (aCurrentMouseState.LeftButton == ButtonState.Pressed && mPreviousMouseState.LeftButton == ButtonState.Pressed && fireingDelay > 0.07f && InMenu == false)
            {
                shot = true;
                bullet.FireBullet(mPlayerSprite,soundBank);
                //Recoil(aCurrentMouseState,mPreviousMouseState);
                fireingDelay = 0.0f;
            }

            if (formCollection["form1"].State == Form.WindowState.Minimized)
                FormCollection.IsCursorVisible = false;
            else
                FormCollection.IsCursorVisible = true;

            #region Network update

            if (networkSession == null)
            {
                // If we are not in a network session, update the
                // menu screen that will let us create or join one.
                UpdateMenuScreen();
            }
            else
            {
                // If we are in a network session, update it.
                UpdateNetworkSession(gameTime);
            }

            #endregion

            mPreviousKeyboardState = aCurrentKeyboardState;
            mPreviousMouseState = aCurrentMouseState;

            base.Update(gameTime);
        }

        private void Recoil(MouseState aCurrentMouseState, MouseState mPreviousMouseState)
        {
            mPlayerSprite.Position.X -= (float)Math.Cos(mPlayerSprite.Rotation) * 5;
            mPlayerSprite.Position.Y -= (float)Math.Sin(mPlayerSprite.Rotation) * 5;
            if (aCurrentMouseState.LeftButton == ButtonState.Released && mPreviousMouseState.LeftButton == ButtonState.Released)
            {
                mPlayerSprite.Position.X += (float)Math.Cos(mPlayerSprite.Rotation) * 5;
                mPlayerSprite.Position.Y += (float)Math.Sin(mPlayerSprite.Rotation) * 5;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Render the form collection (required before drawing)
            formCollection.Render();

            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.Draw(Content.Load<Texture2D>("tile_sand"), new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);

            //spriteBatch.Draw(Content.Load<Texture2D>("test_enemy_collision"), bullet.BulletRectangle, Color.White); //Stomme Collision helper voor Bullet :)
            //spriteBatch.Draw(Content.Load<Texture2D>("test_enemy_collision"), EnemyRectangle, Color.White); //Stomme Collision helper voor Enemy :)

            if (Enemy_hit == true)
                spriteBatch.DrawString(UVfont, "-"+bullet.Damage.ToString(), new Vector2(Enemy.Position.X - (Enemy.Size.Width / 2), Enemy.Position.Y - 60), Color.Red);

            mPlayerSprite.Draw(this.spriteBatch, mPlayerSprite.Customize_body_color);

            Enemy.Draw(this.spriteBatch, Color.White);

            if (formCollection["form1"].State == Form.WindowState.Minimized || Guide.IsVisible == false)
                spriteBatch.Draw(crosshair,new Vector2(aCurrentMouseState.X - crosshair.Width /2,aCurrentMouseState.Y - crosshair.Height /2),Color.Red);

            spriteBatch.Draw(ak47,new Rectangle((int)mPlayerSprite.Position.X,(int)mPlayerSprite.Position.Y,ak47.Width,ak47.Height),null,Color.White,mPlayerSprite.Rotation,new Vector2(ak47.Width /2 -26,ak47.Height /2),SpriteEffects.None,0);

            if (shot == true)
            {
                spriteBatch.Draw(muzzleflash, new Rectangle((int)mPlayerSprite.Position.X, (int)mPlayerSprite.Position.Y, muzzleflash.Width, muzzleflash.Height), null, Color.White, mPlayerSprite.Rotation, new Vector2(muzzleflash.Width / 2 - (23 + ak47.Width), muzzleflash.Height / 2), SpriteEffects.None, 1);
            }
            shot = false;

            foreach (Bullet2 b in bullet.bullets)
            {
                if (b.Alive)
                {
                    spriteBatch.Draw(b.Texture,b.Position,null,Color.White,b.Rotation,b.Center,b.Scale,SpriteEffects.None,1.0f);
                }
            }

            spriteBatch.End();

            //Draw the form collection
            formCollection.Draw();

            base.Draw(gameTime);
        }
    }
}
