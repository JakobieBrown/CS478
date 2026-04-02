using UnityEngine;
using StateMachine.Core;
namespace StateMachine.Player
{
    public class PlayerStateMachine : AbstractStateMachine
    {
        public PlayerController Controller { get; private set; }
        public PlayerStateMachine(IStateFactory stateFactory, PlayerController controller) : base(stateFactory)
        {
            Controller = controller;
        }

        public void Move(Vector2 inputVector)
        {
            ((AbstractPlayerState)current).Move(inputVector);
        }

        public void Jump(InputPayload input)
        {
            ((AbstractPlayerState)current).Jump(input);
        }
    }
    public abstract class AbstractPlayerState : AbstractState
    {
        protected PlayerController controller;
        protected AbstractPlayerState(AbstractStateMachine machine, PlayerController controller) : base(machine)
        {
            this.controller = controller;

        }
        public abstract void Move(Vector2 inputVector);
        public abstract void Jump(InputPayload input);

    }
    public class PlayerStateFactory : IStateFactory
    {
        public PlayerController controller;

        public PlayerStateFactory(PlayerController controller)
        {
            this.controller = controller;
        }

        public AbstractState CreateState(string name, AbstractStateMachine machine)
        {
            switch (name)
            {
                case "PlayerIdle":
                    return new PlayerIdle(machine, controller);
                case "PlayerRun":
                    return new PlayerRun(machine, controller);
                case "PlayerJumpLiftOff":
                    return new PlayerJumpLiftOff(machine, controller);
                case "PlayerJumpRise":
                    return new PlayerJumpRise(machine, controller);
                case "PlayerJumpFall":
                    return new PlayerJumpFall(machine, controller);
                case "PlayerJumpLand":
                    return new PlayerJumpLand(machine, controller);
                default:
                    return new PlayerDefault(machine, controller);
                }
            }
        }
    public class PlayerDefault : AbstractPlayerState
    {
        public PlayerDefault(AbstractStateMachine machine, PlayerController controller) : base(machine, controller)
        {
        }

        public override void Jump(InputPayload input)
        {

        }

        public override void Move(Vector2 inputVector)
        {

        }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

        public override void OnFixedUpdate()
        {

        }
        public override void OnUpdate()
        {
            if (controller.onGround)
            {
                if (controller.input.Move.x == 0)
                {
                    machine.ChangeStates("PlayerIdle");
                }
                else
                {
                    machine.ChangeStates("PlayerRun");
                }
            }
            else
            {
                if (controller.rb.linearVelocityY < 0)
                    machine.ChangeStates("PlayerJumpFall");
                else
                    machine.ChangeStates("PlayerJumpRise");
            }
        }
    }
    public class PlayerIdle : AbstractPlayerState
    {
        public PlayerIdle(AbstractStateMachine machine, PlayerController controller) : base(machine, controller)
        {
        }

        public override void Jump(InputPayload input)
        {

        }

        public override void Move(Vector2 inputVector)
        {
            controller.rb.AddForce(controller.DragForce());
        }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {

        }

        public override void OnFixedUpdate()
        {
                
        }

        public override void OnUpdate()
        {
            if (Mathf.Abs(controller.input.Move.x) > 0)
            {
                machine.ChangeStates("PlayerRun");
            }
            if (controller.input.jumpTriggered)
            {
                machine.ChangeStates("PlayerJumpLiftOff");
            }
            Debug.Log(ToString());
        }
    }
    public class PlayerRun : AbstractPlayerState
    {
        public PlayerRun(AbstractStateMachine machine, PlayerController controller) : base(machine, controller)
        {
        }
        public override void OnEnter()
        {
        }
        public override void OnExit()
        {
        }
        public override void OnFixedUpdate()
        {
        }
        public override void OnUpdate()
        {
            if (controller.input.Move.x == 0)
            {
                machine.ChangeStates("PlayerIdle");
            }
            if (controller.input.jumpTriggered)
                machine.ChangeStates("PlayerJumpLiftOff");
            Debug.Log(ToString());
        }

