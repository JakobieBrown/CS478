using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
namespace StateMachine.Core
{
    public interface IStateBehavior
        {
            void OnEnter();
            void OnUpdate();
            void OnFixedUpdate();
            void OnExit();
        }
    public abstract class AbstractState : IStateBehavior
        {
            public AbstractStateMachine machine;
            public AbstractState(AbstractStateMachine machine)
            {
                this.machine = machine;
            }
            public override string ToString()
            {
                return this.GetType().Name;
            }
            public abstract void OnEnter();
            public abstract void OnUpdate();
            public abstract void OnFixedUpdate();
            public abstract void OnExit();
        }
    public abstract class AbstractStateMachine
        {
            IStateFactory stateFactory;
            public AbstractState current;
            public AbstractStateMachine(IStateFactory stateFactory)
            {
                this.stateFactory = stateFactory;
                current = this.stateFactory.CreateState("default", this);
            }
            public void ChangeStates(string nextState)
            {
                current.OnExit();
                current = stateFactory.CreateState(nextState, this);
                current.OnEnter();
            }
            public void OnFixedUpdate()
            {
                current.OnFixedUpdate();
            }
            public void OnUpdate()
            {
                current.OnUpdate();
            }
        }
    public interface IStateFactory
        {
            AbstractState CreateState(string name, AbstractStateMachine machine);
        }
}


