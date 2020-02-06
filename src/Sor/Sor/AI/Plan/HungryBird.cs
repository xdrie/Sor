using System;
using Activ.GOAP;

namespace Sor.AI.Plan {
    /// <summary>
    /// Represents a hungry bird trying to get energy.
    /// </summary>
    public class HungryBird : Agent, Clonable<HungryBird> {
        public float cost { get; set; }

        public float satiety = 0;

        public int nearbyBeans = 0;
        public int nearbyTrees = 0;

        public const int BEAN_COST = 2;
        public const int TREE_VISIT_COST = 12;
        public const float BEANS_PER_TREE = 1.5f;
        public const float BEAN_ENERGY = 400f;

        Option[] opt; // Caching reduces array alloc overheads
        public Option[] Options() => opt ??= new Option[] {eatBean, visitTree};

        public HungryBird Allocate() => new HungryBird();

        public HungryBird Clone(HungryBird b) {
            b.cost = cost;
            b.nearbyBeans = nearbyBeans;
            b.nearbyTrees = nearbyTrees;
            b.satiety = satiety;
            return b;
        }

        public override bool Equals(object other) {
            if (other is HungryBird that) {
                return Math.Abs(this.satiety - that.satiety) < float.Epsilon;
            } else return false;
        }

        public override int GetHashCode() => satiety.GetHashCode();

        public Cost eatBean() {
            cost += BEAN_COST;
            if (nearbyBeans > 0) {
                nearbyBeans--;
                satiety += BEAN_ENERGY;
                return true;
            }

            return false;
        }

        public Cost visitTree() {
            // TODO: make costs depend on actual distance data
            cost += TREE_VISIT_COST;
            if (nearbyTrees > 0) {
                nearbyTrees--;
                satiety += BEAN_ENERGY * BEANS_PER_TREE;
                // TODO: use actual energy values of beans for satiety increases
                return true;
            }

            return false;
        }
    }
}