# Tabletop RPG Randomizer

## What?

Inspired by retro game randomizers (Super Metroid randomizers, A Link to the Past, Final Fantasy IV, etc) I had the idea - what if a tabletop RPG (think dungeons and dragons) was randomized?

Thus, the tabletop RPG rando was born. Here's the sample idea I had when creating this:


Imagine you start out as 3 adventurers in a bright pink tavern. The barkeep is a dragon. It gives you a quest to go retrieve the wooden potato from the cave of short wrinkles. You travel to the cave and come across a team of librarians who attack. After combat, you loot the librarians. You find a golden spoon, 7 Excaliburs, a quiver of +1 Arrows, and 140,963 gold. You arrive at the cave of short wrinkles. There is a solid green ice door covering the cave. You enter the cave to the interior of the castle. Sitting in Dracula's chair is Mario. Mario dies because he starts the battle with the instant death status effect. Behind the chair is a chest with the Wooden Potato and a reward of a bar of iron, the Crash Bomber, and 7 gold. Also two of you are dressed in a terrifying combinations of colors but one of you looks super cool in randomized colors.

## What's Randomized? And How?

The randomizer uses a json file with a massive list of names, nouns, adjectives, magical effects, etc to randomize from. The system pulls weighted values from this list when you ask it to. the GM will have to pull from those options to create the world, for example as you enter a town the GM may click the building button to generate several businesses. When entering, they can then create an armed person as a guard, a shopkeeper, and a list of sold items. 

* Main Quests
* Big Bad Evil Guy
* Side Quests
* World setting
* Town names and government styles
* Business names, types, colors
* Nouns
* Adjectives
* Colors
* Moods
* Personalities
* Weather
* Skills and Skill lists
* Skill Specialties
* Character Stats
* Character Equipment
* Food/Drink
* Furniture
* Materials
* Damage type/Elements
* Character and Monster Races
* Sizes
* Character Age
* Shop Lists
* Weapons
* Armor
* Objects
* Spell Scrolls
* Potions
* Value of items
* Gold rewards
* Friendly and Enemy Encounters
* Terrain
* Points of Interest
* Spells

## What System Does This Use?

Currently, this is set up to use NWOD, but under a basic ruleset. I use:
Major Stats
Skills, but no skill penalties
Specialties
Initiative
Willpower
Health, but just as hit points

How does it work? You start off with random characters for your players. Randomize the Skill List as well so they have skills. Challenge rolls involve a major stat + a skill, and they roll that many d10 dice (for example dexterity+stealth) and count the number of 8s, 9s, and 0s rolled. Any 0 rolled causes a re-roll of that die, giving further chance for a high roll. Attack rolls are Strength or Dexterity + applicable skill (usually weaponry) + dice modifier of the weapon. The target then rolls dexterity + armor (in this system I use the number of pieces of armor the target has. If the attacker rolled higher, the damage dealt is the difference between the number of successful dice. otherwise it's a miss. Spells and magical effects need to be adjusted on the fly because they're all from different universes with different systems. 

Why NWOD? Its simple enough to use by anyone and is pretty flexible. The code in the randomizer is quick garbage spaghetti code i never planned to release to the public, but I may clean it up and add an interface for 5e and other systems. 

Here is a sample character sheet, trimmed down for the rando:

![character sheet](https://github.com/Komarulon/TabletopRPGRandomizer/blob/master/NWOD%20Basic%20Character%20Sheet.png)

![screenshot](https://i.imgur.com/qDGtYQX.png)

