/*
 * Copyright (C) 2014 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;
using System.Collections;

// Note: DO NOT edit or reomve the GPGSID markers next to the achievement/leaderboard
// IDs. They are used by the git pre-commit script to check if you are accidentally
// trying to commit actual IDs instead of placeholders to the repository.

public static class GameIds {
    // Achievements IDs (as given by Developer Console)
    public class Achievements {
		public const string NotADisaster = "CgkI04esw5EFEAIQAQ"; // <GPGSID>
		public const string PointBlank = "CgkI04esw5EFEAIQAg"; // <GPGSID>
		public const string FullCombo = "CgkI04esw5EFEAIQAw"; // <GPGSID>
		public const string ClearAllLevels = "CgkI04esw5EFEAIQBA"; // <GPGSID>
		public const string PerfectAccuracy = "CgkI04esw5EFEAIQBQ"; // <GPGSID>

        public readonly static string[] ForRank = {
			"CgkI04esw5EFEAIQBw", // <GPGSID>
			"CgkI04esw5EFEAIQCA", // <GPGSID>
			"CgkI04esw5EFEAIQCQ" // <GPGSID>
        };
        public readonly static int[] RankRequired = { 3, 6, 10 };

        public readonly static string[] ForTotalStars = {
			"CgkI04esw5EFEAIQCg", // <GPGSID>
			"CgkI04esw5EFEAIQCw", // <GPGSID>
			"CgkI04esw5EFEAIQDA" // <GPGSID>
        };
        public readonly static int[] TotalStarsRequired = { 12, 24, 36 };

        // incrementals:
        public readonly static string[] IncGameplaySeconds = {
			"CgkI04esw5EFEAIQDQ", // <GPGSID>
			"CgkI04esw5EFEAIQDg", // <GPGSID>
			"CgkI04esw5EFEAIQDw" // <GPGSID>
        };
        public readonly static string[] IncGameplayRounds = {
			"CgkI04esw5EFEAIQEA", // <GPGSID>
			"CgkI04esw5EFEAIQEQ", // <GPGSID>
			"CgkI04esw5EFEAIQEg" // <GPGSID>
        };
    }

    // Leaderboard ID (as given by Developer Console)
	public readonly static string LeaderboardId = "CgkI04esw5EFEAIQBg"; // <GPGSID>
}

