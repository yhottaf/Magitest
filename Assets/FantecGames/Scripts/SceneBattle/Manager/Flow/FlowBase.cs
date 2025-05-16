

namespace fantec.Battle.Manager
{
    public abstract class FlowBase
    {
        /// <summary>
        /// フロー開始時に呼ばれる
        /// </summary>
        public virtual void OnEnter(BattleFlowManager manager,FlowBase prevFlow) { }

        /// <summary>
        /// 毎フレーム呼ばれる
        /// </summary>
        public virtual void OnUpdate(BattleFlowManager manager) { }

        /// <summary>
        /// フロー終了時に呼ばれる
        /// </summary>
        public virtual void OnExit(BattleFlowManager manager,FlowBase nextFlow) { }
    }

}