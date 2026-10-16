using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using HollowSaint.Core;
using HollowSaint.Investigation;

namespace HollowSaint.EditorTools
{
    /// <summary>
    /// Seeds every content asset (tools, bestiary, clues, cases, GameConfig) from code.
    /// Re-running updates the assets in place, keeping their GUIDs, so scene references survive.
    /// After the first build, writers can edit the assets directly in the Inspector; just don't
    /// re-run this unless the text here is the source of truth you want.
    /// </summary>
    public static class ContentBuilder
    {
        const string DataRoot = "Assets/Data";
        public const string ConfigPath = "Assets/Resources/GameConfig.asset";

        public const string HotelScene = "Hotel";
        public const string FamishedScene = "Case_Famished";
        public const string EchoScene = "Case_Echo";
        public const string LodgerScene = "Case_Lodger";

        /// <summary>Case assets in play order.</summary>
        public static readonly string[] CasePaths =
        {
            DataRoot + "/Cases/TheFamished/Case_TheFamished.asset",
            DataRoot + "/Cases/TheEcho/Case_TheEcho.asset",
            DataRoot + "/Cases/TheLodger/Case_TheLodger.asset",
            DataRoot + "/Cases/TheMirrorBorn/Case_TheMirrorBorn.asset",
        };

        const string D = Speakers.Dutch;
        const string N = Speakers.Narration;
        const string R = Speakers.Radio;
        const string M = Speakers.Reflection;

        static DialogueLine L(string speaker, string text) => new DialogueLine(speaker, text);

        static T Asset<T>(string path) where T : ScriptableObject
        {
            var a = AssetDatabase.LoadAssetAtPath<T>(path);
            if (a != null) return a;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            a = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(a, path);
            return a;
        }

        static ToolData Tool(string file, string id, string name, string description, string mirrorBornLine)
        {
            var t = Asset<ToolData>($"{DataRoot}/Tools/{file}.asset");
            t.id = id;
            t.displayName = name;
            t.description = description;
            t.mirrorBornLine = mirrorBornLine;
            EditorUtility.SetDirty(t);
            return t;
        }

        static BestiaryEntry Entry(string file, string id, string name, string classification, string lore,
            string[] symptoms, string weakness, ToolData tool, bool handwritten = false, bool hidden = false)
        {
            var e = Asset<BestiaryEntry>($"{DataRoot}/Bestiary/{file}.asset");
            e.id = id;
            e.displayName = name;
            e.classification = classification;
            e.lore = lore;
            e.symptoms = new List<string>(symptoms);
            e.weaknessText = weakness;
            e.weaknessTool = tool;
            e.handwritten = handwritten;
            e.hiddenUntilItsCase = hidden;
            EditorUtility.SetDirty(e);
            return e;
        }

        static ClueData Clue(string caseFolder, string file, string id, string title, string description, string symptom)
        {
            var c = Asset<ClueData>($"{DataRoot}/Cases/{caseFolder}/{file}.asset");
            c.id = id;
            c.title = title;
            c.description = description;
            c.symptom = symptom;
            EditorUtility.SetDirty(c);
            return c;
        }

