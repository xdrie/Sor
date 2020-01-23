
# changelog

## wk2
+ energy
  + every thing in the environment runs on energy
+ capsules, or "fruits", represent containers of energy
  + pick up capsules by going near then
  + fire capsules containing energy
+ add basic hud
  + hud displays energy
  + pip display on other birds based on opinion
+ trees
  + trees are specified in the map
  + they grow with time and produce capsules
+ ai
  + personality engine adaptation
  + split ai into multiple mind systems that have their own update rates
  + propagate signals when events occur
  + store opinion values
  + receiving capsules increases opinion
+ additional map features
  + convert tilemap to graph representation
  + scan edges of rooms to detect doors
  + boost regions
+ internally reduce mixture of colliders and triggers
