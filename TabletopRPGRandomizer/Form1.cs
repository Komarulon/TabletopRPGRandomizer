using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RicherTextBox;
using System.Windows.Forms;
using static TabletopRPGRandomizer.Form1.Encounter;

namespace TabletopRPGRandomizer
{
    public partial class Form1 : Form
    {
        public class MasterJson
        {
            
            public string[] Race { get; set; }
            public JsonColor[] Colors { get; set; }
            public string[] SimpleColors { get; set; }
            public class JsonColor
            {
                public string ColorName { get; set; }
                public string HexCode { get; set; }
            }
            public string[] Occupations { get; set; }
            public string[] SpellNames { get; set; }

            public string[] Elements { get; set; }
            public string[] NonMagicalElements { get; set; }
            public string[] StatusEffects { get; set; }

            public string[] WeaponNames { get; set; }
            public string[] WeaponTypes { get; set; }

            public string[] Classes { get; set; }
            public string[] WorldNames { get; set; }

            public string[] BusinessNames { get; set; }
            public string[] BuildingTypes { get; set; }


            public string[] Adjectives { get; set; }
            public string[] Objects { get; set; }

            public string[] MagicalItemEffects { get; set; }
            public string[] GrogSubstantialWhimsyEffects { get; set; }
            public string[] Governments { get; set; }
            public string[] TownNames { get; set; }
            
            public string[] SizeAttributes { get; set; }

            public string[] NWODSpecialties { get; set; }
            public string[] NWODSkills { get; set; }

            public string[] ArmorTypes { get; set; }

            public string[] AccessoryTypes { get; set; }

            public string[] Drinks { get; set; }
            public string[] Foods { get; set; }

            public string[] Materials { get; set; }

            public string [] PotionTypes { get; set; }

            public string[] SittingFurniture { get; set; }
            public string[] TableFurniture { get; set; }

            public string[] Personalities { get; set; }
            public string[] Moods { get; set; }

            public string[] TravelLocations { get; set; }
            public string[] EncounterLocations { get; set; }

            public string[] MonsterRaces { get; set; }

            public NameContainer Names { get; set; }
            public class NameContainer {
                public string[] Last { get; set; }
                public FirstNamesContainer First { get; set; }
                public class FirstNamesContainer
                {
                    public string[] Female { get; set; }
                    public string[] Male { get; set; }
                }
            }

            public Encounter[] encounters { get; set; }

            public class Encounter
            {
                public bool? isFriendly { get; set; }
                public string description { get; set; }
                public List<Creature> creatures { get; set; }
                public class Creature
                {
                    public string count { get; set; }
                    public string type { get; set; }
                    public string action { get; set; }
                    public bool? isFriendly { get; set; }
                }
            }
        }

        static MasterJson masterFile;

        public Form1()
        {
            using (StreamReader r = new StreamReader("Data\\Adjectives.json"))
            {
                string json = r.ReadToEnd();
                masterFile = JsonConvert.DeserializeObject<MasterJson>(json);
            }

            foreach (var enc in masterFile.encounters)
            {
                foreach (var c in enc.creatures)
                {
                    var isFriendlyCreature = c.isFriendly ?? (percentChance(50));
                    Person creature;
                    if (c.type == "humanoid")
                    {
                        creature = GeneratePerson();
                    }
                    else if (c.type == "monster")
                    {
                        creature = GenerateMonster(true);
                    }
                    else if (c.type == "any")
                    {
                        creature = GenerateMonster();
                    } else
                    {
                        throw new Exception("INVALID CREATURE TYPE: " + c.type + " (need 'humanoid', 'monster', or 'any')");
                    }

                    var split = c.count.Split('-');
                    if (split.Length == 1)
                    {
                        if (!int.TryParse(c.count, out int p))
                        {
                            throw new Exception("INVALID CREATURE COUNT: " + c.count + " (need 'n-m' like '3-5', or just a number, like '2')");
                        }
                    }
                    else if( split.Length != 2)
                    {
                        throw new Exception("INVALID CREATURE COUNT: " + c.count + " (need 'n-m' like '3-5', or just a number, like '2')");
                    }
                    else
                    {
                        if (!int.TryParse(split[0], out int p))
                        {
                            throw new Exception("INVALID CREATURE COUNT: " + c.count + " (need 'n-m' like '3-5', or just a number, like '2')");
                        }
                        if (!int.TryParse(split[1], out int p2))
                        {
                            throw new Exception("INVALID CREATURE COUNT: " + c.count + " (need 'n-m' like '3-5', or just a number, like '2')");
                        }
                    }
                }
            }

            

            InitializeComponent();

            foreach (var b in masterFile.BuildingTypes.OrderBy(x => x))
            {
                this.ShopType.Items.Add(b);
            }
            this.ShopType.SelectedIndex = 0;

            foreach (Item.ItemType type in Enum.GetValues(typeof(Item.ItemType)))
            {
                this.shopListTypeList.Items.Add(type.ToString());
            }
            this.shopListTypeList.SelectedIndex = 0;
        }

