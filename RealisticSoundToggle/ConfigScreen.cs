﻿using Sandbox.Graphics.GUI;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Utils;
using VRageMath;

namespace SE_RealisticSoundToggle
{
    public class ConfigScreen : MyGuiScreenBase
    {
        public override string GetFriendlyName() => "RealisticSoundToggle_ConfigScreen";

        private MyGuiControlCheckbox _overrideSetting, _enableRealistic;

        private readonly Config _config;
        private readonly string _configPath;

        public ConfigScreen(Config config, string configPath) :
            base(new Vector2(0.5f, 0.5f),
                MyGuiConstants.SCREEN_BACKGROUND_COLOR,
                new Vector2(0.5f, 0.5f),
                false,
                null,
                MySandboxGame.Config.UIBkOpacity,
                MySandboxGame.Config.UIOpacity)
        {
            _config = config;
            _configPath = configPath;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            RecreateControls(true);
        }

        public override void RecreateControls(bool constructor)
        {
            base.RecreateControls(constructor);

            AddCaption("RealisticSoundToggle Config");

            bool isIngame = SessionComp.IsSessionRealisticSound != null;

            var grid = new UniformGrid(2, 3, new Vector2(0.2f, 0.05f));

            grid.Add(new MyGuiControlLabel(text: "World Setting:", originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER), 0, 0);
            grid.Add(new MyGuiControlLabel(text: SessionComp.IsSessionRealisticSound != null ? (SessionComp.IsSessionRealisticSound.Value ? "Realistic" : "Arcade") : "Not Ingame", originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER), 1, 0);

            grid.Add(new MyGuiControlLabel(text: "Override World Setting:", originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER), 0, 1);
            grid.Add(_overrideSetting = new MyGuiControlCheckbox(
                isChecked: _config.OverrideWorldSound,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER,
                toolTip: !isIngame ? "" : "You can only change this on the main menu."), 1, 1);
            _overrideSetting.Enabled = !isIngame;

            grid.Add(new MyGuiControlLabel(text: "Enable Realistic Sound:", originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER), 0, 2);
            grid.Add(_enableRealistic = new MyGuiControlCheckbox(
                isChecked: _config.EnableRealisticSound,
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER,
                toolTip: !isIngame ? "On = Realistic Sound\nOff = Arcade Sound" : "You can only change this on the main menu."), 1, 2);
            _enableRealistic.Enabled = !isIngame;

            grid.AddItemsTo(Controls, new Vector2(-0.04f, 0), false);

            float btnYPos = (Size.Value.Y * 0.5f) - (MyGuiConstants.SCREEN_CAPTION_DELTA_Y / 2f);
            MyGuiControlButton saveBtn = new MyGuiControlButton(
                new Vector2(0f, btnYPos),
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM,
                text: new StringBuilder("Save"),
                onButtonClick: OnSaveButtonClick);
            Controls.Add(saveBtn);

            CloseButtonEnabled = true;
        }

        private void OnSaveButtonClick(MyGuiControlButton sender)
        {
            _config.OverrideWorldSound = _overrideSetting.IsChecked;
            _config.EnableRealisticSound = _enableRealistic.IsChecked;

            _config.Save(_configPath);
            SessionComp.UpdateSoundSetting();
            this.CloseScreen();
        }
    }

    public class GuiControlWrapper
    {
        public MyGuiControlBase Control;
        public int Column;
        public int Row;
    }

    public class UniformGrid
    {
        public int Columns { get; set; }
        public int Rows { get; set; }

        public Vector2 CellSize { get; set; }

        private List<GuiControlWrapper> controls;

        public UniformGrid(int columns, int rows, Vector2 cellSize)
        {
            Columns = columns;
            Rows = rows;
            CellSize = cellSize;

            controls = new List<GuiControlWrapper>();
        }

        public void Add(MyGuiControlBase control, int column, int row)
        {
            controls.Add(new GuiControlWrapper
            {
                Control = control,
                Column = column,
                Row = row,
            });
        }

        public void AddItemsTo(MyGuiControls target, Vector2 position, bool addBorderLines = false)
        {
            Vector2 totalSize = new Vector2(Columns, Rows) * CellSize;
            Vector2 topLeft = position - (totalSize / 2);
            //Vector2 bottomRight = position + (totalSize / 2);
            foreach (var control in controls)
            {
                control.Control.Position =
                    topLeft + (CellSize / 2) + (new Vector2(control.Column, control.Row) * CellSize);

                target.Add(control.Control);
            }

            if (addBorderLines)
            {
                var separators = new MyGuiControlSeparatorList
                {
                    Position = position,
                };

                for (int x = 0; x < Columns + 1; x++)
                {
                    separators.AddVertical(new Vector2(topLeft.X + (CellSize.X * x), topLeft.Y), totalSize.Y, 0.001f);
                }

                for (int y = 0; y < Rows + 1; y++)
                {
                    separators.AddHorizontal(new Vector2(topLeft.X, topLeft.Y + (CellSize.Y * y)), totalSize.X, 0.0015f);
                }

                target.Add(separators);
            }
        }
    }
}
