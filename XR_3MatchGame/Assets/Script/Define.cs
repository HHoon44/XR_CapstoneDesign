namespace XR_3MatchGame.Util
{
    public enum SceneType
    {
        None, Main, InGame, Lobby, Select, Loading, FirstLoading
    }

    public enum PoolType
    {
        None, Block
    }

    public enum GameState
    {
        None, Play, Checking, SKill, End, Item, Spawn
    }

    public enum AtlasType
    {
        None, BlockAtlas, IconAtlas
    }

    public enum SwipeDir
    {
        None, Top, Bottom, Left, Right
    }

    public enum ElementType
    {
        None, Fire, Ice, Grass, Dark, Light, Lightning, Balance
    }

    public enum IntroPhase
    {
        None, Start, Resource, Data, UI, Complete
    }

    public enum ItemType 
    {
        None, Boom, Time, Skill
    }

    public class HighScore
    {
        public const string Score_0 = "Score_0";
        public const string Score_1 = "Score_1";
        public const string Score_2 = "Score_2";

        public const string Name_0 = "Name_0";
        public const string Name_1 = "Name_1";
        public const string Name_2 = "Name_2";
    }
}