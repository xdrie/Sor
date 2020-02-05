using System.Collections.Generic;
using Nez.AI.GOAP;

namespace Sor.AI.Plan {
    public class ObjectivePlanner {
        private ActionPlanner planner;

        private const string k_Hungry = "hungry";

        public ObjectivePlanner() {
            planner = new ActionPlanner();
            
            // TODO: set up better actions
            var eat = new Action("eat");
            eat.SetPrecondition(k_Hungry, true);
            eat.SetPostcondition(k_Hungry, false);
            planner.AddAction(eat);
        }

        WorldState getWorldState() {
            var ws = planner.CreateWorldState();
            ws.Set(k_Hungry, true);

            return ws;
        }

        WorldState getGoalState() {
            var ws = planner.CreateWorldState();
            ws.Set(k_Hungry, false);

            return ws;
        }

        Stack<Action> plan() {
            var plans = planner.Plan(getWorldState(), getGoalState());
            return plans;
        }
    }
}