        public override void Move(Vector2 inputVector)
        {
            int horizontalInput = inputVector.x > 0 ? 1 : inputVector.x < 0 ? -1 : 0; // 1 if x > 0, -1 if x < 0, else 0
            controller.rb.AddForce(new Vector2(horizontalInput * controller.rb.mass * controller.StatBlock.Acceleration(), 0));
            controller.rb.AddForce(controller.DragForce());
        }

        public override void Jump(InputPayload input)
        {

        }
    }
    public class PlayerJumpLiftOff : AbstractPlayerState
    {
        public PlayerJumpLiftOff(AbstractStateMachine machine, PlayerController controller) : base(machine, controller)
        {

        }

        public override void Jump(InputPayload input)
        {

        }

        public override void Move(Vector2 inputVector)
        {
            int horizontalInput = inputVector.x > 0 ? 1 : inputVector.x < 0 ? -1 : 0; // 1 if x > 0, -1 if x < 0, else 0
            controller.rb.linearVelocityY = controller.StatBlock.JumpForce();
            controller.rb.AddForce(new Vector2(horizontalInput * controller.rb.mass * controller.StatBlock.Acceleration(), 0));
            controller.rb.AddForce(controller.DragForce());
        }

        public override void OnEnter()
        {
            controller.rb.AddForce(new Vector2(0, 1) * controller.StatBlock.JumpForce());
            machine.ChangeStates("PlayerJumpRise");
            controller.input.jumpTriggered = false;
        }

        public override void OnExit()
        {
        }

        public override void OnFixedUpdate()
        {
        }

        public override void OnUpdate()
        {
        }
    }
    public class PlayerJumpRise : AbstractPlayerState
    {
        public PlayerJumpRise(AbstractStateMachine machine, PlayerController controller) : base(machine, controller)
        {
        }

        public override void Jump(InputPayload input)
        {
        }

        public override void Move(Vector2 inputVector)
        {
            int horizontalInput = inputVector.x > 0 ? 1 : inputVector.x < 0 ? -1 : 0; // 1 if x > 0, -1 if x < 0, else 0
            controller.rb.AddForce(new Vector2(horizontalInput * controller.rb.mass * controller.StatBlock.Acceleration(), 0));
            controller.rb.AddForce(controller.DragForce());
        }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }

        public override void OnFixedUpdate()
        {
        }

        public override void OnUpdate()
        {
            Debug.Log(ToString());
            if (controller.rb.linearVelocityY <= 0)
                machine.ChangeStates("PlayerJumpFall");
        }
    }
    public class PlayerJumpFall : AbstractPlayerState
    {
        public PlayerJumpFall(AbstractStateMachine machine, PlayerController controller) : base(machine, controller)
        {
        }

        public override void Jump(InputPayload input)
        {

        }

        public override void Move(Vector2 inputVector)
        {
            int horizontalInput = inputVector.x > 0 ? 1 : inputVector.x < 0 ? -1 : 0; // 1 if x > 0, -1 if x < 0, else 0
            controller.rb.AddForce(new Vector2(horizontalInput * controller.rb.mass * controller.StatBlock.Acceleration(), 0));
            controller.rb.AddForce(controller.DragForce());
        }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

        public override void OnFixedUpdate()
        {

        }

        public override void OnUpdate()
        {
            Debug.Log(ToString());
            if (controller.onGround)
                machine.ChangeStates("PlayerJumpLand");
        }
    }
    public class PlayerJumpLand : AbstractPlayerState
    {
        public PlayerJumpLand(AbstractStateMachine machine, PlayerController controller) : base(machine, controller)
        {
        }

        public override void Jump(InputPayload input)
        {

        }

        public override void Move(Vector2 inputVector)
        {
            int horizontalInput = inputVector.x > 0 ? 1 : inputVector.x < 0 ? -1 : 0; // 1 if x > 0, -1 if x < 0, else 0
            controller.rb.AddForce(new Vector2(horizontalInput * controller.rb.mass * controller.StatBlock.Acceleration(), 0));
            controller.rb.AddForce(controller.DragForce());
        }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

        public override void OnFixedUpdate()
        {

        }

        public override void OnUpdate()
        {
            Debug.Log(ToString());
            if (controller.input.Move.x == 0)
                machine.ChangeStates("PlayerIdle");
            else
                machine.ChangeStates("PlayerRun");
        }
    }
}

