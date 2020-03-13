using Glint.Sprites;
using Microsoft.Xna.Framework;
using Nez;
using Sor.AI;
using Sor.Components.Things;
using Sor.Components.UI;

namespace Sor.Components.Units {
    public class Wing : GAnimatedSprite {
        public WingBody body;
        public Mind mind;
        public BoxCollider hitbox;
        public EnergyCore core;
        public Pips pips;

        public string name;
        public WingClass wingClass = WingClass.Wing;

        public Wing(Mind mind) : base(Core.Content.LoadTexture("Data/sprites/ship.png"), 64, 64) {
            this.mind = mind;
        }

        public override void Initialize() {
            base.Initialize();
            
            // set up ship sprite
            spriteRenderer.Color = NGame.context.assets.paletteWhite;

            Entity.AddComponent(mind); // add mind component
            body = Entity.AddComponent(new WingBody());
            hitbox = Entity.AddComponent(new BoxCollider(-6, -10, 12, 18) {Tag = Constants.Colliders.COLLIDER_SHIP});
            core = Entity.AddComponent(new EnergyCore(10_000));
            // add pips
            pips = Entity.AddComponent<Pips>();
            pips.spriteRenderer.LocalOffset = new Vector2(0, 14);

            // set body properties
            changeClass(WingClass.Wing); // change to default class

            // pips setup
            var pipNumber = 1 + Random.NextInt(5);
            pips.setPips(pipNumber, Pips.green);

            var ribbon = Entity.AddComponent(new TrailRibbon(40) {
                StartColor = new Color(175, 158, 180, 255),
                EndColor = new Color(175, 158, 180, 50),
                RibbonRadius = 8
            });
            ribbon.StopEmitting();
            ribbon.Enabled = false;

            // var trail = Entity.AddComponent(new SpriteTrail(animator) {
            //     InitialColor = new Color(200, 200, 200),
            //     MinDistanceBetweenInstances = 40,
            //     MaxSpriteInstances = 10,
            //     FadeDelay = 0.5f,
            // });

            if (name == null && Entity.Name != null) {
                name = Entity.Name;
            }
        }

        public enum WingClass {
            Wing = 0,
            Predator = 1,
            Beak = 2,
        }

        public void changeClass(WingClass newClass) {
            this.wingClass = newClass;
            // set baseline properties
            // properly revert all changes, including transform positions and scales
            var scale = 1f;
            // TODO: other classes are still very experimental!!
            switch (newClass) {
                case WingClass.Wing:
                    scale = 1f;

                    // this should always be the defaults
                    body.mass = Constants.Physics.DEF_MASS;

                    body.turnPower = Constants.Physics.DEF_TURN_POWER;
                    body.thrustPower = Constants.Physics.DEF_THRUST_POWER;
                    body.topSpeed = Constants.Physics.DEF_TOP_SPEED;
                    body.boostTopSpeed = Constants.Physics.DEF_BOOST_TOP_SPEED;
                    body.recalculateValues();

                    break;
                case WingClass.Predator: {
                    scale = 2f;

                    body.mass = Constants.Physics.BIG_MASS;
                    
                    body.turnPower = Constants.Physics.BIG_TURN_POWER;
                    body.thrustPower = Constants.Physics.BIG_THRUST_POWER;
                    body.boostTopSpeed = Constants.Physics.BIG_BOOST_TOP_SPEED;
                    
                    body.recalculateValues();
                    break;
                }
                case WingClass.Beak: {
                    scale = 0.5f;

                    body.mass = Constants.Physics.SML_MASS;
                    
                    body.turnPower = Constants.Physics.SML_TURN_POWER;
                    body.thrustPower = Constants.Physics.SML_THRUST_POWER;
                    body.boostTopSpeed = Constants.Physics.SML_BOOST_TOP_SPEED;
                    
                    body.recalculateValues();
                    break;
                }
            }

            Transform.SetLocalScale(scale);
            pips.spriteRenderer.LocalOffset = pips.spriteRenderer.LocalOffset * scale;
        }
    }
}