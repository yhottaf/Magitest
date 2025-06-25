using UnityEngine;

namespace fantec.Utilities
{
    /// <summary>
    /// 同じ作りのものを複製する際にベースとして使用するクラス
    /// </summary>
    public abstract class ItemDuplicator<T> : MonoBehaviour where T : ItemDuplicator<T>
    {

        /// <summary>
        /// 複製を作成する
        /// </summary>
        /// <param name="data"> 複製の際に使用するデータ </param>
        /// <returns>複製されたアイテム</returns>
        public ItemDuplicator<T>Create(ItemDuplicatorData data)
        {
            ItemDuplicator<T> item=Instantiate(this.gameObject).GetComponent<ItemDuplicator<T>>();
            item.transform.SetParent(this.transform.parent);
            item.transform.localScale = Vector2.one;
            item.gameObject.SetActive(true);

            item.Setup(data);
            item.Release();

            return item;
        }

        /// <summary>
        /// 初期設定
        /// </summary>
        protected abstract void Setup(ItemDuplicatorData data);

        /// <summary>
        /// 複製したものの不要なリソースを取り除く
        /// </summary>
        protected abstract void Release();
    }


    public abstract class ItemDuplicatorData { }
}