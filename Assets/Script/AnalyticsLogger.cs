using Firebase.Analytics;
using UnityEngine;

public static class AnalyticsLogger
{
    private static bool Ready => FirebaseManager.IsFirebaseReady;

    public static void LogGameStart()
    {
        Debug.Log("game_start");
        if (!Ready)
            return;
        FirebaseAnalytics.LogEvent("game_start");
    }

    public static void LogGamePaused()
    {
        Debug.Log("game_paused");
        if (!Ready)
            return;
        FirebaseAnalytics.LogEvent("game_paused");
    }

    public static void LogGameResume()
    {
        Debug.Log("game_resume");
        if (!Ready)
            return;
        FirebaseAnalytics.LogEvent("game_resume");
    }

    public static void LogGameOver(int score, int bestScore)
    {
        Debug.Log("game_over");
        if (!Ready)
            return;
        FirebaseAnalytics.LogEvent(
            "game_over",
            new Parameter("score", score),
            new Parameter("best_score", bestScore)
        );
    }

    public static void LogPinkObstacleCollision()
    {
        Debug.Log("pink_obstacle_collision");
        if (!Ready)
            return;
        FirebaseAnalytics.LogEvent("pink_obstacle_collision");
    }

    public static void LogYellowCollision()
    {
        Debug.Log("yellow_collision");
        if (!Ready)
            return;
        FirebaseAnalytics.LogEvent("yellow_collision");
    }

    public static void LogYellowBreak()
    {
        Debug.Log("yellow_obstacle_broken");
        if (!Ready)
            return;
        FirebaseAnalytics.LogEvent("yellow_obstacle_broken");
    }

    public static void LogEnergyEmpty()
    {
        Debug.Log("energy_empty");

        if (!Ready)
            return;
        FirebaseAnalytics.LogEvent("energy_empty");
    }

    public static void LogReboot(int score)
    {
        Debug.Log("game_reboot");
        if (!Ready)
            return;
        FirebaseAnalytics.LogEvent("game_reboot", new Parameter("score", score));
    }
}
