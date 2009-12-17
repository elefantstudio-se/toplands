#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry Customize_hat_MenuEntry;
        fpsrpg.Player PlayerStuff = new fpsrpg.Player();

        static string[] hats = { "Round Hat", "Forward Cap", "Tactical helmet", "Heavy helmet" };
        static int currenthat = fpsrpg.Properties.Settings.Default.Customize_hat;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            Customize_hat_MenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry backMenuEntry = new MenuEntry("Back");

            // Hook up menu event handlers.
            Customize_hat_MenuEntry.Selected += Customize_hat_MenuEntrySelected;
            backMenuEntry.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(Customize_hat_MenuEntry);
            MenuEntries.Add(backMenuEntry);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            //ungulateMenuEntry.Text = "Preferred ungulate: " + currentUngulate;
            Customize_hat_MenuEntry.Text = "Hat: " + hats[currenthat];
            //frobnicateMenuEntry.Text = "Frobnicate: " + (frobnicate ? "on" : "off");
            //elfMenuEntry.Text = "elf: " + elf;
        }


        #endregion

        #region Handle Input


        ///// <summary>
        ///// Event handler for when the Ungulate menu entry is selected.
        ///// </summary>
        //void UngulateMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        //{
        //    currentUngulate++;

        //    if (currentUngulate > Ungulate.Llama)
        //        currentUngulate = 0;

        //    SetMenuEntryText();
        //}


        /// <summary>
        /// Event handler for when the Language menu entry is selected.
        /// </summary>
        void Customize_hat_MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currenthat = (currenthat + 1) % hats.Length;

            SetMenuEntryText();
            fpsrpg.Properties.Settings.Default.Customize_hat = currenthat;
            fpsrpg.Properties.Settings.Default.Save();
        }


        ///// <summary>
        ///// Event handler for when the Frobnicate menu entry is selected.
        ///// </summary>
        //void FrobnicateMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        //{
        //    frobnicate = !frobnicate;

        //    SetMenuEntryText();
        //}


        ///// <summary>
        ///// Event handler for when the Elf menu entry is selected.
        ///// </summary>
        //void ElfMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        //{
        //    elf++;

        //    SetMenuEntryText();
        //}


        #endregion
    }
}
