using System;
using Activ.GOAP;
using Ducia;

namespace Sor.AI.Model {
    /// <summary>
    /// Represents a hungry bird trying to get energy.
    /// </summary>
    public class HungryBird : ActionPlanningModel<HungryBird> {
        public float satiety = 0;

        public int nearbyBeans = 0;
        public int nearbyTrees = 0;

        public const int BEAN_COST = 2;
        public const int TREE_VISIT_COST = 12;
        public const float BEANS_PER_TREE = 1.5f;
        public const float BEAN_ENERGY = 400f;


        public override Option[] ActionOptions => new Option[] {eatBean, visitTree};

        #region Type Overrides

        public override HungryBird Clone(HungryBird b) {
            b.cost = cost;

            b.nearbyBeans = nearbyBeans;
            b.nearbyTrees = nearbyTrees;
            b.satiety = satiety;
            return b;
        }

        public override bool Equals(object other) {
            if (other is HungryBird that) {
                return Math.Abs(this.satiety - that.satiety) < float.Epsilon;
            }
            else return false;
        }

        public override int GetHashCode() => satiety.GetHashCode();

        #endregion

        public Cost eatBean() {
            if (nearbyBeans <= 0) return false;
            cost += BEAN_COST;
            nearbyBeans--;
            satiety += BEAN_ENERGY;
            return true;
        }

        public Cost visitTree() {
            if (nearbyTrees <= 0) return false;
            // TODO: make costs depend on actual distance data
            cost += TREE_VISIT_COST;
            nearbyTrees--;
            satiety += BEAN_ENERGY * BEANS_PER_TREE;
            // TODO: use actual energy values of beans for satiety increases
            return true;
        }
    }
}