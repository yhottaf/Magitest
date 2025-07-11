using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Live2D.Cubism.Rendering;
using fantec.Extensions;
using fantec;

namespace fantec
{
    [AddComponentMenu("fantec/ADV/Internal/GraphicObject/Live2D")]
    internal class AdvGraphicObjectLive2D : MonoBehaviour, IAdvGraphicObjectCustom
    {
        public AdvEngine Engine => this.AdvObj.Engine;

        AdvGraphicObjectCustom AdvObj
        {
            get
            {
                if (advObj == null)
                {
                    advObj = this.GetComponentInParent<AdvGraphicObjectCustom>();
                }
                return advObj;
            }
        }
        AdvGraphicObjectCustom advObj;

        CubismRenderController RenderController
        {
            get { return this.GetComponentCache<CubismRenderController>(ref renderController); }
        }
        CubismRenderController renderController;

        Live2DLipSynch LipSynch
        {
            get { return this.GetComponentCache<Live2DLipSynch>(ref lipSynch); }
        }
        Live2DLipSynch lipSynch;

        //描画時のリソース変更
        public void ChangeResourceOnDrawSub(AdvGraphicInfo graphic)
        {
            //描画順の設定
            RenderController.SortingOrder = AdvObj.Layer.SettingData.Order;
            //リップシンクありならそれを設定
            if (LipSynch != null)
            {
                LipSynch.Play();
            }
        }

        //エフェクト用の色が変化したとき
        public void OnEffectColorsChange(AdvEffectColor color)
        {
            this.RenderController.Opacity = color.MulColor.a;
        }

        void Start()
        {
            if (LipSynch != null && Engine != null)
            {
                AdvGraphicObjectCustom advObj = this.GetComponentInParent<AdvGraphicObjectCustom>();
                if (advObj != null)
                {
                    //リップシンクのキャラクターラベルを設定
                    LipSynch.CharacterLabel = advObj.ParentObject.gameObject.name;
                    LipSynch.OnCheckTextLipSync.AddListener(
                            (x) =>
                            {
                                x.EnableTextLipSync = (x.CharacterLabel == Engine.Page.CharacterLabel && Engine.Page.IsSendChar);
                            });
                }
            }
        }
    }
}
