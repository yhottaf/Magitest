using UnityEngine;
using UnityEngine.UI;


namespace fantec
{
    /// <summary>
    /// 上下左右の反転
    /// </summary>
    [AddComponentMenu("fantec/Lib/UI/UguiFlip")]
    public class UguiFlip : BaseMeshEffect
    {
        public bool FlipX { get { return flipX; } set { flipX = value; graphic.SetVerticesDirty(); } }
        [SerializeField]
        bool flipX;

        public bool FlipY { get { return flipY; } set { flipY = value; graphic.SetVerticesDirty(); } }
        [SerializeField]
        bool flipY;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!enabled)
                return;
            if (!FlipX && !FlipY)
                return;

            Rect r = graphic.rectTransform.rect;
            Vector2 pivot = graphic.rectTransform.pivot;
            float x_offset = -(pivot.x - 0.5f) * r.width * 2;
            float y_offset = -(pivot.y - 0.5f) * r.height * 2;
            UIVertex vert = new UIVertex();
            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vert, i);
                if (FlipX) vert.position.x = -vert.position.x + x_offset;
                if (FlipY) vert.position.y = -vert.position.y + y_offset;
                vh.SetUIVertex(vert, i);
            }
        }
    }
}