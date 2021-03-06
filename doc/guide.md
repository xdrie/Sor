
# Sor // new player guide

## controls

+ `DPAD` or `IJKL` - move
+ `SHIFT` - boost
+ `[2]` - launch capsule
+ `SPACE` - fire weapon (while equipped)

### platform
+ `F11` - fullscreen

## tips

+ trees are spread all over the map, and larger trees grow energy beans; eat energy beans to replenish energy!
+ the colored pips underneath each bird represent their opinion of you. positive opinions are blue and green, neutral opinions are yellow, and negative opinions are orange and red.
+ feeding birds by launching capsules at them from a distance can help you make friends!
+ depending on their personalities, some birds may respond to your presence by eagerly trying to make friends by feeding you, and some may run away or even attack you

## debug inspection

debug inspectors requires having a debug build (the version number on the menu screen will contain `DEBUG`, and a message will be logged to the console)

a lot of the underlying AI in Sor can be inspected and viewed in action. the debug inspector displays live-updated mind information on a single bird.

hold `CTRL` to re-enable the mouse pointer, and while still holding it down, click near a bird to open its inspector. some text should appear on the top left.

tips to read this debug display:
+ `vision: <seen_birds> | <seen_items>`
+ `opinion: <opinion value> | <attitude>`
+ `ply: <personality vector, with A = anxiety, S = sociability>`
+ `emo: H:<happiness>, F:<fear>`
+ utility ai consideration scoring view, with a `>` symbol denoting the currently chosen consideration. the number next to the consideration name shows its current score to the ai
+ `--BOARD--`: lists various debug values. some have cryptic names nd are just for ensuring that things work correctly (this is a dev/debug build for me) and some can provide hidden useful information, such as the `nearby fear` message when birds get too close to each other.