        private List<Spell> spells = new List<Spell>();
        private void button15_Click(object sender, EventArgs e)
        {
            var s = GenerateSpell();
            // Generate spell

            AppendText(Environment.NewLine);
            AppendText("SPELL: " + s.spellName + " (" + s.mpCost + " MP) | " + s.spellElement + " | ");
            AppendColor(s.color);
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private List<Item> items = new List<Item>();
        private void button16_Click(object sender, EventArgs e)
        {
            // generate new weapon
            var weapon = GenerateRandomLoot(Item.ItemType.Weapon);
            AppendText(Environment.NewLine);
            weapon.AppendMyString(richTextBox1.RichTextBox);
            AppendText(Environment.NewLine);
            scrollToBottom();

        }

        private void button8_Click(object sender, EventArgs e)
        {
            var BBEG = generateBBEG();

            // Generate Main Quest
            var mainQuest = processBuilderString(
$@"You must stop the 
#{masterFile.Adjectives.Random()} Monster
#King
#Thief
#{masterFile.Objects.Random()} Thief
#{masterFile.Objects.Random()} Burglar
#{masterFile.Adjectives.Random()} Cow
#Cultists
#Pixies
#Dragon
#{BBEG.Race} {BBEG.Occupation}
#{BBEG.Race} {BBEG.Occupation}
#{BBEG.Race} {BBEG.Occupation}
 from 
#destroying the town
#torturing someone
#holding someone prisoner
#gaining an unlikely ally
#having a falling out with an old friend
#betraying his superiors
#stealing
#hatching a crazy plan
#finding the {masterFile.Race.Random()} {masterFile.Occupations.Random()}
#finding the {masterFile.Adjectives.Random()} {masterFile.Objects.Random()}
#bestowing a curse
 before they 
#discover the location of the dungeon
#find the artifact
#realise it's all an illusion
#discover how easy being evil is
#become addicted to causing pain
#lose their temper
#learn that they had been lied to
 or else 
#they will destroy the world
#we all will die
#there'll be no more vegetable contests
#I wont have any money to pay you
#the entire town will starve
#war will break out
#the {masterFile.Race.Random()}-{masterFile.Race.Random()} Alliance will fall
#a civil war between the {masterFile.Race.Random()}s and the {masterFile.Race.Random()}s will erupt
#we will see a string of murders
#our gods will no longer protect us
#a new power will rise
#a legion of {masterFile.Race.Random()}s will attack
. However, if you 
#travel to the
#avoid
#seek
#make an ally in
#meet the stranger in
#seek advice from your old mentor
#retrieve the magic {masterFile.Objects.Random()} from
#destroy the magic {masterFile.Objects.Random()} at
 the 
#meddling kids
#{masterFile.Adjectives.Random()} forest
#{masterFile.Adjectives.Random()} river
#{masterFile.Adjectives.Random()} fortress
#{masterFile.Adjectives.Random()} king
#{masterFile.Adjectives.Random()} volcano
#{masterFile.Adjectives.Random()} villain's lair
 you will find 
#a way to increase your power
#knowledge
#valuable insight
#a way to steel your defences
#that you can increase your travelling speed
#a shortcut
#out the foe's weakness
#out who the foe's allies are
 which will 
#help you save the day
#prepare you for the pain
#make you immune to the magic
#give you a plan B
#consolidate any losses
#reward you greatly should you succeed
#etch your name in history
#give you an edge
#give you some time to spare
#allow you to plan your arrival
.");
            AppendTextStatic(this.richTextBox2.RichTextBox, mainQuest + Environment.NewLine + Environment.NewLine);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string quest;
            // generate side quest
            if (percentChance(50))
            {
                quest = processBuilderString(
    $@"You've gotta help me - you see, this Cow, from 
#my Farm
#my Neighbours Farm
#the Wilderness
#town
#the Next Village
#hell
#the Abyss
#my Brother's Farm
 is undertaking acts of 
#arson
#torture
#kidnapping
#corruption in Politics
#treason
#evil Rituals
#thievery
#scheming
#raiding and Pillaging
#murder
 at 
#my Place
#the Centre Of Town
#the Local Tavern
#the Local Church
#the Neighbour's Place
#the Edge of the Forest
#the Cemetary/Graveyard
#a nearby village
 . We tried stopping this cow, but 
#it outsmarts us at every turn
#whenever we think we've cornered it, it escapes
#it won the court case, and has legal immunity
#it kills anyone who question it
#it's protected by an impressive amount of allies
#anyone who stands up to it has a terrible curse bestowed upon them
#it just keeps coming back
#all of the suspects we capture are just regular cows
 . Please, you must help us! I'm afraid it might 
#breed, and over time generate a race of super cows
#scheme with the other cows in the world and incite a rebellion
#claim a right to the throne
#run in the next election
#eat me
#create a cow mafia that will revolutionise the world
 (The Cow is actually a
#messiah Cow, whos existance is the omen of the end times
#druid, with their own selfish agenda
#commoner from the town who was cursed to be a cow by a Hag
#new, dangerous breed of Lycanthrope - A Werecow
#an ancient dragon who has been true-polymorphed by a rival
#two Goblins in a 'cow' costume
#an illusionary cow created by an Aboleth or similar evil creature
#an angel, sent to perform a God's deed in secret.
#an alien lifeform from a plane far, far away, who is just trying to fit in
#a regular, up-to-no-good cow
).");
            } else
            {
                quest = processBuilderString(
$@"(Rescue Noble) The noble’s gender is 
#male
#female
 The party 
#finds a hastily scrawled note that used finger tips as the pen and blood as the ink
#experiences a vision of the keep and noble has been reoccurring in your dreams
#is shown a missing person notice posted by the noble’s family
#stumbles upon the keep by chance while out adventuring
#overhears a pair of knights recounting their failure in retreat
#is asked to investigate the keep(er) due to suspicious activity
#is stopped by woodland creatures who all begin chattering frantically about the situation
#hears gossip in the tavern about the noble’s whereabouts
#is told about the scenario by a traveling oracle
#is captured and taken by the keeper’s henchmen
 The noble is being kept in 
#a large magnificent castle on the mountainside
#a cave burrowed into the side of a hill
#a pocket dimension that opens only on the full moon
#a sunken castle with an enchantment that permits water breathing inside of it
#the brig of a ship anchored just off the coast
#an abandoned, overgrown temple in the woods
#a travelling caravan that is difficult to track down
#a crypt hidden beneath a graveyard
#a magical prison inside an amulet worn by the keeper
#a hidden kingdom under the ground
 The noble is being held by 
#an old witch with imps for henchmen
#a vampire lord with thrall for henchmen
#a beholder with nothic henchmen
#a bandit lord with bandit henchmen
#a green dragon with half-dragon henchmen
#a powerful wizard with golem henchmen
#a medusa with living statues as henchmen
#a lich and his skeleton henchmen
#a satyr prince and his satyr henchmen
#an orc warchief and his orc henchmen
 The keeper wants 
#{randomMoney(500000)} gold in exchange
#the noble as a slave
#to make a blood sacrifice to their god
#to use the noble as bait to lure in more prey
#to sell them to the highest bidder overseas
#to spark war between nations
#the noble as a suitor for their son/daughter
#to fill the void in their life that was caused by a loved one dying
#to keep the noble even though taking them happened by accident
#to fulfill tradition, it’s nothing personal
 The noble in question is 
#beautiful/handsome beyond expression
#ugly as sin
#quite overweight
#much taller than you expected
#much shorted than you expected
#very thin and looking under-fed
#unusually muscular
#missing all of their hair
#much older compared to their description
#completely naked and [roll again]
 The real problem is 
#the noble wants to stay
#the noble insists their family doesn’t really want them to come back
#the noble has been placed into a catatonic dream state
#the noble dies on the way back home
#the noble falls in love with a party member and doesn’t want to leave them
#the noble is an illusion and is being kept somewhere else [Roll another location]
#there are six nobles who all look similar to the description
#the noble is the opposite gender than what you were told
#the noble attacks the party in a frenzy and won’t stop until calmed down or knocked unconscious
#the noble gets captured again by someone else on the way back [Start over at The damsel is being kept in]
 Not until much later the party discovers that 
#it was the wrong noble. Oops
#the noble is a doppleganger looking to infiltrate the family
#the family did it as a publicity stunt to gain attention and sympathy
#the noble has been getting captured on purpose to escape their family
#the family isn’t offering any payment for the noble’s return
#someone else is offering more gold for the noble than the family
#all members that family the noble belongs to have since died
#the noble was likely happier with the keeper
#the noble wants to be freed to go where they choose rather than back home
#the noble is actually a powerful wizard who invented the whole scenario to see if there is anyone of valor still left in the world
.");

            }
            AppendTextStatic(this.richTextBox2.RichTextBox, quest + Environment.NewLine + Environment.NewLine);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            generateBBEG();
        }

        private Person generateBBEG()
        {
            var BBEG = GeneratePerson();
            // generate armor, weapon, accessory, gold, and like 3 objects on person

            BBEG.KnownSpells = new List<Spell>();
            BBEG.OnHand = new List<Item>();
            BBEG.money = randomMoney(10000) + randomMoney(10000) + randomMoney(10000) + randomMoney(10000) + randomMoney(10000);

            BBEG.maxHealth += randomNumber(1, 5) + randomNumber(1, 5) + randomNumber(1, 5);
            BBEG.maxMP += randomNumber(1, 5) + randomNumber(1, 5);

            while (percentChance(70))
            {
                BBEG.KnownSpells.Add(GenerateSpell());
            }

            BBEG.OnHand.Add(GenerateRandomLoot(Item.ItemType.Weapon));
            BBEG.OnHand.Add(GenerateRandomLoot(Item.ItemType.Weapon));
            BBEG.OnHand.Add(GenerateRandomLoot(Item.ItemType.Armor));
            BBEG.OnHand.Add(GenerateRandomLoot(Item.ItemType.Accessory));
            BBEG.OnHand.Add(GenerateRandomLoot(Item.ItemType.Object));
            BBEG.OnHand.Add(GenerateRandomLoot(Item.ItemType.Object));
            BBEG.OnHand.Add(GenerateRandomLoot(Item.ItemType.Potion));
            BBEG.OnHand.Add(GenerateRandomLoot(Item.ItemType.Potion));

            while (percentChance(60))
            {
                BBEG.OnHand.Add(GenerateRandomLoot(null));
            }

            BBEG.AppendMyString(this.richTextBox2.RichTextBox, "BBEG");
            AppendTextStatic(this.richTextBox2.RichTextBox, Environment.NewLine + Environment.NewLine);

            return BBEG;
        }

        public class Spell
        {
            public string spellName { get; set; }
            public string spellElement { get; set; }
            public MasterJson.JsonColor color { get; set; }
            public int mpCost { get; set; }
        }

        public class Item
        {
            public enum ItemType
            {
                Scroll,
                Weapon,
                Object,
                Armor,
                Accessory,
                Potion,
            }
            public ItemType type { get; set; }
            public int buyCost { get; set; }
            public int sellCost { get; set; }
            public string name { get; set; }
            public string fancyName { get; set; }
            public int damage { get; set; }
            public string element { get; set; }
            public string material { get; set; }
            public string statusEffectCaused { get; set; }
            public string statusEffectOnEquip { get; set; }
            public string statusEffectPrevented { get; set; }
            public string otherMagicalEffects { get; set; }
            public MasterJson.JsonColor color { get; set; }
            public MasterJson.JsonColor color2 { get; set; }

            public void AppendMyString(RichTextBox toAppendTo)
            {
                if (this.type == ItemType.Weapon)
                {
                    var damageStr = "Adds " + this.damage + " dice";
                    if (this.damage == 0)
                    {
                        damageStr = "Deals no damage";
                    }

                    AppendTextStatic(toAppendTo, $"WEAPON: ");
                    if (this.color != null)
                    {
                        AppendColorStatic(toAppendTo, this.color);
                        AppendTextStatic(toAppendTo, " ");
                    }
                    AppendTextStatic(toAppendTo, $"{this.name}{(this.fancyName != null ? (" \"" + this.fancyName + "\"") : "")} - {damageStr} ({this.element} damage) | Buy/Sell {this.buyCost}/{this.sellCost} ");

                    if (this.statusEffectCaused != null)
                    {
                        AppendTextStatic(toAppendTo, " | Causes " + this.statusEffectCaused + " on hit");
                    }
                    if (this.statusEffectOnEquip != null)
                    {
                        AppendTextStatic(toAppendTo, " | Gives " + this.statusEffectOnEquip + " on equip");
                    }
                    if (this.statusEffectPrevented != null)
                    {
                        AppendTextStatic(toAppendTo, " | Prevents " + this.statusEffectPrevented + " while equipped");
                    }

                    if (!string.IsNullOrWhiteSpace(this.otherMagicalEffects))
                    {
                        AppendTextStatic(toAppendTo, Environment.NewLine);
                        AppendTextStatic(toAppendTo, "MAGICAL EFFECTS: " + this.otherMagicalEffects);
                    }
                }
                else if (this.type == ItemType.Scroll)
                {
                    AppendTextStatic(toAppendTo, "SPELL SCROLL: " + this.name + " | " + this.element + " | ");
                    AppendColorStatic(toAppendTo, this.color);
                    AppendTextStatic(toAppendTo, $" | Buy/Sell {this.buyCost}/{this.sellCost} ");
                }
                else if (this.type == ItemType.Armor)
                {
                    AppendTextStatic(toAppendTo, $"ARMOR: ");
                    if (this.color != null)
                    {
                        AppendColorStatic(toAppendTo, this.color);
                        AppendTextStatic(toAppendTo, " ");
                    }
                    AppendTextStatic(toAppendTo, $"{this.name}{(this.fancyName != null ? (" \"" + this.fancyName + "\"") : "")} | Buy/Sell {this.buyCost}/{this.sellCost} ");

                    if (this.statusEffectOnEquip != null)
                    {
                        AppendTextStatic(toAppendTo, " | Gives " + this.statusEffectOnEquip + " on equip");
                    }
                    if (this.statusEffectPrevented != null)
                    {
                        AppendTextStatic(toAppendTo, " | Prevents " + this.statusEffectPrevented + " while equipped");
                    }

                    if (!string.IsNullOrWhiteSpace(this.otherMagicalEffects))
                    {
                        AppendTextStatic(toAppendTo, Environment.NewLine);
                        AppendTextStatic(toAppendTo, "MAGICAL EFFECTS: " + this.otherMagicalEffects);
                    }
                }
                else if (this.type == ItemType.Accessory)
                {
                    AppendTextStatic(toAppendTo, $"ACCESSORY: ");
                    if (this.color != null)
                    {
                        AppendColorStatic(toAppendTo, this.color);
                        AppendTextStatic(toAppendTo, " ");
                    }
                    AppendTextStatic(toAppendTo, $"{this.name}{(this.fancyName != null ? (" \"" + this.fancyName + "\"") : "")} | Buy/Sell {this.buyCost}/{this.sellCost} ");

                    if (this.statusEffectOnEquip != null)
                    {
                        AppendTextStatic(toAppendTo, " | Gives " + this.statusEffectOnEquip + " on equip");
                    }
                    if (this.statusEffectPrevented != null)
                    {
                        AppendTextStatic(toAppendTo, " | Prevents " + this.statusEffectPrevented + " while equipped");
                    }

                    if (!string.IsNullOrWhiteSpace(this.otherMagicalEffects))
                    {
                        AppendTextStatic(toAppendTo, Environment.NewLine);
                        AppendTextStatic(toAppendTo, "MAGICAL EFFECTS: " + this.otherMagicalEffects);
                    }
                }
                else if (this.type == ItemType.Object)
                {
                    AppendTextStatic(toAppendTo, $"OBJECT: ");
                    if (this.color != null)
                    {
                        AppendColorStatic(toAppendTo, this.color);
                        AppendTextStatic(toAppendTo, " ");
                    }
                    AppendTextStatic(toAppendTo, $"{UppercaseFirst(this.name)}{(this.fancyName != null ? (" \"" + this.fancyName + "\"") : "")} | Buy/Sell {this.buyCost}/{this.sellCost} ");

