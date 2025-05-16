using UnityEngine;

namespace fantec.Battle
{
    public partial class BD
    {
        public class SortingLayer
        {
            public const string NAME_BACKGROUND = "Background";
            public const string NAME_FIELD_DEFAULT = "Field_Default";
            public const string NAME_FIELD_BLACKOUT = "Field_Blackout";
            public const string NAME_HUD = "Hud";
            public const string NAME_WINDOW = "Window";

            public enum FieldOrder
            {
                Default=0,
                NumeralWhite=1,
                NumeralRed=2,
                NumeralGold=3,
                CharaBoss=4,
                CharaSd=5,
                Particle=6,
                Numeral=7,
                BlowScreen=8,

                // 100以上はアニメーション領域
            }
        }
    }
}