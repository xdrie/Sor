using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tweens;
using Sor.Components.UI;

namespace Sor.Scenes {
    public class IntroScene : BaseGameScene {
#if DEBUG
        private const float intro_length = 0f;
#else
        private const float intro_length = 1f;
#endif

        public override void Initialize() {
            base.Initialize();

            // Hide cursor
            Core.Instance.IsMouseVisible = false;

            ClearColor = new Color(10);
            
            gameContext.loadContent();

            var cover = CreateEntity("cover", Resolution.ToVector2() / 2);
            var logo = cover.AddComponent<LogoAnimation>();
            var targetWidth = Resolution.X * 0.7f;
            
            var baseScale = new Vector2(4f);
            cover.SetLocalScale(baseScale);

            logo.animator.Color = Color.Transparent;
            logo.animator.TweenColorTo(Color.White, 0.4f)
                .SetEaseType(EaseType.QuadIn)
                .SetDelay(0.7f)
                .SetCompletionHandler(t => {
                    logo.animator.Transform.TweenLocalScaleTo(baseScale * 1.2f, 0.4f)
                        .SetEaseType(EaseType.CubicOut)
                        .SetDelay(intro_length)
                        .Start();
                    logo.animator.TweenColorTo(Color.Transparent, 0.4f)
                        .SetEaseType(EaseType.CubicOut)
                        .SetDelay(intro_length)
                        .SetCompletionHandler(async _ => { await loadGame(); }).Start();
                })
                .Start();
        }

        private async Task loadGame() {
            ClearColor = new Color(49, 13, 62);
            
            var ui = CreateEntity("ui");
            
            var versionText = new TextComponent(gameContext.assets.font, "run", new Vector2(Resolution.X / 2f, 240),
                Color.LightGray);
            ui.AddComponent(versionText);
            versionText.SetLocalOffset(new Vector2(-versionText.Width - 4f, 0));

            // - loading logic
            await Task.Delay(0); // await Task.Delay(1000);
            // await GameBootstrapper.bootAsync(text => { versionText.Text = text; });

            // when finished
            transitionScene<MenuScene>(0.2f);
        }
    }
}