﻿namespace RotationAnarchy
{
    using Parkitect.UI;
    using RotationAnarchy.Internal;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public enum ParkitectState
    {
        None,
        Placement
    }

    public class RAController : ModChange
    {
        public event Action<bool> OnActiveChanged;
        public event Action<ParkitectState> OnGameStateChanged;

        public bool IsWindowOpened => RAWindow.Instance != null;

        public bool Active
        {
            get => _active;
            set
            {
                if (_active != value)
                {
                    _active = value;
                    HandleActiveStateChange();
                    OnActiveChanged?.Invoke(Active);
                }
            }
        }

        public bool DeactivatedLastFrame => !Active && _previousFrameActive;
        public bool ActivatedLastFrame => Active && !_previousFrameActive;

        private bool _active;
        private bool _previousFrameActive;

        public ParkitectState GameState
        {
            get => _gameState;
            set
            {
                if (value != _gameState)
                {
                    PreviousGameState = _gameState;
                    _gameState = value;
                    HandleGameStateChange();
                    OnGameStateChanged?.Invoke(value);
                }
            }
        }
        private ParkitectState _gameState = ParkitectState.None;
        public ParkitectState PreviousGameState { get; private set; }

        private HashSet<Type> AllowedBuilderTypes = new HashSet<Type>()
        {
            typeof(DecoBuilder),
            typeof(FlatRideBuilder)
        };

        public override void OnApplied()
        {
            RA.RAActiveHotkey.onKeyDown += ToggleRAActive;
        }

        public override void OnStart()
        {
            HandleActiveStateChange();
            HandleGameStateChange();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            _previousFrameActive = Active;
        }

        public override void OnReverted() { }

        public void NotifyBuildState(bool building, Builder builder)
        {
            // If builder has been opened
            if (building)
            {
                // If this builder is one of the allowed builder types
                if (AllowedBuilderTypes.Contains(builder.GetType()))
                {
                    GameState = ParkitectState.Placement;
                    ModBase.LOG("Building");
                }
            }
            else
            {
                ModBase.LOG("Not Building");
                GameState = ParkitectState.None;
            }
        }

        public void ToggleRAActive()
        {
            Active = !Active;
        }

        private void HandleActiveStateChange()
        {
            ModBase.LOG("HandleActiveStateChange = " + Active);
            if (Active)
            {
                RAWindowButton.Instance.SetButtonEnabled(true);
                HandleGameStateChange();
            }
            else
            {
                RAWindowButton.Instance.SetButtonEnabled(false);
                if(IsWindowOpened)
                    RAWindowButton.Instance.SetWindowOpened(false);
            }
        }

        private void HandleGameStateChange()
        {
            ModBase.LOG("HandleGameStateChange = " + GameState);

            if (ShouldWindowBeOpened())
            {
                if (!IsWindowOpened)
                {
                    RAWindowButton.Instance.SetWindowOpened(true);
                }
            }
        }

        private bool ShouldWindowBeOpened()
        {
            return Active && GameState == ParkitectState.Placement;
        }
    }
}