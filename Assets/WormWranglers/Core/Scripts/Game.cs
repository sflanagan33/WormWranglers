using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WormWranglers.Beetle;

namespace WormWranglers.Core
{
    public static class Game
    {
        // These are global settings that many of the managers either edit directly or use.

        public static int BEETLE_COUNT;
        public static BeetleControls[] BEETLE_CONTROLS;
        public static int[] BEETLE_MODEL_CHOICE;
        public static int[] BEETLE_PALETTE_CHOICE;

        // ====================================================================================================================
        // Initializes the game state with the given beetle count (creating data arrays / assigning controls.)

        public static void Initialize(int beetleCount)
        {
            BEETLE_COUNT = beetleCount;
            BEETLE_CONTROLS = new BeetleControls[beetleCount];
            BEETLE_MODEL_CHOICE = new int[beetleCount];
            BEETLE_PALETTE_CHOICE = new int[beetleCount];

            for (int i = 0; i < beetleCount; i++)
                BEETLE_PALETTE_CHOICE[i] = i;

            // First player

            BEETLE_CONTROLS[0] = new BeetleControls(KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S);

            // Second player

            bool gamepad = Input.GetJoystickNames().Length > 0;

            if (beetleCount >= 2)
            {
                if (gamepad)
                    BEETLE_CONTROLS[1] = new BeetleControls("Gamepad1Horizontal", "Gamepad1Vertical");
                else
                    BEETLE_CONTROLS[1] = new BeetleControls(KeyCode.J, KeyCode.L, KeyCode.I, KeyCode.K);
            }

            // Third player

            bool secondGamepad = Input.GetJoystickNames().Length > 1;

            if (beetleCount >= 3)
            {
                if (secondGamepad)
                    BEETLE_CONTROLS[2] = new BeetleControls("Gamepad2Horizontal", "Gamepad2Vertical");
                else if (gamepad)
                    BEETLE_CONTROLS[2] = new BeetleControls(KeyCode.J, KeyCode.L, KeyCode.I, KeyCode.K);
                else
                    BEETLE_CONTROLS[2] = new BeetleControls(KeyCode.Keypad4, KeyCode.Keypad6, KeyCode.Keypad8, KeyCode.Keypad5);
            }
        }

        // ====================================================================================================================
        // Given a list of beetle cameras and a worm camera, arranges them onscreen (using the known BEETLE_COUNT.)

        public static void ArrangeCameras(List<Camera> beetles, Camera worm)
        {
            Rect upperLeft  = new Rect(0f,   0.5f, 0.5f, 0.5f);
            Rect lowerLeft  = new Rect(0f,   0f,   0.5f, 0.5f);
            Rect lowerRight = new Rect(0.5f, 0f,   0.5f, 0.5f);
            Rect upperRight = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            Rect leftHalf   = new Rect(0f,   0f,   0.5f, 1f);
            Rect rightHalf  = new Rect(0.5f, 0f,   0.5f, 1f);

            if (BEETLE_COUNT == 1)
            {
                beetles[0].rect = leftHalf;
                worm.rect       = rightHalf;
            }

            else if (BEETLE_COUNT == 2)
            {
                beetles[0].rect = upperLeft;
                beetles[1].rect = lowerLeft;
                worm.rect       = rightHalf;
            }

            else if (BEETLE_COUNT == 3)
            {
                beetles[0].rect = upperLeft;
                beetles[1].rect = lowerLeft;
                beetles[2].rect = lowerRight;
                worm.rect       = upperRight;
            }
        }
    }
}