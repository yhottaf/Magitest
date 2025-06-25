using System;
using UniRx;
using UnityEngine;

namespace fantec.Battle.Model
{
    public interface IBattlerParamTransform : IDisposable, IResetable
    {
        IObservable<Vector3> OnMoveObservable { get; }

        IObservable<Unit> OnMoveCompletedObservable { get; }

        void Move(Vector3 targetPosition);

        void MoveCompleted();

        public bool SuppressMoveCompleteNotify { get; set; }
        Vector3 TargetPosition { get; }
    }

    public class BattlerParamTransform : IBattlerParamTransform
    {
        public IObservable<Vector3> OnMoveObservable => m_MoveSubject;
        public IObservable<Unit> OnMoveCompletedObservable => m_OnCompleted;
        public bool SuppressMoveCompleteNotify { get; set; }
        public Vector3 TargetPosition { get; private set; }

        private readonly Subject<Vector3> m_MoveSubject = new Subject<Vector3>();
        private readonly Subject<Unit> m_OnCompleted = new Subject<Unit>();
        public void Setup(Vector3 startPosition)
        {
            TargetPosition = startPosition;
        }

        public void Dispose()
        {
            m_MoveSubject.Dispose();
            m_OnCompleted.Dispose();
        }

        public void Reset()
        {

        }

        public void Move(Vector3 targetPosition)
        {
            m_MoveSubject.OnNext(targetPosition);
            TargetPosition=targetPosition;
        }

        public void MoveCompleted()
        {
            if (!SuppressMoveCompleteNotify)
            {
                m_OnCompleted.OnNext(Unit.Default);
            }
        }
    }
}