        [MenuItem("Hollow Saint/Build Content")]
        public static void BuildAll()
        {
            // ---------------------------------------------------------------- tools
            var revolver = Tool("Tool_Revolver", "revolver", "Revolver",
                "Six rounds of blessed lead. Works on most things with a heartbeat.",
                "Six rounds. Nothing in this room to point them at. Except one thing, and he's the one holding the gun.");
            var nails = Tool("Tool_IronNails", "iron_nails", "Iron nails",
                "A fistful of cold iron. Shuts doors that shouldn't open.",
                "Nail what shut? I'm the one who never opens the door.");
            var holyWater = Tool("Tool_HolyWater", "holy_water", "Holy water",
                "Hip flask, blessed in Lisbon. Probably.",
                "It runs off my hands like tap water. It always has.");
            var tableSalt = Tool("Tool_TableSalt", "table_salt", "Table salt",
                "A paper twist of diner salt. Better than nothing, usually.",
                "A salt line around what? The bed? The chair? Me?");
            var rockSalt = Tool("Tool_RockSalt", "rock_salt", "Rock salt (diner kitchen)",
                "Coarse grey rock salt from the Harbor Street Diner's own kitchen.",
                "Salt from a dead man's kitchen. This isn't his kitchen. It's my room.");
            var spoon = Tool("Tool_SilverSpoon", "silver_spoon", "Silver spoon (L.A.)",
                "Tarnished silver, engraved L.A. The twelfth of a set.",
                "I gave one of these back already. Nothing of mine is missing. That's the problem.");
            var pen = Tool("Tool_RegisterPen", "register_pen", "Fountain pen (Room 9)",
                "The pen the thing in Room 9 signed the register with. Nib still wet.",
                "Strike whose name? Every night the register says the same one. Marlowe.");

            // ---------------------------------------------------------------- bestiary
            // Page text for the four real cases: journal voice, in Dutch's hand (Design/unity_dev_final.md).
            var famished = Entry("Entry_Famished", "famished", "The Famished", "Harbor Street Diner, kitchen",
                "Signs: cold rot under the cold-storage smell. Lights that won't hold steady — it pulls power like it pulls everything else, " +
                "more than it needs, like it forgot how to stop. Claw marks on the inside of the pantry door. Inside. Something that wants in " +
                "claws at the outside. Something that already got what it wanted claws to get out — and didn't make it.",
                new[] { "cold rot smell", "flickering electronics", "claw marks on the inside of doors" },
                "Fix: A line of rock salt. Has to be from that kitchen specifically — whatever was in the shaker the night it died hungry. " +
                "Store-bought doesn't hold. I don't know why. I've stopped asking why about half of what's in this book.\n" +
                "<i>Don't ask the owner before you take the salt. Just don't.</i>",
                rockSalt);
            var saltWidow = Entry("Entry_SaltWidow", "salt_widow", "The Salt Widow", "Drowned revenant",
                "Walks up out of the harbour on the anniversary of the drowning, looking for the house she left. " +
                "Harmless if you don't answer the door.",
                new[] { "wet footprints", "cold rot smell", "weeping at low tide" },
                "Return her wedding ring to the water at the hour she went in.", null);
            var echo = Entry("Entry_Echo", "echo", "The Echo", "Lantern Lane, third-floor flat",
                "Signs: whispers that repeat the same four words, never more. Things moved an inch from where they were left — never thrown, " +
                "never broken, just wrong, like someone tidied a room they used to live in. Cold spots that gather near glass. Mirrors, windows, " +
                "a picture frame. Anything that used to hold a reflection it recognized.",
                new[] { "repeated whispers", "objects slightly moved", "cold spots near mirrors" },
                "Fix: Give it back what's its. A spoon from its own set — not silver in general, not a replacement, its spoon, the one still in " +
                "a drawer somewhere in that flat. It's not angry. It's confused. Confused is worse. Angry wants to hurt you. Confused just wants " +
                "you to stay until it figures out why you're not who it's waiting for.",
                spoon);
            var lanternSaint = Entry("Entry_LanternSaint", "lantern_saint", "The Lantern Saint", "Relic-spirit",
                "A votive flame that outlived its church. Draws moths, stops clocks and leaves wax on the sills of " +
                "people who pray too late at night.",
                new[] { "flickering electronics", "moths in sealed rooms", "stopped clocks" },
                "Snuff the votive with bare fingers. No glove, no breath.", null);
            var lodger = Entry("Entry_Lodger", "lodger", "The Lodger", "Caravanserai Inn, Room 9",
                "Signs: clocks stopped at the same minute, all of them, like the room agreed on a time and quit. Ashtrays that fill overnight " +
                "with nobody smoking. A bed made every morning that nobody's slept in — made better than housekeeping makes it, like someone's " +
                "still proud of the work.",
                new[] { "stopped clocks", "cigarette burns, no smoker", "an unslept bed" },
                "Fix: <i>(unfinished — the ink stops here)</i>\n" +
                "<i>My watch stopped at 3:12 four nights running. I told myself it was the battery. I didn't check the battery.</i>",
                pen);
            var mirrorBorn = Entry("Entry_MirrorBorn", "mirror_born", "[untitled]", "Room 414",
                "This page was already written when I opened to it. Not my handwriting — my hand, moving the way it moves, but I didn't write this. " +
                "Five of the six signs are already checked. I didn't check them. They checked themselves, one case at a time, every night I told " +
                "myself it was nothing: a watch stuck at 3:12, an ashtray I don't remember filling, a bed I don't remember not sleeping in.",
                new[] { "repeated whispers", "cold spots near mirrors", "stopped clocks", "cigarette burns, no smoker",
                        "an unslept bed", "a flight that never leaves" },
                "Fix: Every tool in the bag. Tried them all, in order, out of habit more than hope. Each one fails for its own reason — not because " +
                "the page is wrong. Because I'm reading it wrong. Because I keep looking at the room for the thing that's supposed to be in it.\n" +
                "The sixth line, when everything else is crossed out, is one word: <b>Look.</b>",
                null, handwritten: true, hidden: true);

            // ---------------------------------------------------------------- case 1: The Famished
            var cFam = Asset<CaseData>($"{DataRoot}/Cases/TheFamished/Case_TheFamished.asset");
            cFam.id = "the_famished";
            cFam.title = "The Famished";
            cFam.sceneName = FamishedScene;
            cFam.entity = famished;
            cFam.isFinal = false;
            cFam.clues = new List<ClueData>
            {
                Clue("TheFamished", "Clue_RotPatch", "famished.rot_patch", "Rotten Patch",
                    "Floor's soft here. Cold, too, meat-locker cold, and it stinks of rot.", "cold rot smell"),
                Clue("TheFamished", "Clue_FlickeringLamp", "famished.flickering_lamp", "Flickering Lamp",
                    "Lamp keeps cutting out. Bulb's fine, wiring's fine. It just doesn't want to stay on while I'm near it.",
                    "flickering electronics"),
                Clue("TheFamished", "Clue_ClawedDoor", "famished.clawed_door", "Clawed Door",
                    "Pantry door. Deep gouges, on the inside. Whatever made them was locked in, not out.",
                    "claw marks on the inside of doors"),
            };
            cFam.edition = "Baku  ·  Tuesday edition  ·  Vol. XII, No. 201";
            cFam.flavorHeadlines = new List<string>
            {
                "Caspian Fishery Quotas Under Review Again",
                "Old City Tram Line to Reopen \"Next Spring\", Says Ministry",
                "Heatwave Expected to Break by Friday",
            };
            cFam.leadHeadline = "Night Staff Flee Shuttered Diner. \"Scratching\" Heard From Locked Pantry";
            cFam.leadDetail = "Reported near 2 AM, Harbor Street Diner, Harbor Street.";
            cFam.leadReaction = new List<DialogueLine> { L(D, "Harbor Street. Walking distance. Beats staring at the ceiling.") };
            cFam.arrivalLines = new List<DialogueLine>
            {
                L(D, "Harbor Street Diner. Shut a month, smells like a year."),
                L(D, "Look around before anything looks at me. The Book's in my coat if I need it."),
            };
            cFam.bookRevealLines = new List<DialogueLine>();
            cFam.identifyLines = new List<DialogueLine>
            {
                L(D, "The Famished. A cook who starved in his own kitchen and came back hungry."),
                L(D, "Rock salt from the kitchen where it died. The pantry, then."),
            };
            cFam.wrongToolLine = "I try the {0}. The scratching behind the door gets louder. Wrong tool.";
            cFam.resolveLines = new List<DialogueLine>
            {
                L(D, "A line of the kitchen's own rock salt across the threshold."),
                L(N, "Something behind the pantry door breathes out, long and tired, and the cold goes with it."),
                L(D, "Eat well, chef."),
            };
            EditorUtility.SetDirty(cFam);

            // ---------------------------------------------------------------- case 2: The Echo
            var cEcho = Asset<CaseData>($"{DataRoot}/Cases/TheEcho/Case_TheEcho.asset");
            cEcho.id = "the_echo";
            cEcho.title = "The Echo";
            cEcho.sceneName = EchoScene;
            cEcho.entity = echo;
            cEcho.isFinal = false;
            cEcho.clues = new List<ClueData>
            {
                Clue("TheEcho", "Clue_WhisperingVent", "echo.whispering_vent", "Whispering Vent",
                    "A whisper through the vent. \"Set it for two.\" Four words, then again. Same breath, same pause, never more.",
                    "repeated whispers"),
                Clue("TheEcho", "Clue_MovedCandlesticks", "echo.moved_candlesticks", "Moved Candlesticks",
                    "Candlesticks on the sideboard, each one an inch off its own dust ring. Something keeps moving things back. Almost.",
                    "objects slightly moved"),
                Clue("TheEcho", "Clue_HallMirror", "echo.hall_mirror", "Hall Mirror",
                    "Hallway mirror. My breath fogs in front of it and nowhere else in the flat.",
                    "cold spots near mirrors"),
            };
            cEcho.edition = "Baku  ·  Thursday edition  ·  Vol. XII, No. 203";
            cEcho.flavorHeadlines = new List<string>
            {
                "Carpet Museum Extends Evening Hours",
                "Taxi Fares to Rise Ahead of Festival Season",
                "Local Chess Club Takes Regional Title",
            };
            cEcho.leadHeadline = "Widow's Flat on Lantern Lane Empty Since Spring. Neighbours Still Hear the Table Being Set";
            cEcho.leadDetail = "Heard every evening around 7 PM, Lantern Lane No. 6.";
            cEcho.leadReaction = new List<DialogueLine> { L(D, "Lantern Lane. Dinner for one at seven, every night. Sure.") };
            cEcho.arrivalLines = new List<DialogueLine>
            {
                L(D, "Lantern Lane, number six, third floor. Dining room's been dusted. By who?"),
            };
            cEcho.bookRevealLines = new List<DialogueLine>();
            cEcho.identifyLines = new List<DialogueLine>
            {
                L(D, "The Echo. Not a ghost. A habit with nobody left to have it."),
                L(D, "It's bound to something it lost. The silver on the table, then."),
            };
            cEcho.wrongToolLine = "I try the {0}. The whisper doesn't even pause.";
            cEcho.resolveLines = new List<DialogueLine>
            {
                L(D, "The spoon goes back in its slot. Twelve of twelve."),
                L(N, "The whispering stops mid-word. The table is set. Nobody comes to eat."),
                L(D, "Dinner's served, Mrs. A."),
            };
            EditorUtility.SetDirty(cEcho);

            // ---------------------------------------------------------------- case 3: The Lodger
            var cLodger = Asset<CaseData>($"{DataRoot}/Cases/TheLodger/Case_TheLodger.asset");
            cLodger.id = "the_lodger";
            cLodger.title = "The Lodger";
            cLodger.sceneName = LodgerScene;
            cLodger.entity = lodger;
            cLodger.isFinal = false;
            cLodger.clues = new List<ClueData>
            {
                Clue("TheLodger", "Clue_StoppedClock", "lodger.stopped_clock", "Stopped Clock",
                    "Clock on the wall stopped at 3:12. Huh.", "stopped clocks"),
                Clue("TheLodger", "Clue_BurnedSill", "lodger.burned_sill", "Burned Windowsill",
                    "Windowsill's covered in cigarette burns. Clerk swears nobody on this floor smokes.",
                    "cigarette burns, no smoker"),
                Clue("TheLodger", "Clue_UnsleptBed", "lodger.unslept_bed", "Unslept Bed",
                    "Bed's made tight. Hospital corners. Nobody's slept in it in weeks, and the room's still warm.",
                    "an unslept bed"),
            };
            cLodger.edition = "Baku  ·  Sunday edition  ·  Vol. XII, No. 206";
            cLodger.flavorHeadlines = new List<string>
            {
                "Hotel Occupancy Hits Record High",
                "Airport Delays Continue Into Second Week",
                "Lost Luggage Auction Draws Crowds",
            };
            cLodger.leadHeadline = "Caravanserai Inn: Room 9 Guest \"Never Checked Out\". Staff Report Warm Room, Untouched Bed";
            cLodger.leadDetail = "Light under the door reported at 3 AM, Old City.";
            cLodger.leadReaction = new List<DialogueLine> { L(D, "A guest who never checks out. Funny thing to read in a hotel room.") };
            cLodger.arrivalLines = new List<DialogueLine>
            {
                L(D, "Caravanserai Inn, room nine. Warm in here. Lived-in, without the living."),
            };
            cLodger.bookRevealLines = new List<DialogueLine>();
            cLodger.identifyLines = new List<DialogueLine>
            {
                L(D, "The Lodger. Takes a room, keeps it, forgets it was ever going anywhere."),
                L(D, "...The fix isn't written. The ink just stops."),
                L(D, "Fine. It signs the register under a name that isn't its own. Strike the name, with the pen it signed with."),
            };
            cLodger.wrongToolLine = "I try the {0}. Down the hall a key turns in a lock. It's not going anywhere.";
            cLodger.resolveLines = new List<DialogueLine>
            {
                L(D, "One line through the name, in its own ink."),
                L(N, "The ink runs. The room cools. Somewhere a clock starts ticking again."),
                L(D, "Checkout's at noon, friend."),
            };
            EditorUtility.SetDirty(cLodger);

            // ---------------------------------------------------------------- case 4: The Mirror-Born
            var cMirror = Asset<CaseData>($"{DataRoot}/Cases/TheMirrorBorn/Case_TheMirrorBorn.asset");
            cMirror.id = "the_mirror_born";
            cMirror.title = "Room 414";
            cMirror.sceneName = HotelScene;
            cMirror.entity = mirrorBorn;
            cMirror.isFinal = true;
            cMirror.clues = new List<ClueData>
            {
                Clue("TheMirrorBorn", "Clue_Radio", "mirrorborn.radio", "The Radio",
                    "Same delay. Same words. Same pause before \"three a.m.\" How many nights now?",
                    "a flight that never leaves"),
                Clue("TheMirrorBorn", "Clue_HotelBed", "mirrorborn.bed", "My Bed",
                    "Bed's still made. I haven't slept in it. Not once.", "an unslept bed"),
                Clue("TheMirrorBorn", "Clue_Ashtray", "mirrorborn.ashtray", "My Ashtray",
                    "Ashtray's full. I don't remember smoking any of these.", "cigarette burns, no smoker"),
            };
            cMirror.edition = "Baku  ·  Late edition  ·  Vol. XII, No. 2??";
            cMirror.flavorHeadlines = new List<string>
            {
                "Flight 214 Still Grounded; Airline Declines Comment",
                "Airport Delays Enter Third Week",
                "Clear Skies Forecast All Week",
            };
            cMirror.leadHeadline = "Hotel Caspian Rest: Guest in Room 414 Has Not Left in Weeks. Staff \"Concerned\"";
            cMirror.leadDetail = "Light under the door every night, 3 AM, Hotel Caspian Rest.";
            cMirror.leadReaction = new List<DialogueLine>
            {
                L(D, "Room 414."),
                L(D, "...That's my room."),
            };
            cMirror.arrivalLines = new List<DialogueLine>
            {
                L(R, "...Caspian Air regrets to announce that flight 214 to New York is delayed. Passengers are asked to remain at their hotels. We will update you at three a.m."),
                L(D, "Same announcement. Word for word."),
                L(D, "Fine. Work it like any other scene."),
            };
            cMirror.bookRevealLines = new List<DialogueLine>
            {
                L(D, "There's a page at the back. I didn't write a new page."),
                L(D, "...It's my hand. I didn't write it."),
            };
            cMirror.identifyLines = new List<DialogueLine>
            {
                L(D, "Six signs. Every one of them checked."),
                L(D, "Every one of them mine."),
                L(D, "The fix says every tool in the bag. Fine. The mirror's where this room feels wrong."),
            };
            cMirror.wrongToolLine = "Nothing.";
            cMirror.resolveLines = new List<DialogueLine>();
            EditorUtility.SetDirty(cMirror);

            // ---------------------------------------------------------------- config
            var config = Asset<GameConfig>(ConfigPath);
            config.hotelScene = HotelScene;
            config.cases = new List<CaseData> { cFam, cEcho, cLodger, cMirror };
            config.bestiary = new List<BestiaryEntry> { famished, saltWidow, echo, lanternSaint, lodger, mirrorBorn };
            config.startingTools = new List<ToolData> { revolver, nails, holyWater, tableSalt };
            config.radioLine = L(R, "...Caspian Air regrets to announce that flight 214 to New York is delayed. Passengers are asked to remain at their hotels. We will update you at three a.m.");
            config.radioReactions = new List<string>
            {
                "Delayed again. Watch says 3:12. Watch has said 3:12 since Tuesday. Might as well read the paper.",
                "Same announcement. They must have it on a loop.",
                "...Same words. Same pause before \"three a.m.\"",
                "...",
            };
            config.mirrorLines = new List<string>
            {
                "Hat brim down. Three days of stubble under there, probably. Don't need to check.",
                "Mirror's cold. Cheap hotel.",
                "Still haven't slept. Face doesn't look tired. Can't really see the face.",
                "Don't feel like looking right now.",
            };
            config.endingLines = new List<DialogueLine>
            {
                L(N, "Dutch looks."),
                L(N, "The man in the glass is wearing his coat and his hat, lit the wrong colour."),
                L(M, "Took you long enough."),
                L(D, "You're the new entry."),
                L(M, "Oldest one in the Book. You just never turned to the back."),
                L(D, "How long have I been in this room?"),
                L(M, "Ask the radio."),
                L(R, "...Caspian Air flight 214 to New York is now boarding. All passengers, please proceed to the gate."),
                L(D, "...Huh. On time."),
            };
            config.endTitle = "HOLLOW SAINT";
            config.endSubtitle = "Flight 214 to New York departed on schedule.\nRoom 414 was found empty, the bed made, the mirror cold.";
            config.newspaperName = "THE CASPIAN COURIER";
            EditorUtility.SetDirty(config);

            AssetDatabase.SaveAssets();
            Debug.Log("[ContentBuilder] Content built: 4 cases, 6 bestiary entries, 7 tools.");
        }
    }
}
