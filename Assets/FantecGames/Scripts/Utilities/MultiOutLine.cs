using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fantec.Utilities
{
    [RequireComponent(typeof(Text))]
    public class MultiOutLine : BaseMeshEffect
    {
        [SerializeField]
        [UnityEngine.Range(0, 100)]
        private int _amount;
        [SerializeField]
        private Color _color;
        [SerializeField]
        private float _offset;

        private readonly List<UIVertex>_outlineVertexList=new List<UIVertex>();
        private readonly List<UIVertex> _vertexList=new List<UIVertex>();

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            _vertexList.Clear();
            _outlineVertexList.Clear();
            vh.GetUIVertexStream(_vertexList);

            var splitAngle = 360f / _amount;
            UIVertex v;

            var count = _vertexList.Count;
            for(var i=0;i<_amount;i++)
            {
                var angle=splitAngle*i;
                for(var j=0;j<count;j++)
                {
                    v = _vertexList[j];
                    var pos = v.position;
                    pos.x += Mathf.Cos(angle * Mathf.Deg2Rad) * _offset;
                    pos.y += Mathf.Sin(angle * Mathf.Deg2Rad) * _offset;
                    v.position = pos;
                    v.color = _color;
                    _outlineVertexList.Add(v);
                }
            }

            _outlineVertexList.AddRange(_vertexList);

            vh.Clear();
            vh.AddUIVertexTriangleStream(_outlineVertexList);
        }
    }
}