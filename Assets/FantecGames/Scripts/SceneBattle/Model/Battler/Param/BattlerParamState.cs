using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Model
{
    public class BattlerParamState : IBattlerParamStatePrivate
    {
        public IObservable<List<TokenCell>> OnUpdateTokenCellList => m_UpdateTokenListSubject;
        public IObservable<AffectInfo> OnTakeBuffObservable => m_TakeBuffSubject;

        public int OriginalATK => m_StateEntity.ATK;
        public int OriginalSPD => m_StateEntity.SPD;
        public int OriginalDEX => 100; // 100 %
        public int OriginalLUK => 0;   // 0%
        public int OriginalVIT => 0;
        public int OriginalDMG => 99999;
        public int OriginalMaxHP => m_StateEntity.HP;

        public int OriginalMOVE => m_StateEntity.Move; // デフォルトの移動力

        public int Spec => CurrentMaxHP + CurrentATK + CurrentDEX + CurrentDMG + CurrentLUK + CurrentSPD + CurrentVIT;

        public float RatioHP => (float)CurrentHP / (float)CurrentMaxHP;

        // 現在のステータス

        public int CurrentUnitID => m_StateEntity.originId;
        public int CurrentATK => OriginalATK * (BuffATK + 100) / 100;
        public int CurrentSPD => OriginalSPD * (BuffSPD + 100) / 100;
        public int CurrentDEX => Mathf.Clamp(OriginalDEX + BuffDEX, 0, 100);
        public int CurrentLUK => Mathf.Clamp(OriginalLUK + BuffLUK, 0, 100);
        public int CurrentVIT => Mathf.Clamp(OriginalVIT + BuffVIT, -100, 100);
        public int CurrentDMG => Mathf.Clamp(OriginalDMG + BuffDMG, 0, int.MaxValue);
        public int CurrentHP => m_StateHealth.CurrentHealth;
        public int CurrentMaxHP => OriginalMaxHP + (BuffMaxHP == 0 ? 0 : (OriginalMaxHP * BuffMaxHP / 100));

        public int CurrentMOVE => Mathf.Clamp(OriginalMOVE + BuffMOVE, 1, 8);

        // バフステータス計算
        public int BuffATK => GetBuffValue(AffectCategoryType.ATK_Buff) - GetBuffValue(AffectCategoryType.ATK_Debuff);
        public int BuffSPD => GetBuffValue(AffectCategoryType.SPD_Buff) - GetBuffValue(AffectCategoryType.SPD_Debuff);
        public int BuffDEX => GetBuffValue(AffectCategoryType.DEX_Buff) - GetBuffValue(AffectCategoryType.DEX_Debuff);
        public int BuffLUK => GetBuffValue(AffectCategoryType.LUK_Buff) - GetBuffValue(AffectCategoryType.LUK_Debuff);
        public int BuffVIT => GetBuffValue(AffectCategoryType.VIT_Buff) - GetBuffValue(AffectCategoryType.VIT_Debuff);
        public int BuffDMG => GetBuffValue(AffectCategoryType.DMG_Buff) - GetBuffValue(AffectCategoryType.DMG_Debuff);
        public int BuffMaxHP => GetBuffValue(AffectCategoryType.HP_Buff) - GetBuffValue(AffectCategoryType.HP_Debuff);
        public int BuffMOVE => GetBuffValue(AffectCategoryType.MOVE_Buff) - GetBuffValue(AffectCategoryType.MOVE_Debuff);

        public bool IsConfusion => m_TokenCellList.GetisAnyToken(AffectCategoryType.Confusion);
        public bool IsRestraintDS => m_TokenCellList.GetisAnyToken(AffectCategoryType.OverrideSealed);// オーバーライドスキル封印状態
        public bool IsBreak => m_TokenCellList.GetisAnyToken(AffectCategoryType.Break);
        public bool IsShield => m_TokenCellList.GetisAnyToken(AffectCategoryType.Shield);


        public List<TokenCell> TokenList => m_TokenCellList;
        public CardStateEntity Entity => m_StateEntity;
        public StateHealth Health => m_StateHealth;

        private CardStateEntity m_StateEntity;
        private readonly StateHealth m_StateHealth=new StateHealth();
        private readonly Subject<AffectInfo>m_TakeBuffSubject=new Subject<AffectInfo>();
        private readonly List<TokenCell>m_TokenCellList=new List<TokenCell>();
        private readonly Subject<List<TokenCell>>m_UpdateTokenListSubject=new Subject<List<TokenCell>>();
        private readonly Dictionary<int, int> m_AttributeResisDic;


        public BattlerParamState()
        {
            m_AttributeResisDic = new Dictionary<int, int>()
            {
                //TODO : ここに属性耐性を記載する
            };
        }

        public void Setup(CardStateEntity entity,bool isTakeover=false)
        {
            m_StateEntity = entity;
            // TODO: 属性耐性記述

            m_StateHealth.SetMaxHealth(entity.HP, isTakeover);
        }

        public void Progress(AffectTurnConsumeType turnConsumeType)
        {
            this.UpdateProgress(turnConsumeType);
        }

        public void Dispose()
        {
            m_UpdateTokenListSubject.Dispose();
            m_TakeBuffSubject.Dispose();
            m_StateHealth.Dispose();
        }

        public void Reload()
        {
            m_UpdateTokenListSubject.OnNext(m_TokenCellList.ToList());
        }

        public void Reset()
        {
            m_StateHealth.Reset();
            RemoveToken(m_TokenCellList);
            m_UpdateTokenListSubject.OnNext(m_TokenCellList.ToList());
        }

        public void RemoveToken(IEnumerable<TokenCell>tokenCells)
        {
            var tokens = tokenCells.ToArray();
            for(int i=tokens.Length-1; i>=0; i--)
            {
                RemoveToken(tokens[i]);
            }
        }

        public void RemoveToken(TokenCell tokenCell)
        {
            m_TokenCellList.Remove(tokenCell);
            m_UpdateTokenListSubject.OnNext(m_TokenCellList.ToList());

            // 最大体力値に影響する場合
            if(tokenCell.command.categoryType.GetIsMaxHealthable())
            {
                m_StateHealth.SetMaxHealth(CurrentMaxHP);
            }
        }
        
        public int GetBuffValue(AffectCategoryType categoryType)
        {
            return this.GetValue(categoryType, m_StateHealth.NormalizedHealth);
        }

        public void TakeToken(AffectInfo info)
        {
            if(info.GetIsAffectable())
            {
                m_TokenCellList.Add(TokenCell.Create(info));
                m_UpdateTokenListSubject.OnNext(m_TokenCellList.ToList());

                // 最大体力値に影響する場合
                if(info.Command.categoryType.GetIsMaxHealthable())
                {
                    m_StateHealth.SetMaxHealth(CurrentMaxHP, info.SkillType == SkillType.Gimmic); // MEMO:Gimmic なら現在体力を上書き
                }
            }
            m_TakeBuffSubject.OnNext(info); // 効果反映を通知
        }

        public void TakeDispel(AffectInfo info)
        {
            if(info.GetIsAffectable())
            {
                if(m_TokenCellList.GetTryDiselableToken(out IEnumerable<TokenCell>cells))
                {
                    RemoveToken(cells.First());    // ※最初の1つのみ
                }
            }
            m_TakeBuffSubject.OnNext(info); // 効果反映を通知
        }

        public void TakeAbnormalRecobery(AffectInfo info)
        {
            if(info.GetIsAffectable())
            {
                if(m_TokenCellList.GetTryAbnormalConditionToken(out IEnumerable<TokenCell>cells))
                {
                    RemoveToken(cells);
                }
            }
            m_TakeBuffSubject.OnNext(info);   // 効果反映を通知
        }

        public int GetAttributeResisCurrentValue(AffectAttributeType attributeType)
        {
            return this.GetAttributeResistOriginalValue(attributeType)+
                this.GetAttributeResistBuffValue(attributeType)-
                this.GetAttributeResistDebuffValue(attributeType);
        }

        public int GetAttributeResistOriginalValue(AffectAttributeType attributeType)
        {
            try { return m_AttributeResisDic[(int)attributeType]; }
            catch { throw new KeyNotFoundException($"[{attributeType}] は Key として登録されていません。");  }
        }

        public int GetAttributeResistBuffValue(AffectAttributeType attributeType)
        {
            return m_TokenCellList
                .Where(token=>
                token.command.categoryType==AffectCategoryType.AttributeResistBuff&&
                token.command.attributeType==attributeType)
                .Sum(token=>token.command.affectValue);
        }

        public int GetAttributeResistDebuffValue(AffectAttributeType attributeType)
        {
            return m_TokenCellList
                .Where(token =>
                token.command.categoryType == AffectCategoryType.AttributeResistDebuff &&
                token.command.attributeType == attributeType)
                .Sum(token => token.command.affectValue);
        }
    }
}