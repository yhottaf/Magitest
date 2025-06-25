using Cysharp.Threading.Tasks;
using fantec.Battle;
using fantec.Common;
using fantec.Master;
using fantec.Menu.Manager;
using fantec.PlayFabClient;
using Spine.Unity;
using System;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace fantec.Menu.PartySortie
{
    public class PartySortieCharaObject : MonoBehaviour
    {
        [SerializeField]
        private SkeletonGraphic m_SpineGraphic;

        [SerializeField]
        private Button m_Button;

        [SerializeField]
        private GameObject m_Content;
        
        [SerializeField, Range(1e-2f, 100f)]
        private float ThresholdSeconds = 1.0f; // 1秒長押し

        [SerializeField]
        private ObservableEventTrigger eventTrigger;

        private GameObject ghostObject;

        private readonly Vector3 DefaultScale = new Vector3(10, 10, 10); // ゴーストのデフォルトスケール
        private readonly float alpha = 0.5f; // ゴーストSpineの透明度

        // プロパティ
        private int m_CardId;
        private int m_PositionIndex=0;

        public int CardId => m_CardId;
        public int PositionIndex => m_PositionIndex;


        private Subject<Unit> _onPointerDown = new Subject<Unit>();
        private Subject<Unit> _onPointerUp = new Subject<Unit>();

        private CompositeDisposable _disposables = new CompositeDisposable();

        // 長押し
        public IObservable<Unit> OnPointerDownAsObservable
        {
            get { return _onPointerDown; }
        }

        // 離した時
        public IObservable<Unit> OnPointerUpAsObservable
        {
            get { return _onPointerUp; }
        }

        private Vector2 beginPosition;
        private DateTime beginTime;


        private void Start()
        {
            RegisterEventTriggers();

            this.OnEnableAsObservable()
                .Subscribe(_ =>
                {
                    if (eventTrigger != null)
                    {
                        RegisterEventTriggers();
                    }
                })
                .AddTo(this);
        }

        /// <summary>
        /// 見た目を更新する
        /// </summary>
        public async UniTask Settings(int cardId, CancellationToken cts = default)
        {
            if(cardId==-1)
            {
                this.gameObject.SetActive(false);
                return;
            }
            this.gameObject.SetActive(true);
            PlayerCardData masterData = MasterDataManager.Instance.PlayerCardMaster.GetData(cardId);
            m_CardId = masterData.cardId;

            if (MasterDataManager.Instance.PlayerCardMaster.IsExist(m_CardId))
            {
                int originId = MasterDataManager.Instance.PlayerCardMaster.GetData(m_CardId).originId;

                string address = AssetPath.GetCharacterSpinePath(originId);

                // AssetManager 経由で非同期にロード
                var skeletonData = await AssetManager.Instance.LoadAssetAsync<SkeletonDataAsset>(address, cts);

                if (skeletonData != null)
                {
                    m_SpineGraphic.skeletonDataAsset = skeletonData;
                    m_SpineGraphic.Initialize(true);
                    m_SpineGraphic.allowMultipleCanvasRenderers = true;
                }
                else
                {
                    Debug.LogError($"[Settings] Spineアセットの読み込みに失敗しました。address: {address}");
                }
            }

            CardData cardData = CardManager.GetCardData(m_CardId);

            // 現在のスロット(現状5スロット(デッキ)ある)の配置場所の取得
            m_PositionIndex = cardData.positionIndex[PlayerPrefsManager.SelectPartyIndex];
            if (m_PositionIndex == 9)
            {
                m_PositionIndex = 8;
            }
        }


        public void CreateGhosst()
        {
            if (m_SpineGraphic.skeletonDataAsset == null || m_SpineGraphic == null) return;
            // Ghostオブジェクト生成
            ghostObject = new GameObject("SpineGhost");
            ghostObject.transform.SetParent(m_Content.transform);
            ghostObject.transform.localPosition = Vector3.zero;
            ghostObject.transform.localRotation = Quaternion.identity;
            ghostObject.transform.localScale = DefaultScale;
        
            var ghostRenderer = ghostObject.AddComponent<SkeletonAnimation>();
            ghostRenderer.skeletonDataAsset = m_SpineGraphic.skeletonDataAsset;
            ghostRenderer.Initialize(true);
            ghostRenderer.GetComponent<Renderer>().sortingLayerName = "MainCanvas";


            // スキンとポーズを同期
            ghostRenderer.skeleton.SetSkin(m_SpineGraphic.Skeleton.Skin);
            ghostRenderer.skeleton.SetSlotsToSetupPose();
            ghostRenderer.skeleton.SetBonesToSetupPose();

            var currentTrack = m_SpineGraphic.AnimationState.GetCurrent(0);
            if (currentTrack != null)
            {
                var anim = currentTrack.Animation;
                ghostRenderer.AnimationState.SetAnimation(0, anim, currentTrack.Loop);
                ghostRenderer.AnimationState.Apply(ghostRenderer.skeleton);
            }

            foreach (var slot in ghostRenderer.skeleton.Slots)
            {
                var c = slot.GetColor();
                c.a = alpha;
                slot.SetColor(c);
            }
        }

        public void DestroyGhost()
        {
            if (ghostObject != null)
            {
                Destroy(ghostObject);
                ghostObject = null;
            }
        }

        public void UpdateGhostPosition(Vector3 worldPos)
        {
            if (ghostObject != null)
            {
                ghostObject.transform.position = worldPos;
            }
        }

        public void SetPositionIndex(int index)
        {
            m_PositionIndex = index;
        }

        private void ApplyShader(SkeletonAnimation obj)
        {
            Shader loadedShader = Shader.Find("Spine/Skeleton");
            // SkeletonDataAsset → AtlasAssets → Materialを辿る
            var atlasAssets = obj.skeletonDataAsset.atlasAssets;
            foreach (var atlasAsset in atlasAssets)
            {
                var materials = atlasAsset.Materials;
                foreach (var mat in materials)
                {
                    mat.shader = loadedShader;
                    Debug.Log($"Shaderを適用しました！Material: {mat.name}");
                }
            }
            Debug.Log("Shaderロード完了！マテリアルに適用しました！");
        }

        private void RegisterEventTriggers()
        {
            _disposables.Clear(); // 前回の購読を破棄

            eventTrigger
                .OnBeginDragAsObservable()
                .TakeUntilDisable(this)
                .Where(eventData => eventData.pointerDrag.gameObject == this.gameObject)
                .Subscribe(_ =>
                {
                    this.beginTime = DateTime.Now;
                    if (ghostObject == null)
                    {
                        CreateGhosst();
                    }
                    _onPointerDown.OnNext(Unit.Default);
                })
                .AddTo(_disposables);

            eventTrigger
                .OnEndDragAsObservable()
                .TakeUntilDisable(this)
                .Where(_ => (DateTime.Now - this.beginTime).TotalSeconds < ThresholdSeconds)
                .Subscribe(_ =>
                {
                    DestroyGhost();
                    _onPointerUp.OnNext(Unit.Default);
                })
                .AddTo(_disposables);

            eventTrigger
                .OnPointerUpAsObservable()
                .TakeUntilDisable(this)
                .Subscribe(_ =>
                {
                    DestroyGhost();
                    _onPointerUp.OnNext(Unit.Default);
                })
                .AddTo(_disposables);
        }
    }
}