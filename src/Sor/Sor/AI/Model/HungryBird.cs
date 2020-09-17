using System;
using Activ.GOAP;
using Ducia;

namespace Sor.AI.Model {
    /// <summary>
    /// Represents a hungry bird trying to get energy.
    /// </summary>
    public class HungryBird : SmartActionPlanningModel<HungryBird> {
        public float satiety { get; set; } = 0;
        public int nearbyBeans { get; set; } = 0;
        public int nearbyTrees { get; set; } = 0;

        public const int BEAN_COST = 2;
        public const int TREE_VISIT_COST = 12;
        public const float BEANS_PER_TREE = 1.5f;
        public const float BEAN_ENERGY = 400f;


        protected override Option[] ActionOptions => new Option[] {eatBean, visitTree};

        public Cost eatBean() {
            if (nearbyBeans <= 0) return false;
            nearbyBeans--;
            satiety += BEAN_ENERGY;
            return BEAN_COST;
        }

        public Cost visitTree() {
            if (nearbyTrees <= 0) return false;
            // TODO: make costs depend on actual distance data
            nearbyTrees--;
            satiety += BEAN_ENERGY * BEANS_PER_TREE;
            // TODO: use actual energy values of beans for satiety increases
            return TREE_VISIT_COST;
        }
    }
}