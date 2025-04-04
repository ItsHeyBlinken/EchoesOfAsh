using System;
using System.Collections.Generic;

namespace EchoesOfAshWeb.Data
{
    /// <summary>
    /// Contains static data for the game
    /// </summary>
    public static class GameData
    {
        // Random descriptions for locations when searching
        public static readonly List<string> WastelandDescriptions = new List<string>
        {
            "The barren landscape stretches out before you, broken only by the occasional twisted piece of metal or crumbling concrete.",
            "Dust swirls around your feet as you walk across the scorched earth. Nothing grows here anymore.",
            "The remains of a highway stretch into the distance, littered with rusted vehicles frozen in their final moments.",
            "A hot, irradiated wind blows across the wasteland, carrying the scent of decay and chemicals.",
            "The ground crunches beneath your feet, a mixture of ash and pulverized concrete.",
            "In the distance, you can see the skeletal remains of what might have been a small town.",
            "The sky above is a sickly yellow-gray, the sun barely visible through the perpetual haze."
        };

        public static readonly List<string> CityDescriptions = new List<string>
        {
            "Towering skyscrapers, now broken and hollow, cast long shadows across the debris-filled streets.",
            "The city is eerily quiet, save for the occasional sound of collapsing metal or glass.",
            "Abandoned vehicles line the streets, some crushed beneath fallen concrete, others simply left where they stopped.",
            "You navigate through a maze of collapsed buildings and blocked streets, always alert for danger.",
            "Papers and trash swirl in the wind between the buildings, ghostly remnants of the city's former life.",
            "The buildings here lean precariously, their structural integrity compromised by the blast and years of neglect.",
            "Shattered glass crunches beneath your feet as you move carefully through the urban ruins."
        };

        public static readonly List<string> ForestDescriptions = new List<string>
        {
            "The trees here are twisted and mutated, their bark glowing faintly with an unnatural light.",
            "Strange fungi grow in patches, emitting a soft blue luminescence that provides just enough light to see by.",
            "The vegetation is dense and alien, nothing like the forests of the old world.",
            "Bizarre sounds echo through the trees - clicks, whistles, and low moans that could be animal or something else entirely.",
            "The air is thick with spores and the sweet-rot smell of mutated vegetation.",
            "Vines with thorns as long as your finger hang from the twisted branches, ready to snag the unwary.",
            "Despite the radiation, life finds a way here, though changed into forms barely recognizable."
        };

        // Random encounters
        public static readonly List<string> RandomEncounters = new List<string>
        {
            "You spot a group of raiders in the distance. You hide until they pass by.",
            "A radiation storm approaches. You take shelter until it passes, but still absorb some radiation.",
            "You find a small cache of supplies hidden in a hollow tree stump.",
            "A mutated animal crosses your path, but doesn't seem interested in you.",
            "You come across the remains of another survivor, their supplies picked clean.",
            "The ground trembles slightly beneath your feet. An aftershock, or something else?",
            "You spot a column of smoke in the distance. It could be other survivors, or a trap.",
            "A military drone flies overhead, still following its pre-war patrol route.",
            "You find an old bunker door, but it's sealed tight and the access panel is dead.",
            "The remains of a campsite suggest others passed through here recently."
        };

        // Survival tips that can be displayed to the player
        public static readonly List<string> SurvivalTips = new List<string>
        {
            "Tip: Always keep some anti-radiation medicine with you when exploring irradiated areas.",
            "Tip: Food and water are your most important resources. Never pass up a chance to collect them.",
            "Tip: Some locations are safer to rest in than others. Look for shelter when your health is low.",
            "Tip: Your Geiger counter will click faster in areas with high radiation. Listen for it.",
            "Tip: Not all survivors are friendly. Approach strangers with caution.",
            "Tip: Weapons break with use. Try to have a backup weapon available.",
            "Tip: Radiation sickness will slowly damage your health. Treat it as soon as possible.",
            "Tip: The wasteland is more dangerous at night. Try to find shelter before dark.",
            "Tip: Some mutated plants and animals can be consumed for food, but may increase your radiation.",
            "Tip: Trading with other survivors can be a good way to get supplies you need."
        };

        // Quotes that can be displayed during loading or at the start of the game
        public static readonly List<string> ApocalypticQuotes = new List<string>
        {
            "War. War never changes.",
            "It is not the strongest of the species that survives, but the one most adaptable to change.",
            "In the wasteland, hope is as precious as clean water.",
            "The world ended not with a whimper, but with a bang.",
            "Survival is insufficient.",
            "The end of the world was just the beginning of our story.",
            "When everything is lost, the future still remains.",
            "In a world of ash, even the smallest flame is a miracle.",
            "The old world died so that something new could be born.",
            "We are not the last of the old; we are the first of the new."
        };
    }
}