                    if (!string.IsNullOrWhiteSpace(this.otherMagicalEffects))
                    {
                        AppendTextStatic(toAppendTo, Environment.NewLine);
                        AppendTextStatic(toAppendTo, "MAGICAL EFFECTS: " + this.otherMagicalEffects);
                    }
                }
                else if (this.type == ItemType.Potion)
                {
                    AppendTextStatic(toAppendTo, $"POTION: ");
                    AppendTextStatic(toAppendTo, $"{UppercaseFirst(this.name)}{(this.fancyName != null ? (" \"" + this.fancyName + "\"") : "")} | Buy/Sell {this.buyCost}/{this.sellCost} ");
                    AppendTextStatic(toAppendTo, " | ");
                    AppendColorStatic(toAppendTo, this.color);
                    AppendTextStatic(toAppendTo, " " + this.material + " container, ");
                    AppendColorStatic(toAppendTo, this.color2);
                    AppendTextStatic(toAppendTo, " potion | " + this.statusEffectCaused);
                }
            }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        static Random rnd = new Random();

        static int randomNumber(int min, int max)
        {
            return rnd.Next(min, max + 1);
        }

        static bool percentChance(int percent)
        {
            return rnd.Next(0, 101) <= percent;
        }

        static int randomMoney(int max)
        {
            //2500
            //0-9
            //10-99
            //100-999
            //1000-2500
            // factor ten should be 4
            var factorOfTen = 0;
            decimal n = max;
            while (n > 1)
            {
                n = n / 10;
                factorOfTen++;
            }
            int bracketRange = rnd.Next(0, factorOfTen);
            var min = ((int)Math.Pow(10, bracketRange));
            if (min == 1) { min = 0; }
            var newMax = Math.Min((max + 1), ((int)Math.Pow(10, bracketRange + 1)));
            if (newMax == max) { newMax += 1; }

            return normalizeGold(rnd.Next(min, newMax));
            
        }

        private static int normalizeGold(int gold)
        {
            if (gold < 1)
            {
                return gold;
            }

            var val = gold.ToString();
            if (val.Length <= 2)
            {
                return gold;
            }
            else if (val.Length == 3)
            {
                val = new string(new char[] { val[0], val[1], '0' });
            }
            else
            {
                var chars = new List<char>();
                foreach (var character in val)
                {
                    //if (chars.Count < 3)
                    if (chars.Count < 2)
                    {
                        chars.Add(character);
                    }
                    else
                    {
                        chars.Add('0');
                    }
                }
                val = new string(chars.ToArray());
            }

            return int.Parse(val);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // friendly person
            var person = GeneratePerson();
            person.AppendMyString(richTextBox1.RichTextBox);
            scrollToBottom();
        }

        private void button45_Click(object sender, EventArgs e)
        {
            // armed person
            var person = GeneratePerson();
            if (!person.OnHand.Any(x => x.type == Item.ItemType.Weapon))
            {
                person.OnHand.Add(GenerateRandomLoot(Item.ItemType.Weapon));
            }

            if (!person.OnHand.Any(x => x.type == Item.ItemType.Armor) && percentChance(20))
            {
                person.OnHand.Add(GenerateRandomLoot(Item.ItemType.Armor));
            }

            if (!person.OnHand.Any(x => x.type == Item.ItemType.Accessory) && percentChance(20))
            {
                person.OnHand.Add(GenerateRandomLoot(Item.ItemType.Accessory));
            }

            if (!person.OnHand.Any(x => x.type == Item.ItemType.Potion) && percentChance(20))
            {
                person.OnHand.Add(GenerateRandomLoot(Item.ItemType.Potion));
            }

            person.AppendMyString(richTextBox1.RichTextBox);
            scrollToBottom();
        }

        public Person GeneratePerson()
        {
            var age = randomNumber(18, 150);
            if (age > 100 && percentChance(50))
            {
                age = randomNumber(18, 100);
            }

            var person = new Person
            {
                isPerson = true,
                Race = GenerateRace(),
                FirstName = percentChance(50) ? masterFile.Names.First.Female.Random() : masterFile.Names.First.Male.Random(),
                LastName = masterFile.Names.Last.Random(),
                hairColor = GenerateColor(),
                pantsColor = GenerateColor(),
                skinColor = GenerateColor(),
                shirtColor = GenerateColor(),
                Occupation = masterFile.Occupations.Random(),
                KnownSpells = new List<Spell>(),
                OnHand = new List<Item>(),
                age = age,
                Classes = new List<Person.Class>(),
                PersonalityTraits = new List<string>(),
                otherAdjective = masterFile.Adjectives.Random(),
                size = masterFile.SizeAttributes.Random(),
                Skills = GenerateSkills(),
                friendliness = randomNumber(1, 100),
                deception = randomNumber(1, 60),
            };

            person.SetRandomStats();

            person.Classes = GenerateClasses();

            var nameVarianceChance = 4;

            if (percentChance(nameVarianceChance))
            {
                person.FirstName = masterFile.TownNames.Random();
            }
            else if (percentChance(nameVarianceChance))
            {
                person.FirstName = masterFile.WorldNames.Random();
            }
            else if (percentChance(nameVarianceChance))
            {
                person.FirstName = masterFile.Objects.Random();
            }
            else if (percentChance(nameVarianceChance))
            {
                person.FirstName = masterFile.Adjectives.Random();
            }
            else if (percentChance(nameVarianceChance))
            {
                person.FirstName = masterFile.Classes.Random();
            }
            else if (percentChance(nameVarianceChance))
            {
                person.FirstName = masterFile.Colors.Random().ColorName;
            }
            else if (percentChance(nameVarianceChance))
            {
                person.FirstName = masterFile.Elements.Random();
            }

            if (percentChance(nameVarianceChance))
            {
                person.LastName = masterFile.TownNames.Random();
            }
            else if (percentChance(nameVarianceChance))
            {
                person.LastName = masterFile.WorldNames.Random();
            }
            else if (percentChance(nameVarianceChance))
            {
                person.LastName = masterFile.Objects.Random();
            }
            else if (percentChance(nameVarianceChance))
            {
                person.LastName = masterFile.Adjectives.Random();
            }
            else if (percentChance(nameVarianceChance))
            {
                person.LastName = masterFile.Classes.Random();
            }
            else if (percentChance(nameVarianceChance))
            {
                person.LastName = masterFile.Colors.Random().ColorName;
            }
            else if (percentChance(nameVarianceChance))
            {
                person.LastName = masterFile.Elements.Random();
            }

            person.FirstName = UppercaseFirst(person.FirstName);
            person.LastName = UppercaseFirst(person.LastName);


            if (percentChance(7))
            {
                person.hairColor = new MasterJson.JsonColor { ColorName = "BALD", HexCode = "#000000" };
            }

            person.PersonalityTraits.Add(masterFile.Personalities.Random());
            while (percentChance(45))
            {
                person.PersonalityTraits.Add(masterFile.Personalities.Random());
            }


            


            if (percentChance(75))
            {
                person.OnHand.Add(GenerateRandomLoot(Item.ItemType.Weapon));
            }

            while (rnd.Next(0, 100) < 45)
            {
                var toAdd = GenerateRandomLoot(null);
                if (toAdd != null)
                {
                    person.OnHand.Add(toAdd);
                }
            }

            var knownSpells = (percentChance(60) ? 0 : randomNumber(1, 6));

            while (knownSpells-- > 0)
            {
                var toAdd = GenerateSpell();
                if (toAdd != null)
                {
                    person.KnownSpells.Add(toAdd);
                }
            }

            return person;
        }

        public static List<Person.Class> GenerateClasses(bool forceClasses = false)
        {
            var res = new List<Person.Class>();
            var classes = 0;
            var classChance = randomNumber(0, 100);
            if (classChance <= 20)
            {
                classes = 0;
            }
            else if (classChance <= 70)
            {
                classes = 1;
            }
            else if (classChance <= 90)
            {
                classes = 2;
            }
            else
            {
                classes = 3;
            }

            if (forceClasses && classes == 0)
            {
                classes = 1;
            }

            var maxLevel = randomNumber(15, 30);
            var currentLevel = 0;
            while (classes-- > 0)
            {
                var level = 1;
                if (maxLevel - currentLevel <= 1)
                {
                    currentLevel = 1;
                }
                else
                {
                    level = randomNumber(1, maxLevel - currentLevel);
                }
                res.Add(new Person.Class
                {
                    classLevel = level,
                    className = masterFile.Classes.Random(),
                });
                currentLevel += level;
            }

            return res;
        }

        public Spell GenerateSpell()
        {
            var spell = masterFile.SpellNames.Random();
            Spell s = this.spells.Find(x => x.spellName == spell);
            if (s == null)
            {
                s = new Spell
                {
                    color = GenerateColor(),
                    spellElement = masterFile.Elements.Random(),
                    spellName = spell,
                    mpCost = randomNumber(1, 3),
                };
                spells.Add(s);
            }

            return s;
        }

        public Item GenerateRandomLoot(Item.ItemType? type)
        {
            var existingItem = ((this.items.Where(x => type.HasValue && type.Value == x.type).Count() > 10) && percentChance(3));
            if (existingItem)
            {
                return this.items.ToArray().Random();
            }
            else
            {
                if (type == null)
                {
                    var rand = rnd.Next(1, 101);
                    if (rand <= 10)
                    {
                        type = Item.ItemType.Scroll;
                    }
                    else if (rand <= 25)
                    {
                        type = Item.ItemType.Weapon;
                    }
                    else if (rand <= 40)
                    {
                        type = Item.ItemType.Armor;
                    }
                    else if (rand < 55)
                    {
                        type = Item.ItemType.Accessory;
                    }
                    else if (rand < 65)
                    {
                        type = Item.ItemType.Potion;
                    }
                    else
                    {
                        type = Item.ItemType.Object;
                    }
                }

                if (type == Item.ItemType.Weapon)
                {
                    var weaponType = UppercaseFirst(percentChance(5) ? masterFile.Objects.Random() : masterFile.WeaponTypes.Random());

                    var buyCost = randomMoney(100000);
                    var weapon = new Item
                    {
                        buyCost = buyCost,
                        sellCost = normalizeGold((buyCost / 2) + randomNumber(-10, 50)),
                        damage = randomNumber(1, 4),
                        element = percentChance(5) ? masterFile.Elements.Random() : masterFile.NonMagicalElements.Random(),
                        statusEffectCaused = percentChance(15) ? masterFile.StatusEffects.Random() : null,
                        statusEffectOnEquip = percentChance(5) ? masterFile.StatusEffects.Random() : null,
                        statusEffectPrevented = percentChance(5) ? masterFile.StatusEffects.Random() : null,
                        name = weaponType,
                        type = Item.ItemType.Weapon,
                        color = (percentChance(15) ? (GenerateColor()) : null),
                        fancyName = percentChance(30) ? masterFile.WeaponNames.Random() : null,
                        material = (percentChance(33) ? masterFile.Materials.Random() : null),
                    };

                    if (percentChance(5)) { weapon.damage = 0; }

                    if (percentChance(4) && weapon.statusEffectOnEquip == null) { weapon.statusEffectOnEquip = "50% chance to ignore enemy armor when attacking"; }

                    if (weaponType.ToUpper().Contains("SWORD") && percentChance(1))
                    {
                        weapon.otherMagicalEffects = "Appears as a normal sword in a sheath, however when drawn, extends from the sheath up to 100 feet before being unable to be drawn further.";
                    }
                    else if (weaponType.ToUpper().Contains("SWORD") && percentChance(1))
                    {
                        weapon.otherMagicalEffects = "Is an un-sword, causing un-wounds.";
                    }

                    items.Add(weapon);

                    return weapon;
                }
                else if (type == Item.ItemType.Scroll)
                {
                    var spell = (percentChance(10) ? (new Spell { color = GenerateColor(), spellElement = masterFile.Elements.Random(), spellName = "Identify" }) : (GenerateSpell()));

                    var buyCost = randomMoney(10000);
                    var scroll = new Item
                    {
                        buyCost = buyCost,
                        sellCost = normalizeGold((buyCost / 2) + randomNumber(-10, 50)),
                        element = spell.spellElement,
                        name = spell.spellName,
                        color = spell.color,
                        type = Item.ItemType.Scroll,
                    };

                    return scroll;
                }
                else if (type == Item.ItemType.Armor)
                {
                    var buyCost = randomMoney(100000);
                    var armor = new Item
                    {
                        buyCost = buyCost,
                        sellCost = normalizeGold((buyCost / 2) + randomNumber(-10, 50)),
                        type = Item.ItemType.Armor,
                        statusEffectCaused = null,
                        statusEffectOnEquip = percentChance(15) ? masterFile.StatusEffects.Random() : null,
                        statusEffectPrevented = percentChance(15) ? masterFile.StatusEffects.Random() : null,
                        element = percentChance(15) ? masterFile.Elements.Random() : null,
                        color = (percentChance(25) ? (GenerateColor()) : null),
                        name = UppercaseFirst(percentChance(5) ? masterFile.Objects.Random() : masterFile.ArmorTypes.Random()),
                        material = (percentChance(33) ? masterFile.Materials.Random() : null),
                    };

                    if (percentChance(6))
                    {
                        armor.otherMagicalEffects = "";

                        var did = false;
                        do
                        {
                            if (did)
                            {
                                armor.otherMagicalEffects += " | ";
                            }
                            armor.otherMagicalEffects += GenerateMagicalEffect(true);
                            did = true;
                        }
                        while (percentChance(30));
                    }

                    if (percentChance(3))
                    {
                        var str = "Has a 33% chance to reflect damage to the attacker";
                        if (armor.otherMagicalEffects == null) { armor.otherMagicalEffects = str; }
                        else armor.otherMagicalEffects += " | " + str;
                    }

                    return armor;
                }
                else if (type == Item.ItemType.Accessory)
                {
                    var buyCost = randomMoney(100000);
                    var accessory = new Item
                    {
                        buyCost = buyCost,
                        sellCost = normalizeGold((buyCost / 2) + randomNumber(-10, 50)),
                        type = Item.ItemType.Accessory,
                        color = (percentChance(25) ? (GenerateColor()) : null),
                        statusEffectOnEquip = percentChance(15) ? masterFile.StatusEffects.Random() : null,
                        statusEffectPrevented = percentChance(15) ? masterFile.StatusEffects.Random() : null,

                        name = UppercaseFirst(percentChance(5) ? masterFile.Objects.Random() : masterFile.AccessoryTypes.Random()),
                        material = (percentChance(33) ? masterFile.Materials.Random() : null),
                    };

                    if (percentChance(20))
                    {
                        accessory.otherMagicalEffects = "";

                        var did = false;
                        do
                        {
                            if (did)
                            {
                                accessory.otherMagicalEffects += " | ";
                            }
                            accessory.otherMagicalEffects += GenerateMagicalEffect(true);
                            did = true;
                        }
                        while (percentChance(30));
                    }

                    return accessory;
                }
                else if (type == Item.ItemType.Object)
                {
                    var buyCost = randomMoney(10000);
                    var item = new Item
                    {
                        buyCost = buyCost,
                        sellCost = normalizeGold((buyCost / 2) + randomNumber(-10, 50)),
                        color = (percentChance(25) ? (GenerateColor()) : null),
                        name = (percentChance(33) ? ((percentChance(50) ? (masterFile.Adjectives.Random()) : (masterFile.SizeAttributes.Random())) + " ") : null) + masterFile.Objects.Random().ToLower(),
                        type = Item.ItemType.Object,
                        material = (percentChance(33) ? masterFile.Materials.Random() : null),
                    };

                    if (percentChance(7))
                    {
                        item.otherMagicalEffects = "";

                        var did = false;
                        do
                        {
                            if (did)
                            {
                                item.otherMagicalEffects += " | ";
                            }
                            item.otherMagicalEffects += GenerateMagicalEffect(false);
                            did = true;
                        }
                        while (percentChance(30)) ;
                    }

                    return item;
                }
                else if (type == Item.ItemType.Potion)
                {
                    var buyCost = randomMoney(10000);
                    var potion = new Item
                    {
                        buyCost = buyCost,
                        sellCost = normalizeGold((buyCost / 2) + randomNumber(-10, 50)),
                        color = GenerateColor(),
                        color2 = GenerateColor(),
                        type = Item.ItemType.Potion,
                        material = masterFile.Materials.Random(),
                    };

                    var hours = randomNumber(1, 3);
                    var hrstr = hours + " hour" + (hours != 1 ? "s" : "");
                    if (percentChance(50))
                    {
                        hours = randomNumber(1, 30);
                        hrstr = hours + " minute" + (hours != 1 ? "s" : "");
                    }

                    var effectStr = "";
                    var rollHundo = randomNumber(1, 100);
                    if (rollHundo <= 2)
                    {
                        potion.statusEffectCaused = "Revives one dead creature with 1-4 hitpoints";
                        effectStr = "Revival";
                    }
                    else if (rollHundo <= 20)
                    {
                        potion.statusEffectCaused = "Heals " + randomNumber(2, 12) + " hitpoints";
                        effectStr = "Healing";
                    }
                    else if (rollHundo <= 25)
                    {
                        potion.statusEffectCaused = "Adds " + randomNumber(1, 2) + " dots to skill " + masterFile.NWODSkills.Random() + " for " + hrstr;
                        effectStr = "Skill";
                    }
                    else if (rollHundo <= 30)
                    {
                        var stats = new string[] {
                            "Intelligence",
                            "Wits",
                            "Resolve",
                            "Strength",
                            "Dexterity",
                            "Stamina",
                            "Presence",
                            "Manipulation",
                            "Composure",
                        };

                        potion.statusEffectCaused = "Adds " + randomNumber(1, 2) + " dots to " + stats.Random() + " for " + hrstr;
                        effectStr = "Power";
                    }
                    else if (rollHundo <= 35)
                    {
                        potion.statusEffectCaused = "Restores " + randomNumber(2, 5) + " magic points";
                        effectStr = "Mana";
                    }
                    else
                    {
                        var status = masterFile.StatusEffects.Random();
                        potion.statusEffectCaused = "Causes " + status + " for " + hrstr;
                        effectStr = status;
                    }

                    var name = "";
                    if (percentChance(50))
                    {
                        name = UppercaseFirst(masterFile.PotionTypes.Random().ToLower()) + " of " + UppercaseFirst(effectStr);
                    }
                    else
                    {
                        name = UppercaseFirst(effectStr) + " " + UppercaseFirst(masterFile.PotionTypes.Random().ToLower());
                    }

                    if (percentChance(12))
                    {
                        potion.statusEffectCaused += ". Can be used twice.";
                        var doubleNames = new string[] { "Double", "Twin", "Lasting", "Big", "Dual", "Duplex" };
                        name = doubleNames.Random() + " " + name;
                    }
                    else if (percentChance(12))
                    {
                        var timesUse = randomNumber(1, 3);
                        potion.statusEffectCaused += ". Can be applied to a weapon " + timesUse + " time" + (timesUse != 1 ? "s." : ".");
                    }

                    potion.name = name;

                    return potion;
                }
            }

            return null;
        }

        public class Person
        {
            public bool isPerson { get; set; }
            public Person() { }
            public string Race { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Occupation { get; set; }
            public List<Item> OnHand { get; set; }
            public List<Spell> KnownSpells { get; set; }
            public MasterJson.JsonColor shirtColor { get; set; }
            public MasterJson.JsonColor pantsColor { get; set; }
            public MasterJson.JsonColor hairColor { get; set; }
            public MasterJson.JsonColor skinColor { get; set; }
            public int money { get; set; }
            public int age { get; set; }
            public string size { get; set; }
            public string otherAdjective { get; set; }

            public int intelligence { get; set; }
            public int wits { get; set; }
            public int resolve { get; set; }
            public int strength { get; set; }
            public int dexterity { get; set; }
            public int stamina { get; set; }
            public int presence { get; set; }
            public int manipulation { get; set; }
            public int composure { get; set; }

            public List<Skill> Skills { get; set; }
            public int maxHealth { get; set; }
            public int maxMP { get; set; }

            public int friendliness { get; set; }
            public int deception { get; set; }

            public class Skill
            {
                public int dots { get; set; }
                public string skillName { get; set; }
                public string specialty { get; set; }
            }

            public class Class
            {
                public string className { get; set; }
                public int classLevel { get; set; }
            }
            public List<Class> Classes { get; set; }
            public List<string> PersonalityTraits { get; set; }

            public virtual void AppendMyString(RichTextBox toAppendTo, string personLabel = "PERSON")
            {
                AppendTextStatic(toAppendTo, Environment.NewLine + $"{personLabel}: {FirstName} {LastName} ({Race} {Occupation}) ");
                if (Classes.Any())
                {
                    var strList = new List<string>();
                    foreach (var c in Classes)
                    {
                        strList.Add($"Lv. {c.classLevel} {c.className}");
                    }
                    AppendTextStatic(toAppendTo, string.Join(" | ", strList.ToArray()));
                }
                else
                {
                    AppendTextStatic(toAppendTo, "No Classes");
                }

                AppendTextStatic(toAppendTo, " | MAX HP: " + this.maxHealth + " | MAX MP: " + this.maxMP);

                AppendTextStatic(toAppendTo, Environment.NewLine + $"Intelligence {this.intelligence} | Strength  {this.strength} | Presence     {this.presence}", null, false, true);
                AppendTextStatic(toAppendTo, Environment.NewLine + $"Wit          {this.wits} | Dexterity {this.dexterity} | Manipulation {this.manipulation}", null, false, true);
                AppendTextStatic(toAppendTo, Environment.NewLine + $"Resolve      {this.resolve} | Stamina   {this.stamina} | Composure    {this.composure}", null, false, true);

                var skillstr = new List<string>();
                foreach (var s in this.Skills)
                {
                    if (s.specialty != null)
                        skillstr.Add(s.skillName + ": " + s.dots + (s.specialty != null ? (" (" + s.specialty + ")") : ""));
                }
                if (skillstr.Any())
                {
                    AppendTextStatic(toAppendTo, Environment.NewLine + string.Join(" | ", skillstr.ToArray()));
                }

                AppendTextStatic(toAppendTo, Environment.NewLine + "(1-100, Friend < Deception means likely to lie) ");
                AppendTextStatic(toAppendTo, "Friendliness: " + this.friendliness + " | Deception: " + this.deception, null, (this.friendliness <= this.deception), false);

                AppendTextStatic(toAppendTo, Environment.NewLine + $"{FirstName} ({age} yrs) is {this.size}, {string.Join(", ", PersonalityTraits.ToArray())}, is feeling {masterFile.Moods.Random()}, and is {this.otherAdjective}. They are wearing ");
                AppendColorStatic(toAppendTo, pantsColor);
                AppendTextStatic(toAppendTo, " pants, a ");
                AppendColorStatic(toAppendTo, shirtColor);
                AppendTextStatic(toAppendTo, " shirt, have ");
                AppendColorStatic(toAppendTo, hairColor);
                AppendTextStatic(toAppendTo, " hair, and have ");
                AppendColorStatic(toAppendTo, skinColor);
                AppendTextStatic(toAppendTo, " skin.");
                
                if (KnownSpells.Any())
                {
                    foreach (var s in KnownSpells)
                    {
                        AppendTextStatic(toAppendTo, Environment.NewLine + $"· " + s.spellName + " (" + s.mpCost + " MP) | " + s.spellElement + " | ");
                        AppendColorStatic(toAppendTo, s.color);
                    }
                }

                AppendTextStatic(toAppendTo, Environment.NewLine + $"Carries {money} gold.");
                if (OnHand.Count > 0)
                {
                    foreach (var item in OnHand)
                    {
                        AppendTextStatic(toAppendTo, Environment.NewLine + $"· ");
                        item.AppendMyString(toAppendTo);
                    }
                }
                AppendTextStatic(toAppendTo, Environment.NewLine);
            }

            public void SetRandomStats()
            {
                this.money = randomMoney(10000);
                this.intelligence = randomNumber(1, 5);
                this.wits = randomNumber(1, 5);
                this.resolve = randomNumber(1, 5);
                this.strength = randomNumber(1, 5);
                this.dexterity = randomNumber(1, 5);
                this.stamina = randomNumber(1, 5);
                this.presence = randomNumber(1, 5);
                this.manipulation = randomNumber(1, 5);
                this.composure = randomNumber(1, 5);
                this.maxHealth = randomNumber(1, 5) + 5;
                this.maxMP = randomNumber(1, 6) + 2;
            }
        }

        public static T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(new Random().Next(v.Length));
        }

        private void button14_Click(object sender, EventArgs e)
        {
            // Generate Generic Loot (Gold or items)
            AppendText(Environment.NewLine);
            if (percentChance(30))
            {
                generateGoldReward("LOOT");
            }
            else
            {
                var loot = GenerateRandomLoot(null);
                if (loot == null)
                {
                    AppendText("I haven't programmed in that path yet");
                }
                else
                {
                    loot.AppendMyString(richTextBox1.RichTextBox);
                }
            }
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private void generateGoldReward(string tag)
        {
            if (percentChance(1))
            {
                AppendText(tag + ": Artist's Exposure (so worth it, it's basically free advertising!)");
            }
            else if (percentChance(10))
            {
                AppendText(tag + ": -" + randomMoney(10000) + " gold");
            }
            else
            {
                AppendText(tag + ": " + randomMoney(100000) + " gold");
            }
        }

        private void scrollToBottom()
        {
            scrollToBottomStatic(richTextBox1.RichTextBox);
        }

        private static void scrollToBottomStatic(RichTextBox toScroll)
        {
            toScroll.SelectionStart = toScroll.Text.Length;
            toScroll.ScrollToCaret();
        }

        // Generate Gold Loot
        private void button12_Click(object sender, EventArgs e)
        {
            AppendText(Environment.NewLine);
            generateGoldReward("GOLD");
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            // Generate Item Loot
            AppendText(Environment.NewLine);
            var loot = GenerateRandomLoot(null);
            if (loot == null)
            {
                AppendText("I haven't programmed in that path yet");
            }
            else
            {
                loot.AppendMyString(richTextBox1.RichTextBox);
            }
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            // Generate Armor:
            AppendText(Environment.NewLine);
            var loot = GenerateRandomLoot(Item.ItemType.Armor);
            if (loot == null)
            {
                AppendText("I haven't programmed in that path yet");
            }
            else
            {
                loot.AppendMyString(richTextBox1.RichTextBox);
            }
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            // Generate Accessory:
            AppendText(Environment.NewLine);
            var loot = GenerateRandomLoot(Item.ItemType.Accessory);
            if (loot == null)
            {
                AppendText("I haven't programmed in that path yet");
            }
            else
            {
                loot.AppendMyString(richTextBox1.RichTextBox);
            }
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Generate Object:
            AppendText(Environment.NewLine);
            var loot = GenerateRandomLoot(Item.ItemType.Object);
            if (loot == null)
            {
                AppendText("I haven't programmed in that path yet");
            }
            else
            {
                loot.AppendMyString(richTextBox1.RichTextBox);
            }
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            // Generate Spell Scroll
            AppendText(Environment.NewLine);
            var loot = GenerateRandomLoot(Item.ItemType.Scroll);
            if (loot == null)
            {
                AppendText("I haven't programmed in that path yet");
            }
            else
            {
                loot.AppendMyString(richTextBox1.RichTextBox);
            }
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        // Generate Enemy Monster
        private void button3_Click(object sender, EventArgs e)
        {
            var m = GenerateMonster(true) as Monster;
            AppendText(Environment.NewLine);
            m.AppendMyString(this.richTextBox1.RichTextBox);
            AppendText(Environment.NewLine);
        }

        // Generate Random Friendly Encounter
        private void button6_Click(object sender, EventArgs e)
        {
            GenerateEncounter(true).AppendMyString(this.richTextBox1.RichTextBox);
            AppendText(Environment.NewLine);
            
        }

        // Generate Random Enemy Encounter
        private void button7_Click(object sender, EventArgs e)
        {
            GenerateEncounter(false).AppendMyString(this.richTextBox1.RichTextBox);
            AppendText(Environment.NewLine);
        }

        // Generate Shop List
        private void button11_Click(object sender, EventArgs e)
        {
            int[] itemCounts = new int[] { 3, 3, 3, 3, 3, 4, 4, 5, 5, 6, 6, 7, 10 };
            var itemCount = itemCounts.Random();
            AppendText(Environment.NewLine);
            Item.ItemType? type = null;
            bool sellsFood = false;

            if (this.shopListTypeList.SelectedItem != null && 
                !this.shopListTypeList.SelectedItem.ToString().ToUpper().Contains("Random".ToUpper()) && 
                !string.IsNullOrWhiteSpace(this.shopListTypeList.SelectedItem.ToString()))
            {
                var str = this.shopListTypeList.SelectedItem.ToString();
                if (str.ToUpper().Contains("Food".ToUpper()))
                {
                    sellsFood = true;
                }
                else
                {
                    type = (Item.ItemType)Enum.Parse(typeof(Item.ItemType), str);
                }
            }

            if (type == null && percentChance(20) && !sellsFood)
            {
                var roll = randomNumber(1, 140);
                if (roll <= 20)
                {
                    type = Item.ItemType.Accessory;
                }
                else if (roll <= 40)
                {
                    type = Item.ItemType.Armor;
                }
                else if (roll <= 60)
                {
                    type = Item.ItemType.Object;
                }
                else if (roll <= 80)
                {
                    type = Item.ItemType.Scroll;
                }
                else if (roll <= 100)
                {
                    type = Item.ItemType.Weapon;
                }
                else if (roll <= 120)
                {
                    type = Item.ItemType.Potion;
                }
                else
                {
                    sellsFood = true;
                }
            }

            if (type != null)
            {
                AppendText(type.ToString() + " Shop: ", null, true);
            }
            else if (sellsFood)
            {
                AppendText("Food Store: ");
                itemCount = randomNumber(4, 9);
            }
            else
            {
                AppendText("SHOP: ");
            }

            if (sellsFood)
            {
                while (itemCount-- > 0)
                {
                    AppendText(Environment.NewLine);
                    AppendText("Menu Item: ");
                    AppendFoodDrinkText(percentChance(66));
                }
            }
            else
            {

                var items = new List<Item>();
                while (itemCount-- > 0)
                {
                    items.Add(this.GenerateRandomLoot(type));
                }

                items = items
                    .OrderByDescending(x => !string.IsNullOrEmpty(x.otherMagicalEffects))
                    .ThenByDescending(x => (!string.IsNullOrWhiteSpace(x.statusEffectCaused)) || (!string.IsNullOrWhiteSpace(x.statusEffectOnEquip)) || (!string.IsNullOrWhiteSpace(x.statusEffectPrevented)))
                    .ThenByDescending(x => (!string.IsNullOrWhiteSpace(x.element)))
                    .ThenByDescending(x => x.type != Item.ItemType.Object)
                    .ThenBy(x => x.type)
                    .ToList();

                foreach (var itemForSale in items)
                {
                    int quantity = 1;
                    if (itemForSale.type == Item.ItemType.Potion)
                    {
                        quantity = randomNumber(1, 5);
                    }
                    else if (itemForSale.type == Item.ItemType.Scroll)
                    {
                        quantity = randomNumber(1, 3);
                    }
                    else if (itemForSale.type == Item.ItemType.Object && itemForSale.otherMagicalEffects == null)
                    {
                        quantity = randomNumber(1, 20);
                    }

                    AppendText(Environment.NewLine);
                    if (!string.IsNullOrEmpty(itemForSale.otherMagicalEffects))
                    {
                        if (quantity > 1)
                        {
                            AppendText("For Sale (" + quantity + "): ", null, true);
                        }
                        else
                        {
                            AppendText("For Sale: ", null, true);
                        }
                        itemForSale.AppendMyString(richTextBox1.RichTextBox);
                    }
                    else
                    {
                        if (quantity > 1)
                        {
                            AppendText("For Sale (" + quantity + "): ");
                        }
                        else
                        {
                            AppendText("For Sale: ");
                        }
                        itemForSale.AppendMyString(richTextBox1.RichTextBox);
                    }
                }
            }

            AppendText(Environment.NewLine);
            this.scrollToBottom();
        }

        // World Setting
        private void button20_Click(object sender, EventArgs e)
        {

            AppendText(Environment.NewLine);

            var continentName = masterFile.WorldNames.Random() ;

            var continent = processBuilderString($@"{continentName} is 
#massive; It spans most of the world
#large; It makes an impact on the shape of the world
#medium; About as average as average gets
#small; It may be hard to find with out exploration
#tiny; It's practically an island
. {continentName} 
#polar; It sits one of of the polar ends of the world
#equatorial; It straddles the world's equator
#far North; The sun never sets half the year, and never rises the other half
#far South; The sun never sets half the year, and never rises the other half
#in the Northern Hemisphere
#in the Southern Hemisphere
. Compared to the rest of the the world, {continentName} is 
# warmer than usual
# colder than usual
# windier than usual
# rainier than usual
# rather average
# more tropical than usual
# incredibly Hot
# incredibly Cold
# incredibly wet
# more humid than usual
. 
#{continentName} is home to an ancient volcano that has been erupting for generations
#{continentName} is said to be protected by a god
#{continentName} is home to the tallest mountain in the world
#{continentName} is home to the largest lake in the world
#An unnaturally massive number of chickens roam the wild in {continentName}
#Most of {continentName} is below sea level
#{continentName} is pock marked with craters
#A powerful enchantment has befallen{continentName}
#{continentName} has been afflicted by a powerful curse
#{continentName} is landlocked
#{continentName} has no neighbouring continents
#{continentName} was recently ravaged by a major cataclysm 
#Massive storms make the coast uninhabitable
#At the center of {continentName} is a font of wild magic
#{continentName} used to be joined with another continent, but was split asunder
. {continentName} was discovered 
#long before memory or written history
#centuries ago
#a few generations ago
#a generation ago
#bery recently
, by 
#a travelling merchant looking for new riches
#a nomadic tribe wandering aimlessly
#a small group of pilgrims looking for greener pastures
#refugees from a massive war
#colonists sent by the ruler of another land seeking to expand his borders
#explorers looking for something new and exciting
#map makers looking for fill in gaps of a map
#a small civilization escaping from a stronger, more dominant monster
#a small civilization seeking better a better climate
#frontiersman pushing the edges of the know world.
#no one, it was the cradle of civilization
#priests seeking worshippers
. ");
            

            var business = generateBusiness();

            AppendText($"Welcome to the World of {masterFile.WorldNames.Random()}. Our adventure takes place on the continent of {continentName}. {continent}" +
                $"Our N adventurers have arrived at the {business.FullName}. " +
                $"They come in seek of adventures and deeds. Inside the ");

            AppendColor(business.exteriorColor);
            AppendText($" {business.BuildingType} with {business.BuildingMaterial} walls, they have gathered at a {masterFile.Adjectives.Random()} {masterFile.TableFurniture.Random()}. " +
                       $"On the wall is a ");

            AppendRandomColor();
            AppendText($"bulletin board. " +
                $"One listing stands out more than the others, as it is bright ");
            AppendRandomColor();
            AppendText($", is stamped with the {masterFile.Occupations.Random()}'s seal, and has a reward listing for a whopping {randomMoney(10000000)} gold. " +
                $"The ");
            AppendColor(business.intererioColor);
            AppendText($" interior {business.BuildingType} is bustling with the sound of other adventurers hunting for opportunity. ");

            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private Business generateBusiness(string bType = null)
        {
            var bName = masterFile.BusinessNames.Random();
            if (bType == null)
            {
                bType = masterFile.BuildingTypes.Random();
            }

            var material = masterFile.Materials.Random();
            int nGuards = 0;
            if (percentChance(20))
            {
                nGuards = 1;
            }
            else if (percentChance(15))
            {
                nGuards = randomNumber(2, 10);
            }
            if (bName.Contains("{0}"))
            {
                bName = string.Format(bName, bType);
                return new Business { NumberOfGuards = nGuards, BuildingMaterial = material, FullName = bName, BuildingType = bType, BusinessName = bName, exteriorColor = GenerateColor(), intererioColor = GenerateColor(), };
            }
            else
            {
                return new Business { NumberOfGuards = nGuards, BuildingMaterial = material, FullName = bName + " " + bType, BuildingType = bType, BusinessName = bName, exteriorColor = GenerateColor(), intererioColor = GenerateColor(), };
            }
        }

        private class Business
        {
            /// <summary>
            /// Like "Minstrel's Potato" or "Hotel Extreme"
            /// </summary>
            public string BusinessName { get; set; }

            /// <summary>
            /// Like "Minstrel's Potato Pub"
            /// </summary>
            public string FullName { get; set; }
            /// <summary>
            /// Like "Pub" or "Hotel"
            /// </summary>
            public string BuildingType { get; set; }

            public string BuildingMaterial { get; set; }

            public int NumberOfGuards { get; set; }

            public MasterJson.JsonColor intererioColor { get; set; }
            public MasterJson.JsonColor exteriorColor { get; set; }
        }

        // Generate Business
        private void button4_Click(object sender, EventArgs e)
        {
            string building = null;
            if (this.ShopType.SelectedItem != null && 
                !this.ShopType.SelectedItem.ToString().ToUpper().Contains("Random".ToUpper()) && 
                !string.IsNullOrWhiteSpace(this.ShopType.SelectedItem.ToString()))
            {
                building = this.ShopType.SelectedItem.ToString();
            }

            var busines = this.generateBusiness(building);
            AppendText(Environment.NewLine);
            AppendText("BUSINESS: " + busines.FullName + $" ({busines.BuildingMaterial} walls | ");

            AppendColor(busines.exteriorColor);
            AppendText(" exterior | ");
            AppendColor(busines.intererioColor);
            AppendText(" interior | " + busines.NumberOfGuards + " Guard" + (busines.NumberOfGuards == 1 ? "" : "s") + ")");
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private string processBuilderString(string toBuild)
        {
            string continent = "";
            var list = new List<string>();
            foreach (var l in toBuild.Split(new string[] { "\r\n" }, StringSplitOptions.None))
            {
                if (l.StartsWith("#"))
                {
                    list.Add(l.Substring(1, l.Length - 1));
                }
                else
                {
                    if (list.Any())
                    {
                        continent += list.ToArray().Random();
                    }
                    continent += l;
                    list = new List<string>();
                }
            }
            return continent;
        }

        private void button21_Click(object sender, EventArgs e)
        {
            AppendText(Environment.NewLine);
            AppendText(
                new string[] { "New", "Great", "Decaying", "Fallen", "Ruined", "Bustling", "Prospering", "Mighty", "Walled" }.Random() + " " + 
                (percentChance(50) ? "Town: " : "City: ") + masterFile.TownNames.Random() + " has a government of an " + masterFile.Governments.Random());
            AppendText(Environment.NewLine);
            scrollToBottom();

        }

        private void button22_Click(object sender, EventArgs e)
        {
            // Generate random noun:
            AppendText(Environment.NewLine);
            AppendText("NOUN: " + UppercaseFirst(masterFile.Objects.Random()));
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private void button23_Click(object sender, EventArgs e)
        {
            // Generate random adjective:
            AppendText(Environment.NewLine);
            AppendText("ADJECTIVE: " + UppercaseFirst(masterFile.Adjectives.Random()));
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private void button24_Click(object sender, EventArgs e)
        {
            // Generate random NWOD Specialty:
            AppendText(Environment.NewLine);
            AppendText("NWOD Specialty: " + masterFile.NWODSpecialties.Random());
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private void button25_Click(object sender, EventArgs e)
        {
            // Generate random NWOD Skill:
            AppendText(Environment.NewLine);
            AppendText("NWOD Skill: " + masterFile.NWODSkills.Random());
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private void button26_Click(object sender, EventArgs e)
        {
            // Race
            AppendText(Environment.NewLine);
            AppendText("Race: " + GenerateRace());
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private void button27_Click(object sender, EventArgs e)
        {
            // Size
            AppendText(Environment.NewLine);
            AppendText("Size: " + UppercaseFirst(masterFile.SizeAttributes.Random()));
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private void button28_Click(object sender, EventArgs e)
        {
            // Color
            AppendText(Environment.NewLine);
            AppendText("Color: ");
            AppendRandomColor();
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private void AppendRandomColor()
        {
            AppendColor(GenerateColor());
        }

        private void AppendColor(MasterJson.JsonColor toPrint)
        {
            AppendColorStatic(richTextBox1.RichTextBox, toPrint);   
        }

        private static void AppendColorStatic(RichTextBox toAppendTo, MasterJson.JsonColor toPrint)
        {
            AppendTextStatic(toAppendTo, toPrint.ColorName + " ");
            System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml(toPrint.HexCode);
            AppendTextStatic(toAppendTo, "█", col);
        }

        // Roll Percentage
        private void button30_Click(object sender, EventArgs e)
        {
            AppendText(Environment.NewLine);
            AppendText($"Percent (%): " + randomNumber(0, 100).ToString());
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private void button29_Click(object sender, EventArgs e)
        {
            if (randBetween1.Value < 0 || randBetween2.Value <= randBetween1.Value)
            {
                return;
            }

            AppendText(Environment.NewLine);
            AppendText($"Random ({randBetween1.Value} - {randBetween2.Value}): " + randomNumber((int)randBetween1.Value, (int)randBetween2.Value).ToString());
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        public string GenerateRace()
        {
            if (percentChance(15))
            {
                return "Human";
            }
            return masterFile.Race.Random();
        }

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        // roll n D10s
        private void button31_Click(object sender, EventArgs e)
        {
            
            var nDice = (int)this.nD10s.Value;
            
            if (nDice < 1)
            {
                return;
            }

            int successes = 0;
            while (nDice-- > 0)
            {
                var n = randomNumber(1, 10);
                if (n >= 8)
                {
                    successes++;
                }

                while (n == 10)
                {
                    n = randomNumber(1, 10);
                    if (n >= 8)
                    {
                        successes++;
                    }
                }
            }

            AppendText(Environment.NewLine);
            AppendText($"Roll {nDice} d10s: " + successes + " successes");
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        // roll Material
        private void button33_Click(object sender, EventArgs e)
        {
            AppendText(Environment.NewLine);
            AppendText($"Material: " + masterFile.Materials.Random());
            AppendText(Environment.NewLine);
        }

        // roll Magical Effect
        private void button32_Click(object sender, EventArgs e)
        {
            AppendText(Environment.NewLine);
            AppendText($"Magical Effect: " + GenerateMagicalEffect(false));
            AppendText(Environment.NewLine);
        }

        // roll Element
        private void button34_Click(object sender, EventArgs e)
        {
            AppendText(Environment.NewLine);
            AppendText($"Element: " + masterFile.Elements.Random());
            AppendText(Environment.NewLine);

        }

        // flip coin
        private void button38_Click(object sender, EventArgs e)
        {
            AppendText(Environment.NewLine);
            AppendText($"Coin Flip: " + (percentChance(50) ? "Heads/Good" : "Tails/Bad"));
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        private void AppendText(string text, Color? color = null, bool isBold = false)
        {
            AppendTextStatic(this.richTextBox1.RichTextBox, text, color, isBold);
        }

        private static void AppendTextStatic(RichTextBox toAppendTo, string text, Color? color = null, bool isBold = false, bool isFixedWidth = false)
        {
            toAppendTo.SelectionStart = toAppendTo.Text.Length;
            var rtb = toAppendTo;
            int start = rtb.TextLength;
            rtb.AppendText(text);
            int end = rtb.TextLength; // now longer by length of appended text

            // Select text that was appended
            rtb.Select(start, end - start);

            if (color != null)
            {
                rtb.SelectionColor = color.Value;
            } else
            {
                rtb.SelectionColor = Color.Black;
            }

            if (isFixedWidth)
            {
                rtb.SelectionFont = new Font(
                                     new FontFamily("Consolas"),
                                     rtb.SelectionFont.Size,
                                     isBold ? FontStyle.Bold : FontStyle.Regular);
            }
            else
            {
                rtb.SelectionFont = new Font(
                     new FontFamily("Microsoft Sans Serif"),
                     rtb.SelectionFont.Size,
                     isBold ? FontStyle.Bold : FontStyle.Regular);
            }

                // Unselect text
                rtb.SelectionLength = 0;
            
            scrollToBottomStatic(toAppendTo);
        }

        private string GenerateMagicalEffect(bool excludeGrogEffects)
        {
            var roll1 = randomNumber(1, 100);
            if (roll1 <= 2)
            {
                if (percentChance(33))
                {
                    if (percentChance(80))
                    {
                        return "You can consume this item to gain the specialty \"" + masterFile.NWODSpecialties.Random() + "\" in the skill " + masterFile.NWODSkills.Random();
                    }
                    else
                    {
                        return "You can consume this item to gain the specialty \"" + masterFile.NWODSpecialties.Random() + "\" in the skill of your choice";
                    }
                }

                var skill = masterFile.NWODSkills.Random();
                var roll = randomNumber(1, 100);
                if (roll <= 25)
                {
                    return "While held, increases the skill " + skill + " by 1";
                }
                else if (roll <= 50)
                {
                    return "Can be consumed to permanently increase the skill " + skill + " by 1";
                }
                else if (roll <= 75)
                {
                    return "While held, increases the skill " + skill + " by 1";
                }
                else
                {
                    return "Can be consumed to permanently increase the skill " + skill + " by 1";
                }
            }
            else if (roll1 <= 4)
            {
                var skill = masterFile.NWODSkills.Random();
                var roll = randomNumber(1, 100);
                if (roll <= 33)
                {
                    return "Can be used to add two dots in anyone's skill: " + skill + ", for a short period, " + randomNumber(1, 3) + " times per day";
                }
                else if (roll <= 33)
                {
                    return "Can be used to add two dots in your skill: " + skill + ", for a short period, " + randomNumber(1, 3) + " times per day";
                }
                else
                {
                    return "Can be used to add two dots in someone else's skill: " + skill + ", for a short period, " + randomNumber(1, 3) + " times per day";
                }
            }
            else if (roll1 <= 6)
            {
                var spell = GenerateSpell();
                if (percentChance(50))
                {
                    var times = randomNumber(1, 3);
                    return "Can be used to cast the spell " + spell.spellName + " (" + spell.spellElement + ") " + (times == 1 ? "once" : (times + " times")) + " per day";
                }
                return "Can be consumed to learn the spell " + spell.spellName + " (" + spell.mpCost + " MP) (" + spell.spellElement + ")";
            }
            else if (!excludeGrogEffects && roll1 <= 24)
            {
                return masterFile.GrogSubstantialWhimsyEffects.Random();
            }
            return masterFile.MagicalItemEffects.Random();
        }

        // Generate food
        private void button39_Click(object sender, EventArgs e)
        {
            AppendText(Environment.NewLine + "FOOD: ");
            AppendFoodDrinkText(true);
            AppendText(Environment.NewLine);
        }        

        //  Generate Drink
        private void button40_Click(object sender, EventArgs e)
        {
            AppendText(Environment.NewLine + "DRINK: ");
            AppendFoodDrinkText(false);
            AppendText(Environment.NewLine);
        }

        // Generate Food/Drink
        private void button42_Click(object sender, EventArgs e)
        {
            
            AppendText(Environment.NewLine + "FOOD/DRINK: ");
            AppendFoodDrinkText(percentChance(66));
            AppendText(Environment.NewLine);
        }

        public void AppendFoodDrinkText(bool isFood)
        {
            string str = "";
            if (!isFood)
            {
                str = masterFile.Drinks.Random();
            }
            else
            {
                str = masterFile.Foods.Random();
            }

            if (percentChance(20))
            {
                var temps = new string[]
                {
                    "Hot",
                    "Cold",
                    "Freezing",
                    "Steaming",
                    "Face-melting hot",
                    "Lukewarm",
                    "Room Temperature",
                    "Mild",
                    "Spicy",
                    "Warm",
                    "Sat-under-a-heating-lamp-warm"
                };
                AppendText(temps.Random() + " ");
            }

            if (percentChance(20))
            {
                AppendText(UppercaseFirst(masterFile.Adjectives.Random()) + " ");
            }

            if (percentChance(15))
            {
                AppendRandomColor();
                str = " " + str;
            }

            AppendText(str);
        }

        private MasterJson.JsonColor GenerateColor()
        {
            if (percentChance(15))
            {
                var r = randomNumber(0, 255);
                var g = randomNumber(0, 255);
                var b = randomNumber(0, 255);
                var rgb = "#" + r.ToString("X") + g.ToString("X") + b.ToString("X");
                return new MasterJson.JsonColor
                {
                    ColorName = UppercaseFirst(percentChance(70) ? masterFile.Objects.Random() : masterFile.Adjectives.Random()) + " " + UppercaseFirst(masterFile.SimpleColors.Random()),
                    HexCode = rgb,
                };
            }
            return masterFile.Colors.Random();
        }

        // Roll Furniture:
        private void button35_Click(object sender, EventArgs e)
        {
            AppendText(Environment.NewLine + "TABLE: ");
            AppendRandomColor();
            AppendText(" " + masterFile.Materials.Random() + " " + masterFile.TableFurniture.Random());
            AppendText(" | CHAIR: ");
            AppendRandomColor();
            AppendText(" " + masterFile.Materials.Random() + " " + masterFile.SittingFurniture.Random());
            AppendText(Environment.NewLine);
        }

        // Roll Mood:
        private void button36_Click(object sender, EventArgs e)
        {
            AppendText(Environment.NewLine + "MOOD: " + masterFile.Moods.Random() + Environment.NewLine);
        }

        // Generate Personality
        private void button37_Click(object sender, EventArgs e)
        {
            AppendText(Environment.NewLine + "PERSONALITY TRAIT: " + masterFile.Personalities.Random() + Environment.NewLine);
        }

        // Generate Monster Type
        private void button41_Click(object sender, EventArgs e)
        {
            var m = GenerateMonster(true) as Monster;
            AppendText(Environment.NewLine);
            m.AppendRaceString(this.richTextBox1.RichTextBox);
            AppendText(Environment.NewLine);
        }


        public class Monster : Person
        {
            public string _race1 { get; set; }
            public MasterJson.JsonColor _racecolor { get; set; }
            public string _race2 { get; set; }
            public int level { get; set; }
            public void AppendRaceString(RichTextBox toAppendTo)
            {
                if (!this.isPerson && _racecolor != null)
                {
                    if (this._race2 == null)
                    {
                        AppendColorStatic(toAppendTo, this._racecolor);
                        AppendTextStatic(toAppendTo, " " + this._race1);
                    }
                    else
                    {
                        AppendTextStatic(toAppendTo, this._race1 + " ");
                        AppendColorStatic(toAppendTo, this._racecolor);
                        AppendTextStatic(toAppendTo, " " + this._race2);
                    }

                }
                else
                {
                    AppendTextStatic(toAppendTo, this._race1);
                }
            }

            public override void AppendMyString(RichTextBox toAppendTo, string personLabel = "ENEMY")
            {
                if (this.isPerson)
                {
                    this.AppendMyString(toAppendTo, personLabel);
                }
                else
                {
                    if (personLabel == "ENEMY") { personLabel = "MONSTER"; }

                    AppendTextStatic(toAppendTo, Environment.NewLine + $"{personLabel}: ");
                    this.AppendRaceString(toAppendTo);

                    AppendTextStatic(toAppendTo, " - LEVEL " + this.level + " | MAX HP: " + this.maxHealth + " | MAX MP: " + this.maxMP);
                    AppendTextStatic(toAppendTo, Environment.NewLine + $"Intelligence {this.intelligence} | Strength  {this.strength} | Presence     {this.presence}", null, false, true);
                    AppendTextStatic(toAppendTo, Environment.NewLine + $"Wit          {this.wits} | Dexterity {this.dexterity} | Manipulation {this.manipulation}", null, false, true);
                    AppendTextStatic(toAppendTo, Environment.NewLine + $"Resolve      {this.resolve} | Stamina   {this.stamina} | Composure    {this.composure}", null, false, true);

                    if (KnownSpells.Any())
                    {
                        foreach (var s in KnownSpells)
                        {
                            AppendTextStatic(toAppendTo, Environment.NewLine + $"· " + s.spellName + " (" + s.mpCost + " MP) | " + s.spellElement + " | ");
                            AppendColorStatic(toAppendTo, s.color);
                        }
                    }

                    AppendTextStatic(toAppendTo, Environment.NewLine + $"Carries {money} gold.");
                    if (OnHand.Count > 0)
                    {
                        foreach (var item in OnHand)
                        {
                            AppendTextStatic(toAppendTo, Environment.NewLine + $"· ");
                            item.AppendMyString(toAppendTo);
                        }
                    }
                }
            }
        }

        public int monsterChance = 65;
        public Person GenerateMonster(bool forceMonster = false)
        {
            if (forceMonster || percentChance(monsterChance))
            {

                var m = new Monster()
                {
                    Skills = GenerateSkills(),
                    OnHand = new List<Item>(),
                    KnownSpells = new List<Spell>(),
                };

                m.isPerson = false;
                var race = masterFile.MonsterRaces.Random();

                if (percentChance(10))
                {
                    race = "Giant " + race;
                }
                else if (percentChance(7))
                {
                    race = "Mini " + race;
                }

                if (race.Contains("[Material]")) { race = race.Replace("[Material]", masterFile.Materials.Random()); }
                if (race.Contains("[Element]")) { race = race.Replace("[Element]", masterFile.Elements.Random()); }

                if (race.Contains("[Color]"))
                {
                    m._racecolor = GenerateColor();
                    if (race.StartsWith("[Color]"))
                    {
                        m._race1 = race.Replace("[Color]", "").Trim();
                        m._race2 = null;
                    }
                    else
                    {
                        var colorSplit = race.Split(' ');
                        var strs = new List<string>();
                        foreach (var a in colorSplit)
                        {
                            if (a == "[Color]")
                            {
                                m._race1 = string.Join(" ", strs.ToArray());
                                strs = new List<string>();
                            }
                            else
                            {
                                strs.Add(a);
                            }
                        }

                        if (strs.Any())
                        {
                            m._race2 = string.Join(" ", strs.ToArray());
                        }
                    }
                }
                else
                {
                    m._race1 = race;
                    m._race2 = null;
                    m._racecolor = null;
                }

                var classes = GenerateClasses(true);
                m.level = classes.Sum(x => x.classLevel);
                m.SetRandomStats();

                while (rnd.Next(0, 100) < 45)
                {
                    var toAdd = GenerateRandomLoot(null);
                    if (toAdd != null)
                    {
                        m.OnHand.Add(toAdd);
                    }
                }

                var knownSpells = (percentChance(60) ? 0 : randomNumber(1, 3));

                while (knownSpells-- > 0)
                {
                    var toAdd = GenerateSpell();
                    if (toAdd != null)
                    {
                        m.KnownSpells.Add(toAdd);
                    }
                }

                return m;
            }
            else
            {
                return GeneratePerson();
            }
        }

        // Generate Potion
        private void button43_Click(object sender, EventArgs e)
        {
           
            AppendText(Environment.NewLine);
            var loot = GenerateRandomLoot(Item.ItemType.Potion);
            if (loot == null)
            {
                AppendText("I haven't programmed in that path yet");
            }
            else
            {
                loot.AppendMyString(richTextBox1.RichTextBox);
            }
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        // Skill List:
        private void button46_Click(object sender, EventArgs e)
        {
            
            var skills = GenerateSkills();
            AppendText(Environment.NewLine);
            int n = 0;
            foreach (var s in skills)
            {
                n++;
                if (s.specialty != null)
                {
                    AppendText(s.skillName + ": " + s.dots + (s.specialty != null ? (" (" + s.specialty + ")") : ""), null, true);
                    if (n == skills.Count)
                    {
                        AppendText(Environment.NewLine);
                    }
                    else
                    {
                        AppendText(" | ");
                    }
                }
                else
                {
                    AppendText(s.skillName + ": " + s.dots + (s.specialty != null ? (" (" + s.specialty + ")") : "") + (n == skills.Count ? Environment.NewLine : " | "));
                }
            }
        }

        public List<Person.Skill> GenerateSkills()
        {
            var Skills = new List<Person.Skill>();
            foreach (var skill in masterFile.NWODSkills)
            {
                int dotsInSkill = 0;
                for (var i = 0; i < 5; i++)
                {
                    // So there's 24 skills, 5 dots each
                    // for "fun" randomization, that's 120 potential slots for dots
                    // you get 22 dots to put in, which is 22/120 = 18.3333% for each dot to happen
                    // to make this more fun we'll bump up the chance a little:
                    dotsInSkill += (percentChance(19) ? 1 : 0);
                }
                if (dotsInSkill > 0)
                {
                    Skills.Add(new Person.Skill { dots = dotsInSkill, skillName = skill });
                }
            }

            if (Skills.Any())
            {
                // ok now we need 3ish skills to add specialties to:
                var specialtycount = randomNumber(0, 5);
                while (specialtycount-- > 0)
                {
                    Skills.ToArray().Random().specialty = masterFile.NWODSpecialties.Random();
                }
            }

            return Skills;
        }

        // Roll Initiative
        private void button47_Click(object sender, EventArgs e)
        {

            var roll = randomNumber(1, 100);
            decimal val = ((decimal)roll) + Convert.ToDecimal(initiativeDexValue.Value * 10) + Convert.ToDecimal(initiativeComposureValue.Value * 10);
            val = (val / (decimal)10);
            AppendText(Environment.NewLine);
            AppendText($"Initiative (DEX: {initiativeDexValue.Value}, CMP: {initiativeComposureValue.Value}): " + val.ToString() + " (Rolled " + roll.ToString() + ")");
            AppendText(Environment.NewLine);
            scrollToBottom();
        }

        // roll location
        private void button48_Click(object sender, EventArgs e)
        {
            AppendText(Environment.NewLine + "LOCATION: " + GenerateTravelLocation() + Environment.NewLine);
        }

        private void button49_Click(object sender, EventArgs e)
        {
            var daysToTravel = Convert.ToDecimal(this.travelDays.Value);
            if (daysToTravel <= 0)
            {
                return;
            }

            bool isDay = this.dayNight.Checked;
            int numNights = 0;
            int numDays = 0;


            while (daysToTravel > 0)
            {
                daysToTravel -= 0.5m;

                if (isDay)
                {
                    numDays++;
                    AppendText(Environment.NewLine + "The " + AddOrdinal(numDays) + " day, you travel across a " + GenerateTravelLocation() + "                     (" + GenerateWeather() + ")");
                }
                else
                {
                    numNights++;
                    AppendText(Environment.NewLine + "The " + AddOrdinal(numNights) + " night, you travel across a " + GenerateTravelLocation() + "                    (" + GenerateWeather() + ")");
                }

                if (percentChance(23))
                {
                    var disc = GenerateDiscovery();
                    AppendText(Environment.NewLine + "You discover a ");
                    disc.AppendMyString(this.richTextBox1.RichTextBox);
                    var enc = GenerateEncounter();
                    enc.AppendMyString(this.richTextBox1.RichTextBox);
                    break;
                }
                else if (percentChance(7))
                {
                    var enc = GenerateDiscovery();
                    AppendText(Environment.NewLine + "You discover a ");
                    enc.AppendMyString(this.richTextBox1.RichTextBox);
                    break;
                }
                isDay = !isDay;
            }


            this.dayNight.Checked = isDay;

            if (daysToTravel > 0)
            {
                this.travelDays.Value = daysToTravel;
                AppendText(Environment.NewLine + "There " + (daysToTravel == 1 ? "is" : "are") + " still " + daysToTravel.ToString() + " day" + (daysToTravel == 1 ? "" : "s") + " left to travel" + Environment.NewLine);
            }
            else
            {
                AppendText(Environment.NewLine + "You arrive at your destination" + Environment.NewLine);
            }
        }

        public string GenerateTravelLocation()
        {
            var loc = masterFile.TravelLocations.Random();
            if (loc.Contains("[Material]"))
            {
                loc = loc.Replace("[Material]", masterFile.Materials.Random());
            }
            else if (percentChance(10))
            {
                loc = new string[] {
                    "Windy",
                    "Winding",
                    "Rough",
                    "Poisonous",
                    "Calm",
                    "Safe",
                    "Dark",
                    "Big",
                    "Wide",
                    "Thin",
                    "Small",
                    "Treacherous",
                    "Icy",
                    "Simple",
                    "Sandy",
                    "Mossy",
                    "Wet",
                    "Muddy"
                }.Random() + " " + loc;
            }

            return loc;
        }

        public static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd"; 
                default:
                    return num + "th";
            }

        }
        public class Encounter
        {
            public bool isFriendly { get; set; }
            public string description { get; set; }
            public int carriedLootItems { get; set; }
            public string carriedLootBox { get; set; }

            public class EncounterCreature
            {
                public bool isFriendly { get; set; }
                public int count { get; set; }
                public string action { get; set; }
                public string creatureType { get; set; }
            }

            public List<EncounterCreature> creatures { get; set; }

            public void AppendMyString(RichTextBox toAppendTo)
            {
                AppendTextStatic(toAppendTo, Environment.NewLine + (isFriendly ? "Friendly" : "Enemy") + " Encounter", null, true, false);
                AppendTextStatic(toAppendTo, " " + this.description);
                foreach (var creature in creatures)
                {
                    var str = "";
                    str += (creature.isFriendly ? "Friendly" : "Unfriendly") + " ";
                    if (creature.count == 1)
                    {
                        str += creature.creatureType;
                    }
                    else
                    {
                        str += "group of " + creature.count + " " + creature.creatureType + "s";
                    }
                    str += " | " + creature.action;
                    AppendTextStatic(toAppendTo, Environment.NewLine + str);
                    
                }
                if (carriedLootItems > 0)
                {
                    AppendTextStatic(toAppendTo, Environment.NewLine + "There is a " + this.carriedLootBox + " with " + this.carriedLootItems + " random loot item(s)");
                }
            }
        }

        public Encounter GenerateEncounter(bool? isFriendly = null)
        {
            if (isFriendly == null)
            {
                isFriendly = percentChance(60);
            }

            MasterJson.Encounter enc = masterFile.encounters.Where(x => x.isFriendly == null || (x.isFriendly.HasValue && x.isFriendly.Value == isFriendly)).ToArray().Random();

            var encounter = new Encounter
            {
                isFriendly = isFriendly.Value,
                description = enc.description,
                creatures = new List<EncounterCreature>(),
            };

            foreach (var c in enc.creatures)
            {
                var isFriendlyCreature = c.isFriendly ?? (percentChance(50));
                string creature = null;
                creature = c.type;
                if (c.type == "any")
                {
                    creature = percentChance(monsterChance) ? "monster" : "humanoid";
                }

                var count = 0;
                var split = c.count.Split('-');
                if (split.Length == 1)
                {
                    count = int.Parse(c.count);
                }
                else
                {
                    count = randomNumber(int.Parse(split[0]), int.Parse(split[1]));
                }

                encounter.carriedLootItems = randomNumber(0, 4);
                encounter.carriedLootBox = new string[]
                {
                    "Safe",
                    "Locked Safe",
                    "Safe with Broken Lock",
                    "Chest",
                    "Locked Chest",
                    "Treasure Chest",
                    "Locked Treasure Chest",
                    "Sack",
                    "Backpack",
                    "Saddlebag",
                    "Box",
                    "[Material] Box",
                    "Mimic Chest",
                    "Satchel",
                    "Barrel",
                    "Knapsack"
                }.Random().Replace("[Material]", masterFile.Materials.Random());

                encounter.creatures.Add(new EncounterCreature
                {
                    action = c.action,
                    count = count,
                    isFriendly = isFriendlyCreature,
                    creatureType = creature,
                });
            }

            return encounter;
        }

        public class Discovery
        {
            public string location { get; set; }

            public void AppendMyString(RichTextBox toAppendTo)
            {
                AppendTextStatic(toAppendTo, this.location);
            }
        }

        public Discovery GenerateDiscovery()
        {
            var discovery = new Discovery
            {
                location = GenerateEncounterLocation()
            };

            return discovery;
        }

        public string GenerateEncounterLocation()
        {
            var loc = masterFile.EncounterLocations.Random();

            if (percentChance(10))
            {
                loc = new string[] {
                    "Sunken",
                    "Secluded",
                    "Guarded",
                    "Abandoned",
                    "Abandoned",
                    "Abandoned",
                    "Abandoned",
                    "Fortified",
                }.Random() + " "+ loc;
            }

            return loc;
        }

        // Generate Discovery:
        private void button50_Click(object sender, EventArgs e)
        {
            AppendText(Environment.NewLine + "Discovery: ");
            GenerateDiscovery().AppendMyString(this.richTextBox1.RichTextBox);
            AppendText(Environment.NewLine);
        }

        // Generate Encounter:
        private void button44_Click(object sender, EventArgs e)
        {
            GenerateEncounter().AppendMyString(this.richTextBox1.RichTextBox);
            AppendText(Environment.NewLine);
        }

        // Generate Weather
        private void button51_Click(object sender, EventArgs e)
        {
            AppendText(Environment.NewLine + "Weather: " + GenerateWeather() + Environment.NewLine);
        }

        public string GenerateWeather()
        {
            var str = "";
            str += new string[]
            {
                "Sunny",
                "Foggy",
                "Cloudy",
                "Raining",
                "Sprinkling",
                "Downpouring",
                "Snowing",
                "Hailing",
                "Sleeting",
                "Freezing rain",
                "Misting",
                "Humid",
                "Dry",
                "Sunny",
                "Sunny",
                "Sunny",
                "Sunny",
                "Sunny",
                "Sunny",
                "Thunderstorms",
                "Sandstorms",
                "Blizzard"
            }.Random();

            str += " with " + new string[] {
                "strong",
                "no",
                "slight",
                "occaisonal",
                "fast",
                "gusting",
                "refreshing",
                "breezy",
                "chilling",
                "light"
                }.Random() + " winds, and " + 
                new string[] {
                    "hot",
                    "cold",
                    "very hot",
                    "very cold",
                    "freezing",
                    "extremely hot",
                    "a mild temperature",
                    "warm",
                    "a comfortable temperature"
                    }.Random();

            return str;
        }
    }
}


static class Extension
{
    static Random rnd = new Random();
    public static T Random<T>(this T[] array)
    {
        int r = rnd.Next(array.Length);
        return array[r];
    }


}
